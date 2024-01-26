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
        private static readonly Color[] DEFAULT_COLORS = new Color[]
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

            JsonNeuron.DEFAULT_COLOR_GROUP,
        };
        public static readonly Color STARTING_COLOR = DEFAULT_COLORS[0];

        private int nextSortKey;
        private Dictionary<Color, ColorSelectDrawable> colorToDrawable;
        private Action<Color> onSelect;
        private ColorSelectDrawable selected;

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
            //background.SetX(160 + 2 * EinsteinConfig.PAD);
            //background.SetX(MenuCategoryButton.WIDTH + 2 * EinsteinConfig.PAD);
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
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            foreach (Drawable drawable in sortedOptionDrawables.Values)
            {
                IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(drawable);
            }
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
    }
}
