#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision.polygons {
    /// <summary>
    ///     A rectangle used for collision.
    /// </summary>
    public sealed class Box : IPolygon {
        private readonly Single _boundingRadiusCache;
        private readonly Single _halfHeight;
        private readonly Single _halfWidth;
        private readonly Vector2 _offset;
        private readonly Vector2[] _pointsUnitCache;
        private readonly Single _rotationOffset;
        private Boolean _isRotationDirty = true;

        private Vector2 _position;
        private Vector2 _positionUnitCache;
        private Single _rotation;
        private Single _scale;

        public Box(Single halfWidth, Single halfHeight, Single rotationOffset = 0f, Vector2? offset = null,
            Vector2? pos = null, Single rotation = 0f, Single scale = 1f) {
            _halfWidth = halfWidth;
            _halfHeight = halfHeight;
            _rotationOffset = rotationOffset;
            _offset = offset ?? Vector2.Zero;

            _position = pos ?? Vector2.Zero;
            _rotation = rotation;
            _scale = scale;

            _boundingRadiusCache = new Vector2(halfWidth, halfHeight).Length();
            _pointsUnitCache = new Vector2[4];
        }

        public Vector2 GetPosition() {
            return GetPositionUnit() * _scale + _position;
        }

        public void SetPosition(Vector2 position) {
            _position = position;
        }

        public Single GetRotation() {
            return _rotation + _rotationOffset;
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
                points[a] = _pointsUnitCache[a] * _scale + GetPosition();

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
            var pts = GetPoints();
            var axes = new Vector2[2];

            axes[0] = Utils.AxisNormal(pts[1] - pts[0], false);
            axes[1] = Utils.AxisNormal(pts[2] - pts[1], false);

            return axes;
        }

        public RectangleF GetBoundingSquareUnit() {
            Single radius = _boundingRadiusCache;
            return new RectangleF(-radius, -radius, radius * 2, radius * 2);
        }

        public RectangleF GetBoundingSquare() {
            RectangleF rect = GetBoundingSquareUnit().ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
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

        /// <summary>
        ///     Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        private Vector2 GetPositionUnit() {
            if (!_isRotationDirty)
                return _positionUnitCache;

            _positionUnitCache = Utils.RotateCentered(_offset, GetRotation());
            _isRotationDirty = false;
            return new Vector2(_positionUnitCache.X, _positionUnitCache.Y);
        }

        private Vector2[] GetPtsUnit() {
            if (!_isRotationDirty)
                return _pointsUnitCache;

            _pointsUnitCache[0] = Utils.RotateCentered(new Vector2(-_halfWidth, _halfHeight), GetRotation());
            _pointsUnitCache[1] = Utils.RotateCentered(new Vector2(_halfWidth, _halfHeight), GetRotation());
            _pointsUnitCache[2] = Utils.RotateCentered(new Vector2(_halfWidth, -_halfHeight), GetRotation());
            _pointsUnitCache[3] = Utils.RotateCentered(new Vector2(-_halfWidth, -_halfHeight), GetRotation());

            _isRotationDirty = false;

            return _pointsUnitCache;
        }

        #endregion
    }
}
