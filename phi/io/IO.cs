using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.io;
using phi.graphics;
using phi.control;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace phi.io
{
   public class IO
   {
      public static readonly FrameTimerInputHandler FRAME_TIMER = new FrameTimerInputHandler();
      public static readonly KeyInputHandler KEYS = new KeyInputHandler();
      public struct MOUSE
      {
         public static readonly MouseInputHandler CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler UP = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_UP = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_UP = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK_UP = new MouseInputHandler();
         //public static readonly MouseInputHandler MID_SCROLL_DOWN = new MouseInputHandler();
         //public static readonly MouseInputHandler MID_SCROLL_UP = new MouseInputHandler();
         public static readonly MouseInputHandler MOVE = new MouseInputHandler();
         public static void Clear()
         {
            CLICK.Clear();
            LEFT_CLICK.Clear();
            RIGHT_CLICK.Clear();
            MID_CLICK.Clear();
            DOWN.Clear();
            LEFT_DOWN.Clear();
            RIGHT_DOWN.Clear();
            MID_CLICK_DOWN.Clear();
            UP.Clear();
            LEFT_UP.Clear();
            RIGHT_UP.Clear();
            MID_CLICK_UP.Clear();
            //MID_SCROLL_DOWN.Clear();
            //MID_SCROLL_UP.Clear();
            MOVE.Clear();
         }
      }
      public static readonly Renderer RENDERER = new Renderer();
      
      public struct WINDOW
      {
         private static WindowsForm form;
         internal static void SetWindowsForm(WindowsForm form) { WINDOW.form = form; }
         public static int GetWidth() { return form.Width - WindowsForm.WIDTH_FUDGE; }
         public static int GetHeight() { return form.Height - WindowsForm.HEIGHT_FUDGE; }
      }

      private IO() { }

      public static void Clear()
      {
         FRAME_TIMER.Clear();
         KEYS.Clear();
         MOUSE.Clear();
         RENDERER.Clear();
      }

      // example filters:
      // "XML Files|*.xml"
      // "Image Files (*.bmp, *.jpg)|*.bmp;*.jpg"
      // "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"

      public static string PromptForFile(string initialDirectory,
          string filter, string windowTitle, string defaultName)
      {
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                Title = windowTitle,
                Filter = filter,
                FileName = defaultName,
            };
            dialog.ShowDialog();
         return dialog.FileName;
      }

      public static void ShowPopup(string title, string content)
      {
         MessageBox.Show(content, title, MessageBoxButtons.OK);
      }
      public static void ShowErrorPopup(string title, string content)
      {
         MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      public static void HandleCrash(Exception e, string popupWindowTitle,
          string logFilepath, string githubNewIssueLink, string extraLog)
      {
         // Show message
         string fullLogFilepath = Path.GetFullPath(logFilepath);
         string errorDetails =
               "\n\n---------------- Technical Details ----------------" +
               "\nName: " + e.GetType().Name +
               "\nMessage: " + e.Message +
               "\nTarget Site: " + e.TargetSite;
         int indexOfAtSWF = e.StackTrace.IndexOf("at System.Windows.Forms");
         string stackTrace = "\nStack Trace:\n" +
               e.StackTrace.Substring(0,
               indexOfAtSWF > 0 ? indexOfAtSWF - 1 : e.StackTrace.Length);

         DialogResult result = MessageBox.Show(
            "This application has crashed." +
            "\nDo you want to write a bug report?" + errorDetails,
            popupWindowTitle,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Error,
            MessageBoxDefaultButton.Button2);
         if (result == DialogResult.No) { return; }

         // Open github link w/ default browser
         try
         {
            ProcessStartInfo sInfo = new ProcessStartInfo(githubNewIssueLink);
            Process.Start(sInfo);
         } catch (Exception) { } // if it failed, just continue

         // Write error log file
         try
         {
            File.WriteAllText(logFilepath, errorDetails + stackTrace + extraLog);
         }
         catch (Exception)
         {
            // presume log file failed to be created, continue without it
            MessageBox.Show(
               "To report this crash, submit an issue on github." +
               "\n(You will need a github account; alternatively, you can email quaris314@gmail.com.)" +
               "\n\nIn the report, please describe the steps you took " +
               "immediately prior to the crash, and include a screenshot of the technical details below." +
               "\n\nIf github hasn't opened automatically, type in this address: " + githubNewIssueLink +
               errorDetails + stackTrace,
               popupWindowTitle,
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
            return;
         }

         // Open the created log file in a new notepad window
         try
         {
            Process.Start("notepad.exe", logFilepath);
         } catch (Exception) { } // if it failed, just continue

         MessageBox.Show(
            "To report this crash, submit an issue on github." +
             "\n(You will need a github account; alternatively, you can email quaris314@gmail.com.)" +
            "\n\nIn the report, please describe the steps you took " +
            "immediately prior to the crash, and paste the contents " +
            "of the error log too." +
            "\n\nIf github hasn't opened automatically, type in this address: " + githubNewIssueLink +
            "\n\nIf the error log file hasn't opened automatically, " +
            "you can find it at: " + fullLogFilepath +
            errorDetails,
            popupWindowTitle,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
      }

      public static void Exit() { WindowsForm.Exit(); }
   }
}
