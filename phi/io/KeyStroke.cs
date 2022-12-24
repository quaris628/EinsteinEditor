using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phi.io
{
   public class KeyStroke
   {
      private int code;
      private int keys;
      private int mods;

      public KeyStroke(Keys keys)
      {
         this.code = (int)keys;
         this.keys = (int)keys & (int)Keys.KeyCode;
         mods = this.keys & (int)Keys.Modifiers;
      }
      public KeyStroke(int keycode)
      {
         this.code = keycode;
         this.keys = keycode & (int)Keys.KeyCode;
         mods = keycode & (int)Keys.Modifiers;
      }
      public KeyStroke(System.Windows.Forms.Keys keys)
      {
         this.code = (int)keys;
         this.keys = (int)keys & (int)Keys.KeyCode;
         mods = this.keys & (int)Keys.Modifiers;
      }

      public int GetCode()
      {
         return code;
      }
      public Keys GetMainKey()
      {
         return (Keys)keys;
      }
      public bool Contains(Keys modifier) { return Contains((int)modifier); }
      public bool Contains(int modifier)
      {
         return (mods & modifier) != 0;

         /*
         KeyCode = 65535,  // 0x0ffff
         Shift = 65536,    // 0x10000
         Control = 131072, // 0x20000
         Alt = 262144      // 0x40000
         */
      }
      public bool ContainsNoModifiers()
      {
         return mods == 0;
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int reversed = (mods >> 16) & (keys << 16);
            return 13 ^ reversed;
         }
      }
      public override bool Equals(object obj)
      {
         if (this == obj) { return true; }
         if (!(obj is KeyStroke)) { return false; }
         KeyStroke o = (KeyStroke)obj;
         return this.GetCode() == o.GetCode();
      }

      public class KeyStrokeBuilder
      {
         int code;
         public KeyStrokeBuilder(int keycode)
         {
            this.code = keycode;
         }
         public KeyStrokeBuilder(Keys key)
         {
            this.code = (int)key;
         }
         public KeyStrokeBuilder with(Keys modifier)
         {
            this.code &= (int)modifier;
            return this;
         }
         public KeyStroke Build() { return new KeyStroke(code); }
      }

      public static System.Windows.Forms.Keys GetWindowsKeys(Keys key) { return (System.Windows.Forms.Keys)(int)key; }
      public static Keys GetWrapperKeys(System.Windows.Forms.Keys key) { return (Keys)(int)key; }

   }
}
