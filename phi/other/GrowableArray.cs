using System;
using System.Collections.Generic;
using System.Text;

namespace phi.other
{
   // Author: Quaris
   public class GrowableArray<T>
   {
      private const float GROW_PROPORTION = 1.368f; // approx 1 + 1/e
      private const int START_MAX = 10;

      private T[] arr;
      private int size;
      private int max;

      public GrowableArray()
      {
         this.max = START_MAX;
         this.size = 0;
         this.arr = new T[max];         
      }

      public GrowableArray(int size)
      {
         this.max = Math.Max(START_MAX, (int)(size * GROW_PROPORTION));
         this.size = size;
         this.arr = new T[max];
      }

      // copy constructor
      public GrowableArray(GrowableArray<T> ga)
      {
         this.max = Math.Max(START_MAX, (int)(ga.size * GROW_PROPORTION));
         this.size = ga.size;
         this.arr = new T[max];
         // copy array
         for (int i = 0; i < ga.size; i++)
         {
            arr[i] = ga.arr[i];
         }
      }

      public void GrowToFit(int i)
      {
         if (i >= max)
         {
            int newMax = (int)(Math.Max(max, i + 1) * GROW_PROPORTION);

            // copy old array to new array
            T[] temp = new T[newMax];
            for (int j = 0; j < max; j++)
            {
               temp[j] = arr[j];
            }
            
            max = newMax;
            arr = temp;
         }
      }


      // Setters / Getters

      public void Set(int i, T item)
      {
         GrowToFit(i);
         size = Math.Max(size, i + 1);
         arr[i] = item;
      }

      public T Get(int i)
      {
         if (i >= size) { throw new IndexOutOfRangeException("index must be less than size"); }
         return arr[i];
      }

      public int GetSize() { return size; }


      // Utility Overrides

      public override int GetHashCode()
      {
         int hash = base.GetHashCode();
         unchecked
         {
            for (int j = 0; j < size; j++)
            {
               hash *= 7019 ^ arr[j].GetHashCode();
            }
         }
         return hash;
      }

      public override bool Equals(object obj)
      {
         if (this == obj) { return true; }
         try
         {
            return ((GrowableArray<T>)obj).GetHashCode() == this.GetHashCode();
         }
         catch (InvalidCastException)
         {
            return false;
         }
      }

      public override string ToString()
      {
         return "GrowableArray[" + size.ToString() + "]";
      }
   }
}
