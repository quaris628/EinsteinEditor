using Einstein.config;
using Einstein.model;
using Einstein.model.json;
using Einstein.ui.menu.categories.colors;
using phi.graphics;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class ColorMenuCategory : MenuCategory
    {
        public static readonly Color[] DEFAULT_COLORS = new Color[]
        {
            Color.OrangeRed,
            
            Color.Orange,
            
            Color.Yellow,
            
            Color.Lime,
            Color.ForestGreen,

            Color.Cyan,
            //Color.DeepSkyBlue,
            Color.DodgerBlue,
            
            Color.MediumPurple,
            //Color.BlueViolet,

            Color.Violet,

            Color.Tan,
            Color.SaddleBrown,

            BaseNeuron.DEFAULT_COLOR_GROUP,
        };

        public static readonly Color STARTING_COLOR = DEFAULT_COLORS[0];

        public static readonly Color[] STARTING_SHORTCUTS =
            new ArraySegment<Color>(DEFAULT_COLORS, 0, 9)
            .Append(BaseNeuron.DEFAULT_COLOR_GROUP).ToArray();

        private int nextSortKey;
        private Dictionary<Color, ColorSelectDrawable> colorToDrawable;
        private Action<Color> onSelect;
        private ColorSelectDrawable selected;
        private Color[] shortcuts;

        public ColorMenuCategory(SelectableButton button, Action<Color> onSelect): base(button)
        {
            nextSortKey = 0;
            colorToDrawable = new Dictionary<Color, ColorSelectDrawable>();
            this.onSelect = onSelect;

            foreach (Color color in DEFAULT_COLORS)
            {
                ColorSelectDrawable drawable = new ColorSelectDrawable(color, nextSortKey);
                sortedOptionDrawables.Add(nextSortKey, drawable);
                colorToDrawable[color] = drawable;
                nextSortKey++;
            }
            selected = (ColorSelectDrawable)sortedOptionDrawables[0];
            selected.IsSelected = true;

            shortcuts = STARTING_SHORTCUTS;
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (Drawable drawable in sortedOptionDrawables.Values)
            {
                ColorSelectDrawable colorSelectDrawable = (ColorSelectDrawable)drawable;
                IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
                {
                    selected.IsSelected = false;
                    colorSelectDrawable.IsSelected = true;
                    selected = colorSelectDrawable;
                    onSelect.Invoke(colorSelectDrawable.Color);
                }, colorSelectDrawable);
            }
            IO.KEYS.Subscribe(Shortcut1, (int)Keys.D1 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut2, (int)Keys.D2 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut3, (int)Keys.D3 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut4, (int)Keys.D4 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut5, (int)Keys.D5 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut6, (int)Keys.D6 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut7, (int)Keys.D7 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut8, (int)Keys.D8 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut9, (int)Keys.D9 + (int)Keys.Control);
            IO.KEYS.Subscribe(Shortcut0, (int)Keys.D0 + (int)Keys.Control);

            for (int i = 0; i < 9; i++)
            {
                colorToDrawable[shortcuts[i]].Label = (i + 1).ToString();
            }
            colorToDrawable[shortcuts[9]].Label = "0";
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            foreach (Drawable drawable in sortedOptionDrawables.Values)
            {
                IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(drawable);
            }
            IO.KEYS.Unsubscribe(Shortcut1, (int)Keys.D1 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut2, (int)Keys.D2 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut3, (int)Keys.D3 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut4, (int)Keys.D4 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut5, (int)Keys.D5 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut6, (int)Keys.D6 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut7, (int)Keys.D7 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut8, (int)Keys.D8 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut9, (int)Keys.D9 + (int)Keys.Control);
            IO.KEYS.Unsubscribe(Shortcut0, (int)Keys.D0 + (int)Keys.Control);
        }

        public void Add(Color color)
        {
            ColorSelectDrawable drawable = new ColorSelectDrawable(color, nextSortKey);
            AddOption(nextSortKey, drawable);
            colorToDrawable[color] = drawable;
            nextSortKey++;
        }

        public void ResetColorsToDefault()
        {
            ClearAllOptions();
            nextSortKey = 0;
            colorToDrawable = new Dictionary<Color, ColorSelectDrawable>();
            foreach (Color color in DEFAULT_COLORS)
            {
                Add(color);
            }
            RepositionOptions();
        }

        private void Shortcut(int index)
        {
            onSelect.Invoke(shortcuts[index]);
            selected.IsSelected = false;
            selected = colorToDrawable[shortcuts[index]];
            selected.IsSelected = true;
        }

        private void Shortcut1() { Shortcut(0); }
        private void Shortcut2() { Shortcut(1); }
        private void Shortcut3() { Shortcut(2); }
        private void Shortcut4() { Shortcut(3); }
        private void Shortcut5() { Shortcut(4); }
        private void Shortcut6() { Shortcut(5); }
        private void Shortcut7() { Shortcut(6); }
        private void Shortcut8() { Shortcut(7); }
        private void Shortcut9() { Shortcut(8); }
        private void Shortcut0() { Shortcut(9); }
    }
}
