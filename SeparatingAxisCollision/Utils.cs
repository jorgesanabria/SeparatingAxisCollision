#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

#endregion

namespace SeparatingAxisCollision {
    public static class Utils {
        /// <summary>
        ///     Returns a vector whose length squared is float.MaxValue.
        /// </summary>
        public static Vector2 LargeVector = new Vector2((Single)Math.Pow(Single.MaxValue, 0.25));

        /// <summary>
        ///     Generates a 1x1 white pixel from a GraphicsDevice.
        /// </summary>
        /// <param name="device">The GraphicsDevice to create the texture from.</param>
        public static Texture2D GeneratePixel(GraphicsDevice device) {
            Texture2D pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new Color[1] {Color.White});
            return pixel;
        }

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
        ///     Draws an approximation of the RectangleF.
        /// </summary>
        public static void Draw(this RectangleF rect, Texture2D pixel, SpriteBatch spriteBatch, Color? color = null) {
            pixel.DrawLine(spriteBatch, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top), color);
            pixel.DrawLine(spriteBatch, new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom), color);
            pixel.DrawLine(spriteBatch, new Vector2(rect.Right, rect.Bottom), new Vector2(rect.Left, rect.Bottom), color);
            pixel.DrawLine(spriteBatch, new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Left, rect.Top), color);
        }

        /// <summary>
        ///     Draws a basic approximate line between two points.
        /// </summary>
        /// <param name="pixel">A white pixel texture.</param>
        /// <param name="sb">A SpriteBatch used to draw with.</param>
        /// <param name="a">First point in line drawing.</param>
        /// <param name="b">Second point in line drawing.</param>
        /// <param name="color">Color of line. If null, defaults to Gray.</param>
        internal static void DrawLine(this Texture2D pixel, SpriteBatch sb, Vector2 a, Vector2 b, Color? color = null) {
            Point start = a.ToPoint();
            Point end = b.ToPoint();
            pixel.DrawLine(sb, start, (Int32)Math.Round(DifferenceLength(start, end)), (Single)Angle(start, end),
                color);
        }

        /// <summary>
        ///     Draws a basic approximate line.
        /// </summary>
        /// <param name="pixel">A white pixel texture.</param>
        /// <param name="spriteBatch">A SpriteBatch used to draw with.</param>
        /// <param name="start">Starting point in line drawing.</param>
        /// <param name="length">Length of line.</param>
        /// <param name="angle">Counter-clockwise angle in radians from y-axis.</param>
        /// <param name="color">Color of line. If null, defaults to Gray.</param>
        internal static void DrawLine(this Texture2D pixel, SpriteBatch spriteBatch, Point start, Int32 length,
            Single angle, Color? color = null) {
            if (!color.HasValue)
                color = Color.Gray;
            spriteBatch.Draw(pixel, destinationRectangle: new Rectangle(start.X, start.Y, length, 1), color: color,
                rotation: angle);
        }

        /// <summary>
        ///     Gets the length between two points.
        /// </summary>
        internal static Single DifferenceLength(Point a, Point b) {
            Vector2 diff = (a - b).ToVector2();
            return diff.Length();
        }

        /// <summary>
        ///     Gets the angle in radians from two points, from the y-axis.
        /// </summary>
        internal static Double Angle(Point a, Point b) {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        /// <summary>
        ///     Gets the angle from reference point to target point, based on the y-axis.
        /// </summary>
        /// <param name="reference">The reference point of the angle measurement.</param>
        /// <param name="target">The target point of the angle measurement.</param>
        /// <returns>Angle in radians.</returns>
        internal static Double Angle(Vector2 reference, Vector2 target) {
            return Angle(target - reference);
        }

        /// <summary>
        ///     Gets the angle from the y-axis.
        /// </summary>
        /// <param name="length">The length from origin that is being compared.</param>
        /// <returns>Angle in radians.</returns>
        internal static Double Angle(Vector2 length) {
            return Math.Atan2(length.Y, length.X);
        }

        /// <summary>
        ///     Rotates a point around another.
        /// </summary>
        /// <param name="input">The point to be rotated.</param>
        /// <param name="center">The center of rotation.</param>
        /// <param name="rotation">The angle of rotation in radians.</param>
        /// <returns>The result of the transformation.</returns>
        internal static Vector2 RotateAround(Vector2 input, Vector2 center, Single rotation) {
            Single sin = (Single)Math.Sin(rotation);
            Single cos = (Single)Math.Cos(rotation);

            Vector2 ret = input - center;

            ret.X = ret.X * cos - ret.Y * sin;
            ret.Y = ret.X * sin + ret.Y * cos;

            ret += center;
            return ret;
        }

        /// <summary>
        ///     Rotates a point around the origin.
        /// </summary>
        /// <param name="input">The point to be rotated.</param>
        /// <param name="rotation">The angle of rotation in radians.</param>
        /// <returns>The result of the transformation.</returns>
        internal static Vector2 RotateCentered(Vector2 input, Single rotation) {
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
