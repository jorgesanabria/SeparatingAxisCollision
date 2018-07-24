#region using

using System;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision.polygons {
    /// <summary>
    ///     A representation of all the points away from a center, used for collision.
    /// </summary>
    public sealed class Circle : IPolygon {
        private readonly Double _boundingRadiusCache;
        public readonly Vector2D CenterOffset;
        public readonly Double Radius;
        private Boolean _isRotationDirty = true;

        private Vector2D _position;
        private Vector2D _positionUnitCache;
        private Double _rotation;
        private Double _scale;

        public Circle(Double radius, Vector2D? offset = null, Vector2D? pos = null, Double rotation = 0f,
            Double scale = 1f) {
            offset = offset ?? Vector2D.Zero;
            pos = pos ?? Vector2D.Zero;

            Radius = radius;
            CenterOffset = offset.Value;

            _position = pos.Value;
            _rotation = rotation;
            _scale = scale;

            _boundingRadiusCache = offset.Value.Magnitude + radius;
        }

        public Vector2D GetPosition() {
            return GetPositionUnit() * _scale + _position;
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
            return new[] {GetPosition()};
        }

        public Interval GetProjection(Vector2D axis) {
            Double start = Vector2D.Dot(axis, GetPosition());
            return new Interval(start - GetRadius(), start + GetRadius());
        }

        public Vector2D[] GetNormals(IPolygon other) {
            var points = other.GetPoints();
            var axes = new Vector2D[points.Length];
            for (Int32 a = 0; a < axes.Length; a++)
                axes[a] = Vector2D.Axis(GetPosition(), points[a]);

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
            Vector2D position = GetPositionUnit();
            return new RectD(position.X - Radius, position.Y - Radius, Radius * 2, Radius * 2);
        }

        public RectD GetBoundingBox() {
            RectD rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(_scale);
            rect = rect.ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
        }

        #region Private/Protected Methods

        /// <summary>
        ///     Gets the scaled radius of the Circle.
        /// </summary>
        private Double GetRadius() {
            return Radius * _scale;
        }

        /// <summary>
        ///     Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        private Vector2D GetPositionUnit() {
            if (!_isRotationDirty)
                return new Vector2D(_positionUnitCache.X, _positionUnitCache.Y);

            _positionUnitCache = Vector2D.RotateCentered(CenterOffset, _rotation);
            _isRotationDirty = false;
            return _positionUnitCache;
        }

        #endregion
    }
}
