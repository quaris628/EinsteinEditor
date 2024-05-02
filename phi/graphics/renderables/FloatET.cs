using LibraryFunctionReplacements;
using phi.graphics.drawables;
using phi.other;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phi.graphics.renderables
{
   public class FloatET : EditableText
   {
      public const string ALLOWED_CHARS = "-.,0123456789";
      public const float DEFAULT_DEFAULT_VALUE = 0f;

      private float defaultValue;
      private float minValue;
      private float maxValue;
      private PrecisionType precisionType; // 0 = none, 1 = decimal places, 2 = sig figs
      private int precisionAmount; // num of decimal places or sig figs

      protected FloatET(FloatETBuilder b) : base(b)
      {
         minValue = b.minValue;
         maxValue = b.maxValue;
         precisionType = b.precisionType;
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
         try
         {
            floatValue = CustomNumberParser.StringToFloat(message);
         }
         catch (ArgumentException)
         {
            return false;
         }
         catch (ArithmeticException)
         {
            return false;
         }
         if (floatValue < minValue || floatValue > maxValue) {
            return false;
         }

         return isWithinPrecision(message);
      }

      public override void DisableEditing()
      {
         base.DisableEditing();
         float value = IsMessageValidAsFinal() ? CustomNumberParser.StringToFloat(text.GetMessage()) : defaultValue;
         // convert to float and back to string again, so that the format is refreshed (e.g. leading and trailing zeroes are removed)
         int maxSigFigs = precisionType == PrecisionType.SignificantFigures ? precisionAmount : 8;
         int maxDecimals = precisionType == PrecisionType.DecimalPlaces ? precisionAmount : int.MaxValue;
         text.SetMessage(CustomNumberParser.FloatToString(value, maxSigFigs, maxDecimals));
        }

      private bool isWithinPrecision(string message)
      {
         switch (precisionType)
         {
            case PrecisionType.DecimalPlaces:
               return isWithinPrecisionDecimalPlaces(message);
            case PrecisionType.SignificantFigures:
               return isWithinPrecisionSignificantFigures(message);
            default:
               return true;
         }
      }

      private bool isWithinPrecisionDecimalPlaces(string message)
      {
         // find decmal point index
         int decimalIndex = Math.Max(message.IndexOf(","), message.IndexOf("."));
         if (decimalIndex == -1) { return true; }
         // count all digits after the decimal point
         int decimalPlacesCount = message.Length - decimalIndex - 1;
         return decimalPlacesCount <= precisionAmount;
      }

      private bool isWithinPrecisionSignificantFigures(string message)
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
         if (firstNonzeroDigit == '$')
         {
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
      
      // Wrapper for EditableTextBuilder that just hides the WithValidateMessage option
      public class FloatETBuilder : EditableTextBuilder
      {
         public float defaultValue { get;  private set; }
         public float minValue { get; private set; }
         public float maxValue { get; private set; }
         internal PrecisionType precisionType { get; private set; }
         public int precisionAmount { get; private set; }

         public FloatETBuilder(Text text) : base(text)
         {
            defaultValue = DEFAULT_DEFAULT_VALUE;
            minValue = float.MinValue;
            maxValue = float.MaxValue;
            precisionType = PrecisionType.None;
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
            precisionType = PrecisionType.DecimalPlaces;
            precisionAmount = numDecimals;
            return this;
         }
         public virtual FloatETBuilder WithMaxSigFigs(int numSigFigs)
         {
            precisionType = PrecisionType.SignificantFigures;
            precisionAmount = numSigFigs;
            return this;
         }

         public new FloatET Build() { return new FloatET(this); }
      }

      internal enum PrecisionType
      {
         None = 0,
         DecimalPlaces = 1,
         SignificantFigures = 2,
      }
   }
}
