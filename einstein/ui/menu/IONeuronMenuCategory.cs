using Einstein.model;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class IONeuronMenuCategory : NeuronMenuCategory
    {
        private Action<BaseNeuron> onRemove;
        private bool disableOnRemove;
        public IONeuronMenuCategory(NeuronMenuButton button,
            ICollection<BaseNeuron> neuronOptions,
            Action<BaseNeuron> onRemove)
            : base(button, neuronOptions)
        {
            this.onRemove = onRemove;
            disableOnRemove = false;
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
            IO.RENDERER.Add(neuronDrawable, OPTION_LAYER);

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

            if (!disableOnRemove)
            {
                onRemove.Invoke(neuron);
            }
        }

        public void ClearAllOptions()
        {
            // must avoid concurrent modification exception
            LinkedList<BaseNeuron> neuronsToRemove = new LinkedList<BaseNeuron>();
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                neuronsToRemove.AddFirst(neuronDrawable.Neuron);
            }
            disableOnRemove = true; // kinda hacky but easiest solution, and doesn't add duplicate code
            foreach (BaseNeuron neuron in neuronsToRemove)
            {
                RemoveOption(neuron);
            }
            disableOnRemove = false;
        }

        public override string LogDetailsForCrash()
        {
            string log = base.LogDetailsForCrash();
            log += "\nonRemove.Method.Name = " + onRemove.Method.Name;
            log += "\nonRemove.Method.GetParameters() = " + string.Join<ParameterInfo>(",", onRemove.Method.GetParameters());
            log += "\nonRemove.Method.ReturnType = " + onRemove.Method.ReturnType;
            log += "\nonRemove.Method.GetMethodBody().LocalVariables = ";
            if (onRemove.Method.GetMethodBody().LocalVariables == null) { log += "null"; }
            else if (onRemove.Method.GetMethodBody().LocalVariables.Count == 0) { log += "empty"; }
            else { log += string.Join(",", onRemove.Method.GetMethodBody().LocalVariables); }
            return log;
        }
    }
}
