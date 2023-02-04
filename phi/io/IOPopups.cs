using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phi.io
{
   public partial class IO
   {
      public struct POPUPS
      {
         public static void ShowPopup(string title, string content)
         {
            MessageBox.Show(content, title, MessageBoxButtons.OK);
         }
         public static void ShowErrorPopup(string title, string content)
         {
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         public static bool ShowYesNoPopup(string title, string content)
         {
            return DialogResult.Yes == MessageBox.Show(content, title, MessageBoxButtons.YesNo);
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

         public static void HandleCrash(Exception e, string popupWindowTitle,
            string logFilepath, string githubNewIssueLink, string extraLog)
         {
            try {
               IO.FRAME_TIMER.Stop();
            } catch (NullReferenceException) { }
                
            // Show message
            string errorDetails =
                  "\n\n---------------- Technical Details ----------------" +
                  "\nName: " + e.GetType().Name +
                  "\nMessage: " + e.Message +
                  "\nTarget Site: " + e.TargetSite;
            int indexOfAtSWF = e.StackTrace.IndexOf("at System.Windows.Forms");
            string stackTrace = "\nStack Trace:\n" +
                  e.StackTrace.Substring(0,
                  indexOfAtSWF > 0 ? indexOfAtSWF - 1 : e.StackTrace.Length);
            string phiDetails = IO.LogDetailsForCrash();
            string fullLog = errorDetails + stackTrace + extraLog + phiDetails;

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
            }
            catch (Exception) { } // if it failed, just continue

            // Write error log file
            try
            {
               File.WriteAllText(logFilepath, fullLog);
            }
            catch (Exception)
            {
               // presume log file failed to be created, continue without it
               MessageBox.Show(
                  "To report this crash," +
                  "\nsubmit an issue on github (requires an account), or" +
                  "\nemail quaris314@gmail.com, or" +
                  "\ncontact Quaris via carrier pigeon or another available method." +
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
            }
            catch (Exception) { } // if it failed, just continue
            
            MessageBox.Show(
               "To report this crash," +
               "\nsubmit an issue on github (requires an account), or" +
               "\nemail quaris314@gmail.com, or" +
               "\ncontact Quaris via carrier pigeon or another available method." +
               "\n\nIn the report, please describe the steps you took " +
               "immediately prior to the crash, and paste the contents " +
               "of the error log too." +
               "\n\nIf github hasn't opened automatically, type in this address: " + githubNewIssueLink +
               "\n\nIf the error log file hasn't opened automatically, " +
               "you can find it at: " + Path.GetFullPath(logFilepath) +
               errorDetails,
               popupWindowTitle,
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
         }
      }
   }
}
