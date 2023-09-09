﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Einstein.config;
using phi;
using phi.io;

namespace Einstein
{
    public static class EinsteinMain
    {
        public const string CRASH_LOG_FILEPATH = "../EinsteinErrorLog.txt";

        // to enable reference to the phi namescape:
        // in VS solution explorer, project -> references -> Add reference

        // to hide console for a project (this has already been done for this project):
        // in VS, project -> properties -> Output type, set to Windows Application instead of Console Application
	   
        // Entry point
        [STAThread]
        public static void Main()
        {
            #if DEBUG
            PhiMain.Main(new EditorScene(null), new EinsteinConfig());
            #else
            EditorScene entryScene = null;
            try
            {
                entryScene = new EditorScene(null);
                PhiMain.Main(entryScene, new EinsteinConfig());
            }
            catch (Exception e)
            {
                string extraLog = "\n\n---------------- Einstein Data State ----------------\n\n" +
                    entryScene?.LogDetailsForCrash();
                if (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                IO.POPUPS.HandleCrash(e,
                   EinsteinConfig.Window.TITLE,
                   CRASH_LOG_FILEPATH,
                   "https://github.com/quaris628/EinsteinEditor/issues/new",
                   extraLog);
            }
            #endif
        }
    }
}
