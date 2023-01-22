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
      private static readonly Brush DEFAULT_BACKGROUND_COLOR = null;
      private const int DEFAULT_MIN_WIDTH = 10;
      private const int DEFAULT_MIN_HEIGHT = 22;

      private string message;
      private Font font;
      private Brush color;
      private Brush backgroundColor;
      private Size calculatedSize;
      private bool sizeUpToDate;
      private int minWidth;
      private int minHeight;

      public Text(string message) : this(new TextBuilder(message)) { }

      protected Text(TextBuilder builder) : base(builder.GetX(), builder.GetY(), 0, 0)
      {
         this.message = builder.GetMessage();
         this.font = builder.GetFont();
         this.color = builder.GetColor();
         this.backgroundColor = builder.GetBackgroundColor();
         this.sizeUpToDate = false;
         this.minWidth = builder.minWidth;
         this.minHeight = builder.minHeight;
      }

      // extend Drawable
      protected override void DrawAt(Graphics g, int x, int y)
      {
         if (backgroundColor != null)
         {
            g.FillRectangle(backgroundColor, x, y, GetWidth(), GetHeight());
         }
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
            calculatedSize.Width = Math.Max(calculatedSize.Width, minWidth);
            calculatedSize.Height = Math.Max(calculatedSize.Height, minHeight);
            sizeUpToDate = true;
         }
         return calculatedSize;
      }

      public void SetMessage(string message) { this.message = message; sizeUpToDate = false; FlagChange(); }
      public string GetMessage() { return message; }
      public Font GetFont() { return font; }
      public void SetBackgroundColor(Brush backgroundColor) { this.backgroundColor = backgroundColor; FlagChange(); }
      public void ClearBackground() { this.backgroundColor = null; FlagChange(); }

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
         private Brush backgroundColor;
         private int x;
         private int y;
         public int minWidth { get; private set; }
         public int minHeight { get; private set; }
         private bool wasMinHeightManuallySet;

         public TextBuilder(string message)
         {
            this.message = message;
            this.x = DEFAULT_X;
            this.y = DEFAULT_Y;
            this.fontName = DEFAULT_FONT_NAME;
            this.fontSize = DEFAULT_FONT_SIZE;
            this.color = DEFAULT_COLOR;
            this.backgroundColor = DEFAULT_BACKGROUND_COLOR;
            this.minWidth = DEFAULT_MIN_WIDTH;
            this.minHeight = DEFAULT_MIN_HEIGHT;
            this.wasMinHeightManuallySet = false;
         }

         public TextBuilder WithX(int x) { this.x = x; return this; }
         public TextBuilder WithY(int y) { this.y = y; return this; }
         public TextBuilder WithXY(int x, int y) { this.x = x; this.y = y; return this; }
         public TextBuilder WithFontName(string fontName) { this.fontName = fontName; return this; }
         public TextBuilder WithFontSize(float fontSize)
         {
            this.fontSize = fontSize;
            if (!wasMinHeightManuallySet)
            {
               minHeight = (int)(fontSize + 0.5f);
            }
            return this;
         }
         public TextBuilder WithColor(Brush color) { this.color = color; return this; }
         public TextBuilder WithBackgroundColor(Brush backgroundColor) { this.backgroundColor = backgroundColor; return this; }
         public TextBuilder WithMinWidth(int minWidth) { this.minWidth = minWidth; return this; }
         public TextBuilder WithMinHeight(int minHeight) { this.minHeight = minHeight; wasMinHeightManuallySet = true; return this; }

         public string GetMessage() { return message; }
         public int GetX() { return x; }
         public int GetY() { return y; }
         public Font GetFont() { return new Font(fontName, fontSize); }
         public Brush GetColor() { return color; }
         public Brush GetBackgroundColor() { return backgroundColor; }

         public Text Build() { return new Text(this); }
      }

   }
}
