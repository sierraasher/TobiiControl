using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace TobiiControl {
    public static class Head {
        public static float frequencyDefault = 1f / 3f;
        public static bool detected = false;
        public static bool detectedPrevious = false;
        public static Vector3 position;
        public static Vector3 rotation;

        public static class Position {
            public static Vector3 current;
            public static Vector3 previous;
            public static Vector3 target;
            public static Vector3 speed;

            public static float frequency = frequencyDefault;

            public static void Update() {
                previous = current;
                if (frequency == 1) current = target;
                else current = Vector3.Lerp(current, target, frequency*Time.delta);
                speed = current - previous;
                position = current;
                //Console.WriteLine("Head Position current{0} speed{1}", current, speed);
            }

            public static void SetTarget() {
                target = Stream.Head.position;
            }
        }

        public static class Rotation {
            public static float x;
            public static float y;
            public static float z;
            public static Vector3 current;
            public static Vector3 previous;
            public static Vector3 target;
            public static Vector3 speed;

            public static float frequency = frequencyDefault;

            public static void Update() {
                previous = current;
                if (frequency == 1) current = target;
                else current = Vector3.Lerp(current, target, frequency*Time.delta);
                speed = current - previous;
                rotation = current;

                //Console.WriteLine("eye Left cr{0} tr{1}", current, target);
            }

            public static void SetTarget() {
                target = Stream.Head.rotation;
            }
        }

        public static void Update() {
            #region get from stream
            var dtc = Stream.Head.detected;
            detectedPrevious = detected;
            detected = dtc;
            if (dtc) {
                Position.SetTarget();
                Rotation.SetTarget();
            }
            #endregion

            Position.Update();
            Rotation.Update();
        }
    }
}
