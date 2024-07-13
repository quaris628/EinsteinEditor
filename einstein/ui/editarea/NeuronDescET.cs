using Einstein.model;
using phi.graphics.drawables;
using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class NeuronDescET : EditableText
    {
        public const string DESC_ALLOWED_CHARS = UPPERCASE_ALPHABET_CHARS + LOWERCASE_ALPHABET_CHARS + NUMBER_CHARS;

        public BaseBrain Brain { get; private set; }
        public BaseNeuron Neuron { get; private set; }

        public NeuronDescET(Text text, BaseBrain brain, BaseNeuron neuron)
            : base(new EditableTextBuilder(text)
                  .WithAllowedChars(DESC_ALLOWED_CHARS)
                  .WithEditingDisabled() // must 'select' before fully 'enable'd
                  .WithAnchorPosition(AnchorPosition.TopCenter))
        {
            Brain = brain;
            Neuron = neuron;
        }

        protected override bool IsMessageValidAsFinalInternal(string message)
        {
            return base.IsMessageValidAsFinalInternal(message)
                && !Brain.ContainsNeuronDescription(message);
        }
        public override void DisableEditing()
        {
            base.DisableEditing();
            Brain.UpdateNeuronDescription(Neuron, text.GetMessage());
        }

        public override void RecenterOnAnchor()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            GetDrawable().SetCenterX(anchorX);
            GetDrawable().SetY(anchorY);
        }
    }
}
