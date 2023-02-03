using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.renderables
{
   public class SelectableEditableText : Renderable
   {
      private static readonly Color DEFAULT_SELECTED_BACKGROUND_COLOR = Color.CornflowerBlue;
      private static readonly Color DEFAULT_UNSELECTED_BACKGROUND_COLOR = Color.White;
      private const string DEFAULT_DEFAULT_MESSAGE = "";

      private static SelectableEditableText selected = null;

      public EditableText EditableText { get; private set; }
      private Text text; // just a shortcut reference
      private Brush selectedBackColor;
      private Brush unselectedBackColor;
      private string defaultMessage;
      protected bool isInit;

      public SelectableEditableText(EditableText et)
         : this(et, DEFAULT_DEFAULT_MESSAGE) { }
      public SelectableEditableText(EditableText et, string defaultMessage)
         : this(et, defaultMessage,
              DEFAULT_SELECTED_BACKGROUND_COLOR,
              DEFAULT_UNSELECTED_BACKGROUND_COLOR) { }
      public SelectableEditableText(EditableText et, Color selectedBackColor, Color unselectedBackColor)
         : this(et, DEFAULT_DEFAULT_MESSAGE, selectedBackColor, unselectedBackColor) { }
      public SelectableEditableText(EditableText et, string defaultMessage,
         Color selectedBackColor, Color unselectedBackColor)
      {
         this.EditableText = et;
         this.text = (Text)et.GetDrawable();
         this.defaultMessage = defaultMessage;
         this.selectedBackColor = new SolidBrush(selectedBackColor);
         this.unselectedBackColor = new SolidBrush(unselectedBackColor);
      }

      public void Initialize()
      {
         EditableText.Initialize();

         this.text.SetBackgroundColor(this.unselectedBackColor);

         IO.MOUSE.LEFT_UP.SubscribeOnDrawable(Select, text);
         isInit = true;
      }

      public void Uninitialize()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         isInit = false;
         EditableText.Uninitialize();
         
         if (selected == this)
         {
            selected = null;
            IO.MOUSE.LEFT_DOWN.Unsubscribe(Deselect);
            IO.KEYS.Unsubscribe(Deselect, Keys.Return);
         }
         else
         {
            IO.MOUSE.LEFT_UP.UnsubscribeFromDrawable(Select, text);
         }
      }

      public void Select()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         // note: not thread-safe
         if (IsSelected()) { throw new InvalidOperationException(); }
         selected?.Deselect();
         selected = this;

         EditableText.EnableEditing();
         text.SetBackgroundColor(selectedBackColor);
         
         IO.MOUSE.LEFT_DOWN.Subscribe(Deselect);
         IO.KEYS.Subscribe(Deselect, Keys.Return);
         IO.MOUSE.LEFT_UP.UnsubscribeFromDrawable(Select, text);
      }

      public void Deselect()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         // note: not thread-safe
         if (!IsSelected()) { throw new InvalidOperationException(); }
         selected = null;
         
         EditableText.DisableEditing();
         text.SetBackgroundColor(unselectedBackColor);

         if (!EditableText.IsMessageValidAsFinal())
         {
            text.SetMessage(defaultMessage);
         }
         
         IO.MOUSE.LEFT_DOWN.Unsubscribe(Deselect);
         IO.KEYS.Unsubscribe(Deselect, Keys.Return);
         IO.MOUSE.LEFT_UP.SubscribeOnDrawable(Select, text);
      }

      public bool IsSelected()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return selected == this;
      }

      public Drawable GetDrawable()
      {
         return EditableText.GetDrawable();
      }
   }
}
