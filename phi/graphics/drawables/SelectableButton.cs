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

        public SelectableButton(Button.ButtonBuilder unselectedButton,
            Button.ButtonBuilder selectedButton)
            : base(unselectedButton.Build().GetBoundaryRectangle()) {
            onSelect = new List<Action>();
            onDeselect = new List<Action>();
            onSelect.Add(unselectedButton.GetOnClick());
            onDeselect.Add(selectedButton.GetOnClick());
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
        }

        public void Select()
        {
            selectedButton.SetDisplaying(true);
            unselectedButton.SetDisplaying(false);
            selected = true;
            FlagChange();
            foreach (Action action in onSelect)
            {
                action.Invoke();
            }
        }
        public void Deselect()
        {
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
            onSelect.Add(action);
        }

        public void UnsubscribeFromSelect(Action action)
        {
            onSelect.Remove(action);
        }

        public void SubscribeOnDeselect(Action action)
        {
            onDeselect.Add(action);
        }

        public void UnsubscribeFromDeselect(Action action)
        {
            onDeselect.Remove(action);
        }

        public bool IsSelected() { return selected; }

        protected override void DrawAt(Graphics g, int x, int y)
        {
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
