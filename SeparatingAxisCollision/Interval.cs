#region using

using System;

#endregion

namespace SeparatingAxisCollision {
    /// <summary>
    ///     An interval of two values, with minimum and maximum.
    /// </summary>
    public struct Interval {
        /// <summary>
        ///     Lowest point of the interval.
        /// </summary>
        public Single Min;

        /// <summary>
        ///     Highest point of the interval.
        /// </summary>
        public Single Max;

        public Interval(Single min, Single max) {
            Min = min;
            Max = max;
        }

        /// <summary>
        ///     Checks if this Interval contains a value.
        /// </summary>
        public Boolean Contains(Single value) {
            return value > Min && value < Max;
        }

        public Boolean Equals(Interval obj) {
            return Min == obj.Min && Max == obj.Max;
        }

        /// <summary>
        ///     Checks if this Interval overlaps with another.
        /// </summary>
        public Boolean Overlaps(Interval other) {
            if (Equals(other))
                return true;

            if (other.Contains(Min))
                return true;

            if (other.Contains(Max))
                return true;

            if (Contains(other.Min))
                return true;

            if (Contains(other.Max))
                return true;

            return false;
        }

        /// <summary>
        ///     Gets the minimum translation that moves this Interval away from another.
        /// </summary>
        public Single GetMinimumTranslation(Interval other) {
            Single a = other.Max - Min;
            Single b = Max - other.Min;

            if (a < b)
                return a;

            return -b;
        }
    }
}
