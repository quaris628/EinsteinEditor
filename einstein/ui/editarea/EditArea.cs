using Einstein.model;
using Einstein.ui.menu;
using phi.graphics.renderables;
using phi.io;
using phi.other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class EditArea
    {
        public static readonly Rectangle BOUNDS = new Rectangle(
            NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3,
            EinsteinPhiConfig.Window.WIDTH - NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            EinsteinPhiConfig.Window.HEIGHT - (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3);

        private BaseBrain brain;

        private Dictionary<int, NeuronDraggable> displayedNeuronsIndex;
        private Action<BaseNeuron> onRemove;
        private List<SynapseMultiRenderable> displayedSynapses;

        public EditArea(BaseBrain brain, Action<BaseNeuron> onRemove)
        {
            this.brain = brain;
            displayedNeuronsIndex = new Dictionary<int, NeuronDraggable>();
            this.onRemove = onRemove;
            displayedSynapses = new List<SynapseMultiRenderable>();
        }

        
        public void AddNeuron(BaseNeuron neuron)
        {
            brain.Add(neuron);

            NeuronDraggable dragNeuron = new NeuronDraggable(neuron);
            displayedNeuronsIndex.Add(neuron.Index, dragNeuron);

            dragNeuron.Initialize();
            IO.RENDERER.Add(dragNeuron);
            IO.MOUSE.MID_CLICK.SubscribeOnDrawable(() => {
                RemoveNeuron(neuron);
            }, dragNeuron.GetDrawable());
            IO.MOUSE.RIGHT_UP.SubscribeOnDrawable((x, y) => {
                StartSynapse(dragNeuron, x, y);
            }, dragNeuron.GetDrawable());
        }

        private void RemoveNeuron(BaseNeuron neuron)
        {
            brain.Remove(neuron);

            NeuronDraggable dragNeuron = displayedNeuronsIndex[neuron.Index];
            displayedNeuronsIndex.Remove(neuron.Index);

            dragNeuron.Uninitialize();
            IO.RENDERER.Remove(dragNeuron);
            IO.MOUSE.MID_CLICK.UnsubscribeAllFromDrawable(dragNeuron.GetDrawable());
            IO.MOUSE.RIGHT_UP.UnsubscribeAllFromDrawable(dragNeuron.GetDrawable());

            onRemove.Invoke(neuron);
        }

        private void StartSynapse(NeuronDraggable from, int x, int y)
        {
            SynapseMultiRenderable synapseMR = new SynapseMultiRenderable(
                this, from, x, y);
            displayedSynapses.Add(synapseMR);
            synapseMR.Initialize();
        }

        public void AddSynapse(BaseSynapse synapse)
        {
            brain.Add(synapse);
        }

        private void RemoveSynapse() { }

        // if there are none, returns null
        public NeuronDraggable HasNeuronAtPosition(int x, int y)
        {
            foreach (NeuronDraggable dragNeuron in displayedNeuronsIndex.Values)
            {
                if (dragNeuron.GetDrawable().GetBoundaryRectangle().Contains(x, y)
                    && dragNeuron.GetDrawable().IsDisplaying())
                {
                    return dragNeuron;
                }
            }
            return null;
        }
    }
}
