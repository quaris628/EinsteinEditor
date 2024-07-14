using Einstein.model;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
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
        public const int NOT_EDITING_LAYER = 7;
        public const int EDITING_LAYER = 11;

        public BaseBrain Brain { get; private set; }
        public BaseNeuron Neuron { get; private set; }

        public NeuronDescET(Text text, BaseBrain brain, BaseNeuron neuron)
            : base(new EditableTextBuilder(text)
                  .WithAllowedChars(DESC_ALLOWED_CHARS)
                  .WithEditingDisabled() // must 'select' before fully 'enable'd
                  .WithAnchor(text.GetCenterX(), text.GetY())
                  .WithAnchorPosition(AnchorPosition.TopCenter))
        {
            Brain = brain;
            Neuron = neuron;
        }

        public override void Initialize()
        {
            base.Initialize();
            IO.RENDERER.Add(text, NOT_EDITING_LAYER);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            IO.RENDERER.Remove(text);
        }

        protected override bool IsMessageValidAsFinalInternal(string message)
        {
            return base.IsMessageValidAsFinalInternal(message)
                && !Brain.ContainsNeuronDescription(message);
        }

        public override void EnableEditing()
        {
            base.EnableEditing();
            SetAnchor(text.GetCenterX(), text.GetY());
            IO.RENDERER.Remove(text);
            IO.RENDERER.Add(text, EDITING_LAYER);
        }

        public override void DisableEditing()
        {
            base.DisableEditing();
            Brain.UpdateNeuronDescription(Neuron, text.GetMessage());
            SetAnchor(text.GetCenterX(), text.GetY());
            IO.RENDERER.Remove(text);
            IO.RENDERER.Add(text, NOT_EDITING_LAYER);
        }
    }
}
