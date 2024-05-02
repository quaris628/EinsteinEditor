using Einstein.config;
using Einstein.ui.editarea;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class ZoomControls
    {
        public const string OUT_IMAGE = EinsteinConfig.RES_DIR + "ZoomOut.png";
        public const string ICON_IMAGE = EinsteinConfig.RES_DIR + "ZoomIcon.png";
        public const string IN_IMAGE = EinsteinConfig.RES_DIR + "ZoomIn.png";
        public const string RESET_IMAGE = EinsteinConfig.RES_DIR + "ZoomReset.png";
        public const int IMAGES_SIZE = 32;

        private Sprite zoomIcon;
        private Button zoomOut;
        private Button zoomIn;
        private Button reset;

        public ZoomControls(EditArea editArea)
        {
            zoomOut = new Button.ButtonBuilder(
                new ImageWrapper(OUT_IMAGE),
                EinsteinConfig.PAD,
                EinsteinConfig.Window.INITIAL_HEIGHT - EinsteinConfig.PAD - IMAGES_SIZE)
                .withOnClick(editArea.ZoomOutCentered)
                .Build();
            zoomIcon = new Sprite(new ImageWrapper(ICON_IMAGE),
                EinsteinConfig.PAD * 2 + IMAGES_SIZE,
                EinsteinConfig.Window.INITIAL_HEIGHT - EinsteinConfig.PAD - IMAGES_SIZE);
            zoomIn = new Button.ButtonBuilder(
                new ImageWrapper(IN_IMAGE),
                EinsteinConfig.PAD * 3 + IMAGES_SIZE * 2,
                EinsteinConfig.Window.INITIAL_HEIGHT - EinsteinConfig.PAD - IMAGES_SIZE)
                .withOnClick(editArea.ZoomInCentered)
                .Build();
            reset = new Button.ButtonBuilder(
                new ImageWrapper(RESET_IMAGE),
                EinsteinConfig.PAD + MenuCategoryButton.WIDTH - IMAGES_SIZE,
                EinsteinConfig.Window.INITIAL_HEIGHT - EinsteinConfig.PAD - IMAGES_SIZE)
                .withOnClick(editArea.ResetZoomLevel)
                .Build();
        }

        public void Initialize()
        {
            zoomOut.Initialize();
            zoomIn.Initialize();
            reset.Initialize();
            IO.RENDERER.Add(zoomOut);
            IO.RENDERER.Add(zoomIcon);
            IO.RENDERER.Add(zoomIn);
            IO.RENDERER.Add(reset);
        }

        public void Uninitialize()
        {
            zoomOut.Uninitialize();
            zoomIn.Uninitialize();
            reset.Uninitialize();
            IO.RENDERER.Remove(zoomOut);
            IO.RENDERER.Remove(zoomIcon);
            IO.RENDERER.Remove(zoomIn);
            IO.RENDERER.Remove(reset);
        }

        public void Reposition()
        {
            int y = IO.WINDOW.GetHeight() - EinsteinConfig.PAD - IMAGES_SIZE;
            zoomOut.SetY(y);
            zoomIcon.SetY(y);
            zoomIn.SetY(y);
            reset.SetY(y);
        }

    }
}
