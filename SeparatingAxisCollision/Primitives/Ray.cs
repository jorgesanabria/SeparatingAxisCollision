#region using

using System;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision.Primitives {
    public sealed class Ray : IPolygon {
        private readonly Double _length;
        private Vector2D _endPointUnitCache;

        private Boolean _isRotationDirty = true;

        private Vector2D _position;
        private Double _rotation;
        private Double _scale;

        public Ray(Double length, Vector2D? pos = null, Double rotation = 0f, Double scale = 1f) {
            _length = length;
            _position = pos ?? Vector2D.Zero;
            _rotation = rotation;
            _scale = scale;
        }

        public Vector2D GetPosition() {
            return _position;
        }

        public void SetPosition(Vector2D position) {
            _position = position;
        }

        public Double GetRotation() {
            return _rotation;
        }

        public void SetRotation(Double rotation) {
            _rotation = rotation;
            _isRotationDirty = true;
        }

        public Double GetScale() {
            return _scale;
        }

        public void SetScale(Double scale) {
            _scale = scale;
        }

        public Vector2D[] GetPoints() {
            return new[] {
                _position,
                _endPointUnitCache * _scale + _position
            };
        }

        public Interval GetProjection(Vector2D axis) {
            var pts = GetPoints();
            Double start = Vector2D.Dot(axis, pts[0]);
            Interval proj = new Interval(start, start);
            Double d = Vector2D.Dot(axis, pts[1]);

            if (d > proj.Max)
                proj.Max = d;
            else if (d < proj.Min)
                proj.Min = d;

            return proj;
        }

        public Vector2D[] GetNormals(IPolygon other) {
            var pts = GetPoints();
            var axes = new Vector2D[2];

            axes[0] = Vector2D.Axis(pts[0], pts[1]);
            axes[1] = Vector2D.AxisNormalLeft(pts[0], pts[1]);

            return axes;
        }

        public RectD GetBoundingSquareUnit() {
            return new RectD(-_length, -_length, _length * 2, _length * 2);
        }

        public RectD GetBoundingSquare() {
            RectD rect = GetBoundingSquareUnit().ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
        }

        public RectD GetBoundingBoxUnit() {
            var points = GetPtsUnit();
            Double yMin = Double.MaxValue;
            Double yMax = Double.MinValue;
            Double xMin = Double.MaxValue;
            Double xMax = Double.MinValue;
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

            return new RectD(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public RectD GetBoundingBox() {
            RectD rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(_scale);
            rect = rect.ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
        }

        public IPolygon Clone() {
            return new Ray(_length, _position, _rotation, _scale);
        }

        public void Translate(Vector2D translation) {
            _position += translation;
        }

        public void Rotate(Double angle) {
            _rotation += angle;
            _isRotationDirty = true;
        }

        public void AddScale(Double amount) {
            _scale += amount;
        }

        public void MultiplyScale(Double factor) {
            _scale *= factor;
        }

        #region Private Methods

        private Vector2D GetEndPointUnit() {
            if (!_isRotationDirty)
                return _endPointUnitCache;

            _endPointUnitCache = Vector2D.RotateCentered(Vector2D.Right, _rotation);
            _isRotationDirty = false;
            return _endPointUnitCache;
        }

        private Vector2D[] GetPtsUnit() {
            return new[] {
                Vector2D.Zero,
                GetEndPointUnit()
            };
        }

        #endregion
    }
}
