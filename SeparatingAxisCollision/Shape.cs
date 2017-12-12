using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SeparatingAxisCollision
{
    /// <summary>
    /// Shape data only. While this allows creation of freeform shapes, it is designed with simple, convex shapes in mind.
    /// </summary>
    public struct Shape
    {
        /// <summary>
        /// Each vector represents an offset from the center.
        /// Example of connection (4 points): 0 to 1; 1 to 2; 2 to 3; 3 to 0;
        /// </summary>
        public readonly Vector2[] Points;

        /// <summary>
        /// Each vector represents a directional sidelength vector.
        /// Example of connection (4 points): 0 to 1; 1 to 2; 2 to 3; 3 to 0;
        /// </summary>
        public readonly Vector2[] Connections;

        public Shape(params Vector2[] points)
        {
            if (points == null || points.Length < 3)
                throw new NotSupportedException("Must pass a minimum of 3 points to create a Shape!");
            Points = points;
            Connections = new Vector2[points.Length];
            for (int a = 0; a < points.Length; a++)
            {
                if (a == points.Length - 1)
                    Connections[a] = points[0] - points[a];
                else
                    Connections[a] = points[a + 1] - points[a];
            }
        }

        /// <summary>
        /// Returns a mathematically similar shape with a bounding radius of 1.
        /// </summary>
        /// <returns></returns>
        public Shape Unit()
        {
            var r = BoundingRadius();
            var pts = new Vector2[Points.Length];
            for (int b = 0; b < pts.Length; b++)
                pts[b] = Points[b] / r;

            return new Shape(pts);
        }

        /// <summary>
        /// Gets the minimum radius of a bounding circle for this Shape.
        /// </summary>
        public float BoundingRadius()
        {
            float radius = 1.0f;
            for (int a = 0; a < Points.Length; a++)
                if (Points[a].Length() > radius)
                    radius = Points[a].Length();
            return radius;
        }

        /// <summary>
        /// Draws the Shape.
        /// </summary>
        /// <param name="pixel">A white pixel texture.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="position">The position of the center of the shape.</param>
        /// <param name="scale">The size of the shape.</param>
        /// <param name="rotation">The rotation of the shape in radians, counter-clockwise.</param>
        /// <param name="color">Color of the shape. If null, defaults to Gray.</param>
        public void Draw(Texture2D pixel, SpriteBatch spriteBatch, Vector2? position = null, float scale = 1.0f, float rotation = 0.0f, Color? color = null)
        {
            Vector2 center = position ?? Vector2.Zero;
            for (int a = 0; a < Connections.Length; a++)
            {
                Point pos = ((Utils.RotateCentered(Points[a], rotation) * scale) + center).ToPoint();
                int length = (int)(Connections[a].Length() * scale);
                float angle = (float)(Utils.Angle(Connections[a]) + rotation);
                pixel.DrawLine(spriteBatch, pos, length, angle, color);
            }
        }
    }
}
