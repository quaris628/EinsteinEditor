using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics;
using phi.control;
using phi.io;
using phi.graphics.renderables;
using phi.graphics.drawables;
using Einstein.model;
using Einstein.ui;
using Einstein.ui.menu;

namespace Einstein
{
    class EditorScene : Scene
    {
        // ----------------------------------------------------------------
        //  UI Config
        // ----------------------------------------------------------------

        private const string TITLE = "Einstein Bibite Editor";

        private const int PAD = EinsteinPhiConfig.PAD;
        
        private struct SELECTABLE_BUTTONS
        {
            public const string UNSELECTED_IMAGE_PATH =
            EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
            public const string SELECTED_IMAGE_PATH =
                EinsteinPhiConfig.RES_DIR + "ButtonBackgroundSelected.png";
            public const int WIDTH = 128;
            public const int HEIGHT = 32;

            private static SelectableButton construct(
                int x, int y, string text,
                Action onSelect, Action onDeselect)
            {
                Button.ButtonBuilder unselectedButton = new Button.ButtonBuilder(
                    new ImageWrapper(UNSELECTED_IMAGE_PATH), x, y)
                    .withText(text)
                    .withOnClick(onSelect);
                Button.ButtonBuilder selectedButton = new Button.ButtonBuilder(
                    new ImageWrapper(SELECTED_IMAGE_PATH), x, y)
                    .withText(text)
                    .withOnClick(onDeselect);
                return new SelectableButton(unselectedButton, selectedButton);
            }

            public static SelectableButton ConstructInput(
                Action onSelect, Action onDeselect)
            {
                return construct(PAD, PAD, "Input Neurons",
                    onSelect, onDeselect);
            }
            public static SelectableButton ConstructOutput(
                Action onSelect, Action onDeselect)
            {
                return construct(PAD, 2 * PAD + HEIGHT, "Output Neurons",
                    onSelect, onDeselect);
            }
            public static SelectableButton ConstructAdd(
                Action onSelect, Action onDeselect)
            {
                return construct(PAD, 3 * PAD + 2 * HEIGHT, "Add Neurons",
                    onSelect, onDeselect);
            }

        }

        // ----------------------------------------------------------------
        //  Data/Constructor
        // ----------------------------------------------------------------

        private NeuronMenu menu;
        private int prevWindowWidth;

        public EditorScene(Scene prevScene) : base(prevScene, new ImageWrapper(EinsteinPhiConfig.Render.DEFAULT_BACKGROUND))
        {
            menu = new NeuronMenu();
            prevWindowWidth = EinsteinPhiConfig.Window.WIDTH;
        }

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------

        protected override void InitializeMe()
        {
            menu.Initialize();
            IO.FRAME_TIMER.Subscribe(checkForResize);
        }

        protected override void CloseMe()
        {
            IO.FRAME_TIMER.Unsubscribe(checkForResize);
        }

        // ----------------------------------------------------------------
        //  Behavior
        // ----------------------------------------------------------------

        // TODO look at increasing performance of re-clicking the same drawable very quickly?

        // check if the window has been resized
        // if so, we probably need to reposition the menu buttons
        private void checkForResize()
        {
            if (IO.WINDOW.GetWidth() != prevWindowWidth)
            {
                menu.repositionMenuButtons();
            }
        }

    }
}
