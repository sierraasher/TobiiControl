using System;

namespace TobiiControl {
    public static class Time {
        public static DateTime current;
        public static DateTime previous;
        public static TimeSpan difference;
        public static float delta;

        public static int desiredFPS = 120;
        public static float desiredInterval = (int)(1000 / desiredFPS);
        public static int sleepInterval;

        public static void Update() {
            previous = current;
            current = DateTime.UtcNow;
            difference = current - previous;
            delta = (float)difference.Milliseconds / (1000f / 60.0f);
            sleepInterval = (int)(Math.Max(1.0, desiredInterval - difference.TotalMilliseconds + sleepInterval));
        }
    }
}
