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
    public class HiddenNeuronMenuCategory : NeuronMenuCategory
    {
        private Action<BaseNeuron> onSelect;
        public HiddenNeuronMenuCategory(NeuronMenuButton button,
            ICollection<BaseNeuron> neuronOptions,
            Action<BaseNeuron> onSelect)
            : base(button, neuronOptions)
        {
            this.onSelect = onSelect;
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
                {
                    onSelect.Invoke(neuronDrawable.Neuron);
                }, neuronDrawable);
            }
        }

        public override string LogDetailsForCrash()
        {
            string log = base.LogDetailsForCrash();
            log += "\nonSelect.Method.Name = " + onSelect.Method.Name;
            log += "\nonSelect.Method.GetParameters() = " + string.Join<ParameterInfo>(",", onSelect.Method.GetParameters());
            log += "\nonSelect.Method.ReturnType = " + onSelect.Method.ReturnType;
            log += "\nonSelect.Method.GetMethodBody().LocalVariables = " + string.Join(",", onSelect.Method.GetMethodBody().LocalVariables);
            return log;
        }
    }
}
