#region using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeparatingAxisCollision;
using SeparatingAxisCollision.Primitives;
using Starry.Math;
using Ray = SeparatingAxisCollision.Primitives.Ray;

#endregion

namespace CollisionTester.DesktopGL {
    internal class PolyDrawer {
        private readonly Texture2D _pixel;
        internal readonly GraphicsDevice Device;
        internal readonly SpriteBatch SpriteBatch;

        // Circle things
        private Double _lastScale;
        private Double _lastVisibleRadius;
        private Texture2D _texture;
        internal IPolygon Polygon;

        internal PolyDrawer(GraphicsDevice device, SpriteBatch spriteBatch, Texture2D pixel, IPolygon polygon) {
            Device = device;
            SpriteBatch = spriteBatch;
            _pixel = pixel;
            Polygon = polygon;

            if (polygon is Circle c) {
                _lastScale = c.GetScale();
                _lastVisibleRadius = c.Radius * _lastScale;
                _texture = CreateTexture();
            }
        }

        /// <summary>
        ///     Draws an approximation of the RectD.
        /// </summary>
        internal static void Draw(RectD rect, Texture2D pixel, SpriteBatch spriteBatch, Color? color = null) {
            DrawLine(pixel, spriteBatch, new Vector2D(rect.Left, rect.Top), new Vector2D(rect.Right, rect.Top), color);
            DrawLine(pixel, spriteBatch, new Vector2D(rect.Right, rect.Top), new Vector2D(rect.Right, rect.Bottom),
                color);
            DrawLine(pixel, spriteBatch, new Vector2D(rect.Right, rect.Bottom), new Vector2D(rect.Left, rect.Bottom),
                color);
            DrawLine(pixel, spriteBatch, new Vector2D(rect.Left, rect.Bottom), new Vector2D(rect.Left, rect.Top),
                color);
        }

        /// <summary>
        ///     Draws a basic approximate line between two points.
        /// </summary>
        /// <param name="pixel">A white pixel texture.</param>
        /// <param name="sb">A SpriteBatch used to draw with.</param>
        /// <param name="a">First point in line drawing.</param>
        /// <param name="b">Second point in line drawing.</param>
        /// <param name="color">Color of line. If null, defaults to Gray.</param>
        private static void DrawLine(Texture2D pixel, SpriteBatch sb, Vector2D a, Vector2D b, Color? color = null) {
            DrawLine(pixel, sb, ToPoint(a), (Int32)Math.Round((a - b).Magnitude),
                (Single)Math.Atan2(b.Y - a.Y, b.X - a.X),
                color);
        }

        private static Point ToPoint(Vector2D vec) {
            return new Point((Int32)vec.X, (Int32)vec.Y);
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
        private static void DrawLine(Texture2D pixel, SpriteBatch spriteBatch, Point start, Int32 length,
            Single angle, Color? color = null) {
            if (!color.HasValue)
                color = Color.Gray;
            spriteBatch.Draw(pixel, destinationRectangle: new Rectangle(start.X, start.Y, length, 1), color: color,
                rotation: angle);
        }

        internal void Draw(Color? color = null) {
            if (!color.HasValue)
                color = Color.Gray;

            switch (Polygon) {
                case Box box:
                    var boxPts = box.GetPoints();
                    DrawLine(_pixel, SpriteBatch, boxPts[0], boxPts[1], color);
                    DrawLine(_pixel, SpriteBatch, boxPts[1], boxPts[2], color);
                    DrawLine(_pixel, SpriteBatch, boxPts[2], boxPts[3], color);
                    DrawLine(_pixel, SpriteBatch, boxPts[3], boxPts[0], color);
                    break;
                case Circle circle:
                    if (_lastScale != circle.GetScale()) {
                        _lastScale = circle.GetScale();
                        _lastVisibleRadius = circle.Radius * _lastScale;
                        _texture = CreateTexture();
                    }
                    Vector2D circPos = circle.GetPosition();
                    SpriteBatch.Draw(_texture, new Vector2((Single)circPos.X, (Single)circPos.Y), color: color.Value,
                        origin: new Vector2((Int32)_lastVisibleRadius, (Int32)_lastVisibleRadius));
                    break;
                case FreeConvex poly:
                    Vector2D center = poly.GetPosition();
                    for (Int32 a = 0; a < poly.Shape.Connections.Length; a++) {
                        Point pos = ToPoint(Vector2D.RotateCentered(poly.Shape.Points[a], poly.GetRotation())
                                            * poly.GetScale()
                                            + center);
                        Int32 length = (Int32)(poly.Shape.Connections[a].Magnitude * poly.GetScale());
                        Single angle = (Single)(Math.Atan2(poly.Shape.Connections[a].Y, poly.Shape.Connections[a].X)
                                                + poly.GetRotation());
                        DrawLine(_pixel, SpriteBatch, pos, length, angle, color);
                    }

                    break;
                case Ray ray:
                    var rayPts = ray.GetPoints();
                    DrawLine(_pixel, SpriteBatch, rayPts[0], rayPts[1], color);
                    break;
            }
        }

        private Texture2D CreateTexture() {
            Int32 diameter = (Int32)(_lastVisibleRadius * 2);
            Single radius = diameter / 2f;
            Single radiusSqr = radius * radius;

            Texture2D texture = new Texture2D(Device, diameter, diameter);
            var colorData = new Color[diameter * diameter];

            for (Int32 x = 0; x < diameter; x++) {
                for (Int32 y = 0; y < diameter; y++) {
                    Int32 index = x * diameter + y;
                    Vector2D pos = new Vector2D(x - radius, y - radius);
                    if (pos.SqrMagnitude <= radiusSqr)
                        colorData[index] = Color.White;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
