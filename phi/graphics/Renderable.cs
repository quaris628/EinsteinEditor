using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics
{
   /**
    * Renderable interface implemented on objects that need to
    *    have a drawable object rendered in the output display
    * @author Benjamin Lippincott (and Nathan Swartz)
    */
   public interface Renderable
   {
      /**
       * Get the Drawable, to be displayed
       * @Return Drawable to be displayed
       */
      Drawable GetDrawable();
   }
}
