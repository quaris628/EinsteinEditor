using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics
{
   /**
    * MultiRenderable interface implemented on objects that need to
    *    have multiple drawable objects rendered in the output display
    * @author Nathan Swartz
    */
   public interface MultiRenderable
   {
      /**
       * Get the Drawables, to be displayed
       * @Return IEnumerable<Drawable> iterable of drawables to be displayed
       */
      IEnumerable<Drawable> GetDrawables();
   }
}
