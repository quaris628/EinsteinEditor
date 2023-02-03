using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.renderables
{
   public class FloatET : EditableText
   {
      public const string ALLOWED_CHARS = "-.,0123456789";
      public const float DEFAULT_DEFAULT_VALUE = 0f;

      private float defaultValue;
      private float minValue;
      private float maxValue;
      private int precisionTypeCode; // 0 = none, 1 = decimal places, 2 = sig figs
      private int precisionAmount; // num of decimal places or sig figs

      protected FloatET(FloatETBuilder b) : base(b)
      {
         minValue = b.minValue;
         maxValue = b.maxValue;
         precisionTypeCode = b.precisionTypeCode;
         precisionAmount = b.precisionAmount;
      }

      protected override bool IsMessageValidWhileTyping(string message)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if ("-.".Contains(message) || "-,".Contains(message)) { return true; }
         return IsMessageValidAsFinalInternal(message);
      }

      protected override bool IsMessageValidAsFinalInternal(string message)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         float floatValue;
         if (!float.TryParse(message, NumberStyles.Any, CultureInfo.InvariantCulture,
            out floatValue)) { return false; }
         if (floatValue < minValue) { return false; }
         if (floatValue > maxValue) { return false; }

         if (precisionTypeCode == 1) // decimal places
         {
            // find decmal point index
            int decimalIndex = Math.Max(message.IndexOf(","), message.IndexOf("."));
            if (decimalIndex == -1) { return true; }
            // count all digits after the decimal point
            int decimalPlacesCount = message.Length - decimalIndex - 1;
            return decimalPlacesCount <= precisionAmount;
         }
         else if (precisionTypeCode == 2) // significant figures
         {
            // find first nonzero digit
            char firstNonzeroDigit = '$';
            foreach (char c in message)
            {
               if ("123456789".Contains(c))
               {
                  firstNonzeroDigit = c;
                  break;
               }
            }
            int startCountIndex = firstNonzeroDigit;
            // if there are no nonzero digits, i.e. 0.00000000000...
            if (firstNonzeroDigit == '$') {
               // start counting sig figs at first zero after decimal point
               startCountIndex = Math.Max(message.IndexOf(","), message.IndexOf("."));
            }

            int count = 0;
            foreach (char c in message.Substring(startCountIndex))
            {
               if (!(",.".Contains(c)))
               {
                  count++;
               }
            }

            return count <= precisionAmount;
         }
         return true;
      }

      public override void DisableEditing()
      {
         base.DisableEditing();
         if (IsMessageValidAsFinal())
         {
            text.SetMessage(float.Parse(text.GetMessage(),
             NumberStyles.Any, CultureInfo.InvariantCulture).ToString());
         }
         else
         {
            text.SetMessage(defaultValue.ToString());
         }
      }

      // Wrapper for EditableTextBuilder that just hides the WithValidateMessage option
      public class FloatETBuilder : EditableTextBuilder
      {
         internal float defaultValue;
         internal float minValue;
         internal float maxValue;
         internal int precisionTypeCode; // 0 = none, 1 = decimal places, 2 = sig figs
         internal int precisionAmount; // num of decimal places or sig figs

         public FloatETBuilder(Text text) : base(text)
         {
            defaultValue = DEFAULT_DEFAULT_VALUE;
            minValue = float.MinValue;
            maxValue = float.MaxValue;
            precisionTypeCode = 0;
            base.WithAllowedChars(ALLOWED_CHARS);
         }

         private new FloatETBuilder WithAllowedChars(string allowedChars) { return null; } // never use
         public new FloatETBuilder WithEditingEnabled() { base.WithEditingEnabled(); return this; }
         public new FloatETBuilder WithEditingDisabled() { base.WithEditingDisabled(); return this; }

         public virtual FloatETBuilder WithDefaultValue(float defaultValue) { this.defaultValue = defaultValue; return this; }
         public virtual FloatETBuilder WithMinValue(float minValue) { this.minValue = minValue; return this; }
         public virtual FloatETBuilder WithMaxValue(float maxValue) { this.maxValue = maxValue; return this; }
         // mutually exclusive with having a maximum number of significant figures
         public virtual FloatETBuilder WithMaxDecimalPlaces(int numDecimals)
         {
            precisionTypeCode = 1;
            precisionAmount = numDecimals;
            return this;
         }
         public virtual FloatETBuilder WithMaxSigFigs(int numSigFigs)
         {
            precisionTypeCode = 2;
            precisionAmount = numSigFigs;
            return this;
         }

         public new FloatET Build() { return new FloatET(this); }
      }
   }
}
