using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Tobii.StreamEngine;

namespace TobiiControl{
    public static class Program{
        public static UpdateLoop _updateLoop;
        public static MainWindow _mainWindow;
        //public static SettingsForm _settingsForm;
        public static DebugForm _debugform;

        public static Thread streamThread;
        public static Thread updateThread;
        //public static Thread windowThread;
        public static Thread debugThread;

        [STAThread]
        public static void Update() {
            Debug.WriteLine("program run");

            streamThread = new Thread(RunStream);
            streamThread.Start();

            updateThread = new Thread(RunUpdate);
            updateThread.Start();

            //windowThread = new Thread(RunSettings);
            //windowThread.Start();

            //debugThread = new Thread(RunDeubg);
            //debugThread.Start();
        }

        static void RunStream() {
            Stream.Update();
        }

        static void RunSettings() {

        }

        static void RunDeubg() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(_debugform = new DebugForm());
        }

        static void RunUpdate() {
            _updateLoop = new UpdateLoop();
        }
    }
}
