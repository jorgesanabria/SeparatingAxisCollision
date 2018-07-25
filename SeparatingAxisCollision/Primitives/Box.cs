#region using

using System;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision.Primitives {
    /// <summary>
    ///     A rectangle used for collision.
    /// </summary>
    public sealed class Box : IPolygon {
        private readonly Double _boundingRadiusCache;
        private readonly Double _halfHeight;
        private readonly Double _halfWidth;
        private readonly Vector2D _offset;
        private readonly Vector2D[] _pointsUnitCache;
        private readonly Double _rotationOffset;
        private Boolean _isRotationDirty = true;

        private Vector2D _position;
        private Vector2D _positionUnitCache;
        private Double _rotation;
        private Double _scale;

        public Box(Double halfWidth, Double halfHeight, Double rotationOffset = 0f, Vector2D? offset = null,
            Vector2D? pos = null, Double rotation = 0f, Double scale = 1f) {
            _halfWidth = halfWidth;
            _halfHeight = halfHeight;
            _rotationOffset = rotationOffset;
            _offset = offset ?? Vector2D.Zero;

            _position = pos ?? Vector2D.Zero;
            _rotation = rotation;
            _scale = scale;

            _boundingRadiusCache = new Vector2D(halfWidth, halfHeight).Magnitude;
            _pointsUnitCache = new Vector2D[4];
        }

        public Vector2D GetPosition() {
            return GetPositionUnit() * _scale + _position;
        }

        public void SetPosition(Vector2D position) {
            _position = position;
        }

        public Double GetRotation() {
            return _rotation + _rotationOffset;
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
            var points = new Vector2D[GetPtsUnit().Length];
            for (Int32 a = 0; a < points.Length; a++)
                points[a] = _pointsUnitCache[a] * _scale + GetPosition();

            return points;
        }

        public Interval GetProjection(Vector2D axis) {
            var pts = GetPoints();
            Double start = Vector2D.Dot(axis, pts[0]);
            Interval proj = new Interval(start, start);
            for (Int32 a = 1; a < pts.Length; a++) {
                Double d = Vector2D.Dot(axis, pts[a]);
                if (d > proj.Max)
                    proj.Max = d;
                else if (d < proj.Min)
                    proj.Min = d;
            }

            return proj;
        }

        public Vector2D[] GetNormals(IPolygon other) {
            var pts = GetPoints();
            var axes = new Vector2D[2];

            axes[0] = Vector2D.AxisNormalLeft(pts[0], pts[1]);
            axes[1] = Vector2D.AxisNormalLeft(pts[1], pts[2]);

            return axes;
        }

        public RectD GetBoundingSquareUnit() {
            Double radius = _boundingRadiusCache;
            return new RectD(-radius, -radius, radius * 2, radius * 2);
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

        #region Private Methods

        /// <summary>
        ///     Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        private Vector2D GetPositionUnit() {
            if (!_isRotationDirty)
                return _positionUnitCache;

            _positionUnitCache = Vector2D.RotateCentered(_offset, GetRotation());
            _isRotationDirty = false;
            return _positionUnitCache;
        }

        private Vector2D[] GetPtsUnit() {
            if (!_isRotationDirty)
                return _pointsUnitCache;

            _pointsUnitCache[0] = Vector2D.RotateCentered(new Vector2D(-_halfWidth, _halfHeight), GetRotation());
            _pointsUnitCache[1] = Vector2D.RotateCentered(new Vector2D(_halfWidth, _halfHeight), GetRotation());
            _pointsUnitCache[2] = Vector2D.RotateCentered(new Vector2D(_halfWidth, -_halfHeight), GetRotation());
            _pointsUnitCache[3] = Vector2D.RotateCentered(new Vector2D(-_halfWidth, -_halfHeight), GetRotation());

            _isRotationDirty = false;

            return _pointsUnitCache;
        }

        #endregion
    }
}
