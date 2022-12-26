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
        private Action onSelect;
        private Action onDeselect;
        private bool selected;

        public SelectableButton(Button.ButtonBuilder unselectedButton,
            Button.ButtonBuilder selectedButton)
            : base(unselectedButton.Build().GetBoundaryRectangle()) {
            onSelect = unselectedButton.GetOnClick();
            onDeselect = selectedButton.GetOnClick();
            unselectedButton.withOnClick(Select);
            selectedButton.withOnClick(Deselect);
            this.unselectedButton = unselectedButton.Build();
            this.selectedButton = selectedButton.Build();
        }

        public void Initialize()
        {
            unselectedButton.Initialize();
            selectedButton.Initialize();
        }

        private void Select()
        {
            selectedButton.SetDisplaying(true);
            unselectedButton.SetDisplaying(false);
            selected = true;
            FlagChange();
            onSelect.Invoke();
        }
        private void Deselect()
        {
            selectedButton.SetDisplaying(false);
            unselectedButton.SetDisplaying(true);
            selected = false;
            FlagChange();
            onDeselect.Invoke();
        }

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
