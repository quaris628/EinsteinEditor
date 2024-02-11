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
            //Color.OrangeRed,
            Color.IndianRed,
            
            //Color.Orange,
            Color.FromArgb(236, 151, 6), // Honey
            
            //Color.Yellow,
            Color.FromArgb(250, 218, 94), // Royal Yellow
            
            //Color.Lime,
            Color.FromArgb(178, 236, 93), // Inchworm
            Color.ForestGreen,

            //Color.Cyan,
            Color.FromArgb(153, 255, 255), // Ice Blue
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
            IO.KEYS.Subscribe(Shortcut1, Keys.D1);
            IO.KEYS.Subscribe(Shortcut2, Keys.D2);
            IO.KEYS.Subscribe(Shortcut3, Keys.D3);
            IO.KEYS.Subscribe(Shortcut4, Keys.D4);
            IO.KEYS.Subscribe(Shortcut5, Keys.D5);
            IO.KEYS.Subscribe(Shortcut6, Keys.D6);
            IO.KEYS.Subscribe(Shortcut7, Keys.D7);
            IO.KEYS.Subscribe(Shortcut8, Keys.D8);
            IO.KEYS.Subscribe(Shortcut9, Keys.D9);
            IO.KEYS.Subscribe(Shortcut0, Keys.D0);

            for (int i = 0; i < 10; i++)
            {
                colorToDrawable[shortcuts[i]].Label = ShortcutIndexToLabel(i);
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            foreach (Drawable drawable in sortedOptionDrawables.Values)
            {
                IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(drawable);
            }
            IO.KEYS.Unsubscribe(Shortcut1, Keys.D1);
            IO.KEYS.Unsubscribe(Shortcut2, Keys.D2);
            IO.KEYS.Unsubscribe(Shortcut3, Keys.D3);
            IO.KEYS.Unsubscribe(Shortcut4, Keys.D4);
            IO.KEYS.Unsubscribe(Shortcut5, Keys.D5);
            IO.KEYS.Unsubscribe(Shortcut6, Keys.D6);
            IO.KEYS.Unsubscribe(Shortcut7, Keys.D7);
            IO.KEYS.Unsubscribe(Shortcut8, Keys.D8);
            IO.KEYS.Unsubscribe(Shortcut9, Keys.D9);
            IO.KEYS.Unsubscribe(Shortcut0, Keys.D0);
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

        public void RebindShortcut(Color targetColor, int index)
        {
            // get the keybind's old color
            ColorSelectDrawable sourceColor = colorToDrawable[shortcuts[index]];
            sourceColor.Label = "";

            // check if this NEW color has an old keybind
            for (int i = 0; i < 10; i++)
            {
                if (shortcuts[i] == targetColor)
                {
                    int targetColorOldIndex = i;
                    // if so, swap the keybinds
                    shortcuts[targetColorOldIndex] = sourceColor.Color;
                    sourceColor.Label = ShortcutIndexToLabel(targetColorOldIndex);
                }
            }
            shortcuts[index] = targetColor;
            colorToDrawable[targetColor].Label = ShortcutIndexToLabel(index);
        }

        private void Shortcut(int index)
        {
            // while editing neuron descriptions or synapse strengths, don't trigger the shortcuts
            if (SelectableEditableText.IsAnyCurrentlySelected())
            {
                return;
            }
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

        private string ShortcutIndexToLabel(int index)
        {
            return index == 9 ? "0" : (index + 1).ToString();
        }
    }
}
