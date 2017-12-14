#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace SeparatingAxisCollision {
    /// <summary>
    ///     A representation of all the points away from a center point. Has state data, like Polygon.
    /// </summary>
    public class Circle : IPolygon {
        private readonly Single _boundingRadiusCache;
        protected readonly Vector2 CenterOffset;
        protected readonly Single Radius;
        private Boolean _isRotationDirty = true;
        private Vector2 _positionUnitCache;

        private Single _rotation;
        public Vector2 Position;

        public Single Scale;

        public Circle(Single radius, Vector2? offset = null, Single scale = 1.0f, Single rotation = 0.0f,
            Vector2? pos = null) {
            Radius = radius;
            CenterOffset = offset ?? Vector2.Zero;
            Scale = scale;
            Rotation = rotation;
            Position = pos ?? Vector2.Zero;
            GetPositionUnit();
            _boundingRadiusCache = CenterOffset.Length() + radius;
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
        ///     Gets the position of the axial center of the Circle.
        /// </summary>
        public Vector2 GetPosition() {
            return GetPositionUnit() * Scale + Position;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an interval that represents the projection of this IPolygon onto passed axis.
        /// </summary>
        public Interval GetProjection(Vector2 axis) {
            Single start = Utils.DotProduct(axis, GetPosition());
            return new Interval(start - GetRadius(), start + GetRadius());
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the rotation-less bounding square of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingSquare() {
            RectangleF rect = GetBoundingSquareUnit().ScaledFromCenter(Scale);
            rect.Offset(Position.X, Position.Y);
            return rect;
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
            Vector2 position = GetPositionUnit();
            return new RectangleF(position.X - Radius, position.Y - Radius, Radius * 2, Radius * 2);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Checks for collision between this IPolygon and another, and returns a suggested Minimum Translation Vector for
        ///     projection-based resolution.
        ///     Vector2.Zero means there was no collision.
        /// </summary>
        public Vector2 CheckCollisionAndRespond(IPolygon other) {
            switch (other) {
                case Circle circle:
                    return CheckCollisionAndRespond(circle);
                case Polygon polygon:
                    return CheckCollisionAndRespond(polygon);
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
        ///     Gets the scaled radius of the Circle.
        /// </summary>
        public Single GetRadius() {
            return Radius * Scale;
        }

        /// <summary>
        ///     Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        public Vector2 GetPositionUnit() {
            if (_isRotationDirty) {
                _positionUnitCache = Utils.RotateCentered(CenterOffset, Rotation);
                _isRotationDirty = false;
            }
            return new Vector2(_positionUnitCache.X, _positionUnitCache.Y);
        }

        /// <summary>
        ///     Gets an array of the axes of this circle relative to a Polygon.
        /// </summary>
        public Vector2[] GetRelativeAxes(Polygon other) {
            return GetRelativeAxes(other.GetPoints());
        }

        /// <summary>
        ///     Gets an array of the axes of this circle relative to a set of points.
        /// </summary>
        public Vector2[] GetRelativeAxes(Vector2[] points) {
            var axes = new Vector2[points.Length];
            for (Int32 a = 0; a < axes.Length; a++)
                axes[a] = Utils.Axis(points[a] - GetPosition());

            return axes;
        }

        #region Private Methods

        private Vector2 CheckCollisionAndRespond(Polygon other) {
            Vector2 mtv = Utils.LargeVector;

            var thisAxes = GetRelativeAxes(other);
            var otherAxes = other.GetNormalAxes();

            for (Int32 a = 0; a < otherAxes.Length; a++) {
                Interval thisProj = GetProjection(otherAxes[a]);
                Interval otherProj = other.GetProjection(otherAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[a];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            for (Int32 b = 0; b < thisAxes.Length; b++) {
                Interval thisProj = GetProjection(thisAxes[b]);
                Interval otherProj = other.GetProjection(thisAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;

                Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[b];
                if (overlap.LengthSquared() < mtv.LengthSquared())
                    mtv = overlap;
            }

            return mtv;
        }

        private Vector2 CheckCollisionAndRespond(Circle other) {
            Vector2 axis = Utils.Axis(other.GetPosition() - GetPosition());
            Interval thisProj = GetProjection(axis);
            Interval otherProj = other.GetProjection(axis);
            if (!thisProj.Overlaps(otherProj))
                return Vector2.Zero;

            return thisProj.GetMinimumTranslation(otherProj) * axis;
        }

        private Boolean CheckCollision(Polygon other) {
            var thisAxes = GetRelativeAxes(other);
            var otherAxes = other.GetNormalAxes();

            for (Int32 a = 0; a < otherAxes.Length; a++) {
                Interval thisProj = GetProjection(otherAxes[a]);
                Interval otherProj = other.GetProjection(otherAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            for (Int32 b = 0; b < thisAxes.Length; b++) {
                Interval thisProj = GetProjection(thisAxes[b]);
                Interval otherProj = other.GetProjection(thisAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            return true;
        }

        private Boolean CheckCollision(Circle other) {
            Vector2 axis = Utils.Axis(other.GetPosition() - GetPosition());
            Interval thisProj = GetProjection(axis);
            Interval otherProj = other.GetProjection(axis);

            return thisProj.Overlaps(otherProj);
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     A wrapper over Circle containing fields and methods for drawing. Functions in Collision identically to Circle.
    /// </summary>
    public class DrawableCircle : Circle {
        private Boolean _isGraphicDirty = true;
        private Texture2D _texture;

        public DrawableCircle(Single radius, Vector2? offset = null, Single scale = 1.0f, Single rotation = 0.0f,
            Vector2? pos = null)
            : base(radius, offset, scale, rotation, pos) { }

        public new Single Scale {
            get => base.Scale;
            set {
                base.Scale = value;
                _isGraphicDirty = true;
            }
        }

        public new Single Rotation {
            get => base.Rotation;
            set {
                base.Rotation = value;
                _isGraphicDirty = true;
            }
        }

        /// <summary>
        ///     Draws an approximation of the Circle.
        /// </summary>
        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, Color? color = null) {
            if (!color.HasValue)
                color = Color.Gray;

            if (_isGraphicDirty) {
                _texture = CreateTexture(device, (Int32)(GetRadius() * 2));
                _isGraphicDirty = false;
            }
            spriteBatch.Draw(_texture, GetPosition(), color: color.Value,
                origin: new Vector2((Int32)GetRadius(), (Int32)GetRadius()));
        }

        #region Private Methods

        private Texture2D CreateTexture(GraphicsDevice device, Int32 diameter) {
            Texture2D texture = new Texture2D(device, diameter, diameter);
            var colorData = new Color[diameter * diameter];

            Single radius = diameter / 2f;
            Single radiusSqr = radius * radius;

            for (Int32 x = 0; x < diameter; x++) {
                for (Int32 y = 0; y < diameter; y++) {
                    Int32 index = x * diameter + y;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (pos.LengthSquared() <= radiusSqr)
                        colorData[index] = Color.White;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        #endregion
    }
}
