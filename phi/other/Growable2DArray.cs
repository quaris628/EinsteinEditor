using System;
using System.Collections.Generic;
using System.Text;

namespace phi.other
{
   // Author: Quaris
   public class Growable2DArray<T>
   {
      private const float GROW_PROPORTION = 1.368f; // 1 + 1/e
      private const int START_MAX = 10;

      private T[,] arr;
      private int sizeX;
      private int sizeY;
      private int maxX;
      private int maxY;

      public Growable2DArray()
      {
         this.arr = new T[START_MAX, START_MAX];
         this.sizeX = 0;
         this.sizeY = 0;
         this.maxX = START_MAX;
         this.maxY = START_MAX;
      }

      public Growable2DArray(int sizeX, int sizeY)
      {
         this.maxX = Math.Max((int)(sizeX * GROW_PROPORTION), START_MAX);
         this.maxY = Math.Max((int)(sizeY * GROW_PROPORTION), START_MAX);
         this.arr = new T[maxX, maxY];
         this.sizeX = sizeX;
         this.sizeY = sizeY;
         
      }

      // Main-Purpose Functions

      // If growing O(n^2), else O(1)
      public void Set(int x, int y, T item)
      {
         GrowToFit(x, y);
         sizeX = Math.Max(sizeX, x + 1);
         sizeY = Math.Max(sizeY, y + 1);
         arr[x, y] = item;
      }

      // O(n^2)
      // Row is inserted at xIndex, with the row at xIndex + 1 containing
      //    duplicate values of the row at xIndex
      public void InsertRow(int xIndex)
      {
         if (xIndex < 0) { throw new ArgumentOutOfRangeException("xIndex must be non-negative"); }
         if (xIndex > sizeX) { throw new ArgumentOutOfRangeException("xIndex must be less than X size"); }
         GrowToFit(sizeX + 1, sizeY);
         for (int i = sizeX - 1; i >= xIndex; i--)
         {
            // copy each row to one row above
            for (int j = 0; j < sizeY; j++)
            {
               arr[i + 1, j] = arr[i, j];
            }
         }
         sizeX++;
      }

      // O(n^2)
      public void InsertColumn(int yIndex)
      {
         if (yIndex < 0) { throw new ArgumentOutOfRangeException("yIndex must be non-negative"); }
         if (yIndex > sizeY) { throw new ArgumentOutOfRangeException("yIndex must be less than Y size"); }
         GrowToFit(sizeX, sizeY + 1);
         for (int j = sizeY - 1; j >= yIndex; j--)
         {
            // copy each row to one row above
            for (int i = 0; i < sizeX; i++)
            {
               arr[i, j + 1] = arr[i, j];
            }
         }
         sizeY++;
      }

      // O(n^2)
      public void GrowToFit(int x, int y)
      {
         if (x >= maxX || y >= maxY)
         {
            int newMaxX = (int)(Math.Max(maxX, x + 1) * GROW_PROPORTION);
            int newMaxY = (int)(Math.Max(maxY, y + 1) * GROW_PROPORTION);
            
            T[,] temp = new T[newMaxX, newMaxY];

            // copy old array
            for (int i = 0; i < maxX; i++)
            {
               for (int j = 0; j < maxY; j++)
               {
                  temp[i, j] = arr[i, j];
               }
            }

            maxX = newMaxX;
            maxY = newMaxY;
            arr = temp;
         }
      }

      // Setters/Getters

      public T Get(int x, int y)
      {
         if (x >= sizeX) { throw new IndexOutOfRangeException("x is larger than the size of the array"); }
         if (y >= sizeY) { throw new IndexOutOfRangeException("y is larger than the size of the array"); }
         return arr[x, y];
      }
      public int GetSizeX() { return sizeX; }
      public int GetSizeY() { return sizeY; }


      // Utility Overrides

      // O(n^2)
      public override int GetHashCode()
      {
         int hash = base.GetHashCode();
         unchecked
         {
            for (int i = 0; i < sizeX; i++)
            {
               for (int j = 0; j < sizeX; j++)
               {
                  hash *= 7019 ^ arr[i, j].GetHashCode();
               }
            }
         }
         return hash;
      }

      public override bool Equals(object obj)
      {
         if (this == obj) { return true; }
         try
         {
            return ((Growable2DArray<T>)obj).GetHashCode() == this.GetHashCode();
         }
         catch (InvalidCastException)
         {
            return false;
         }
      }

      public override string ToString()
      {
         return "Growable2DArray[" + sizeX.ToString() + ", " + sizeY.ToString() + "]";
      }
   }
}
