#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision.polygons {
    /// <summary>
    ///     A representation of all the points away from a center, used for collision.
    /// </summary>
    public sealed class Circle : IPolygon {
        private readonly Single _boundingRadiusCache;
        public readonly Vector2 CenterOffset;
        public readonly Single Radius;
        private Boolean _isRotationDirty = true;

        private Vector2 _position;
        private Vector2 _positionUnitCache;
        private Single _rotation;
        private Single _scale;

        public Circle(Single radius, Vector2? offset = null, Vector2? pos = null, Single rotation = 0f,
            Single scale = 1f) {
            Radius = radius;
            CenterOffset = offset ?? Vector2.Zero;

            _position = pos ?? Vector2.Zero;
            _rotation = rotation;
            _scale = scale;

            _boundingRadiusCache = CenterOffset.Length() + radius;
        }

        public Vector2 GetPosition() {
            return GetPositionUnit() * _scale + _position;
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
            return new[] {GetPosition()};
        }

        public Interval GetProjection(Vector2 axis) {
            Single start = Utils.DotProduct(axis, GetPosition());
            return new Interval(start - GetRadius(), start + GetRadius());
        }

        public Vector2[] GetNormals(IPolygon other) {
            var points = other.GetPoints();
            var axes = new Vector2[points.Length];
            for (Int32 a = 0; a < axes.Length; a++)
                axes[a] = Utils.Axis(points[a] - GetPosition());

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
            Vector2 position = GetPositionUnit();
            return new RectangleF(position.X - Radius, position.Y - Radius, Radius * 2, Radius * 2);
        }

        public RectangleF GetBoundingBox() {
            RectangleF rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(_scale);
            rect = rect.ScaledFromCenter(_scale);
            rect.Offset(_position.X, _position.Y);
            return rect;
        }

        #region Private/Protected Methods

        /// <summary>
        ///     Gets the scaled radius of the Circle.
        /// </summary>
        protected Single GetRadius() {
            return Radius * _scale;
        }

        /// <summary>
        ///     Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        private Vector2 GetPositionUnit() {
            if (!_isRotationDirty)
                return new Vector2(_positionUnitCache.X, _positionUnitCache.Y);

            _positionUnitCache = Utils.RotateCentered(CenterOffset, _rotation);
            _isRotationDirty = false;
            return new Vector2(_positionUnitCache.X, _positionUnitCache.Y);
        }

        #endregion
    }
}
