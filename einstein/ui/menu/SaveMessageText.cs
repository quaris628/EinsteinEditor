using Einstein.ui.menu;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui
{
    public class SaveMessageText : Text
    {
        public const string MESSAGE = "Save Successful";
        public const int SECONDS_TO_SHOW = 5;

        private DateTime hideAt;

        public SaveMessageText(int centerX, int y) : base(
            new TextBuilder(MESSAGE)
            .WithFontSize(10f)
            .WithBackgroundColor(new SolidBrush(Color.LightGreen))
            .WithY(y)
            )
        {
            SetCenterX(centerX);
        }

        public void Show()
        {
            hideAt = DateTime.Now.AddSeconds(SECONDS_TO_SHOW);
            IO.FRAME_TIMER.Subscribe(WaitToHide);
            IO.RENDERER.Add(this);
        }

        private void WaitToHide()
        {
            if (0 >= hideAt.CompareTo(DateTime.Now))
            {
                Hide();
            }
        }

        private void Hide()
        {
            IO.FRAME_TIMER.Unsubscribe(WaitToHide);
            IO.RENDERER.Remove(this);
        }

    }
}