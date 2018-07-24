#region using

using System;

#endregion

namespace Starry.Math {
    public struct MathD {
        public const Double PI = 3.141593d;
        public const Double INFINITY = Double.PositiveInfinity;
        public const Double NEGATIVE_INFINITY = Double.NegativeInfinity;

        public const Double DEG_TO_RAD = 0.01745329d;
        public const Double RAD_TO_DEG = 57.29578d;
        public const Double EPSILON = 1.401298E-45d;

        public static Double Sin(Double d) {
            return System.Math.Sin(d);
        }

        public static Double Cos(Double d) {
            return System.Math.Cos(d);
        }

        public static Double Tan(Double d) {
            return System.Math.Tan(d);
        }

        public static Double Asin(Double d) {
            return System.Math.Asin(d);
        }

        public static Double Acos(Double d) {
            return System.Math.Acos(d);
        }

        public static Double Atan(Double d) {
            return System.Math.Atan(d);
        }

        public static Double Atan2(Double y, Double x) {
            return System.Math.Atan2(y, x);
        }

        public static Double Sqrt(Double d) {
            return System.Math.Sqrt(d);
        }

        public static Double Abs(Double d) {
            return System.Math.Abs(d);
        }

        public static Int32 Abs(Int32 value) {
            return System.Math.Abs(value);
        }

        public static Double Min(Double a, Double b) {
            if (a < b)
                return a;

            return b;
        }

        public static Double Min(params Double[] values) {
            Int32 length = values.Length;
            if (length == 0)
                return 0.0d;

            Double num = values[0];
            for (Int32 index = 1; index < length; ++index) {
                if (values[index] < num)
                    num = values[index];
            }

            return num;
        }

        public static Int32 Min(Int32 a, Int32 b) {
            if (a < b)
                return a;

            return b;
        }

        public static Int32 Min(params Int32[] values) {
            Int32 length = values.Length;
            if (length == 0)
                return 0;

            Int32 num = values[0];
            for (Int32 index = 1; index < length; ++index) {
                if (values[index] < num)
                    num = values[index];
            }

            return num;
        }

        public static Double Max(Double a, Double b) {
            if (a > b)
                return a;

            return b;
        }

        public static Double Max(params Double[] values) {
            Int32 length = values.Length;
            if (length == 0)
                return 0d;

            Double num = values[0];
            for (Int32 index = 1; index < length; ++index) {
                if (values[index] > num)
                    num = values[index];
            }

            return num;
        }

        public static Int32 Max(Int32 a, Int32 b) {
            if (a > b)
                return a;

            return b;
        }

        public static Int32 Max(params Int32[] values) {
            Int32 length = values.Length;
            if (length == 0)
                return 0;

            Int32 num = values[0];
            for (Int32 index = 1; index < length; ++index) {
                if (values[index] > num)
                    num = values[index];
            }

            return num;
        }

        public static Double Pow(Double d, Double p) {
            return System.Math.Pow(d, p);
        }

        public static Double Exp(Double power) {
            return System.Math.Exp(power);
        }

        public static Double Log(Double d, Double p) {
            return System.Math.Log(d, p);
        }

        public static Double Log(Double d) {
            return System.Math.Log(d);
        }

        public static Double Log10(Double d) {
            return System.Math.Log10(d);
        }

        public static Double Ceil(Double d) {
            return System.Math.Ceiling(d);
        }

        public static Double Floor(Double d) {
            return System.Math.Floor(d);
        }

        public static Double Round(Double d) {
            return System.Math.Round(d);
        }

        public static Int32 CeilToInt(Double d) {
            return (Int32)System.Math.Ceiling(d);
        }

        public static Int32 FloorToInt(Double d) {
            return (Int32)System.Math.Floor(d);
        }

        public static Int32 RoundToInt(Double d) {
            return (Int32)System.Math.Round(d);
        }

        public static Double Sign(Double d) {
            return d >= 0.0 ? 1d : -1d;
        }

        public static Double Clamp(Double value, Double min, Double max) {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static Int32 Clamp(Int32 value, Int32 min, Int32 max) {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static Double Clamp01(Double value) {
            if (value < 0.0)
                return 0.0d;
            if (value > 1.0)
                return 1d;

            return value;
        }

        public static Double Lerp(Double from, Double to, Double t) {
            return from + (to - from) * Clamp01(t);
        }

        public static Double LerpAngle(Double a, Double b, Double t) {
            Double num = Repeat(b - a, 360d);
            if (num > 180.0d)
                num -= 360d;
            return a + num * Clamp01(t);
        }

        public static Double MoveTowards(Double current, Double target, Double maxDelta) {
            if (Abs(target - current) <= maxDelta)
                return target;

            return current + Sign(target - current) * maxDelta;
        }

        public static Double MoveTowardsAngle(Double current, Double target, Double maxDelta) {
            target = current + DeltaAngle(current, target);
            return MoveTowards(current, target, maxDelta);
        }

        public static Double SmoothStep(Double from, Double to, Double t) {
            t = Clamp01(t);
            t = -2.0 * t * t * t + 3.0 * t * t;
            return to * t + from * (1.0 - t);
        }

        public static Double Gamma(Double value, Double absmax, Double gamma) {
            Boolean flag = value < 0.0;
            Double num1 = Abs(value);
            if (num1 > absmax)
                if (flag)
                    return -num1;
                else
                    return num1;

            Double num2 = Pow(num1 / absmax, gamma) * absmax;
            if (flag)
                return -num2;

            return num2;
        }

        public static Boolean Approximately(Double a, Double b) {
            return Abs(b - a) < Max(1E-06d * Max(Abs(a), Abs(b)), 1.121039E-44d);
        }

        public static Double SmoothDamp(Double current, Double target, ref Double currentVelocity, Double smoothTime,
            Double maxSpeed, Double deltaTime) {
            smoothTime = Max(0.0001d, smoothTime);
            Double num1 = 2d / smoothTime;
            Double num2 = num1 * deltaTime;
            Double num3 = 1.0d
                          / (1.0d + num2 + 0.479999989271164d * num2 * num2 + 0.234999999403954d * num2 * num2 * num2);
            Double num4 = current - target;
            Double num5 = target;
            Double max = maxSpeed * smoothTime;
            Double num6 = Clamp(num4, -max, max);
            target = current - num6;
            Double num7 = (currentVelocity + num1 * num6) * deltaTime;
            currentVelocity = (currentVelocity - num1 * num7) * num3;
            Double num8 = target + (num6 + num7) * num3;
            if (num5 - current > 0.0 == num8 > num5) {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        public static Double SmoothDampAngle(Double current, Double target, ref Double currentVelocity,
            Double smoothTime, Double maxSpeed, Double deltaTime) {
            target = current + DeltaAngle(current, target);
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Double Repeat(Double t, Double length) {
            return t - Floor(t / length) * length;
        }

        public static Double PingPong(Double t, Double length) {
            t = Repeat(t, length * 2d);
            return length - Abs(t - length);
        }

        public static Double InverseLerp(Double from, Double to, Double value) {
            if (from < to) {
                if (value < from)
                    return 0d;
                if (value > to)
                    return 1d;

                value -= from;
                value /= to - from;
                return value;
            }

            if (from <= to)
                return 0d;
            if (value < to)
                return 1d;
            if (value > from)
                return 0d;

            return 1.0d - (value - to) / (from - to);
        }

        public static Double DeltaAngle(Double current, Double target) {
            Double num = Repeat(target - current, 360d);
            if (num > 180.0d)
                num -= 360d;
            return num;
        }

        internal static Boolean LineIntersection(Vector2D p1, Vector2D p2, Vector2D p3, Vector2D p4,
            ref Vector2D result) {
            Double num1 = p2.X - p1.X;
            Double num2 = p2.Y - p1.Y;
            Double num3 = p4.X - p3.X;
            Double num4 = p4.Y - p3.Y;
            Double num5 = num1 * num4 - num2 * num3;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (num5 == 0.0d)
                return false;

            Double num6 = p3.X - p1.X;
            Double num7 = p3.Y - p1.Y;
            Double num8 = (num6 * num4 - num7 * num3) / num5;
            result = new Vector2D(p1.X + num8 * num1, p1.Y + num8 * num2);
            return true;
        }

        internal static Boolean LineSegmentIntersection(Vector2D p1, Vector2D p2, Vector2D p3, Vector2D p4,
            ref Vector2D result) {
            Double num1 = p2.X - p1.X;
            Double num2 = p2.Y - p1.Y;
            Double num3 = p4.X - p3.X;
            Double num4 = p4.Y - p3.Y;
            Double num5 = num1 * num4 - num2 * num3;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (num5 == 0.0d)
                return false;

            Double num6 = p3.X - p1.X;
            Double num7 = p3.Y - p1.Y;
            Double num8 = (num6 * num4 - num7 * num3) / num5;
            if (num8 < 0.0d || num8 > 1.0d)
                return false;

            Double num9 = (num6 * num2 - num7 * num1) / num5;
            if (num9 < 0.0d || num9 > 1.0d)
                return false;

            result = new Vector2D(p1.X + num8 * num1, p1.Y + num8 * num2);
            return true;
        }
    }
}
