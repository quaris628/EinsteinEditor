using Einstein.config;
using Einstein.model;
using phi.graphics;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public abstract class MenuCategory : MultiRenderable
    {
        public const int OPTION_LAYER = 15;
        public const int BACKGROUND_LAYER = 14;

        public SelectableButton Button { get; protected set; }
        protected RectangleDrawable background;
        protected SortedDictionary<int, Drawable> sortedOptionDrawables;
        protected bool isInit { get; private set; }
        
        protected MenuCategory(SelectableButton button)
        {
            Button = button;
            background = new RectangleDrawable(MenuCategoryButton.WIDTH + 2 * EinsteinConfig.PAD, 0, 0, 0);
            background.SetPen(new Pen(new SolidBrush(EinsteinConfig.COLOR_MODE.MenuBackground)));
            sortedOptionDrawables = new SortedDictionary<int, Drawable>();
            // width and height will be set later
        }


        public virtual void Initialize()
        {
            Button.Initialize();
            Button.SubscribeOnSelect(ShowOptions);
            Button.SubscribeOnDeselect(HideOptions);
            IO.RENDERER.Add(Button);
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                IO.RENDERER.Add(optionDrawable, OPTION_LAYER);
            }
            IO.RENDERER.Add(background, BACKGROUND_LAYER);
            isInit = true;
            RepositionOptions();
            HideOptions();
        }

        public virtual void Uninitialize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            isInit = false;
            Button.Uninitialize();
            IO.RENDERER.Remove(Button);
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                IO.RENDERER.Remove(optionDrawable);
            }
            IO.RENDERER.Remove(background);
        }

        protected virtual void HideOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            background.SetDisplaying(false);
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                optionDrawable.SetDisplaying(false);
            }
        }

        protected virtual void ShowOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            background.SetDisplaying(true);
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                optionDrawable.SetDisplaying(true);
            }
            RepositionOptions();
        }

        public void AddOption(int sortKey, Drawable optionDrawable)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            optionDrawable.SetDisplaying(Button.IsSelected());

            sortedOptionDrawables.Add(sortKey, optionDrawable);

            IO.MOUSE.LEFT_UP.SubscribeOnDrawable(() =>
            {
                RemoveOption(sortKey);
            }, optionDrawable);
            IO.RENDERER.Add(optionDrawable, OPTION_LAYER);

            RepositionOptions();
        }

        public void RemoveOption(int sortKey)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            Drawable optionDrawable = sortedOptionDrawables[sortKey];
            sortedOptionDrawables.Remove(sortKey);

            IO.MOUSE.LEFT_UP.UnsubscribeAllFromDrawable(optionDrawable);
            IO.RENDERER.Remove(optionDrawable);

            RepositionOptions();
        }

        public void ClearAllOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            
            // must avoid concurrent modification exception
            LinkedList<int> sortKeysToRemove = new LinkedList<int>();
            foreach (int sortKey in sortedOptionDrawables.Keys)
            {
                sortKeysToRemove.AddFirst(sortKey);
            }
            foreach (int sortKey in sortKeysToRemove)
            {
                RemoveOption(sortKey);
            }
        }

        // places options left to right on the screen, wrapping
        // around if any would overflow across the right edge
        // of the screen, just like the words in this comment.
        public virtual void RepositionOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            int startX = background.GetX() + EinsteinConfig.PAD;
            int x = startX;
            int y = EinsteinConfig.PAD;
            int deltaY = 0;
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                int width = optionDrawable.GetWidth();
                if (x + width + EinsteinConfig.PAD > IO.WINDOW.GetWidth())
                {
                    // wrap around
                    x = startX;
                    y += deltaY + EinsteinConfig.PAD * 2;
                    deltaY = 0;
                }
                optionDrawable.SetXY(x, y);
                x += width + EinsteinConfig.PAD * 2;
                deltaY = Math.Max(deltaY, optionDrawable.GetHeight());
            }
            background.SetHeight(y + deltaY + EinsteinConfig.PAD);
            background.SetWidth(IO.WINDOW.GetWidth() - startX);
        }

        public virtual IEnumerable<Drawable> GetDrawables()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            yield return Button;
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                yield return optionDrawable;
            }
            yield return background;
        }

        public virtual string LogDetailsForCrash()
        {
            string log = "Button.IsSelected() = " + Button.IsSelected();
            log += "\nsortedOptionDrawables = " + string.Join(",\n\t", sortedOptionDrawables);
            log += "\nisInit = " + isInit;
            return log;
        }
    }
}
