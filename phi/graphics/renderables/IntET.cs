using LibraryFunctionReplacements;
using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.renderables
{
    public class IntET : EditableText
    {
        public const string ALLOWED_CHARS = "-0123456789";
        public const int DEFAULT_DEFAULT_VALUE = 0;

        private int defaultValue;
        private int minValue;
        private int maxValue;

        protected IntET(IntETBuilder b) : base(b)
        {
            minValue = b.minValue;
            maxValue = b.maxValue;
        }

        protected override bool IsMessageValidWhileTyping(string message)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if ("-" == message && minValue < 0) { return true; }
            return IsMessageValidAsFinalInternal(message);
        }

        protected override bool IsMessageValidAsFinalInternal(string message)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            try
            {
                CustomNumberParser.StringToInt(message);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (ArithmeticException)
            {
                return false;
            }

            return true;
        }

        public override void DisableEditing()
        {
            base.DisableEditing();
            int value = IsMessageValidAsFinal() ? CustomNumberParser.StringToInt(text.GetMessage()) : defaultValue;
            if (value < minValue)
            {
                value = minValue;
            }
            else if (maxValue < value)
            {
                value = maxValue;
            }
            // convert to int and back to string again, so that the format is refreshed (e.g. leading and trailing zeroes are removed)
            string message = CustomNumberParser.IntToString(value);
            text.SetMessage(message);
            RecenterOnAnchor();
            onEdit?.Invoke(message);
        }

        public class IntETBuilder : EditableTextBuilder
        {
            public int defaultValue { get;  private set; }
            public int minValue { get; private set; }
            public int maxValue { get; private set; }

            public IntETBuilder(Text text) : base(text)
            {
                defaultValue = DEFAULT_DEFAULT_VALUE;
                minValue = int.MinValue;
                maxValue = int.MaxValue;
                base.WithAllowedChars(ALLOWED_CHARS);
            }

            private new IntETBuilder WithAllowedChars(string allowedChars) { return null; } // never use
            public new IntETBuilder WithEditingEnabled() { base.WithEditingEnabled(); return this; }
            public new IntETBuilder WithEditingDisabled() { base.WithEditingDisabled(); return this; }

            public virtual IntETBuilder WithDefaultValue(int defaultValue) { this.defaultValue = defaultValue; return this; }
            public virtual IntETBuilder WithMinValue(int minValue) { this.minValue = minValue; return this; }
            public virtual IntETBuilder WithMaxValue(int maxValue) { this.maxValue = maxValue; return this; }
            // mutually exclusive with having a maximum number of significant figures
            
            public new IntET Build() { return new IntET(this); }
        }

    }
}
