using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Accessibility;

namespace TobiiControl.Classes {
    public static class Magnifier {

        static void Update() {
            //var vl = Head.Position.current.Z;
            //var mn1 = 600;
            //var mx1 = 250;
            //var mn2 = 0;
            //var mx2 = 2;
            //var clmp = false;
            //var lv = remapFloat(vl, mn1, mx1, mn2, mx2, clmp);
            //var zm = Math.Pow(2, lv);

            //zm = Clamp(zm, 1.1, 10);

            //var scrn = Screen.PrimaryScreen;
            //var bnds = scrn.Bounds;
            //var rdX = bnds.Width / 2;
            //var rdY = bnds.Height / 2;
            //var cX = bnds.X + rdX;
            //var cY = bnds.Y + rdY;
            //var fX = cX + Head.Position.current.X * 4 - Head.Rotation.current.Y * 1000;
            //var fY = cY - Head.Position.current.Y * 4 - Head.Rotation.current.X * 1000;

            //var zmX = fX - (rdX / zm);
            //var zmY = fY - (rdY / zm);

            //SetMagnificationDesktopMagnification(zm, -(int)(zmX * zm), -(int)(zmY * zm));
        }

        #region dll imports 
        [DllImport("Magnification.dll")]
        public static extern bool MagInitialize();

        [DllImport("Magnification.dll")]
        public static extern bool MagGetFullscreenTransform([Out] out float magLevel, [Out] out int xOffset, [Out] out int yOffset);

        [DllImport("Magnification.dll")]
        public static extern bool MagSetFullscreenTransform(float magLevel, int xOffset, int yOffset);

        [DllImport("Magnification.dll", SetLastError = true)]
        public static extern bool MagSetInputTransform(bool fEnabled,
            [In, MarshalAs(UnmanagedType.Struct)] ref Rectangle prcSource,
            [In, MarshalAs(UnmanagedType.Struct)] ref Rectangle prcDest);

        [DllImport("Magnification.dll")]
        public static extern bool MagSetWindowSource(
            [In] IntPtr hwnd,
            [In, MarshalAs(UnmanagedType.Struct)] Rectangle rect);

        [DllImport("Magnification.dll")]
        public static extern bool MagSetWindowTransform(IntPtr hwnd, [Out] float[,] pTransform);

        [DllImport("Magnification.dll")]
        public static extern bool MagSetFullscreenColorEffect([In] float[] pEffect);

        [DllImport("Magnification.dll")]
        public static extern bool MagSetColorEffect(IntPtr hwnd, [In] float[] pEffect);

        [DllImport("Magnification.dll")]
        public static extern bool MagShowSystemCursor(bool fEnabled);

        [DllImport("Magnification.dll")]
        public static extern bool MagUninitialize();

        [DllImport("user32.dll")]
        public static extern bool SetMagnificationDesktopMagnification(double scale, int x, int y);

        [DllImport("user32.dll")]
        public static extern bool SetMagnificationDesktopColorEffect([In] float[] pEffect);
        #endregion
    }
}
