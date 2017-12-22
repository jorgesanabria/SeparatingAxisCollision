#region using

using System;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision {
    /// <summary>
    ///     Shape data only. While this allows creation of freeform shapes, it is designed with simple, convex shapes in mind.
    /// </summary>
    public struct Shape {
        /// <summary>
        ///     Each vector represents an offset from the center.
        ///     Example of connection (4 points): 0 to 1; 1 to 2; 2 to 3; 3 to 0;
        /// </summary>
        public readonly Vector2[] Points;

        /// <summary>
        ///     Each vector represents a directional sidelength vector.
        ///     Example of connection (4 points): 0 to 1; 1 to 2; 2 to 3; 3 to 0;
        /// </summary>
        public readonly Vector2[] Connections;

        public Shape(params Vector2[] points) {
            if (points == null || points.Length < 3)
                throw new NotSupportedException("Must pass a minimum of 3 points to create a Shape!");

            Points = points;
            Connections = new Vector2[points.Length];
            for (Int32 a = 0; a < points.Length; a++) {
                if (a == points.Length - 1)
                    Connections[a] = points[0] - points[a];
                else
                    Connections[a] = points[a + 1] - points[a];
            }
        }

        /// <summary>
        ///     Returns a mathematically similar shape with a bounding radius of 1.
        /// </summary>
        /// <returns></returns>
        public Shape Unit() {
            Single r = BoundingRadius();
            var pts = new Vector2[Points.Length];
            for (Int32 b = 0; b < pts.Length; b++)
                pts[b] = Points[b] / r;

            return new Shape(pts);
        }

        /// <summary>
        ///     Gets the minimum radius of a bounding circle for this Shape.
        /// </summary>
        public Single BoundingRadius() {
            Single radius = 1.0f;
            for (Int32 a = 0; a < Points.Length; a++) {
                if (Points[a].Length() > radius)
                    radius = Points[a].Length();
            }

            return radius;
        }
    }
}
