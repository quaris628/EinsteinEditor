using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.renderables
{
   public class EditableText : Renderable
   {

      protected Text text;
      protected readonly string allowedChars;
      public bool IsEditingEnabled { get; protected set; }
      protected bool isInit;

      protected EditableText(EditableTextBuilder b)
      {
         this.text = b.text;
         this.allowedChars = b.allowedChars;
         this.IsEditingEnabled = b.isEditingEnabled;
      }

      public virtual void Initialize()
      {
         foreach (char c in allowedChars)
         {
            foreach (Keys key in CHAR_KEY_MAP[c])
            {
               IO.KEYS.Subscribe(() => {
                  TypeChar(c);
               }, key);
            }
         }
         IO.KEYS.Subscribe(Backspace, Keys.Back);
         isInit = true;
      }

      // warning, will unsubscribe everything from the keys this was subscribed to
      // (because it's so much easier implementation-wise to use a lambda expression when subscribing)
      public virtual void Uninitialize()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         isInit = false;
         IO.RENDERER.Remove(this);
         foreach (char c in allowedChars)
         {
            foreach (Keys key in CHAR_KEY_MAP[c])
            {
               IO.KEYS.UnsubscribeAll(key);
            }
         }
         IO.KEYS.Unsubscribe(Backspace, Keys.Back);
      }

      public virtual void TypeChar(char c)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (!IsEditingEnabled) { return; }
         string newMessage = text.GetMessage() + c; // don't forget that ;)
         if (IsMessageValidWhileTyping(newMessage)) {
            text.SetMessage(newMessage);
         }
      }

      public virtual void Backspace()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (!IsEditingEnabled || text.GetMessage().Length == 0) { return; }
         text.SetMessage(text.GetMessage().Substring(0, text.GetMessage().Length - 1));
      }

      public virtual void Clear()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (!IsEditingEnabled) { return; }
         text.SetMessage("");
      }

      protected virtual bool IsMessageValidWhileTyping(string message)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return true;
      }
      protected virtual bool IsMessageValidAsFinalInternal(string message)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return IsMessageValidWhileTyping(message);
      }
      public bool IsMessageValidAsFinal()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return IsMessageValidAsFinalInternal(text.GetMessage());
      }

      public virtual void EnableEditing()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         IsEditingEnabled = true;
      }
      public virtual void DisableEditing() {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         IsEditingEnabled = false;
      }

      public Drawable GetDrawable()
      {
         return text;
      }

      public static readonly Dictionary<char, Keys[]> CHAR_KEY_MAP = generateCharKeyMap();
      //public static readonly Dictionary<Keys, char> KEY_CHAR_MAP = generateKeyCharMap();
      private static Dictionary<char, Keys[]> generateCharKeyMap()
      {
         Dictionary<char, Keys[]> map = new Dictionary<char, Keys[]>();

         // letters (just uppercase, lowercase can be added later if really needed)
         foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
         {
            Keys key = (Keys)Enum.Parse(typeof(Keys), c.ToString());
            map.Add(c, new Keys[] { key });
         }

         // numbers
         foreach (char c in "1234567890")
         {
            Keys key1 = (Keys)Enum.Parse(typeof(Keys), "D" + c.ToString());
            Keys key2 = (Keys)Enum.Parse(typeof(Keys), "NumPad" + c.ToString());
            map.Add(c, new Keys[] { key1, key2});
         }

         // special characters
         map.Add(' ', new Keys[] { Keys.Space });
         map.Add('*', new Keys[] { Keys.Multiply });
         map.Add('+', new Keys[] { Keys.Add, Keys.Oemplus });
         map.Add('-', new Keys[] { Keys.Subtract, Keys.OemMinus });
         map.Add('.', new Keys[] { Keys.Decimal, Keys.OemPeriod });
         map.Add('/', new Keys[] { Keys.Divide });
         map.Add(';', new Keys[] { Keys.OemSemicolon });
         map.Add(',', new Keys[] { Keys.Oemcomma });
         map.Add('?', new Keys[] { Keys.OemQuestion });
         map.Add('~', new Keys[] { Keys.Oemtilde });
         map.Add('[', new Keys[] { Keys.OemOpenBrackets });
         map.Add(']', new Keys[] { Keys.OemCloseBrackets });
         map.Add('|', new Keys[] { Keys.OemPipe });
         map.Add('\'', new Keys[] { Keys.OemQuotes });
         map.Add('\\', new Keys[] { Keys.OemBackslash });

         // (not doing characters that would require a Shift + something for now)

         return map;
      }

      /*
      private static Dictionary<Keys, char> generateKeyCharMap()
      {
         Dictionary<Keys, char> map = new Dictionary<Keys, char>();

         // letters (just uppercase, lowercase can be added later if really needed)
         foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
         {
            Keys key = (Keys)Enum.Parse(typeof(Keys), c.ToString());
            map.Add(key, c);
         }

         // numbers
         foreach (char c in "1234567890")
         {
            Keys key1 = (Keys)Enum.Parse(typeof(Keys), "D" + c.ToString());
            Keys key2 = (Keys)Enum.Parse(typeof(Keys), "NumPad" + c.ToString());
            map.Add(key1, c);
            map.Add(key2, c);
         }

         // special characters
         map.Add(Keys.Space, ' ');
         map.Add(Keys.Multiply, '*');
         map.Add(Keys.Add, '+');
         map.Add(Keys.Oemplus, '+');
         map.Add(Keys.Subtract, '-');
         map.Add(Keys.OemMinus, '-');
         map.Add(Keys.Decimal, '.');
         map.Add(Keys.OemPeriod, '.');
         map.Add(Keys.Divide, '/');
         map.Add(Keys.OemSemicolon, ';');
         map.Add(Keys.Oemcomma, ',');
         map.Add(Keys.OemQuestion, '?');
         map.Add(Keys.Oemtilde, '~');
         map.Add(Keys.OemOpenBrackets, '[');
         map.Add(Keys.OemCloseBrackets, ']');
         map.Add(Keys.OemPipe, '|');
         map.Add(Keys.OemQuotes, '\'');
         map.Add(Keys.OemBackslash, '\\');

         // (not doing characters that would require a Shift + something for now)

         return map;
      }
      */

      public class EditableTextBuilder
      {
         public Text text { get; private set; }
         public string allowedChars { get; private set; }
         public bool isEditingEnabled { get; private set; }

         public EditableTextBuilder(Text text)
         {
            this.text = text;
            this.allowedChars = new string(CHAR_KEY_MAP.Keys.ToArray()); // everything
            this.isEditingEnabled = true;
         }

         public EditableTextBuilder WithAllowedChars(string allowedChars) { this.allowedChars = allowedChars; return this; }
         public EditableTextBuilder WithEditingEnabled() { this.isEditingEnabled = true; return this; }
         public EditableTextBuilder WithEditingDisabled() { this.isEditingEnabled = false; return this; }

         public virtual EditableText Build() { return new EditableText(this); }
      }

   }
}
