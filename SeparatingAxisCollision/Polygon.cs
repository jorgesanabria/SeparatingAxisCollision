#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace SeparatingAxisCollision {
    /// <summary>
    ///     A shape with state data.
    /// </summary>
    public partial class Polygon : IPolygon {
        private readonly Single _boundingRadiusCache;
        public readonly Shape Shape;
        private Boolean _isRotationDirty = true;
        private readonly Vector2[] _pointsUnitCache;

        private Single _rotation;
        public Vector2 Position;

        public Single Scale;

        public Polygon(Shape shape, Single scale = 1.0f, Single rotation = 0.0f, Vector2? pos = null) {
            Shape = shape;
            Scale = scale;
            Rotation = rotation;
            Position = pos ?? Vector2.Zero;
            _pointsUnitCache = new Vector2[Shape.Points.Length];

            Single radius = Single.MinValue;
            for (Int32 a = 0; a < Shape.Points.Length; a++) {
                if (Shape.Points[a].LengthSquared() > radius)
                    radius = Shape.Points[a].LengthSquared();
            }

            _boundingRadiusCache = (Single)Math.Sqrt(radius);
        }

        public Single Rotation {
            get => _rotation;
            set {
                _rotation = value;
                _isRotationDirty = true;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the position of the axial center of the polygon.
        /// </summary>
        public Vector2 GetPosition() {
            return Position;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an interval that represents the projection of this Polygon onto passed axis.
        /// </summary>
        public Interval GetProjection(Vector2 axis) {
            var pts = GetPoints();
            Single start = Utils.DotProduct(axis, pts[0]);
            Interval proj = new Interval(start, start);
            for (Int32 a = 1; a < pts.Length; a++) {
                Single d = Utils.DotProduct(axis, pts[a]);
                if (d > proj.Max)
                    proj.Max = d;
                else if (d < proj.Min)
                    proj.Min = d;
            }

            return proj;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the rotation-less bounding square of this Polygon.
        /// </summary>
        public RectangleF GetBoundingSquare() {
            RectangleF scaledRect = GetBoundingSquareUnit().ScaledFromCenter(Scale);
            scaledRect.Offset(Position.X, Position.Y);
            return scaledRect;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the unscaled, untranslated rotation-less bounding square of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingSquareUnit() {
            Single radius = _boundingRadiusCache;
            return new RectangleF(-radius, -radius, radius * 2, radius * 2);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the minimum bounding box of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingBox() {
            RectangleF rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(Scale);
            rect = rect.ScaledFromCenter(Scale);
            rect.Offset(Position.X, Position.Y);
            return rect;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the unscaled, untranslated bounding box of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingBoxUnit() {
            var points = GetPointsUnit();
            Single yMin = Single.MaxValue;
            Single yMax = Single.MinValue;
            Single xMin = Single.MaxValue;
            Single xMax = Single.MinValue;
            for (Int32 a = 0; a < points.Length; a++) {
                if (points[a].Y < yMin)
                    yMin = points[a].Y;
                if (points[a].Y > yMax)
                    yMax = points[a].Y;
                if (points[a].X < xMin)
                    xMin = points[a].X;
                if (points[a].X > xMax)
                    xMax = points[a].X;
            }

            return new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Checks for collision between this Polygon and another IPolygon, and returns a suggested Minimum Translation Vector
        ///     for projection-based resolution.
        ///     Vector2.Zero means there was no collision.
        /// </summary>
        public Vector2 CheckCollisionAndRespond(IPolygon other) {
            switch (other) {
                case Polygon polygon:
                    return CheckCollisionAndRespond(polygon);
                case Circle circle:
                    return CheckCollisionAndRespond(circle);
            }

            throw new NotImplementedException("Collision with this type of IPolygon is not handled!");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Checks for collision between this IPolygon and another. Returns true if they collide.
        /// </summary>
        public Boolean CheckCollision(IPolygon other) {
            switch (other) {
                case Polygon polygon:
                    return CheckCollision(polygon);
                case Circle circle:
                    return CheckCollision(circle);
            }

            throw new NotImplementedException("Collision with this type of IPolygon is not handled!");
        }

        /// <summary>
        ///     Returns a mathematically similar Polygon with a bounding radius of 1.
        /// </summary>
        public Polygon Unit() {
            return new Polygon(Shape.Unit());
        }

        /// <summary>
        ///     Returns a mathematically congruent Polygon with a unit shape.
        /// </summary>
        public Polygon UnitCongruent() {
            return new Polygon(Shape.Unit(), Shape.BoundingRadius() * Scale, Rotation, Position);
        }

        /// <summary>
        ///     Gets the minimum radius of a bounding circle, or the minimum any-rotation halfwidth of a bounding box for this
        ///     Polygon.
        /// </summary>
        public Single BoundingRadius() {
            return Shape.BoundingRadius() * Scale;
        }

        /// <summary>
        ///     Draws a pixel approximation of the Polygon.
        /// </summary>
        public void Draw(Texture2D pixel, SpriteBatch spriteBatch, Color? color = null) {
            Shape.Draw(pixel, spriteBatch, Position, Scale, Rotation, color);
        }

        /// <summary>
        ///     Gets array of each untranslated, unrotated point in the Polygon.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetPointsUnit() {
            if (_isRotationDirty) {
                for (Int32 a = 0; a < _pointsUnitCache.Length; a++)
                    _pointsUnitCache[a] = Utils.RotateCentered(Shape.Points[a], Rotation);

                _isRotationDirty = false;
            }

            return _pointsUnitCache;
        }

        /// <summary>
        ///     Gets an array of the fully-transformed points of the Polygon.
        /// </summary>
        public Vector2[] GetPoints() {
            var points = new Vector2[GetPointsUnit().Length];
            for (Int32 a = 0; a < points.Length; a++)
                points[a] = _pointsUnitCache[a] * Scale + Position;

            return points;
        }

        /// <summary>
        ///     Gets an array of the fully-transformed directional sidelengths of the Polygon.
        /// </summary>
        public Vector2[] GetConnections() {
            var ret = new Vector2[Shape.Connections.Length];
            for (Int32 a = 0; a < Shape.Connections.Length; a++)
                ret[a] = Utils.RotateCentered(Shape.Connections[a], Rotation) * Scale;

            return ret;
        }

        /// <summary>
        ///     Gets an array of each axis normal of each side of this Polygon.
        /// </summary>
        public Vector2[] GetNormalAxes() {
            var cons = GetConnections();
            var axes = new Vector2[cons.Length];
            for (Int32 a = 0; a < cons.Length; a++)
                axes[a] = Utils.AxisNormal(cons[a], false);

            return axes;
        }

        #region Private Methods

        private Vector2 CheckCollisionAndRespond(Polygon other) {
            Vector2 mtv = Utils.LargeVector;
            var thisAxes = GetNormalAxes();
            for (Int32 a = 0; a < thisAxes.Length; a++) {
                Interval thisProj = GetProjection(thisAxes[a]);
                Interval otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[a];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            var otherAxes = other.GetNormalAxes();
            for (Int32 b = 0; b < otherAxes.Length; b++) {
                Interval thisProj = GetProjection(otherAxes[b]);
                Interval otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[b];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            return mtv;
        }

        private Vector2 CheckCollisionAndRespond(Circle other) {
            Vector2 mtv = Utils.LargeVector;

            var thisAxes = GetNormalAxes();
            for (Int32 a = 0; a < thisAxes.Length; a++) {
                Interval thisProj = GetProjection(thisAxes[a]);
                Interval otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[a];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            var otherAxes = other.GetRelativeAxes(this);
            for (Int32 b = 0; b < otherAxes.Length; b++) {
                Interval thisProj = GetProjection(otherAxes[b]);
                Interval otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[b];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            return mtv;
        }

        private Boolean CheckCollision(Polygon other) {
            var thisAxes = GetNormalAxes();
            for (Int32 a = 0; a < thisAxes.Length; a++) {
                Interval thisProj = GetProjection(thisAxes[a]);
                Interval otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            var otherAxes = other.GetNormalAxes();
            for (Int32 b = 0; b < otherAxes.Length; b++) {
                Interval thisProj = GetProjection(otherAxes[b]);
                Interval otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            return true;
        }

        private Boolean CheckCollision(Circle other) {
            var thisAxes = GetNormalAxes();
            for (Int32 a = 0; a < thisAxes.Length; a++) {
                Interval thisProj = GetProjection(thisAxes[a]);
                Interval otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            var otherAxes = other.GetRelativeAxes(this);
            for (Int32 b = 0; b < otherAxes.Length; b++) {
                Interval thisProj = GetProjection(otherAxes[b]);
                Interval otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
