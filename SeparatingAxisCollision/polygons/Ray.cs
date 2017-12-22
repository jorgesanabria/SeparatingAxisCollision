#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision.polygons {
    public sealed class Ray : IPolygon {
        private readonly Single _length;
        private Vector2 _endPointUnitCache;

        private Boolean _isRotationDirty = true;

        private Vector2 _position;
        private Single _rotation;
        private Single _scale;

        public Ray(Single length, Vector2? pos = null, Single rotation = 0f, Single scale = 1f) {
            _length = length;
            _position = pos ?? Vector2.Zero;
            _rotation = rotation;
            _scale = scale;
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
            return new[] {
                _position,
                _endPointUnitCache * _scale + _position
            };
        }

        public Interval GetProjection(Vector2 axis) {
            var pts = GetPoints();
            Single start = Utils.DotProduct(axis, pts[0]);
            Interval proj = new Interval(start, start);
            Single d = Utils.DotProduct(axis, pts[1]);

            if (d > proj.Max)
                proj.Max = d;
            else if (d < proj.Min)
                proj.Min = d;

            return proj;
        }

        public Vector2[] GetNormals(IPolygon other) {
            var pts = GetPoints();
            var axes = new Vector2[2];

            axes[0] = Utils.Axis(pts[1] - pts[0]);
            axes[1] = Utils.AxisNormal(pts[1] - pts[0], false);

            return axes;
        }

        public RectangleF GetBoundingSquareUnit() {
            return new RectangleF(-_length, -_length, _length * 2, _length * 2);
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

        private Vector2 GetEndPointUnit() {
            if (!_isRotationDirty)
                return _endPointUnitCache;

            _endPointUnitCache = Utils.RotateCentered(Vector2.UnitX, _rotation);
            _isRotationDirty = false;
            return _endPointUnitCache;
        }

        private Vector2[] GetPtsUnit() {
            return new[] {
                Vector2.Zero,
                GetEndPointUnit()
            };
        }

        #endregion
    }
}
