using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using phi.io;
using phi.other;

namespace phi.control
{
   partial class WindowsForm : Form
   {
      public const int WIDTH_FUDGE = 14;
      public const int HEIGHT_FUDGE = 39;

      private Scene entryScene;
      private Config config;
      private PictureBox pictureBox;
      
      public WindowsForm(Scene entryScene, Config config)
      {
         this.entryScene = entryScene;
         this.config = config;
         this.pictureBox = new PictureBox();

         // misc configs
         IO.RENDERER.SetBackground(config.GetRenderDefaultBackground());
         IO.RENDERER.SetDefaultLayer(config.GetRenderDefaultLayer());
         IO.FRAME_TIMER.SetFPS(config.GetRenderFPS());
         IO.FRAME_TIMER.LockedSubscribe(new Random().Next(), RefreshPictureBox);

         InitializeComponent();
         try { Icon = new Icon(config.GetWindowIcon()); } catch (Exception) { }
         Load += new EventHandler(FormLoad);
      }

      private void FormLoad(object sender, EventArgs e)
      {
         Image image = new Bitmap(config.GetMaxWindowWidth(), config.GetMaxWindowHeight());
         IO.RENDERER.SetOutput(image);

         // Set window properites
         IO.WINDOW.SetWindowsForm(this);
         Size = new Size(
            config.GetInitialWindowWidth() + WIDTH_FUDGE,
            config.GetInitialWindowHeight() + HEIGHT_FUDGE);
         Text = config.GetWindowTitle();
         
         // Set pictureBox properties
         pictureBox.Size = Size;
         pictureBox.Image = image;
         Controls.Add(pictureBox); // is this line needed? -- Yes, I think so
         
         // Setup key input event handling
         KeyPreview = true;
         KeyDown += new KeyEventHandler(IO.KEYS.KeyInputEvent);
         // Setup mouse input event handling
         pictureBox.MouseClick += new MouseEventHandler((sender1, e1) => {
            IO.MOUSE.CLICK.Event(sender1, e1);
            if (e1.Button == MouseButtons.Left) { IO.MOUSE.LEFT_CLICK.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Right) { IO.MOUSE.RIGHT_CLICK.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Middle) { IO.MOUSE.MID_CLICK.Event(sender1, e1); }
         });
         pictureBox.MouseDown += new MouseEventHandler((sender1, e1) => {
            IO.MOUSE.DOWN.Event(sender1, e1);
            if (e1.Button == MouseButtons.Left) { IO.MOUSE.LEFT_DOWN.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Right) { IO.MOUSE.RIGHT_DOWN.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Middle) { IO.MOUSE.MID_CLICK_DOWN.Event(sender1, e1); }
         });
         pictureBox.MouseUp += new MouseEventHandler((sender1, e1) => {
            IO.MOUSE.UP.Event(sender1, e1);
            if (e1.Button == MouseButtons.Left) { IO.MOUSE.LEFT_UP.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Right) { IO.MOUSE.RIGHT_UP.Event(sender1, e1); }
            else if (e1.Button == MouseButtons.Middle) { IO.MOUSE.MID_CLICK_UP.Event(sender1, e1); }
         });
         pictureBox.MouseWheel += new MouseEventHandler((sender1, e1) => {
             IO.MOUSE.MID_SCROLL.Event(sender1, e1);
             if (e1.Delta > 0) { IO.MOUSE.MID_SCROLL_UP.Event(sender1, e1); }
             else { IO.MOUSE.MID_SCROLL_DOWN.Event(sender1, e1); }
         });
         pictureBox.MouseMove += new MouseEventHandler(IO.MOUSE.MOVE.Event);

         // Let's go!
         entryScene.Initialize();
         IO.FRAME_TIMER.Start();
      }

      private void RefreshPictureBox()
      {
         pictureBox.Image = pictureBox.Image; // Do not delete; forces some sort of update
         pictureBox.Size = Size;
      }

      public static void Exit() { Application.Exit(); }
   }
}
