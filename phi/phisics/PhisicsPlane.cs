using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics;

namespace phi.phisics
{
   public class PhisicsPlane
   {

      private HashSet<PhisicsObject> objs;

      /**
       * Constructs a rectangularly bounded phisics plane
       */
      public PhisicsPlane(int originX, int originY, int width, int height)
      {

      }

      public bool AddObject(PhisicsObject o)
      {
         return objs.Add(o);
      }

      public bool RemoveObject(PhisicsObject o)
      {
         return objs.Remove(o);
      }

      public void updateObjects()
      {
         foreach(PhisicsObject o in objs)
         {
            o.update(60);
         }
      }

   }
}
