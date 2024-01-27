using phi.graphics;
using phi.graphics.drawables;
using phi.other;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu.categories.colors
{
    public class ColorSelectDrawable : Drawable
    {
        public const int SIZE = NeuronDrawable.CIRCLE_DIAMETER;
        public static readonly Pen SELECTED_PEN = new Pen(Color.White, 4);

        public Color Color { get; private set; }
        private Brush colorBrush;
        public int SortKey { get; private set; }
        public bool IsSelected;
        public string Label;

        public ColorSelectDrawable(Color color, int sortKey) : base(-SIZE, -SIZE, SIZE, SIZE)
        {
            Color = color;
            colorBrush = new Pen(color).Brush;
            SortKey = sortKey;
            IsSelected = false;
            Label = "";
        }

        protected override void DrawAt(Graphics g, int x, int y)
        {
            g.FillRectangle(colorBrush, x, y, SIZE, SIZE);
            if (IsSelected)
            {
                g.DrawRectangle(SELECTED_PEN, x, y, SIZE, SIZE);
            }
            g.DrawString(Label,
                new Font("Arial", 12),
                new Pen(Color.Black).Brush,
                x + SIZE / 2 - 7,
                y + SIZE / 2 - 9);
        }
    }
}
