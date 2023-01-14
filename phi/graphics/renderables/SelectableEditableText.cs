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
   public class SelectableEditableText : EditableText
   {
      private static readonly Color DEFAULT_SELECTED_BACKGROUND_COLOR = Color.CornflowerBlue;
      private static readonly Color DEFAULT_UNSELECTED_BACKGROUND_COLOR = Color.White;
      private const string DEFAULT_DEFAULT_MESSAGE = "";

      private static SelectableEditableText selected = null; // TODO

      private Brush selectedBackgroundColor;
      private Brush unselectedBackgroundColor;
      private string defaultMessage;

      public SelectableEditableText(SETextBuilder b) : base(b)
      {
         this.selectedBackgroundColor = b.selectedBackColor;
         this.unselectedBackgroundColor = b.unselectedBackColor;
         this.defaultMessage = b.defaultMessage;
         text.SetBackgroundColor(this.unselectedBackgroundColor);
      }

      public override void Initialize()
      {
         base.Initialize();
         IO.MOUSE.LEFT_UP.SubscribeOnDrawable(EnableEditing, text);
      }

      public override void Uninitialize()
      {
         base.Uninitialize();
         IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(text);
      }

      public override void EnableEditing()
      {
         base.EnableEditing();
         selected?.DisableEditing(); // note: not thread-safe
         selected = this;

         text.SetBackgroundColor(selectedBackgroundColor);

         IO.MOUSE.LEFT_UP.UnsubscribeFromDrawable(EnableEditing, text);
         IO.MOUSE.LEFT_DOWN.Subscribe(DisableEditing);
         IO.KEYS.Subscribe(DisableEditing, Keys.Return);
      }

      public override void DisableEditing()
      {
         base.DisableEditing();
         selected = null;

         if (text.GetMessage() == "" || !validateMessage.Invoke(text.GetMessage()))
         {
            text.SetMessage(defaultMessage);
         }
         text.SetBackgroundColor(unselectedBackgroundColor);

         IO.MOUSE.LEFT_DOWN.Unsubscribe(DisableEditing); // TODO: does this work?
         IO.KEYS.Unsubscribe(DisableEditing, Keys.Return);
         IO.MOUSE.LEFT_UP.SubscribeOnDrawable(EnableEditing, text);
      }

      public class SETextBuilder : EditableTextBuilder
      {
         public Brush selectedBackColor { get; private set; }
         public Brush unselectedBackColor { get; private set; }
         public string defaultMessage { get; private set; }

         public SETextBuilder(Text text) : base(text)
         {
            this.selectedBackColor = new SolidBrush(DEFAULT_SELECTED_BACKGROUND_COLOR);
            this.unselectedBackColor = new SolidBrush(DEFAULT_UNSELECTED_BACKGROUND_COLOR);
            this.defaultMessage = DEFAULT_DEFAULT_MESSAGE;
         }

         public SETextBuilder WithSelectedBackColor(Brush selectedBackColor) { this.selectedBackColor = selectedBackColor; return this; }
         public SETextBuilder WithUnselectedBackColor(Brush unselectedBackColor) { this.unselectedBackColor = unselectedBackColor; return this; }
         public SETextBuilder WithDefaultMessage(string defaultMessage) { this.defaultMessage = defaultMessage; return this; }

         public override EditableText Build() { return new SelectableEditableText(this); }

      }
   }
}
