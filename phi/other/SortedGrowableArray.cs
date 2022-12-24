using System;
using System.Collections.Generic;
using System.Text;

namespace phi.other
{
   public class SortedGrowableArray<T> where T : IComparable
   {
      private GrowableArray<T> ga;

      public SortedGrowableArray()
      {
         ga = new GrowableArray<T>();
      }

      public SortedGrowableArray(int size)
      {
         ga = new GrowableArray<T>(size);
      }

      // Returns the index the item was inserted at
      // If item already present in the collection, returns the index
      //    the item was found at with a negative sign
      public int Put(T item)
      {
         if (ga.GetSize() == 0)
         {
            ga.Set(0, item);
            return 0;
         }
         else
         {
            int index = BinarySearch(item);
            if (index < 0) // if item not found, as we expect
            {
               index = -index - 1;
               // shift everything at and above index up one slot
               for (int i = ga.GetSize() - 1; i >= index; i--)
               {
                  ga.Set(i + 1, ga.Get(i));
               }
               ga.Set(index, item);
               index = -index;
            }
            return index;
         }
      }

      // if value not found, returns negatively-signed index of the element just after
      // the location where the value would have been
      public int BinarySearch(T toFind)
      {
         if (ga.GetSize() == 0) { throw new InvalidOperationException("BinarySearch cannot be called on an empty array"); }
         return BinarySearch(toFind, 0, ga.GetSize() - 1);
      }

      // max and min are both inclusive
      private int BinarySearch(T toFind, int min, int max)
      {
         if (max < min)
         {
            return -min - 1; // - 1 ?
         }

         int index = (min + max) / 2;
         int comp = toFind.CompareTo(ga.Get(index));
         
         if (comp == 0)
         {
            return index;
         }
         else if (comp < 0)
         {
            return BinarySearch(toFind, min, index - 1);
         }
         else
         {
            return BinarySearch(toFind, index + 1, max);
         }
      }

      public bool Contains(T item)
      {
         return 0 <= BinarySearch(item);
      }

      public void GrowToFit(int n)
      {
         ga.GrowToFit(n);
      }

      // Setters / Getters

      public T Get(int i) { return ga.Get(i); }
      public int GetSize() { return ga.GetSize(); }


      // Utility overloads and overrides

      public static implicit operator GrowableArray<T>(SortedGrowableArray<T> sga)
      {
         return new GrowableArray<T>(sga.ga);
      }

      public override int GetHashCode()
      {
         return ga.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         if (this == obj) { return true; }
         try
         {
            return ((SortedGrowableArray<T>)obj).GetHashCode() == this.GetHashCode();
         }
         catch (InvalidCastException)
         {
            return false;
         }
      }

      public override string ToString()
      {
         return "SortedGrowableArray[" + ga.GetSize().ToString() + "]";
      }
   }
}
