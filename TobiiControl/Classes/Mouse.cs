using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace TobiiControl {
    public static class Mouse {
        #region input
        public static class Hotkey {
            static int keyCode = (int)Keys.CapsLock;
            public static bool down = false;

            public static void Update() {
                var inDn = TobiiControl.Input.Keyboard.key[keyCode].down;
                down = inDn;
            }
        }
        #endregion

        #region click
        public static class Click {
            public static bool down = false;
            public static int keyCode = (int)Keys.RControlKey;

            public static void Update() {
                var clk = Input.Keyboard.key[keyCode].down;
                if (down) {
                    if (!clk) {
                        var x = Cursor.Position.X;
                        var y = Cursor.Position.Y;

                        Input.Mouse.mouse_event(Input.Mouse.MOUSEEVENTF_LEFTUP, x, y, 0, (UIntPtr)0);
                        down = false;
                    }
                }
                else {
                    if (clk) {
                        var x = Cursor.Position.X;
                        var y = Cursor.Position.Y;

                        Input.Mouse.mouse_event(Input.Mouse.MOUSEEVENTF_LEFTDOWN, x, y, 0, (UIntPtr)0);
                        down = true;
                    }
                }
            }
        }
        #endregion

        #region drag
        public static class Drag {
            public static bool active = false;
            public static bool update = false;

            static float radiusMin = 16;
            static float radiusMax = 64;

            public static class Position {
                public static Vector2 current;
                public static Vector2 target;
                public static Vector2 speed;
                private static Vector2 previous;

                public static bool Update() {
                    bool rtn = false;
                    if (current != target) {
                        #region prepare
                        var vCu = current;
                        var vTr = target;
                        var rdMn = radiusMin;
                        var rdMx = radiusMax;
                        var vDf = vTr - vCu;
                        #endregion

                        #region subtract min radius
                        var vNm = Vector2.Normalize(vDf);
                        var vMn = vNm * rdMn;
                        vDf -= vMn;
                        #endregion

                        #region amount
                        var dst = vDf.Length();
                        if (dst >= 0.01f) {
                            var nrm = MathE.Normal(dst, rdMn, rdMx, true);
                            var vPls = vDf * nrm;
                            var spd = vPls * Time.delta;

                            speed = spd;
                            previous = current;
                            current += spd;
                        }
                        else {
                            speed = Vector2.Zero;
                            previous = current = target;
                        }
                        #endregion

                        //Debug.WriteLine("Drag current{0} speed{1}", current, speed);
                    }

                    return rtn;
                }
                public static void SetTarget() {
                    if (Head.detected) {
                        #region prepare
                        var tX = target.X;
                        var tY = target.Y;
                        #endregion

                        #region reset
                        tX = 0;
                        tY = 0;
                        #endregion

                        #region add position
                        var pA = 4;
                        var pX = Head.position.X * pA;
                        var pY = -Head.position.Y * pA;
                        tX += pX;
                        tY += pY;
                        #endregion

                        #region add head roation
                        var rA = 1000;
                        var rX = -Head.rotation.Y * rA;
                        var rY = -Head.rotation.X * rA;
                        tX += rX;
                        tY += rY;
                        #endregion

                        #region set
                        target.X = tX;
                        target.Y = tY;
                        #endregion

                        #region reset
                        if (!Head.detectedPrevious) {
                            Reset();
                        }
                        #endregion
                    }
                }

                public static void Reset() {
                    previous = current = target;
                    speed = Vector2.Zero;
                }
            }

            public static void Update() {
                #region set target
                if (active) {
                    if (Hotkey.down) {
                        update = true;
                        Position.SetTarget();
                    }
                    else {
                        active = false;
                        Position.Reset();
                    }
                }
                else {
                    if (Hotkey.down) {
                        active = true;
                        update = true;

                        Position.SetTarget();
                        Position.Reset();
                    }
                }
                #endregion

                if (update) {
                    if (!Position.Update()) update = false;
                }
            }
        }

        #endregion

        #region move
        public static class Move {
            #region variable
            static bool update = false;
            static bool active = false;
            static Enum type = Types.Linear;

            enum Types {
                Linear,
                Power,
                number
            }

            public static Vector2 speed;
            public static float speedMin = 1 / 10f;
            static float friction = 1 / 10f;
            static Vector2 amount;
            static Vector2 move;
            #endregion

            #region functions
            public static void Update() {
                #region active
                if (active) {
                    if (Hotkey.down) {

                    }
                    else {
                        active = false;
                    }
                }
                else {
                    if (Hotkey.down) {
                        active = true;
                        update = true;
                        Reset();
                    }
                }
                #endregion

                if (update) {
                    if (active) DragToSpeed();
                    else {
                        if (!SpeedSlide()) update = false;
                    }
                    SpeedToAmount();
                    AmountToMove();
                    MoveCursor();
                }
            }

            static void DragToSpeed() {
                var spd = Drag.Position.speed;
                if (spd != Vector2.Zero) {
                    var typ = type;

                    var cntr = TobiiControl.Input.Keyboard.key[(int)Keys.LControlKey].down;
                    var lSf = TobiiControl.Input.Keyboard.key[(int)Keys.RShiftKey].down;
                    var rSf = TobiiControl.Input.Keyboard.key[(int)Keys.LShiftKey].down;

                    if (cntr) {
                        spd /= 2;
                        if (lSf) {
                            spd /= 2;
                        }
                        if (rSf) {
                            spd /= 2;
                        }
                    }
                    else {
                        if (lSf) {
                            spd *= 2;
                        }
                        if (rSf) {
                            spd *= 2;
                        }
                    }

                    if (TobiiControl.Input.Keyboard.key[(int)Keys.LMenu].down) {
                        typ = Types.Power;
                    }

                    switch (typ) {
                        #region Linear
                        case Types.Linear:
                            speed = spd;
                            break;
                        #endregion

                        #region power
                        case Types.Power:
                            var nrm = Vector2.Normalize(spd);
                            var lng = spd.Length();
                            var pwr = 1.5f;
                            lng = (float)Math.Pow(lng, pwr);
                            //lng = MathE.Clamp(lng, 0, 64.0F);
                            speed = nrm * lng;
                            break;
                        #endregion

                        #region default
                        default:

                            break;
                            #endregion
                    }
                }
                else {
                    speed = Vector2.Zero;
                }
            }

            static bool SpeedSlide() {
                var rtn = false;
                if (speed != Vector2.Zero) {
                    var dst = speed.Length();
                    if (dst >= speedMin) {
                        speed -= speed * friction * Time.delta;
                        rtn = true;
                    }
                    else {
                        speed = Vector2.Zero;
                    }
                }
                return rtn;
            }

            static void SpeedToAmount() {
                amount += speed;
            }

            static void AmountToMove() {
                #region X
                var amX = amount.X;
                var abX = Math.Abs(amX);
                if (abX >= 1f) {
                    var sgn = Math.Sign(amX);
                    var flr = Math.Floor(abX);
                    var pls = (int)flr * sgn;
                    move.X += pls;
                    amount.X -= pls;
                }
                #endregion

                #region Y
                var amY = amount.Y;
                var abY = Math.Abs(amY);
                if (abY >= 1) {
                    var sgn = Math.Sign(amY);
                    var flr = Math.Floor(abY);
                    var pls = (int)flr * sgn;
                    move.Y += pls;
                    amount.Y -= pls;
                }
                #endregion
            }

            static void MoveCursor() {
                if (move.X != 0
                || move.Y != 0) {
                    var cP = Cursor.Position;
                    var cX = cP.X;
                    var cY = cP.Y;
                    var offX = 0;
                    var offY = 0;

                    if (move.X != 0) {
                        offX = (int)move.X;
                        cX += (int)move.X;
                        move.X = 0;
                    }

                    if (move.Y != 0) {
                        offY = (int)move.Y;
                        cY += (int)move.Y;
                        move.Y = 0;
                    }

                    Cursor.Position = new System.Drawing.Point(cX, cY);
                }
            }

            static void Reset() {
                speed = Vector2.Zero;
                amount = Vector2.Zero;
                move = Vector2.Zero;
            }
            #endregion
        }
        #endregion

        #region update
        public static void Update() {
            Hotkey.Update();
            Drag.Update();
            Move.Update();
            Click.Update();
        }
        #endregion
    }
}
