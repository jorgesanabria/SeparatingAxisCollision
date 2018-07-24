#region using

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Starry.Math {
    public struct Vector2D {
        public const Double K_EPSILON = 1E-05d;
        /// <summary>
        ///     A vector whose square length is pretty close to <see cref="Double.MaxValue" />.
        /// </summary>
        public static readonly Vector2D LargeVector =
            new Vector2D(MathD.Pow(Double.MaxValue, 0.25), MathD.Pow(Double.MaxValue, 0.25));
        public Double X;
        public Double Y;

        public Double this[Int32 index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
            set {
                switch (index) {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
        }

        public Vector2D Normalized {
            get {
                Vector2D vector2D = new Vector2D(X, Y);
                vector2D.Normalize();
                return vector2D;
            }
        }

        public Double Magnitude {
            get => MathD.Sqrt(X * X + Y * Y);
        }

        public Double SqrMagnitude {
            get => X * X + Y * Y;
        }

        public static Vector2D Zero {
            get => new Vector2D(0.0d, 0.0d);
        }

        public static Vector2D One {
            get => new Vector2D(1d, 1d);
        }

        public static Vector2D Up {
            get => new Vector2D(0.0d, 1d);
        }

        public static Vector2D Right {
            get => new Vector2D(1d, 0.0d);
        }

        public Vector2D(Double x, Double y) {
            X = x;
            Y = y;
        }

        public static Vector2D operator +(Vector2D a, Vector2D b) {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b) {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2D operator -(Vector2D a) {
            return new Vector2D(-a.X, -a.Y);
        }

        public static Vector2D operator *(Vector2D a, Double d) {
            return new Vector2D(a.X * d, a.Y * d);
        }

        public static Vector2D operator *(Double d, Vector2D a) {
            return new Vector2D(a.X * d, a.Y * d);
        }

        public static Vector2D operator /(Vector2D a, Double d) {
            return new Vector2D(a.X / d, a.Y / d);
        }

        public static Boolean operator ==(Vector2D lhs, Vector2D rhs) {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static Boolean operator !=(Vector2D lhs, Vector2D rhs) {
            return lhs.X != rhs.X || lhs.Y != rhs.Y;
        }

        public void Set(Double x, Double y) {
            X = x;
            Y = y;
        }

        public static Vector2D Lerp(Vector2D from, Vector2D to, Double t) {
            t = MathD.Clamp01(t);
            return new Vector2D(from.X + (to.X - from.X) * t, from.Y + (to.Y - from.Y) * t);
        }

        public static Vector2D MoveTowards(Vector2D current, Vector2D target, Double maxDistanceDelta) {
            Vector2D vector2 = target - current;
            Double magnitude = vector2.Magnitude;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (magnitude <= maxDistanceDelta || magnitude == 0.0d)
                return target;

            return current + vector2 / magnitude * maxDistanceDelta;
        }

        public static Vector2D Scale(Vector2D a, Vector2D b) {
            return new Vector2D(a.X * b.X, a.Y * b.Y);
        }

        public void Scale(Vector2D scale) {
            X *= scale.X;
            Y *= scale.Y;
        }

        public void Normalize() {
            Double magnitude = Magnitude;
            if (magnitude > 9.99999974737875E-06)
                this = this / magnitude;
            else
                this = Zero;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override Int32 GetHashCode() {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2);
        }

        public override Boolean Equals(Object other) {
            if (!(other is Vector2D))
                return false;

            Vector2D vector2D = (Vector2D)other;
            return X.Equals(vector2D.X) && Y.Equals(vector2D.Y);
        }

        public static Double Dot(Vector2D lhs, Vector2D rhs) {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        public static Double Angle(Vector2D from, Vector2D to) {
            return MathD.Acos(MathD.Clamp(Dot(from.Normalized, to.Normalized), -1d, 1d)) * 57.29578d;
        }

        public static Double Distance(Vector2D a, Vector2D b) {
            return (a - b).Magnitude;
        }

        public static Vector2D ClampMagnitude(Vector2D vector, Double maxLength) {
            if (vector.SqrMagnitude > maxLength * maxLength)
                return vector.Normalized * maxLength;

            return vector;
        }

        public static Vector2D Min(Vector2D lhs, Vector2D rhs) {
            return new Vector2D(MathD.Min(lhs.X, rhs.X), MathD.Min(lhs.Y, rhs.Y));
        }

        public static Vector2D Max(Vector2D lhs, Vector2D rhs) {
            return new Vector2D(MathD.Max(lhs.X, rhs.X), MathD.Max(lhs.Y, rhs.Y));
        }

        /// <summary>
        ///     Rotates a Vector2D around the origin, preserving its magnitude.
        /// </summary>
        public static Vector2D RotateCentered(Vector2D input, Double rotation) {
            Double sin = MathD.Sin(rotation);
            Double cos = MathD.Cos(rotation);
            return new Vector2D(input.X * cos - input.Y * sin, input.X * sin + input.Y * cos);
        }

        /// <summary>
        ///     Rotates a Vector2D around a specifiable axis, preserving its relative magnitude.
        /// </summary>
        public static Vector2D RotateAround(Vector2D input, Vector2D axisPoint, Double rotation) {
            return RotateCentered(input - axisPoint, rotation) + axisPoint;
        }

        /// <summary>
        ///     Gets the normalized axis between two points.
        /// </summary>
        public static Vector2D Axis(Vector2D a, Vector2D b) {
            return (b - a).Normalized;
        }

        /// <summary>
        ///     Gets the right normal of an axis between two points.
        /// </summary>
        /// >
        public static Vector2D AxisNormalRight(Vector2D a, Vector2D b) {
            Vector2D axis = Axis(a, b);
            return new Vector2D(-axis.Y, axis.X);
        }

        /// <summary>
        ///     Gets the right normal of a difference vector.
        /// </summary>
        public static Vector2D AxisNormalRight(Vector2D diff) {
            Vector2D axis = diff.Normalized;
            return new Vector2D(-axis.Y, axis.X);
        }

        /// <summary>
        ///     Gets the left normal of an axis between two points.
        /// </summary>
        public static Vector2D AxisNormalLeft(Vector2D a, Vector2D b) {
            Vector2D axis = Axis(a, b);
            return new Vector2D(axis.Y, -axis.X);
        }

        /// <summary>
        ///     Gets the left normal of a difference vector.
        /// </summary>
        public static Vector2D AxisNormalLeft(Vector2D diff) {
            Vector2D axis = diff.Normalized;
            return new Vector2D(axis.Y, -axis.X);
        }
    }
}
