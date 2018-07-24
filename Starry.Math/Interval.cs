#region using

using System;

#endregion

namespace Starry.Math {
    /// <summary>
    ///     An interval of two values, with minimum and maximum.
    /// </summary>
    public struct Interval {
        /// <summary>
        ///     Lowest point of the interval.
        /// </summary>
        public Double Min;

        /// <summary>
        ///     Highest point of the interval.
        /// </summary>
        public Double Max;

        public Interval(Double min, Double max) {
            if (min > max) {
                // Unexpected? Switch the two. Don't need an exception for something so trivial.
                Max = min;
                Min = max;
            } else {
                // Expected result.
                Min = min;
                Max = max;
            }
        }

        /// <summary>
        ///     Checks if this Interval overlaps with a 1D point.
        /// </summary>
        public Boolean Overlaps(Double value) {
            return value >= Min && value <= Max;
        }

        public Boolean Equals(Interval obj) {
            return MathD.Abs(Min - obj.Min) < MathD.EPSILON && MathD.Abs(Max - obj.Max) < MathD.EPSILON;
        }

        /// <summary>
        ///     Checks if this Interval overlaps with another.
        /// </summary>
        public Boolean Overlaps(Interval other) {
            return MathD.Max(Min, other.Min) <= MathD.Min(Max, other.Max);
        }

        /// <summary>
        ///     Gets the minimum translation that moves this Interval away from another.
        /// </summary>
        public Double GetMinimumTranslation(Interval other) {
            Double a = other.Max - Min;
            Double b = Max - other.Min;

            return a < b ? a : -b;
        }
    }
}
