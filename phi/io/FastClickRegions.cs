using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace phi.other
{
   public class FastClickRegions<T>
   {
      // Warning, this class has some problems. Trust with caution.

      private Growable2DArray<LinkedList<T>> regionItems;
      private SortedGrowableArray<int> xBounds; // screen cdts
      private SortedGrowableArray<int> yBounds;
      // a coordinate corresponds to the index in regionItems such that
      //   that index in bounds holds a value larger than that coordinate
      //   and [ the index - 1 in bounds holds a value smaller than that coordinate
      //      OR the index is 0 ]
      
      public FastClickRegions()
      {
         regionItems = new Growable2DArray<LinkedList<T>>(1, 1);
         regionItems.Set(0, 0, null);
         xBounds = new SortedGrowableArray<int>();
         xBounds.Put(Int32.MaxValue);
         yBounds = new SortedGrowableArray<int>();
         yBounds.Put(Int32.MaxValue);
      }

      public FastClickRegions(int maxXCdt, int maxYCdt)
      {
         regionItems = new Growable2DArray<LinkedList<T>>(1, 1);
         regionItems.Set(0, 0, null);
         xBounds = new SortedGrowableArray<int>();
         xBounds.Put(maxXCdt);
         yBounds = new SortedGrowableArray<int>();
         yBounds.Put(maxYCdt);
      }

      /**
       * Increases array sizes to accomodate nX by nY partitions. 
       * Each item added typically introduces 2 partitions, sometimes less.
       */
      public void MakeRoomFor(int nX, int nY)
      {
         xBounds.GrowToFit(nX);
         yBounds.GrowToFit(nY);
      }

      public void Add(T item, Rectangle rect)
      {
         int xMin = InsertXPartition(rect.X);
         int xMax = InsertXPartition(rect.X + rect.Width);
         int yMin = InsertYPartition(rect.Y);
         int yMax = InsertYPartition(rect.Y + rect.Height);
         
         for (int i = xMin + 1; i <= xMax; i++)
         {
            for (int j = yMin + 1; j <= yMax; j++)
            {
               AddItemTo(i, j, item);
            }
         }
      }

      public void Remove(T item, Rectangle rect)
      {
         int[] minIndices = FindIndexes(rect.X, rect.Y);
         int[] maxIndices = FindIndexes(rect.X + rect.Width, rect.Y + rect.Height);

         for (int i = minIndices[0]; i < maxIndices[0]; i++)
         {
            for (int j = minIndices[1]; j < maxIndices[1]; j++)
            {
               LinkedList<T> items = regionItems.Get(i, j);
               if (items != null)
               {
                  items.Remove(item);
               }
            }
         }
         // TODO: need to update xBounds and yBounds?
         // i.e. check if a partition can be removed?
         // (but is that worth it for performance?
         // It seems intensive to remove, and doesn't get a large performance benefit
      }

      public IEnumerable<T> GetClickItems(int xcdt, int ycdt)
      {
         int[] indices = FindIndexes(xcdt, ycdt);
         try
         {
            return regionItems.Get(indices[0], indices[1]);
         }
         catch (IndexOutOfRangeException e)
         {
            return null;
         }
      }

      private int[] FindIndexes(int xcdt, int ycdt)
      {
         int x = xBounds.BinarySearch(xcdt);
         int y = yBounds.BinarySearch(ycdt);
         if (x < 0) { x = -x - 1; }
         if (y < 0) { y = -y - 1; }
         return new int[] { x, y };
      }

      private void AddItemTo(int xIndex, int yIndex, T item)
      {
         if (regionItems.Get(xIndex, yIndex) == null)
         {
            regionItems.Set(xIndex, yIndex, new LinkedList<T>());
         }
         regionItems.Get(xIndex, yIndex).AddLast(item);
      }

      // Returns index above the partition
      // If partition already exists, does nothing, and returns index of existing partition
      private int InsertXPartition(int cdt)
      {
         int index = xBounds.Put(cdt);
         if (index < 0)
         {
            index = -index;
            regionItems.InsertRow(index);
         }
         return index;
      }

      // Returns index above the partition
      // If partition already exists, does nothing, and returns index of existing partition
      private int InsertYPartition(int cdt)
      {
         int index = yBounds.Put(cdt);
         if (index < 0)
         {
            index = -index;
            regionItems.InsertColumn(index);
         }
         return index;
      }

   }
}
