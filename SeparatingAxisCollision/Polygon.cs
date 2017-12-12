using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using RectangleF = System.Drawing.RectangleF;

namespace SeparatingAxisCollision
{
    /// <summary>
    /// A shape with state data.
    /// </summary>
    public partial class Polygon : IPolygon
    {
        public readonly Shape Shape;

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
        private Vector2[] _pointsUnitCache;
        private bool _isRotationDirty = true;

        public Polygon(Shape shape, float scale = 1.0f, float rotation = 0.0f, Vector2? pos = null)
        {
            Shape = shape;
            Scale = scale;
            Rotation = rotation;
            Position = pos ?? Vector2.Zero;
            _pointsUnitCache = new Vector2[Shape.Points.Length];

            var radius = float.MinValue;
            for (int a = 0; a < Shape.Points.Length; a++)
            {
                if (Shape.Points[a].LengthSquared() > radius)
                    radius = Shape.Points[a].LengthSquared();
            }
            _boundingRadiusCache = (float)Math.Sqrt(radius);
        }

        /// <summary>
        /// Returns a mathematically similar Polygon with a bounding radius of 1.
        /// </summary>
        public Polygon Unit()
        {
            return new Polygon(Shape.Unit());
        }

        /// <summary>
        /// Returns a mathematically congruent Polygon with a unit shape.
        /// </summary>
        public Polygon UnitCongruent()
        {
            return new Polygon(Shape.Unit(), Shape.BoundingRadius() * Scale, Rotation, Position);
        }

        /// <summary>
        /// Gets the minimum radius of a bounding circle, or the minimum any-rotation halfwidth of a bounding box for this Polygon.
        /// </summary>
        public float BoundingRadius()
        {
            return Shape.BoundingRadius() * Scale;
        }

        /// <summary>
        /// Draws a pixel approximation of the Polygon.
        /// </summary>
        public void Draw(Texture2D pixel, SpriteBatch spriteBatch, Color? color = null)
        {
            Shape.Draw(pixel, spriteBatch, Position, Scale, Rotation, color);
        }

        /// <summary>
        /// Gets array of each untranslated, unrotated point in the Polygon.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetPointsUnit()
        {
            if (_isRotationDirty)
            {
                for (int a = 0; a < _pointsUnitCache.Length; a++)
                    _pointsUnitCache[a] = Utils.RotateCentered(Shape.Points[a], Rotation);
                _isRotationDirty = false;
            }
            return _pointsUnitCache;
        }

        /// <summary>
        /// Gets the position of the axial center of the polygon.
        /// </summary>
        public Vector2 GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Gets an array of the fully-transformed points of the Polygon.
        /// </summary>
        public Vector2[] GetPoints()
        {
            var points = new Vector2[GetPointsUnit().Length];
            for (int a = 0; a < points.Length; a++)
                points[a] = (_pointsUnitCache[a] * Scale) + Position;
            return points;
        }

        /// <summary>
        /// Gets an array of the fully-transformed directional sidelengths of the Polygon.
        /// </summary>
        public Vector2[] GetConnections()
        {
            var ret = new Vector2[Shape.Connections.Length];
            for (int a = 0; a < Shape.Connections.Length; a++)
                ret[a] = Utils.RotateCentered(Shape.Connections[a], Rotation) * Scale;
            return ret;
        }

        /// <summary>
        /// Returns an interval that represents the projection of this Polygon onto passed axis.
        /// </summary>
        public Interval GetProjection(Vector2 axis)
        {
            var pts = GetPoints();
            var start = Utils.DotProduct(axis, pts[0]);
            var proj = new Interval(start, start);
            for (int a = 1; a < pts.Length; a++)
            {
                var d = Utils.DotProduct(axis, pts[a]);
                if (d > proj.Max)
                    proj.Max = d;
                else if (d < proj.Min)
                    proj.Min = d;
            }
            return proj;
        }

        /// <summary>
        /// Gets the rotation-less bounding square of this Polygon.
        /// </summary>
        public RectangleF GetBoundingSquare()
        {
            var scaledRect = GetBoundingSquareUnit().ScaledFromCenter(Scale);
            scaledRect.Offset(Position.X, Position.Y);
            return scaledRect;
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
            var points = GetPointsUnit();
            var yMin = float.MaxValue;
            var yMax = float.MinValue;
            var xMin = float.MaxValue;
            var xMax = float.MinValue;
            for (int a = 0; a < points.Length; a++)
            {
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

        /// <summary>
        /// Gets an array of each axis normal of each side of this Polygon.
        /// </summary>
        public Vector2[] GetNormalAxes()
        {
            var cons = GetConnections();
            var axes = new Vector2[cons.Length];
            for (int a = 0; a < cons.Length; a++)
                axes[a] = Utils.AxisNormal(cons[a], false);
            return axes;
        }

        /// <summary>
        /// Checks for collision between this Polygon and another IPolygon, and returns a suggested Minimum Translation Vector for projection-based resolution.
        /// Vector2.Zero means there was no collision.
        /// </summary>
        public Vector2 CheckCollisionAndRespond(IPolygon other)
        {
            if (other is Polygon)
                return CheckCollisionAndRespond(other as Polygon);
            else if (other is Circle)
                return CheckCollisionAndRespond(other as Circle);
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
            var thisAxes = GetNormalAxes();
            for (int a = 0; a < thisAxes.Length; a++)
            {
                var thisProj = GetProjection(thisAxes[a]);
                var otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[a];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }

            var otherAxes = (other as Polygon).GetNormalAxes();
            for (int b = 0; b < otherAxes.Length; b++)
            {
                var thisProj = GetProjection(otherAxes[b]);
                var otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    Vector2 overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[b];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }

            return mtv;
        }

        private Vector2 CheckCollisionAndRespond(Circle other)
        {
            var mtv = Utils.LargeVector;

            var thisAxes = GetNormalAxes();
            for (int a = 0; a < thisAxes.Length; a++)
            {
                var thisProj = GetProjection(thisAxes[a]);
                var otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    var overlap = thisProj.GetMinimumTranslation(otherProj) * thisAxes[a];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }

            var otherAxes = (other as Circle).GetRelativeAxes(this);
            for (int b = 0; b < otherAxes.Length; b++)
            {
                var thisProj = GetProjection(otherAxes[b]);
                var otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return Vector2.Zero;
                else
                {
                    var overlap = thisProj.GetMinimumTranslation(otherProj) * otherAxes[b];
                    if (overlap.LengthSquared() < mtv.LengthSquared())
                        mtv = overlap;
                }
            }
            return mtv;
        }

        private bool CheckCollision(Polygon other)
        {
            var thisAxes = GetNormalAxes();
            for (int a = 0; a < thisAxes.Length; a++)
            {
                var thisProj = GetProjection(thisAxes[a]);
                var otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            var otherAxes = (other as Polygon).GetNormalAxes();
            for (int b = 0; b < otherAxes.Length; b++)
            {
                var thisProj = GetProjection(otherAxes[b]);
                var otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }
            return true;
        }

        private bool CheckCollision(Circle other)
        {
            var thisAxes = GetNormalAxes();
            for (int a = 0; a < thisAxes.Length; a++)
            {
                var thisProj = GetProjection(thisAxes[a]);
                var otherProj = other.GetProjection(thisAxes[a]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }

            var otherAxes = (other as Circle).GetRelativeAxes(this);
            for (int b = 0; b < otherAxes.Length; b++)
            {
                var thisProj = GetProjection(otherAxes[b]);
                var otherProj = other.GetProjection(otherAxes[b]);

                if (!thisProj.Overlaps(otherProj))
                    return false;
            }
            return true;
        }
        #endregion
    }
}
