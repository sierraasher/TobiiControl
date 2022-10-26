using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace TobiiControl {
    public class UpdateLoop {
        #region variables
        public bool active = true;
        #endregion

        #region main
        public UpdateLoop() {
            #region update
            while (active) {
                Time.Update();
                Head.Update();
                Eyes.Update();
                Mouse.Update();

                #region sleep
                var slp = Time.sleepInterval;
                Thread.Sleep(slp);
                #endregion
            }
            #endregion
        }
        #endregion

    }
}

