using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class Sprite : Drawable
   {
      private Image image;

      public Sprite(ImageWrapper img, int x, int y) : base(x, y, img.GetImage().Width, img.GetImage().Height)
      {
         this.image = img.GetImage() ?? throw new ArgumentNullException();
      }

      // extend Drawable
      protected override void DrawAt(Graphics g, int x, int y)
      {
         g.DrawImage(image, x, y);
      }

      public Image GetImage() { return image; }
      public void SetImage(Image image)
      {
         this.image = image;
         this.height = image.Height;
         this.width = image.Width;
         FlagChange();
      }

      public Sprite FlipX()
      {
         image.RotateFlip(RotateFlipType.Rotate180FlipNone);
         FlagChange();
         return this;
      }

      public Sprite FlipY()
      {
         image.RotateFlip(RotateFlipType.RotateNoneFlipY);
         FlagChange();
         return this;
      }

      public Sprite RotateLeft()
      {
         image.RotateFlip(RotateFlipType.Rotate270FlipNone);
         FlagChange();
         return this;
      }

      public Sprite RotateRight()
      {
         image.RotateFlip(RotateFlipType.Rotate90FlipNone);
         FlagChange();
         return this;
      }

      public override int GetHashCode()
      {
         unchecked // allow arithmetic overflow
         {
            int result = 1046527;
            result *= 106033 ^ base.GetHashCode();
            result *= 106033 ^ image.GetHashCode();
            return result;
         }
      }

      public override string ToString()
      {
         return "Sprite " + base.ToString();
      }

   }
}
