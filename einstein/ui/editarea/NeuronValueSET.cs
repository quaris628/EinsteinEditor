using Einstein.config;
using Einstein.model;
using LibraryFunctionReplacements;
using phi.graphics.drawables;
using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class NeuronValueSET : SelectableEditableText
    {
        private NeuronValueET nvet;
        public NeuronValueSET(NeuronValueET nvet)
            : base(nvet,
                  CustomNumberParser.FloatToString(nvet.Neuron.Bias),
                  EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                  EinsteinConfig.COLOR_MODE.Text) // intentionally reversed
        {
            this.nvet = nvet;
        }

        public override void Select()
        {
            if (nvet.Neuron.BibiteVersion.IsConstantInputNeuron(nvet.Neuron.Index))
            {
                // Don't allow editing the constant neuron's value (always 1)
                return;
            }
            base.Select();
            ((Text)nvet.GetDrawable()).SetColor(EinsteinConfig.COLOR_MODE.Text);
        }

        public override void Deselect()
        {
            base.Deselect();
            ((Text)nvet.GetDrawable()).SetColor(EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);

        }
    }
}
