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
        //private List<SynapseInEditArea> synapses;

        private Dictionary<int, DraggableNeuron> displayedNeuronsIndex;
        private Action<BaseNeuron> onRemove;

        public EditArea(BaseBrain brain, Action<BaseNeuron> onRemove)
        {
            this.brain = brain;
            displayedNeuronsIndex = new Dictionary<int, DraggableNeuron>();
            this.onRemove = onRemove;
        }

        
        public void AddNeuron(BaseNeuron neuron)
        {
            brain.Add(neuron);

            DraggableNeuron dragNeuron = new DraggableNeuron(neuron);
            dragNeuron.Initialize();
            IO.RENDERER.Add(dragNeuron);
            IO.MOUSE.MID_CLICK.SubscribeOnDrawable(() => {
                RemoveNeuron(neuron);
            }, dragNeuron.GetDrawable());
            displayedNeuronsIndex.Add(neuron.Index, dragNeuron);
        }

        public void RemoveNeuron(BaseNeuron neuron)
        {
            brain.Remove(neuron);

            DraggableNeuron dragNeuron = displayedNeuronsIndex[neuron.Index];
            dragNeuron.Uninitialize();
            IO.RENDERER.Remove(dragNeuron);
            IO.MOUSE.MID_CLICK.UnsubscribeAllFromDrawable(dragNeuron.GetDrawable());
            displayedNeuronsIndex.Remove(neuron.Index);
            onRemove.Invoke(neuron);
        }

    }
}
