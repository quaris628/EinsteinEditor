using Einstein.model;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class IONeuronMenuCategory : NeuronMenuCategory
    {
        private Action<BaseNeuron> onRemove;
        public IONeuronMenuCategory(NeuronMenuButton button,
            ICollection<BaseNeuron> neuronOptions,
            Action<BaseNeuron> onRemove)
            : base(button, neuronOptions)
        {
            this.onRemove = onRemove;
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
                {
                    RemoveOption(neuronDrawable.Neuron);
                }, neuronDrawable);
            }
        }

        public void AddOption(BaseNeuron neuron)
        {
            NeuronDrawable neuronDrawable = new NeuronDrawable(neuron);
            neuronDrawable.SetDisplaying(Button.IsSelected());

            neuronDrawables.Add(neuron.Index, neuronDrawable);

            IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
            {
                RemoveOption(neuronDrawable.Neuron);
            }, neuronDrawable);
            IO.RENDERER.Add(neuronDrawable);

            RepositionOptions();
        }

        public void RemoveOption(BaseNeuron neuron)
        {
            NeuronDrawable neuronDrawable = neuronDrawables[neuron.Index];

            if (!neuronDrawables.Remove(neuron.Index)) {
                throw new KeyNotFoundException(neuronDrawable + " not found"); // TODO remove this?
            }

            IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(neuronDrawable);
            IO.RENDERER.Remove(neuronDrawable);
            RepositionOptions();

            onRemove.Invoke(neuron);
        }

    }
}
