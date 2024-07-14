using Einstein.config;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using Einstein.ui.menu;
using phi.io;
using phi.other;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Einstein.ui.editarea
{
    public class EditArea
    {
        // must be exact reciprocals of each other
        public const float ZOOM_IN_RATIO = 1.25f;
        public const float ZOOM_OUT_RATIO = 0.8f;
        public const int ZOOM_MIN_LEVEL = -32;
        public const int ZOOM_MAX_LEVEL = 8;

        public BaseBrain Brain { get; private set; }
        public BibiteVersion BibiteVersion { get; private set; }
        public Color PaintColor;
        public bool isPainting;

        private Dictionary<int, NeuronRenderable> neuronIndexToNR;
        public IEnumerable<NeuronRenderable> NeuronRenderables { get { return neuronIndexToNR.Values; } }
        private Action<BaseNeuron> onRemove;
        private bool disableOnRemove;
        private Dictionary<(int, int), SynapseRenderable> synapseIndicesToSR;
        private Dictionary<int, int> indexToLayer;
        private SynapseRenderable startedSynapse;
        private int nextHiddenNeuronIndex;
        private bool justFinishedSynapse;
        private int zoomLevel;
        private bool shiftView;
        private int shiftViewInitX;
        private int shiftViewInitY;
        private bool displayValues;

        public EditArea(BaseBrain brain, Action<BaseNeuron> onRemove, BibiteVersion bibiteVersion)
        {
            BibiteVersion = bibiteVersion;
            PaintColor = BaseNeuron.DEFAULT_COLOR_GROUP;
            isPainting = false;
            neuronIndexToNR = new Dictionary<int, NeuronRenderable>();
            this.onRemove = onRemove;
            disableOnRemove = false;
            synapseIndicesToSR = new Dictionary<(int, int), SynapseRenderable>();
            nextHiddenNeuronIndex = bibiteVersion.HIDDEN_NODES_INDEX_MIN;
            justFinishedSynapse = false;
            Brain = brain;
            zoomLevel = 0;
            shiftView = false;
            displayValues = false;
        }

        public void Initialize()
        {
            IO.MOUSE.MID_SCROLL_UP.Subscribe(ZoomIn);
            IO.MOUSE.MID_SCROLL_DOWN.Subscribe(ZoomOut);
            IO.MOUSE.LEFT_DOWN.Subscribe(EnableShiftingView);
            IO.MOUSE.MOVE.Subscribe(ShiftViewTo);
            IO.MOUSE.LEFT_UP.Subscribe(DisableShiftingView);
        }

        public void Uninitialize()
        {
            IO.MOUSE.MID_SCROLL_UP.Unsubscribe(ZoomIn);
            IO.MOUSE.MID_SCROLL_DOWN.Unsubscribe(ZoomOut);
            IO.MOUSE.LEFT_DOWN.Unsubscribe(EnableShiftingView);
            IO.MOUSE.MOVE.Unsubscribe(ShiftViewTo);
            IO.MOUSE.LEFT_UP.Unsubscribe(DisableShiftingView);
        }

        // ----- Manage neurons -----

        public void AddNeuron(BaseNeuron neuron, bool tryPainting)
        {
            Brain.Add(neuron);

            NeuronRenderable dragNeuron = new NeuronRenderable(this, neuron, tryPainting);
            dragNeuron.Initialize();
            dragNeuron.SetIsValueDisplaying(displayValues);

            neuronIndexToNR.Add(neuron.Index, dragNeuron);
        }

        public void CreateHiddenNeuron(NeuronType type)
        {
            string desc = type.ToString() + (nextHiddenNeuronIndex - BibiteVersion.HIDDEN_NODES_INDEX_MIN);
            while (Brain.ContainsNeuronDescription(desc))
            {
                nextHiddenNeuronIndex++;
                desc = type.ToString() + (nextHiddenNeuronIndex - BibiteVersion.HIDDEN_NODES_INDEX_MIN);
            }
            AddNeuron(new JsonNeuron(nextHiddenNeuronIndex,
                type,
                type == NeuronType.Mult ? 1f : 0f,
                type.ToString() + (nextHiddenNeuronIndex - BibiteVersion.HIDDEN_NODES_INDEX_MIN),
                BibiteVersion),
                true);

            nextHiddenNeuronIndex++;
        }

        public void RemoveNeuron(BaseNeuron neuron)
        {
            // remove connecting synapses first
            // can't remove inside the loops b/c would be concurrent modification
            LinkedList<BaseSynapse> toRemove = new LinkedList<BaseSynapse>();
            foreach (BaseSynapse from in Brain.GetSynapsesFrom(neuron))
            {
                toRemove.AddFirst(from);
            }
            foreach (BaseSynapse to in Brain.GetSynapsesTo(neuron))
            {
                toRemove.AddFirst(to);
            }
            foreach (BaseSynapse synapse in toRemove)
            {
                RemoveSynapse(synapse);
            }
            if (startedSynapse != null && startedSynapse.From.Neuron.Equals(neuron))
            {
                IO.FRAME_TIMER.QueueUninit(startedSynapse.Uninitialize);
                startedSynapse = null;
            }

            Brain.Remove(neuron);

            NeuronRenderable nr = neuronIndexToNR[neuron.Index];
            IO.FRAME_TIMER.QueueUninit(nr.Uninitialize);
            neuronIndexToNR.Remove(neuron.Index);

            if (!disableOnRemove)
            {
                onRemove.Invoke(neuron);
            }
        }

        // ----- Manage synapses -----

        // For starting to add a synapse via the UI
        public void StartSynapse(NeuronRenderable from, int x, int y)
        {
            if (justFinishedSynapse || startedSynapse != null)
            {
                // don't start a new synapse every time you complete one
                justFinishedSynapse = false;
                return;
            }
            startedSynapse = new SynapseRenderable(this, from, x, y);
            startedSynapse.Initialize();
        }

        // For finishing a synapse via the UI
        // (should probably only be run from Finalize inside SynapseRenderable)
        public void FinishSynapse(BaseSynapse synapse)
        {
            if (startedSynapse == null)
            {
                throw new InvalidOperationException(
                    "Every FinishSynapse call must be paired with a prior StartSynapse call");
            }
            Brain.Add(synapse);

            synapseIndicesToSR.Add((synapse.From.Index, synapse.To.Index), startedSynapse);
            startedSynapse = null;
            justFinishedSynapse = true;
        }

        // For programmatically adding a synapse (e.g. during loading)
        public void AddSynapse(BaseSynapse synapse)
        {
            NeuronRenderable from = neuronIndexToNR[synapse.From.Index];
            NeuronRenderable to = neuronIndexToNR[synapse.To.Index];
            SynapseRenderable sr = new SynapseRenderable(this, synapse, from, to);

            SynapseRenderable oldStartedSynapse = startedSynapse;
            bool oldJustFinishedSynapse = justFinishedSynapse;
            startedSynapse = sr;
            sr.Initialize(); // Calls FinishSynapse, which calls Brain.Add(synapse)
            startedSynapse = oldStartedSynapse;
            justFinishedSynapse = oldJustFinishedSynapse;
        }

        public void RemoveSynapse(BaseSynapse synapse)
        {
            if (!Brain.ContainsSynapse(synapse)) { return; } // so that removing a synapse and a linked neuron with 1 shift+click doesn't crash
            Brain.Remove(synapse);

            (int, int) key = (synapse.From.Index, synapse.To.Index);
            IO.FRAME_TIMER.QueueUninit(synapseIndicesToSR[key].Uninitialize);
            synapseIndicesToSR.Remove(key);
        }

        // ----- Manage (i.e. load) brain -----

        public void LoadBrain(BaseBrain brain, BibiteVersion bibiteVersion)
        {
            // Clear any data currently in the brain
            NeuronRenderable[] neuronsToClearFromOldBrain = new NeuronRenderable[neuronIndexToNR.Values.Count];
            neuronIndexToNR.Values.CopyTo(neuronsToClearFromOldBrain, 0);
            disableOnRemove = true;
            foreach (NeuronRenderable nr in neuronsToClearFromOldBrain)
            {
                RemoveNeuron(nr.Neuron);
            }
            disableOnRemove = false;

            // re-init for a new brain
            BibiteVersion = bibiteVersion;
            neuronIndexToNR = new Dictionary<int, NeuronRenderable>();
            synapseIndicesToSR = new Dictionary<(int, int), SynapseRenderable>();
            nextHiddenNeuronIndex = BibiteVersion.HIDDEN_NODES_INDEX_MIN;

            // Ensures shallow copies are equal (in case that matters) and more importantly,
            // if any Neuron/Synapse/Brain class is a subclass of the base class, that is preserved
            Brain = brain;

            // Remove neurons that aren't connected to anything
            LinkedList<BaseNeuron> neuronsToRemoveFromNewBrain = new LinkedList<BaseNeuron>(); // avoid concurrent modification
            foreach (BaseNeuron neuron in Brain.Neurons)
            {
                if (Brain.GetSynapsesFrom(neuron).Count == 0
                    && Brain.GetSynapsesTo(neuron).Count == 0)
                {
                    neuronsToRemoveFromNewBrain.AddFirst(neuron);
                    continue;
                }
                if (neuron.Index >= nextHiddenNeuronIndex)
                {
                    nextHiddenNeuronIndex = neuron.Index + 1;
                }
            }
            foreach (BaseNeuron neuron in neuronsToRemoveFromNewBrain)
            {
                Brain.Remove(neuron);
            }

            // Remove and re-add everything else, so that the right setup can happen when re-adding them (e.g. indexes are up to date)
            // deep copy stuff
            LinkedList<BaseNeuron> neurons = new LinkedList<BaseNeuron>();
            LinkedList<BaseSynapse> synapses = new LinkedList<BaseSynapse>();
            foreach (BaseNeuron neuron in Brain.Neurons)
            {
                neurons.AddFirst(neuron);
            }
            foreach (BaseSynapse synapse in Brain.Synapses)
            {
                synapses.AddFirst(synapse);
            }
            foreach (BaseNeuron neuron in neurons)
            {
                Brain.Remove(neuron); // also removes all synapses
            }
            foreach (BaseNeuron neuron in neurons)
            {
                AddNeuron(neuron, false);
            }
            foreach (BaseSynapse synapse in synapses)
            {
                AddSynapse(synapse);
            }

            AutoArrange();

            if (neuronIndexToNR.Any() && neuronIndexToNR.First().Value.Neuron is JsonNeuron)
            {
                foreach (NeuronRenderable nr in neuronIndexToNR.Values)
                {
                    JsonNeuron jn = (JsonNeuron)nr.Neuron;
                    nr.Reposition(jn.DiagramX, jn.DiagramY);
                }
            }

            Brain.MarkChangesAsSaved();
        }

        // ----- Background drag -----

        public void EnableShiftingView(int startX, int startY)
        {
            if (shiftView || startX < GetX()) { return; } // if click not in edit area
            shiftView = true;
            shiftViewInitX = startX;
            shiftViewInitY = startY;
        }

        public void ShiftViewTo(int x, int y)
        {
            if (!shiftView) { return; }
            int dx = x - shiftViewInitX;
            int dy = y - shiftViewInitY;
            shiftViewInitX = x;
            shiftViewInitY = y;
            ShiftView(dx, dy);
        }

        public void DisableShiftingView()
        {
            shiftView = false;
        }

        public void ShiftView(int dx, int dy)
        {
            if (dx == 0 && dy == 0) { return; }
            foreach (NeuronRenderable nr in neuronIndexToNR.Values)
            {
                float x = nr.NeuronDrawable.GetCircleCenterXfloat();
                float y = nr.NeuronDrawable.GetCircleCenterYfloat();
                nr.NeuronDrawable.SetCircleCenterXY(x + dx, y + dy);
                nr.Reposition();
            }
        }

        // ----- Zoom -----

        public void ZoomInCentered() { ZoomIn(GetX() + GetWidth() / 2, GetY() + GetHeight() / 2); }
        public void ZoomIn(int x, int y)
        {
            if (x < GetX() || zoomLevel <= ZOOM_MIN_LEVEL) { return; }
            zoomLevel--;
            if (zoomLevel == 0)
            {
                foreach (NeuronRenderable nr in neuronIndexToNR.Values)
                {
                    // Make the distance to the mouse pointer a fraction of what it was
                    float dx = nr.NeuronDrawable.GetCircleCenterXfloat() - x;
                    float dy = nr.NeuronDrawable.GetCircleCenterYfloat() - y;
                    dx *= ZOOM_IN_RATIO;
                    dy *= ZOOM_IN_RATIO;
                    nr.NeuronDrawable.SetCircleCenterXY((int)(x + dx + 0.5f), (int)(y + dy + 0.5f));
                    nr.Reposition();
                }
            }
            else
            {
                foreach (NeuronRenderable nr in neuronIndexToNR.Values)
                {
                    // Make the distance to the mouse pointer a fraction of what it was
                    float dx = nr.NeuronDrawable.GetCircleCenterXfloat() - x;
                    float dy = nr.NeuronDrawable.GetCircleCenterYfloat() - y;
                    dx *= ZOOM_IN_RATIO;
                    dy *= ZOOM_IN_RATIO;
                    nr.NeuronDrawable.SetCircleCenterXY(x + dx, y + dy);
                    nr.Reposition();
                }
            }
        }

        public void ZoomOutCentered() { ZoomOut(GetX() + GetWidth() / 2, GetY() + GetHeight() / 2); }
        public void ZoomOut(int x, int y)
        {
            if (x < GetX() || zoomLevel >= ZOOM_MAX_LEVEL) { return; }
            zoomLevel++;
            foreach (NeuronRenderable nr in neuronIndexToNR.Values)
            {
                // Make the distance to the mouse pointer a fraction of what it was
                float dx = nr.NeuronDrawable.GetCircleCenterXfloat() - x;
                float dy = nr.NeuronDrawable.GetCircleCenterYfloat() - y;
                dx *= ZOOM_OUT_RATIO;
                dy *= ZOOM_OUT_RATIO;
                nr.NeuronDrawable.SetCircleCenterXY(x + dx, y + dy);
                nr.Reposition();
            }
        }

        public void ResetZoomLevel()
        {
            // re-center view
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            foreach (NeuronRenderable nr in neuronIndexToNR.Values)
            {
                int x = nr.NeuronDrawable.GetCircleCenterX();
                int y = nr.NeuronDrawable.GetCircleCenterY();
                minX = Math.Min(x, minX);
                minY = Math.Min(y, minY);
                maxX = Math.Max(x, maxX);
                maxY = Math.Max(y, maxY);
            }
            int neuronsCenterX = (minX + maxX) / 2;
            int neuronsCenterY = (minY + maxY) / 2;
            int editAreaCenterX = GetX() + GetWidth() / 2;
            int editAreaCenterY = GetY() + GetHeight() / 2;
            ShiftView(editAreaCenterX - neuronsCenterX, editAreaCenterY - neuronsCenterY);

            // undo zoom
            if (zoomLevel == 0) { return; }
            float ratio;
            if (zoomLevel > 0) // if we are currently zoomed out
            {
                // ratio = ZOOM_IN_RATIO ^ zoomLevel
                ratio = 1f;
                for (int i = 0; i < zoomLevel; i++)
                {
                    ratio *= ZOOM_IN_RATIO;
                }
            }
            else // if we are currently zoomed in
            {
                // ratio = ZOOM_OUT_RATIO ^ -zoomLevel
                ratio = 1f;
                for (int i = 0; i < -zoomLevel; i++)
                {
                    ratio *= ZOOM_OUT_RATIO;
                }
            }
            foreach (NeuronRenderable nr in neuronIndexToNR.Values)
            {
                // Make the distance to the mouse pointer a fraction of what it was
                float dx = nr.NeuronDrawable.GetCircleCenterXfloat() - editAreaCenterX;
                float dy = nr.NeuronDrawable.GetCircleCenterYfloat() - editAreaCenterY;
                dx *= ratio;
                dy *= ratio;
                nr.NeuronDrawable.SetCircleCenterXY((int)(editAreaCenterX + dx + 0.5f), (int)(editAreaCenterY + dy + 0.5f));
                nr.Reposition();
            }
            zoomLevel = 0;
        }

        // ----- Auto-Arrange neuron renderables -----

        private enum LayerType
        {
            NeverOutputsToInput = -0x60000000,    // 111
            Input = -0x40000000,                  // 110
            NeverOutputsFromInput = -0x20000000,  // 101
            Main = 0,                             // 000
            Output = 0x40000000,                  // 010
            NeverOutputsFromOutput = 0x60000000,  // 011
        }

        private enum LayerTypeMasks
        {
            GetLayer = 0x1fffffff,                // 0001111...
            GetType = -0x60000000,                // 111
            MidValue = 0x10000000,                // 0001
            IsNeverOutputs = 0x20000000,          // 001 (currently unused)
        }

        public void AutoArrange()
        {
            ResetZoomLevel();
            indexToLayer = new Dictionary<int, int>();

            LinkedList<BaseNeuron> inputNeurons = new LinkedList<BaseNeuron>();
            LinkedList<BaseNeuron> outputNeurons = new LinkedList<BaseNeuron>();
            LinkedList<BaseNeuron> hiddenNeurons = new LinkedList<BaseNeuron>();
            foreach (BaseNeuron neuron in Brain.Neurons)
            {
                if (neuron.IsInput())
                {
                    inputNeurons.AddLast(neuron);
                }
                else if (neuron.IsOutput())
                {
                    outputNeurons.AddLast(neuron);
                }
                else
                {
                    hiddenNeurons.AddLast(neuron);
                }
            }

            // assign layers of hidden neurons
            // note that later assignments overwrite the earlier assignments
            foreach (BaseNeuron hiddenNeuron in hiddenNeurons)
            {
                // kinda fudgy to just assign these unconnected never-outputs to the same layers as the to-input never-outputs, but meh
                indexToLayer[hiddenNeuron.Index] = (int)LayerType.NeverOutputsToInput | (int)LayerTypeMasks.MidValue;
            }
            foreach (BaseNeuron inNeuron in inputNeurons)
            {
                AssignLayersBefore(indexToLayer, inNeuron, ((int)LayerType.NeverOutputsToInput | (int)LayerTypeMasks.MidValue), new HashSet<int>());
            }
            foreach (BaseNeuron outNeuron in outputNeurons)
            {
                AssignLayersAfter(indexToLayer, outNeuron, ((int)LayerType.NeverOutputsFromOutput | (int)LayerTypeMasks.MidValue), new HashSet<int>());
            }
            foreach (BaseNeuron inNeuron in inputNeurons)
            {
                AssignLayersAfter(indexToLayer, inNeuron, ((int)LayerType.NeverOutputsFromInput | (int)LayerTypeMasks.MidValue), new HashSet<int>());
            }
            foreach (BaseNeuron outNeuron in outputNeurons)
            {
                AssignLayersBefore(indexToLayer, outNeuron, ((int)LayerType.Main | (int)LayerTypeMasks.MidValue), new HashSet<int>());
            }
            foreach (BaseNeuron outNeuron in outputNeurons)
            {
                indexToLayer[outNeuron.Index] = ((int)LayerType.Output | (int)LayerTypeMasks.MidValue);
            }
            foreach (BaseNeuron inNeuron in inputNeurons)
            {
                indexToLayer[inNeuron.Index] = ((int)LayerType.Input | (int)LayerTypeMasks.MidValue);
            }

            // Count number of neurons in each layer
            Dictionary<int, int> layerToTotalNeurons = new Dictionary<int, int>();
            foreach (BaseNeuron neuron in Brain.Neurons)
            {
                int layer = indexToLayer[neuron.Index];
                layerToTotalNeurons[layer] = layerToTotalNeurons.ContainsKey(layer) ? layerToTotalNeurons[layer] + 1 : 1;
            }

            int totalWidth = EditArea.GetWidth() - NeuronDrawable.CIRCLE_DIAMETER;
            int totalHeight = EditArea.GetHeight() - NeuronDrawable.CIRCLE_DIAMETER;

            // TODO? find a better vertical order for each layer to have its neurons be put in?

            // horizontally divide up layers
            // vertically divide up neurons in each layer
            if (layerToTotalNeurons.Count == 0) { return; } // avoid divide by 0
            // used floats to not let rounding make everything get slightly further off from ideal
            // positions the further down and right you go
            float eachWidth = totalWidth / (float)layerToTotalNeurons.Count;
            float x = EditArea.GetX() + NeuronDrawable.CIRCLE_RADIUS - eachWidth / 2;
            List<int> sortedLayers = layerToTotalNeurons.Keys.ToList();
            sortedLayers.Sort();
            foreach (int layer in sortedLayers)
            {
                if (layerToTotalNeurons[layer] == 0) { continue; }
                x += eachWidth;
                float eachHeight = totalHeight / (float)layerToTotalNeurons[layer];
                float y = EditArea.GetY() + NeuronDrawable.CIRCLE_RADIUS - eachHeight / 2;

                // maybe there's a more efficient way but meh, "this is fine"
                foreach (BaseNeuron neuron in Brain.Neurons)
                {
                    if (indexToLayer[neuron.Index] == layer)
                    {
                        y += eachHeight;
                        // position neuron
                        neuronIndexToNR[neuron.Index].Reposition((int)x, (int)y);
                    }
                }
            }
        }

        public void AssignLayersBefore(Dictionary<int, int> layers,
            BaseNeuron toThisNeuron, int thisNeuronLayer,
            HashSet<int> dontAssign)
        {
            int prevLayer = thisNeuronLayer - 1;
            foreach (BaseSynapse synapse in Brain.GetSynapsesTo(toThisNeuron))
            {
                BaseNeuron neuron = synapse.From;
                // if it's in the don't assign set, never assign it
                // if it's an input or output, never assign it (just for efficiency)
                // if it's not already assigned as part of this layer, always assign it
                // otherwise,
                // reassign to this layer only if the previous layer assignment is further forwards
                if (!dontAssign.Contains(neuron.Index) &&
                    !neuron.IsInput() && !neuron.IsOutput() &&
                    (!layers.ContainsKey(neuron.Index) ||
                    (layers[neuron.Index] & (int)LayerTypeMasks.GetType) != (prevLayer & (int)LayerTypeMasks.GetType) ||
                    prevLayer <= layers[neuron.Index]))
                {
                    layers[neuron.Index] = prevLayer;
                    dontAssign.Add(neuron.Index);
                    AssignLayersBefore(layers, neuron, prevLayer, dontAssign);
                    dontAssign.Remove(neuron.Index);
                }
            }
        }
        public void AssignLayersAfter(Dictionary<int, int> layers,
            BaseNeuron fromThisNeuron, int thisNeuronLayer,
            HashSet<int> dontAssign)
        {
            int nextLayer = thisNeuronLayer + 1;
            foreach (BaseSynapse synapse in Brain.GetSynapsesFrom(fromThisNeuron))
            {
                BaseNeuron neuron = synapse.To;
                // if it's in the don't assign set, never assign it
                // if it's an input or output, never assign it (just for efficiency)
                // if it's not already assigned as part of this layer, always assign it
                // otherwise,
                // reassign to this layer only if the previous layer assignment is further backwards
                if (!dontAssign.Contains(neuron.Index) &&
                    !neuron.IsInput() && !neuron.IsOutput() &&
                    (!layers.ContainsKey(neuron.Index) ||
                    (layers[neuron.Index] & (int)LayerTypeMasks.GetType) != (nextLayer & (int)LayerTypeMasks.GetType) ||
                    layers[neuron.Index] <= nextLayer))
                {
                    layers[neuron.Index] = nextLayer;
                    dontAssign.Add(neuron.Index);
                    AssignLayersAfter(layers, neuron, nextLayer, dontAssign);
                    dontAssign.Remove(neuron.Index);
                }
            }
        }

        // ----- Misc -----

        // if there are none, returns null
        public NeuronRenderable HasNeuronAtPosition(int x, int y)
        {
            foreach (NeuronRenderable dragNeuron in neuronIndexToNR.Values)
            {
                if (dragNeuron.GetDrawable().GetBoundaryRectangle().Contains(x, y)
                    && dragNeuron.GetDrawable().IsDisplaying())
                {
                    return dragNeuron;
                }
            }
            return null;
        }

        public void SetValuesDisplaying(bool displaying)
        {
            if (displayValues == displaying)
            {
                return;
            }
            displayValues = displaying;
            foreach (NeuronRenderable nr in NeuronRenderables)
            {
                nr.SetIsValueDisplaying(displaying);
            }
        }

        public void RefreshValuesText(IEnumerable<int> updatedNeuronsIndexes)
        {
            foreach (int neuronIndex in updatedNeuronsIndexes)
            {
                neuronIndexToNR[neuronIndex].RefreshValueText();
            }
        }

        public static phi.other.Rectangle GetBounds()
        {
            return new phi.other.Rectangle(GetX(), GetY(), GetWidth(), GetHeight());
        }
        public static int GetWidth()
        {
            return IO.WINDOW.GetWidth() - EditArea.GetX();
        }
        public static int GetHeight()
        {
            return IO.WINDOW.GetHeight() - EditArea.GetY();
        }
        public static int GetX()
        {
            return MenuCategoryButton.WIDTH + EinsteinConfig.PAD * 2;
        }
        public static int GetY()
        {
            return 0;
        }

        public void ClearStartedSynapse(bool clickedOnNeuron)
        {
            IO.FRAME_TIMER.QueueUninit(startedSynapse.Uninitialize);
            startedSynapse = null;
            justFinishedSynapse = clickedOnNeuron;
        }
        public NeuronRenderable GetNROf(BaseNeuron neuron)
        {
            return GetNROf(neuron.Index);
        }

        public NeuronRenderable GetNROf(int index)
        {
            return neuronIndexToNR[index];
        }

        public SynapseRenderable GetSROf(BaseSynapse synapse)
        {
            return GetSROf(synapse.From.Index, synapse.To.Index);
        }

        public SynapseRenderable GetSROf(int fromIndex, int toIndex)
        {
            return synapseIndicesToSR[(fromIndex, toIndex)];
        }

        // ----- Logging -----

        public string LogDetailsForCrash()
        {
            string log = "";
            log += "\nsynapseIndicesToSR = ";
            if (BibiteVersion == null) { log += "null"; }
            else { log += BibiteVersion.ToString(); }
            try
            {
                log += "\nBrain = " + Brain.GetSave(BibiteVersion);
            }
            catch (Exception) { }
            log += "\nneuronIndexToNR = ";
            if (neuronIndexToNR == null) { log += "null"; }
            else if (neuronIndexToNR.Count == 0) { log += "empty"; }
            else { log += string.Join(":", neuronIndexToNR); }
            log += "\nonRemove.Method.Name = " + onRemove.Method.Name;
            log += "\nonRemove.Target = " + onRemove.Target;
            log += "\ndisableOnRemove = " + disableOnRemove;
            log += "\nsynapseIndicesToSR = ";
            if (synapseIndicesToSR == null) { log += "null"; }
            else if (synapseIndicesToSR.Count == 0) { log += "empty"; }
            else { log += string.Join(":", synapseIndicesToSR); }
            log += "\nstartedSynapse = " + (startedSynapse != null ? startedSynapse.ToString() : "null");
            log += "\nhiddenNeuronIndex = " + nextHiddenNeuronIndex;
            return log;
        }
    }
}
