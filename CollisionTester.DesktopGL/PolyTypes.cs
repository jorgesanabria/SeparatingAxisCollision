#region using

using System;
using Microsoft.Xna.Framework;
using SeparatingAxisCollision;
using SeparatingAxisCollision.polygons;
using Ray = SeparatingAxisCollision.polygons.Ray;

#endregion

namespace CollisionTester.DesktopGL {
    public static class PolyTypes {
        public const Byte BOX = 0;
        public const Byte CIRCLE = 1;
        public const Byte FREE_CONVEX = 2;
        public const Byte RAY = 3;

        public static Shape CreateRegularNShape(Byte sides) {
            if (sides < 3)
                sides = 3;

            var points = new Vector2[sides];
            for (Byte k = 1; k != sides + 1; k += 1) {
                points[k - 1] = new Vector2((Single)Math.Cos(2 * k * Math.PI / sides),
                    (Single)Math.Sin(2 * k * Math.PI / sides));
            }

            return new Shape(points);
        }

        /// <summary>
        ///     Helper function for the CollisionTester. Not recommended for production use due to DrawableCircles.
        /// </summary>
        public static IPolygon CreateDefaultPolygon(Byte polyType, Vector2? offset = null, Vector2? pos = null,
            Single rotation = 0f, Single scale = 1f) {
            if (offset == null)
                offset = Vector2.Zero;
            switch (polyType) {
                case BOX:
                    return new Box(0.5f, 0.5f, 0, offset, pos, rotation, scale);
                case CIRCLE:
                    return new Circle(0.5f, offset, pos, rotation, scale);
                case FREE_CONVEX:
                    // This will be an octagon.
                    return new FreeConvex(CreateRegularNShape(8), pos, rotation, scale);
                case RAY:
                    return new Ray(1f, pos, rotation, scale);
                default:
                    return new Box(0.5f, 0.5f, 0, offset, pos, rotation, scale);
            }
        }
    }
}
