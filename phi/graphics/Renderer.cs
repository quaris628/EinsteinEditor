using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.other;
using phi.control;

namespace phi.graphics
{
   public class Renderer : DynamicContainer
   {
      // default layer before any default (for overload Add method) is set
      private const int DEFAULT_DEFAULT_LAYER = 0;

      private Image output;
      private Image background;
      private SortedList<int, LinkedList<Drawable>> layers;
      private int defaultLayer;

      private bool isCentered;
      private Drawable centerDrawable; //used to center the camera on a specific drawable
      private int center_X_Displacement; //How close to the edge the center gets before the screen displaces
      private int center_Y_Displacement; //How close to the top and bottom edges before the screen displaces
      private int displacementX;
      private int displacementY;

      public Renderer(Image output, Image background)
      {
         this.output = output;
         this.background = background;
         this.layers = new SortedList<int, LinkedList<Drawable>>();
         this.defaultLayer = DEFAULT_DEFAULT_LAYER;

         this.isCentered = false;
         this.centerDrawable = null;
         this.center_X_Displacement = 0;
         this.center_Y_Displacement = 0;
         this.displacementX = 0;
         this.displacementY = 0;
      }

      public Renderer()
      {
         this.output = null;
         this.background = null;
         this.layers = new SortedList<int, LinkedList<Drawable>>();
         this.defaultLayer = DEFAULT_DEFAULT_LAYER;

         this.isCentered = false;
         this.centerDrawable = null;
         this.center_X_Displacement = 0;
         this.center_Y_Displacement = 0;
         this.displacementX = 0;
         this.displacementY = 0;
      }

      /**
       * Combines all images in the queue onto the output image
       * Starts with a clean background each time
       * Images appear stacked on top of each other, with the last added to the
       *    queue on top
       * Sprites are rendered first, text is always rendered last
       * 
       * (Note by Nathan, this method could be optimized (?*) to distribute
       *   the load of drawing each sprite in the queue to instead be done
       *   whenever a sprite is added. *Wouldn't change the total time needed.)
       * @author Nathan Swartz, Benjamin Lippincott
       */
      public void Render()
      {
         if (HasChanged())
         {
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(background, 0, 0);
            CalculateDisplacement();

            foreach (LinkedList<Drawable> drawables in layers.Values)
            {
               foreach (Drawable drawable in drawables)
               {
                  if (drawable.IsDisplaying())
                  {
                     drawable.DrawOffset(g, displacementX, displacementY);
                  }
               }
            }

            g.Dispose();
            UnflagChanges();
         }
      }

      public void Add(Drawable item, int layer)
      {
         if (!layers.ContainsKey(layer))
         {
            // create new sub-list for a new layer
            layers.Add(layer, new LinkedList<Drawable>());
         }
         layers[layer].AddFirst(item);
         item.PutIn(this);
         FlagChange();
      }
      public void Add(IEnumerable<Drawable> items, int layer)
      {
         if (!layers.ContainsKey(layer))
         {
            // create new sub-list for a new layer
            layers.Add(layer, new LinkedList<Drawable>());
         }
         foreach (Drawable item in items)
         {
            layers[layer].AddFirst(item);
            item.PutIn(this);
         }
         FlagChange();
      }
      
      // renderable overloads
      public void Add(Renderable r, int layer) { Add(r, layer); }
      public void Add(MultiRenderable mr, int layer) { Add(mr, layer); }
      public void Add(Renderable r) { Add(r.GetDrawable()); }
      public void Add(MultiRenderable mr) { Add(mr.GetDrawables()); }

      // default layer overloads & etc
      public void Add(Drawable item) { Add(item, defaultLayer); }
      public void Add(IEnumerable<Drawable> items) { Add(items, defaultLayer); }
      public void SetDefaultLayer(int defaultLayer) { this.defaultLayer = defaultLayer; }
      public int GetDefaultLayer() { return defaultLayer; }

      public bool Remove(Drawable item)
      {
         bool success = false;
         // for each layer, or stop if item successfully found
         IEnumerator<KeyValuePair<int, LinkedList<Drawable>>> enumerator = layers.GetEnumerator();
         while (enumerator.MoveNext() && !success)
         {
            LinkedList<Drawable> drawables = enumerator.Current.Value;

            success = drawables.Remove(item);
         }

         if (success)
         {
            item.TakeOut(this);
            FlagChange();
         }
         return success;
      }

      public void ClearLayer(int layer) { layers.Remove(layer); FlagChange(); }
      
      public void Clear() { layers.Clear(); RemoveCenter(); FlagChange(); }

      /**
        * Sets the image to draw images on top of
        * Is also the output (pass by reference)
        */
      public void SetOutput(Image outputImage) { this.output = outputImage; FlagChange(); }

      /**
       * Sets the background that all drawn images are stacked on top of
       */
      public void SetBackground(Image background)
      {
         this.background = background ?? throw new ArgumentNullException();
         FlagChange();
      }

      public Image GetBackground()
      {
         return this.background;
      }

      /**
       * Attempts to center the camera on the sprite
       */
      public void CenterCamera(Drawable s)
      {
         isCentered = true;
         centerDrawable = s;
         this.center_X_Displacement = output.Width / 2 - s.GetWidth();
         this.center_Y_Displacement = output.Height / 2 - s.GetHeight();
         FlagChange();
      }

      /**
       * Sets Sprite s the center of the camera, and displaces the camera when the sprite gets within x of the width and y within the height
       */
      public void BorderCamera(Drawable s, int x, int y)
      {
         isCentered = true;
         centerDrawable = s;
         this.center_X_Displacement = x;
         this.center_Y_Displacement = y;
         FlagChange();
      }

      public void RemoveCenter()
      {
         centerDrawable = null;
         isCentered = false;
         FlagChange();
      }

      private void CalculateDisplacement()
      {
         if (isCentered)
         {
            displacementX += centerDrawable.GetWidth() * IsDisplacedX();
            displacementY += centerDrawable.GetHeight() * isDisplacedY();
         }
         else
         {
            displacementX = 0;
            displacementY = 0;
         }
      }

      /**
       * Returns 1 if the center sprite is getting too close to the right edge,
       * Returns -1 if the center sprite is getting too close to the left edge
       * Returns 0 if the center sprite is not near any edge
       */
      private int IsDisplacedX()
      {
         if (centerDrawable.GetX() + centerDrawable.GetWidth() + displacementX + center_X_Displacement > output.Width)
         {
            return -1;
         }
         else if (centerDrawable.GetX() + displacementX < center_X_Displacement && displacementX < 0)
         {
            return 1;
         }
         else
            return 0;

      }

      private int isDisplacedY()
      {
         if (centerDrawable.GetY() + centerDrawable.GetHeight() + displacementY + center_Y_Displacement > output.Height)
         {
            return -1;
         }
         else if (centerDrawable.GetY() + displacementY < center_Y_Displacement && displacementY < 0)
         {
            return 1;
         }
         else
            return 0;

      }
   }
}
