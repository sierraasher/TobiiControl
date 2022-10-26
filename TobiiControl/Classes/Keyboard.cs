using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TobiiControl {
    public static class Keyboard {
        public static class Events {
            public static void Subscribe() {
                Debug.WriteLine("keyboard events subscribed");
            }

            static void KeyDown() {
                Debug.WriteLine("Key Down");
            }
            static void KeyPress() {
                Debug.WriteLine("Key Press");
            }
            static void KeyUp() {
                Debug.WriteLine("Key Up");
            }

        }   
    }
}
