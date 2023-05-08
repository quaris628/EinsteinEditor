using phi.graphics;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class SelectableButton : Drawable
   {
      private Button unselectedButton;
      private Button selectedButton;
      private ICollection<Action> onSelect;
      private ICollection<Action> onDeselect;
      private bool selected;
      private bool isInit;

      public SelectableButton(Button.ButtonBuilder unselectedButton,
         Button.ButtonBuilder selectedButton)
         : base(unselectedButton.Build().GetBoundaryRectangle()) {
         onSelect = new List<Action>();
         onDeselect = new List<Action>();
         if (unselectedButton.GetOnClick() != null)
         {
            onSelect.Add(unselectedButton.GetOnClick());
         }
         if (selectedButton.GetOnClick() != null)
         {
            onDeselect.Add(selectedButton.GetOnClick());
         }
         unselectedButton.withOnClick(Select);
         selectedButton.withOnClick(Deselect);
         this.unselectedButton = unselectedButton.Build();
         this.selectedButton = selectedButton.Build();
         this.selectedButton.SetDisplaying(false);
         selected = false;
      }

      public virtual void Initialize()
      {
         unselectedButton.Initialize();
         selectedButton.Initialize();
         isInit = true;
      }

      public virtual void Uninitialize()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         isInit = false;
         unselectedButton.Uninitialize();
         unselectedButton.Uninitialize();
      }

      public virtual void Select()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         selectedButton.SetDisplaying(true);
         unselectedButton.SetDisplaying(false);
         selected = true;
         FlagChange();
         foreach (Action action in onSelect)
         {
               action.Invoke();
         }
      }
      public virtual void Deselect()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         selectedButton.SetDisplaying(false);
         unselectedButton.SetDisplaying(true);
         selected = false;
         FlagChange();
         foreach (Action action in onDeselect)
         {
               action.Invoke();
         }
      }

      public void SubscribeOnSelect(Action action)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         onSelect.Add(action);
      }

      public void UnsubscribeFromSelect(Action action)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         onSelect.Remove(action);
      }

      public void SubscribeOnDeselect(Action action)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         onDeselect.Add(action);
      }

      public void UnsubscribeFromDeselect(Action action)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         onDeselect.Remove(action);
      }

      public bool IsSelected()
      {
         return selected;
      }

      protected override void DrawAt(Graphics g, int x, int y)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (selected)
         {
               selectedButton.SetXY(x, y);
               selectedButton.Draw(g);
         }
         else
         {
               unselectedButton.SetXY(x, y);
               unselectedButton.Draw(g);
         }
      }
   }
}
