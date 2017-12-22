#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision.polygons {
    /// <summary>
    ///     A freeform variable-point polygon used for collision. Must be concave for collisions to work properly.
    /// </summary>
    public sealed class FreeConvex : IPolygon {
        private readonly Single _boundingRadiusCache;
        private readonly Vector2[] _pointsUnitCache;
        public readonly Shape Shape;
        private Boolean _isRotationDirty = true;

        private Vector2 _position;
        private Single _rotation;
        private Single _scale;

        public FreeConvex(Shape shape, Vector2? pos = null, Single rotation = 0f, Single scale = 1f) {
            Shape = shape;

            _position = pos ?? Vector2.Zero;
            _rotation = rotation;
            _scale = scale;

            Single radius = Single.MinValue;
            for (Int32 a = 0; a < Shape.Points.Length; a++) {
                if (Shape.Points[a].LengthSquared() > radius)
                    radius = Shape.Points[a].LengthSquared();
            }

            _boundingRadiusCache = (Single)Math.Sqrt(radius);
            _pointsUnitCache = new Vector2[Shape.Points.Length];
        }

        public Vector2 GetPosition() {
            return _position;
        }

        public void SetPosition(Vector2 position) {
            _position = position;
        }

        public Single GetRotation() {
            return _rotation;
        }

        public void SetRotation(Single rotation) {
            _rotation = rotation;
            _isRotationDirty = true;
        }

        public Single GetScale() {
            return _scale;
        }

        public void SetScale(Single scale) {
            _scale = scale;
        }

        public Vector2[] GetPoints() {
            var points = new Vector2[GetPtsUnit().Length];
            for (Int32 a = 0; a < points.Length; a++)
                points[a] = _pointsUnitCache[a] * _scale + _position;

            return points;
        }

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

        public Vector2[] GetNormals(IPolygon other) {
            var cons = GetSides();
            var axes = new Vector2[cons.Length];
            for (Int32 a = 0; a < cons.Length; a++)
                axes[a] = Utils.AxisNormal(cons[a], false);

            return axes;
        }

        public RectangleF GetBoundingSquareUnit() {
            Single radius = _boundingRadiusCache;
            return new RectangleF(-radius, -radius, radius * 2, radius * 2);
        }

        public RectangleF GetBoundingSquare() {
            RectangleF scaledRect = GetBoundingSquareUnit().ScaledFromCenter(_scale);
            scaledRect.Offset(_position.X, _position.Y);
            return scaledRect;
        }

        public RectangleF GetBoundingBoxUnit() {
            var points = GetPtsUnit();
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

        public RectangleF GetBoundingBox() {
            RectangleF rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(_scale);
            rect = rect.ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
        }

        #region Private Methods

        private Vector2[] GetPtsUnit() {
            if (!_isRotationDirty)
                return _pointsUnitCache;

            for (Int32 a = 0; a < _pointsUnitCache.Length; a++)
                _pointsUnitCache[a] = Utils.RotateCentered(Shape.Points[a], _rotation);

            _isRotationDirty = false;

            return _pointsUnitCache;
        }

        private Vector2[] GetSides() {
            var ret = new Vector2[Shape.Connections.Length];
            for (Int32 a = 0; a < Shape.Connections.Length; a++)
                ret[a] = Utils.RotateCentered(Shape.Connections[a], _rotation) * _scale;

            return ret;
        }

        #endregion
    }
}
