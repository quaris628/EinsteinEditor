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
        
        public IONeuronMenuCategory(NeuronMenuButton button,
            IEnumerable<BaseNeuron> neuronOptions,
            Action<BaseNeuron> onRemove)
            : base(button, neuronOptions)
        {
            this.onRemove = onRemove;
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (NeuronDrawable neuronDrawable in GetNeuronDrawables())
            {
                IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
                {
                    RemoveOption(neuronDrawable.Neuron);
                }, neuronDrawable);
            }
        }
        public override void Uninitialize()
        {
            base.Uninitialize();
            foreach (NeuronDrawable neuronDrawable in GetNeuronDrawables())
            {
                IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(neuronDrawable);
            }
        }

        public void AddOption(BaseNeuron neuron)
        {
            AddOption(neuron.Index, new NeuronDrawable(neuron));
        }

        public void RemoveOption(BaseNeuron neuron)
        {
            RemoveOption(neuron.Index);
            onRemove.Invoke(neuron);
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
            log += "\nisInit = " + isInit;
            return log;
        }
    }
}
