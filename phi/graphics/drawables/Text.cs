using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class Text : Drawable
   {
      private const int DEFAULT_X = 0;
      private const int DEFAULT_Y = 0;
      private const string DEFAULT_FONT_NAME = "Arial";
      private const float DEFAULT_FONT_SIZE = 14;
      private static readonly Brush DEFAULT_COLOR = Brushes.Black;

      private string message;
      private Font font;
      private Brush color;
      private Size calculatedSize;
      private bool sizeUpToDate;

      public Text(string message) : base(DEFAULT_X, DEFAULT_Y, 0, 0)
      {
         this.message = message;
         this.font = new Font(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE);
         this.color = DEFAULT_COLOR;
         this.sizeUpToDate = false;
      }

      private Text(TextBuilder builder) : base(builder.GetX(), builder.GetY(), 0, 0)
      {
         this.message = builder.GetMessage();
         this.font = builder.GetFont();
         this.color = builder.GetColor();
         this.sizeUpToDate = false;
      }

      // extend Drawable
      protected override void DrawAt(Graphics g, int x, int y)
      {
         g.DrawString(message, font, color, new PointF(x, y));
      }
      public override int GetHeight() { return GetSize().Height; }
      public override int GetWidth() { return GetSize().Width; }
      private Size GetSize()
      {
         if (!sizeUpToDate)
         {
            // Calculation may be intensive? Implemented a cache for better performance.
            calculatedSize = System.Windows.Forms.TextRenderer.MeasureText(message, font);
         }
         return calculatedSize;
      }

      public void SetMessage(string message) { this.message = message; sizeUpToDate = false; FlagChange(); }
      public string GetMessage() { return message; }
      public Font GetFont() { return font; }

      public override string ToString()
      {
         return "Text '" + message + "' " + base.ToString();
      }

      public class TextBuilder
      {
         private string message;
         private string fontName;
         private float fontSize;
         private Brush color;
         private int x;
         private int y;

         public TextBuilder(string message)
         {
            this.message = message;
            this.x = DEFAULT_X;
            this.y = DEFAULT_Y;
            this.fontName = DEFAULT_FONT_NAME;
            this.fontSize = DEFAULT_FONT_SIZE;
            this.color = DEFAULT_COLOR;
         }

         public TextBuilder WithX(int x) { this.x = x; return this; }
         public TextBuilder WithY(int y) { this.y = y; return this; }
         public TextBuilder WithXY(int x, int y) { this.x = x; this.y = y; return this; }
         public TextBuilder WithFontName(string fontName) { this.fontName = fontName; return this; }
         public TextBuilder WithFontSize(float fontSize) { this.fontSize = fontSize; return this; }
         public TextBuilder WithColor(Brush color) { this.color = color; return this; }

         public string GetMessage() { return message; }
         public int GetX() { return x; }
         public int GetY() { return y; }
         public Font GetFont() { return new Font(fontName, fontSize); }
         public Brush GetColor() { return color; }

         public Text Build() { return new Text(this); }
      }

   }
}
