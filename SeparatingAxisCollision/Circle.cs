using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using RectangleF = System.Drawing.RectangleF;

namespace SeparatingAxisCollision
{
    /// <summary>
    /// A representation of all the points away from a center point. Has state data, like Polygon.
    /// </summary>
    public class Circle : IPolygon
    {
        protected readonly float _radius;
        protected readonly Vector2 _centerOffset;

        public float Scale;
        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                _isRotationDirty = true;
            }
        }
        public Vector2 Position;

        private float _rotation;
        private readonly float _boundingRadiusCache;
        private Vector2 _positionUnitCache;
        private bool _isRotationDirty = true;

        public Circle(float radius, Vector2? offset = null, float scale = 1.0f, float rotation = 0.0f, Vector2? pos = null)
        {
            _radius = radius;
            _centerOffset = offset ?? Vector2.Zero;
            Scale = scale;
            Rotation = rotation;
            Position = pos ?? Vector2.Zero;
            GetPositionUnit();
            _boundingRadiusCache = _centerOffset.Length() + radius;
        }

        /// <summary>
        /// Gets the scaled radius of the Circle.
        /// </summary>
        public float GetRadius()
        {
            return _radius * Scale;
        }

        /// <summary>
        /// Gets the unscaled, untranslated center position of the Circle.
        /// </summary>
        public Vector2 GetPositionUnit()
        {
            if (_isRotationDirty)
            {
                _positionUnitCache = Utils.RotateCentered(_centerOffset, Rotation);
                _isRotationDirty = false;
            }
            return new Vector2(_positionUnitCache.X, _positionUnitCache.Y);
        }

        /// <summary>
        /// Gets the position of the axial center of the Circle.
        /// </summary>
        public Vector2 GetPosition()
        {
            return (GetPositionUnit() * Scale) + Position;
        }

        /// <summary>
        /// Gets an array of the axes of this circle relative to a Polygon.
        /// </summary>
        public Vector2[] GetRelativeAxes(Polygon other)
        {
            return GetRelativeAxes(other.GetPoints());
        }

        /// <summary>
        /// Gets an array of the axes of this circle relative to a set of points.
        /// </summary>
        public Vector2[] GetRelativeAxes(Vector2[] points)
        {
            var axes = new Vector2[points.Length];
            for (int a = 0; a < axes.Length; a++)
                axes[a] = Utils.Axis(points[a] - GetPosition());
            return axes;
        }

        /// <summary>
        /// Returns an interval that represents the projection of this IPolygon onto passed axis.
        /// </summary>
        public Interval GetProjection(Vector2 axis)
        {
            var start = Utils.DotProduct(axis, GetPosition());
            return new Interval(start - GetRadius(), start + GetRadius());
        }

        /// <summary>
        /// Gets the rotation-less bounding square of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingSquare()
        {
            var rect = GetBoundingSquareUnit().ScaledFromCenter(Scale);
            rect.Offset(Position.X, Position.Y);
            return rect;
        }

        /// <summary>
        /// Gets the unscaled, untranslated rotation-less bounding square of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingSquareUnit()
        {
            var radius = _boundingRadiusCache;
            return new RectangleF(-radius, -radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// Gets the minimum bounding box of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingBox()
        {
            var rect = GetBoundingBoxUnit();
            rect = rect.ScaledPositionFromOrigin(Scale);
            rect = rect.ScaledFromCenter(Scale);
            rect.Offset(Position.X, Position.Y);
            return rect;
        }

        /// <summary>
        /// Gets the unscaled, untranslated bounding box of this IPolygon.
        /// </summary>
        public RectangleF GetBoundingBoxUnit()
        {
            var position = GetPositionUnit();
            return new RectangleF(position.X - _radius, position.Y - _radius, _radius * 2, _radius * 2);
        }

        /// <summary>
        /// Checks for collision between this IPolygon and another, and returns a suggested Minimum Translation Vector for projection-based resolution.
        /// Vector2.Zero means there was no collision.
        /// </summary>
        public Vector2 CheckCollisionAndRespond(IPolygon other)
        {
            if (other is Circle)
                return CheckCollisionAndRespond(other as Circle);
            else if (other is Polygon)
                return CheckCollisionAndRespond(other as Polygon);
            throw new NotImplementedException("Collision with this type of IPolygon is not handled!");
        }

        /// <summary>
        /// Checks for collision between this IPolygon and another. Returns true if they collide.
        /// </summary>
        public bool CheckCollision(IPolygon other)
        {
            if (other is Polygon)
                return CheckCollision(other as Polygon);
            else if (other is Circle)
                return CheckCollision(other as Circle);
            throw new NotImplementedException("Collision with this type of IPolygon is not handled!");
        }

        #region Private Methods
        private Vector2 CheckCollisionAndRespond(Polygon other)
        {
            var mtv = Utils.LargeVector;

            var thisAxes = GetRelativeAxes(other);
            var otherAxes = other.GetNormalAxes();

            for (int a = 0; a < otherAxes.Length; a++)
            {
                var thisProj = GetProjection(otherAxes[a]);
                var otherProj = other.GetProjection(otherAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[a];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }

            for (int b = 0; b < thisAxes.Length; b++)
            {
                var thisProj = GetProjection(thisAxes[b]);
                var otherProj = other.GetProjection(thisAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[b];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }

            return mtv;
        }

        private Vector2 CheckCollisionAndRespond(Circle other)
        {
            var axis = Utils.Axis(other.GetPosition() - GetPosition());
            var thisProj = GetProjection(axis);
            var otherProj = other.GetProjection(axis);
            if (!thisProj.Overlaps(otherProj))
                return Vector2.Zero;
            else
                return thisProj.GetMinimumTranslation(otherProj) * axis;
        }

        private bool CheckCollision(Polygon other)
        {
            var thisAxes = GetRelativeAxes(other);
            var otherAxes = other.GetNormalAxes();

            for (int a = 0; a < otherAxes.Length; a++)
            {
                var thisProj = GetProjection(otherAxes[a]);
                var otherProj = other.GetProjection(otherAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            for (int b = 0; b < thisAxes.Length; b++)
            {
                var thisProj = GetProjection(thisAxes[b]);
                var otherProj = other.GetProjection(thisAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }
            return true;
        }

        private bool CheckCollision(Circle other)
        {
            var axis = Utils.Axis(other.GetPosition() - GetPosition());
            var thisProj = GetProjection(axis);
            var otherProj = other.GetProjection(axis);

            return thisProj.Overlaps(otherProj);
        }
        #endregion
    }

    /// <summary>
    /// A wrapper over Circle containing fields and methods for drawing. Functions in Collision identically to Circle.
    /// </summary>
    public class DrawableCircle : Circle, IPolygon
    {
        public new float Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                base.Scale = value;
                isGraphicDirty = true;
            }
        }
        public new float Rotation
        {
            get
            {
                return base.Rotation;
            }
            set
            {
                base.Rotation = value;
                isGraphicDirty = true;
            }
        }
        private bool isGraphicDirty = true;
        private Texture2D Texture;

        public DrawableCircle(float radius, Vector2? offset = null, float scale = 1.0f, float rotation = 0.0f, Vector2? pos = null)
            : base(radius, offset, scale, rotation, pos)
        {
        }

        /// <summary>
        /// Draws an approximation of the Circle.
        /// </summary>
        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, Color? color = null)
        {
            if (!color.HasValue)
                color = Color.Gray;

            if (isGraphicDirty)
            {
                Texture = CreateTexture(device, (int)(GetRadius() * 2));
                isGraphicDirty = false;
            }
            spriteBatch.Draw(Texture, GetPosition(), color: color.Value, origin: new Vector2((int)GetRadius(), (int)GetRadius()));
        }

        #region Private Methods
        private Texture2D CreateTexture(GraphicsDevice device, int diameter)
        {
            Texture2D texture = new Texture2D(device, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            float radius = diameter / 2f;
            float radiusSqr = radius * radius;

            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    int index = x * diameter + y;
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
