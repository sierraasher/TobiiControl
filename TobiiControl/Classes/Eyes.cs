using System.Numerics;

namespace TobiiControl {
    public static class Eyes {
        static float frequencyDefault = 1f / 3f;

        public static class Gaze {
            public static bool detected = false;
            public static Vector2 position;

            public static class Position {
                public static Vector2 current;
                public static Vector2 speed;
                public static Vector2 previous;
                public static Vector2 target;
                static float frequency = frequencyDefault;

                public static void Update() {
                    previous = current;
                    if (frequency == 1) current = target;
                    else current = Vector2.Lerp(current, target, frequency*Time.delta);
                    speed = current - previous;
                    position = current;
                    //Console.WriteLine("eye gaze ps{0} tr{1}", current, target);
                }

                public static void SetTarget() {
                    target = Stream.Eyes.Gaze.position;
                }
            }
        }

        public static class Left {
            public static bool detected = false;
            public static Vector3 position;

            public static class Position {
                public static Vector3 target;
                static Vector3 current;
                static Vector3 previous;
                static Vector3 speed;
                static float frequency = frequencyDefault;

                public static void Update() {
                    previous = current;
                    if (frequency == 1) current = target;
                    else current = Vector3.Lerp(current, target, frequency*Time.delta);
                    speed = current - previous;
                    position = current;
                    //Console.WriteLine("eye Left ps{0} tr{1}", current, target);
                }

                public static void SetTarget() {
                    target = Stream.Eyes.Left.position;
                }
            }
        }

        public static class Right {
            public static bool detected = false;
            public static Vector3 position;
            public static class Position {
                static Vector3 current;
                static Vector3 previous;
                static Vector3 target;
                static Vector3 speed;
                static float frequency = frequencyDefault;

                public static void Update() {
                    previous = current;
                    if (frequency == 1) current = target;
                    else current = Vector3.Lerp(current, target, frequency*Time.delta);
                    speed = current - previous;
                    position = current;
                    //Console.WriteLine("eye Left ps{0} tr{1}", current, target);
                }

                public static void SetTarget() {
                    target = Stream.Eyes.Right.position;
                }
            }
        }

        public static void Update() {
            #region get from stream
            var dtc = Stream.Eyes.Gaze.detected;
            Gaze.detected = dtc;
            Left.detected = Stream.Eyes.Left.detected;
            Right.detected = Stream.Eyes.Right.detected;

            if (dtc) {
                Gaze.Position.SetTarget();
                Left.Position.SetTarget();
                Right.Position.SetTarget();
            }
            #endregion

            Gaze.Position.Update();
            Left.Position.Update();
            Right.Position.Update();
        }
    }
}
