using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class NeuronDescSET : SelectableEditableText
    {
        private NeuronDescET et; // just a shortcut reference

        public NeuronDescSET(NeuronDescET et) : base(et)
        {
            this.et = et;
        }

        public NeuronDescSET(NeuronDescET et, Color selectedBackColor, Color unselectedBackColor) : base(et, selectedBackColor, unselectedBackColor)
        {
            this.et = et;
        }

        protected override string CorrectInvalidMessageOnDeselect(string invalidDesc)
        {
            string nonNumberDesc = invalidDesc.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            int descNumberInt = int.TryParse(invalidDesc.Substring(nonNumberDesc.Length), out int parseResult) ? parseResult : 0;
            string newDesc = invalidDesc;
            while (et.Brain.ContainsNeuronDescription(newDesc) && et.Brain.GetNeuron(newDesc) != et.Neuron)
            {
                newDesc = nonNumberDesc + ++descNumberInt;
            }
            return newDesc;
        }
    }
}
