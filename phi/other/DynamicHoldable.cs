using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.other
{
   /**
    * A thing that can be put in 0..* dynamic container(s).
    * 
    * The holdable should call FlagChange() on its container
    *    whenever the holdable is considered to have changed.
    *    (What constitutes a change is entirely at the holdable's discretion.)
    * @Author Nathan Swartz
    */
   public class DynamicHoldable
   {
      protected LinkedList<DynamicContainer> containers;

      public DynamicHoldable()
      {
         this.containers = new LinkedList<DynamicContainer>();
      }

      public virtual void PutIn(DynamicContainer container)
      {
         this.containers.AddLast(container);
      }

      public virtual void TakeOut(DynamicContainer container)
      {
         this.containers.Remove(container);
      }

      protected void FlagChange()
      {
         foreach (DynamicContainer container in containers)
         {
            container.FlagChange();
         }
      }

      public override int GetHashCode()
      {
         return containers.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         if (!base.Equals(obj)) { return false; }
         if (this.GetType() != obj.GetType()) { return false; }
         return this.containers.GetHashCode() == ((DynamicHoldable)obj).containers.GetHashCode();
      }
      
   }
}
