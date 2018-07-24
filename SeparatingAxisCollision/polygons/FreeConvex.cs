#region using

using System;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision.polygons {
    /// <summary>
    ///     A freeform variable-point polygon used for collision. Must be concave for collisions to work properly.
    /// </summary>
    public sealed class FreeConvex : IPolygon {
        private readonly Double _boundingRadiusCache;
        private readonly Vector2D[] _pointsUnitCache;
        public readonly Shape Shape;
        private Boolean _isRotationDirty = true;

        private Vector2D _position;
        private Double _rotation;
        private Double _scale;

        public FreeConvex(Shape shape, Vector2D? pos = null, Double rotation = 0f, Double scale = 1f) {
            Shape = shape;

            _position = pos ?? Vector2D.Zero;
            _rotation = rotation;
            _scale = scale;

            Double radius = Double.MinValue;
            for (Int32 a = 0; a < Shape.Points.Length; a++) {
                if (Shape.Points[a].SqrMagnitude > radius)
                    radius = Shape.Points[a].SqrMagnitude;
            }

            _boundingRadiusCache = Math.Sqrt(radius);
            _pointsUnitCache = new Vector2D[Shape.Points.Length];
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
            var points = new Vector2D[GetPtsUnit().Length];
            for (Int32 a = 0; a < points.Length; a++)
                points[a] = _pointsUnitCache[a] * _scale + _position;

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
            var cons = GetSides();
            var axes = new Vector2D[cons.Length];
            for (Int32 a = 0; a < cons.Length; a++)
                axes[a] = Vector2D.AxisNormalLeft(cons[a]);

            return axes;
        }

        public RectD GetBoundingSquareUnit() {
            Double radius = _boundingRadiusCache;
            return new RectD(-radius, -radius, radius * 2, radius * 2);
        }

        public RectD GetBoundingSquare() {
            RectD scaledRect = GetBoundingSquareUnit().ScaledFromCenter(_scale);
            scaledRect.Offset(_position.X, _position.Y);
            return scaledRect;
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

        private Vector2D[] GetPtsUnit() {
            if (!_isRotationDirty)
                return _pointsUnitCache;

            for (Int32 a = 0; a < _pointsUnitCache.Length; a++)
                _pointsUnitCache[a] = Vector2D.RotateCentered(Shape.Points[a], _rotation);

            _isRotationDirty = false;

            return _pointsUnitCache;
        }

        private Vector2D[] GetSides() {
            var ret = new Vector2D[Shape.Connections.Length];
            for (Int32 a = 0; a < Shape.Connections.Length; a++)
                ret[a] = Vector2D.RotateCentered(Shape.Connections[a], _rotation) * _scale;

            return ret;
        }

        #endregion
    }
}
