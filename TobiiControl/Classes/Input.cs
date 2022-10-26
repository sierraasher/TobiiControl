using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

namespace TobiiControl {
    public static class Input {
        #region constants
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WK_KEYALT = 260;
        public const int WM_HOTKEY = 0x0312;
        #endregion

        public static void Initialize() {
            Hook.Start();
        }

        public static class Mouse {
            public const int MOUSEEVENTF_LEFTDOWN   = 0x02;
            public const int MOUSEEVENTF_LEFTUP     = 0x04;
            public const int MOUSEEVENTF_RIGHTDOWN  = 0x08;
            public const int MOUSEEVENTF_RIGHTUP    = 0x10;

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, UIntPtr dwExtraInfo);
        }

        public static class Keyboard {
            #region keys
            public struct Key {
                public bool down;
                public void Update(int keyCode, int wParam) {
                    var pr = down;
                    if (wParam == WM_KEYDOWN
                    || wParam == WK_KEYALT) down = true;
                    else down = false;

                    if (pr != down) {
                        Debug.WriteLine("key {0} code {1}", Enum.GetName(typeof(Keys), keyCode), down);
                    }
                }
            }
            public static int keyNumber = 256;
            public static Key[] key = new Key[keyNumber];
            #endregion
        }

        public static class Hook {
            #region variabl
            public static int _hookID;
            public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
            public static HookProc _proc = HookCallback;
            #endregion

            #region functions
            public static void Start() {
                _hookID = SetHook(_proc);
            }

            public static void End() {
                UnsetHook();
            }

            public static int SetHook(HookProc proc) {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule) {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            public static void UnsetHook() {
                UnhookWindowsHookEx(_hookID);
            }

            public static int HookCallback(int nCode, int wParam, IntPtr lParam) {
                bool handled = false;

                if (nCode >= 0) {
                    int keyCode = Marshal.ReadInt32(lParam);

                    #region block capslock
                    if (keyCode == (int)Keys.CapsLock
                    && !Keyboard.key[(int)Keys.LShiftKey].down) {
                        handled = true;
                    }

                    if (keyCode == TobiiControl.Mouse.Click.keyCode) {
                        handled = true;
                    }
                    #endregion

                    #region update
                    if (keyCode >= 0
                    && keyCode < Keyboard.keyNumber) {
                        Keyboard.key[keyCode].Update(keyCode, wParam);
                    }
                    #endregion
                }

                if (handled) {
                    Debug.WriteLine("Handle");
                    return -1;
                }
                else
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            #endregion

            #region dll inports
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int SetWindowsHookEx(
                int idHook,
                HookProc lpfn, 
                IntPtr hMod, 
                uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(int idHook);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int CallNextHookEx(
                int idHook, 
                int nCode,
                int wParam, 
                IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
            #endregion
        }
    }
}
