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
      private const int WIDTH_FUDGE = 14;
      private const int HEIGHT_FUDGE = 39;

      private Scene entryScene;
      private Config config;
      private PictureBox pictureBox;
      
      public WindowsForm(Scene entryScene, Config config)
      {
         this.entryScene = entryScene;
         this.config = config;
         this.pictureBox = new PictureBox();

         // misc configs
         IO.RENDERER.SetBackground(Image.FromFile(config.GetRenderDefaultBackground()));
         IO.RENDERER.SetDefaultLayer(config.GetRenderDefaultLayer());
         IO.FRAME_TIMER.SetFPS(config.GetRenderFPS());
         IO.FRAME_TIMER.LockedSubscribe(new Random().Next(), RefreshPictureBox);

         InitializeComponent();
         Load += new EventHandler(FormLoad);
      }

      private void FormLoad(object sender, EventArgs e)
      {
         // Set window properites
         Size = new Size(config.GetWindowWidth() + WIDTH_FUDGE,
            config.GetWindowHeight() + HEIGHT_FUDGE);
         Text = config.GetWindowTitle();
         // Set pictureBox properties
         pictureBox.Size = Size;
         pictureBox.Image = Image.FromFile(config.GetRenderDefaultBackground());
         Controls.Add(pictureBox); // is this line needed? -- Yes, I think so
         IO.RENDERER.SetOutput(pictureBox.Image);

         // Setup key input event handling
         KeyPreview = true;
         KeyDown += new KeyEventHandler(IO.KEYS.KeyInputEvent);
         // Setup mouse input event handling
         pictureBox.MouseClick += new MouseEventHandler(IO.MOUSE.CLICK.Event);
         pictureBox.MouseDown += new MouseEventHandler(IO.MOUSE.DOWN.Event);
         pictureBox.MouseUp += new MouseEventHandler(IO.MOUSE.UP.Event);
         pictureBox.MouseMove += new MouseEventHandler(IO.MOUSE.MOVE.Event);

         // Let's go!
         entryScene.Initialize();
         IO.FRAME_TIMER.Start();
      }

      private void RefreshPictureBox()
      {
         pictureBox.Image = pictureBox.Image; // Do not delete; forces some sort of update
      }

      public static void Exit() { Application.Exit(); }
   }
}
