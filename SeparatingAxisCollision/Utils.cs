#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision {
    public static class Utils {
        /// <summary>
        ///     A vector whose length squared is float.MaxValue.
        /// </summary>
        public static Vector2 LargeVector = new Vector2((Single)Math.Pow(Single.MaxValue, 0.25),
            (Single)Math.Pow(Single.MaxValue, 0.25));

        /// <summary>
        ///     Returns true if a rectangle intersects, contains, or is contained by another.
        /// </summary>
        public static Boolean CollidesWith(this RectangleF a, RectangleF b) {
            return a.IntersectsWith(b) || a.Contains(b) || b.Contains(a);
        }

        /// <summary>
        ///     Scales a rectangle's center position away from origin.
        /// </summary>
        public static RectangleF ScaledPositionFromOrigin(this RectangleF a, Single scale) {
            RectangleF rect = a;
            Single centerX = rect.Left + (rect.Right - rect.Left) / 2;
            Single centerY = rect.Top + (rect.Bottom - rect.Top) / 2;
            rect.X = a.X + (centerX * scale - centerX);
            rect.Y = a.Y + (centerY * scale - centerY);
            return rect;
        }

        /// <summary>
        ///     Scales a rectangle's bounds from its own center.
        /// </summary>
        public static RectangleF ScaledFromCenter(this RectangleF a, Single scale) {
            RectangleF rect = a;
            rect.Width *= scale;
            rect.Height *= scale;
            rect.X -= (rect.Width - a.Width) / 2;
            rect.Y -= (rect.Height - a.Height) / 2;
            return rect;
        }

        /// <summary>
        ///     Rotates a point around the origin.
        /// </summary>
        /// <param name="input">The point to be rotated.</param>
        /// <param name="rotation">The angle of rotation in radians.</param>
        /// <returns>The result of the transformation.</returns>
        public static Vector2 RotateCentered(Vector2 input, Single rotation) {
            Single sin = (Single)Math.Sin(rotation);
            Single cos = (Single)Math.Cos(rotation);
            return new Vector2(input.X * cos - input.Y * sin, input.X * sin + input.Y * cos);
        }

        /// <summary>
        ///     Gets the axis between two points.
        /// </summary>
        /// <returns>A normalized axis vector.</returns>
        internal static Vector2 Axis(Vector2 a, Vector2 b) {
            return Axis(b - a);
        }

        /// <summary>
        ///     Gets the axis of a length vector.
        /// </summary>
        /// <returns>A normalized axis vector.</returns>
        internal static Vector2 Axis(Vector2 length) {
            if (length == Vector2.Zero)
                return Vector2.Zero;

            Vector2 ret = length;
            ret.Normalize();
            return ret;
        }

        /// <summary>
        ///     Gets a normal of an axis between two points.
        /// </summary>
        /// <param name="right">If true, returns the right-hand normal. If false, returns the left-hand normal.</param>
        /// <returns>A normalized normal axis vector.</returns>
        internal static Vector2 AxisNormal(Vector2 a, Vector2 b, Boolean right = true) {
            Vector2 axis = Axis(a, b);
            return right ? new Vector2(-axis.Y, axis.X) : new Vector2(axis.Y, -axis.X);
        }

        /// <summary>
        ///     Gets a normal of an axis of a length vector.
        /// </summary>
        /// <param name="right">If true, returns the right-hand normal. If false, returns the left-hand normal.</param>
        /// <returns>A normalized normal axis vector.</returns>
        internal static Vector2 AxisNormal(Vector2 length, Boolean right = true) {
            Vector2 axis = Axis(length);
            return right ? new Vector2(-axis.Y, axis.X) : new Vector2(axis.Y, -axis.X);
        }

        /// <summary>
        ///     Gets the dot product of two vectors.
        /// </summary>
        internal static Single DotProduct(Vector2 a, Vector2 b) {
            return a.X * b.X + a.Y * b.Y;
        }
    }
}
