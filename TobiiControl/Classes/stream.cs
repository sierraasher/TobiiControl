using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Tobii.StreamEngine;

namespace TobiiControl {
    public static class Stream {
        #region variables

        #region program
        public static bool active = true;
        #endregion

        #region head stucts
        public static class Head {
            public static bool detected = false;
            public static Vector3 position;
            public static Vector3 rotation;
        }
        #endregion

        #region eye stucts
        public static class Eyes {
            public static class Gaze {
                public static bool detected = false;
                public static Vector2 position;
            }

            public static class Left {
                public static bool detected = false;
                public static Vector3 position;
            }

            public static class Right {
                public static bool detected = false;
                public static Vector3 position;
            }
        }
        #endregion

        #endregion

        #region main
        public static void Update() {
            #region setup
            IntPtr deviceContext;
            IntPtr apiContext;
            tobii_error_t result;

            #region api
            result = Interop.tobii_api_create(out apiContext, null);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            #endregion

            #region connect devive
            // Enumerate devices to find connected eye trackers
            List<string> urls;
            result = Interop.tobii_enumerate_local_device_urls(apiContext, out urls);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            if (urls.Count == 0) {
                Console.WriteLine("Error: No device found");
                return;
            }

            // Connect to the first tracker found
            result = Interop.tobii_device_create(apiContext, urls[0], Interop.tobii_field_of_use_t.TOBII_FIELD_OF_USE_INTERACTIVE, out deviceContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            #endregion

            #region subscribe
            // gaze
            result = Interop.tobii_gaze_point_subscribe(deviceContext, OnGazePoint);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);

            // origin
            result = Interop.tobii_gaze_origin_subscribe(deviceContext, OnGazeOrigin);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);

            // head
            result = Interop.tobii_head_pose_subscribe(deviceContext, OnHeadPose);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            #endregion

            #endregion

            #region update
            while (active) {
                #region Update stream
                // optionally block this thread until data is available. especially useful if running in a separate thread.
                Interop.tobii_wait_for_callbacks(new[] { deviceContext });
                Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR || result == tobii_error_t.TOBII_ERROR_TIMED_OUT);

                // Process callbacks on this thread if data is available
                Interop.tobii_device_process_callbacks(deviceContext);
                Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
                #endregion
            }
            #endregion

            #region cleanup

            #region unsubscribe
            // gaze point
            result = Interop.tobii_gaze_point_unsubscribe(deviceContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);

            // gaze origin
            result = Interop.tobii_gaze_origin_unsubscribe(deviceContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);

            // head
            result = Interop.tobii_head_pose_unsubscribe(deviceContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            #endregion

            #region destroy
            result = Interop.tobii_device_destroy(deviceContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);

            result = Interop.tobii_api_destroy(apiContext);
            Debug.Assert(result == tobii_error_t.TOBII_ERROR_NO_ERROR);
            #endregion

            #endregion
        }
        #endregion

        #region functions
        private static void OnHeadPose(ref tobii_head_pose_t headPoint, IntPtr userData) {
            // Check that the data is valid before using it
            if (headPoint.position_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                var p = headPoint.position_xyz;
                Head.position = new Vector3(p.x, p.y, p.z);
                Head.detected = true;
                //Console.WriteLine("Head Position x:{0} y:{1} z:{2}", headPoint.position_xyz.x, headPoint.position_xyz.y, headPoint.position_xyz.z);
            }
            else {
                Head.detected = false;
            }

            var r = headPoint.rotation_xyz;
            if (headPoint.rotation_x_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                Head.rotation.X = r.x;
                //Console.WriteLine("Head Rotation:{0}", headPoint.rotation_xyz.x);
            }
            if (headPoint.rotation_y_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                Head.rotation.Y = r.y;
                //Console.WriteLine("Head Rotation:{0}", headPoint.rotation_xyz.y);
            }
            if (headPoint.rotation_z_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                Head.rotation.Z = r.z;
                //Console.WriteLine("Head Rotation:{0}", headPoint.rotation_xyz.z);
            }
        }
        private static void OnGazePoint(ref tobii_gaze_point_t gazePoint, IntPtr userData) {
            //Check that the data is valid before using it
            if (gazePoint.validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                var p = gazePoint.position;
                Eyes.Gaze.position = new Vector2(p.x, p.y);
                Eyes.Gaze.detected = true;
                //Console.WriteLine($"Gaze point: {gazePoint.position.x}, {gazePoint.position.y}");
            }
            else {
                Eyes.Gaze.detected = false;
            }
        }
        private static void OnGazeOrigin(ref tobii_gaze_origin_t gazeOrigin, IntPtr userData) {
            if (gazeOrigin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                var p = gazeOrigin.left;
                Eyes.Left.position = new Vector3(p.x, p.y, p.z);
                Eyes.Left.detected = true;
                //Console.WriteLine("origin Left x:{0} y:{1} z:{2}", gazeOrigin.left.x, gazeOrigin.left.y, gazeOrigin.left.z);
            }
            else {
                Eyes.Left.detected = false;
            }
            if (gazeOrigin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID) {
                var p = gazeOrigin.right;
                Eyes.Right.position = new Vector3(p.x, p.y, p.z);
                Eyes.Right.detected = true;
                //Console.WriteLine("origin right x:{0} y:{1} z:{2}", gazeOrigin.right.x, gazeOrigin.right.y, gazeOrigin.right.z);
            }
            else {
                Eyes.Right.detected = false;
            }
        }
        #endregion
    }
}