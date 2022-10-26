using System;

namespace TobiiControl {
    public static class MathE {
        public static float remapFloat(float value, float min1, float max1, float min2, float max2, bool clamp) {
            var vl = value;
            var mn1 = min1;
            var mx1 = max1;
            var mn2 = min2;
            var mx2 = max2;
            var cl = clamp;

            var df = mx1 - mn1;
            if (df != 0) {
                // normal
                var nrm = (vl - mn1) / df;

                // clamp
                if (cl) nrm = Clamp(nrm, 0, 1);

                // lerp
                var lrp = mn2 + nrm * (mx2 - mn2);

                return lrp;
            }
            else return 0;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float Normal(float value, float min, float max, bool clamp) {
            var df = max - min;
            var am = 0f;
            if (df != 0f) am = (value - min) / df;
            if (clamp) am = Clamp(am, 0, 1);

            return am;
        }
    }
}
