#region using

using System;
using SeparatingAxisCollision.Primitives;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision {
    public static class Collision {
        public static Vector2D CheckCollisionAndRespond(IPolygon a, IPolygon b) {
            if (a is Circle ac && b is Circle bc)
                return CheckCircleCollisionAndRespond(ac, bc);

            Vector2D mtv = Vector2D.LargeVector;

            var aNormals = a.GetNormals(b);
            var bNormals = b.GetNormals(a);

            //Check projections against b's normals and the overlap...
            foreach (Vector2D normal in bNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return Vector2D.Zero;

                Vector2D overlap = aProjection.GetMinimumTranslation(bProjection) * normal;
                if (overlap.SqrMagnitude < mtv.SqrMagnitude)
                    mtv = overlap;
            }

            //Check projection against a's normals and the overlap...
            foreach (Vector2D normal in aNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return Vector2D.Zero;

                Vector2D overlap = aProjection.GetMinimumTranslation(bProjection) * normal;
                if (overlap.SqrMagnitude < mtv.SqrMagnitude)
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
            foreach (Vector2D normal in bNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return false;
            }

            //Check projection against a's normals and the overlap...
            foreach (Vector2D normal in aNormals) {
                Interval aProjection = a.GetProjection(normal);
                Interval bProjection = b.GetProjection(normal);

                if (!aProjection.Overlaps(bProjection))
                    return false;
            }

            return true;
        }

        private static Vector2D CheckCircleCollisionAndRespond(Circle a, Circle b) {
            Vector2D axis = Vector2D.Axis(a.GetPosition(), b.GetPosition());
            Interval aProjection = a.GetProjection(axis);
            Interval bProjection = b.GetProjection(axis);
            if (!aProjection.Overlaps(bProjection))
                return Vector2D.Zero;

            return aProjection.GetMinimumTranslation(bProjection) * axis;
        }

        private static Boolean CheckCircleCollision(Circle a, Circle b) {
            Vector2D axis = Vector2D.Axis(a.GetPosition(), b.GetPosition());
            return a.GetProjection(axis).Overlaps(b.GetProjection(axis));
        }
    }
}
