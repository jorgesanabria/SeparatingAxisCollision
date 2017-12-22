#region using

using System;
using Microsoft.Xna.Framework;
using SeparatingAxisCollision.polygons;

#endregion

namespace SeparatingAxisCollision {
    public static class Collision {
        public static Vector2 CheckCollisionAndRespond(IPolygon a, IPolygon b) {
            if (a is Circle ac && b is Circle bc)
                return CheckCircleCollisionAndRespond(ac, bc);

            Vector2 mtv = Utils.LargeVector;

            var aNormals = a.GetNormals(b);
            var bNormals = b.GetNormals(a);

            //Check projections against b's normals and the overlap...
            foreach (Vector2 normal in bNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return Vector2.Zero;

                Vector2 overlap = aProjection.GetMinimumTranslation(bProjection) * normal;
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            //Check projection against a's normals and the overlap...
            foreach (Vector2 normal in aNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return Vector2.Zero;

                Vector2 overlap = aProjection.GetMinimumTranslation(bProjection) * normal;
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            return mtv;
        }

        public static Boolean CheckCollision(IPolygon a, IPolygon b) {
            if (a is Circle ac && b is Circle bc)
                return CheckCircleCollision(ac, bc);

            var aNormals = a.GetNormals(b);
            var bNormals = b.GetNormals(a);

            //Check projections against b's normals and the overlap...
            foreach (Vector2 normal in bNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return false;
            }

            //Check projection against a's normals and the overlap...
            foreach (Vector2 normal in aNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return false;
            }

            return true;
        }

        private static Vector2 CheckCircleCollisionAndRespond(Circle a, Circle b) {
            Vector2 axis = Utils.Axis(b.GetPosition() - a.GetPosition());
            Interval aProjection = a.GetProjection(axis);
            Interval bProjection = b.GetProjection(axis);
            if (!aProjection.Overlaps(bProjection))
                return Vector2.Zero;

            return aProjection.GetMinimumTranslation(bProjection) * axis;
        }

        private static Boolean CheckCircleCollision(Circle a, Circle b) {
            Vector2 axis = Utils.Axis(b.GetPosition() - a.GetPosition());
            return a.GetProjection(axis).Overlaps(b.GetProjection(axis));
        }
    }
}
