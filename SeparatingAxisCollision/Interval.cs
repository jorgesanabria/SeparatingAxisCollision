namespace SeparatingAxisCollision
{
    /// <summary>
    /// An interval of two values, with minimum and maximum.
    /// </summary>
    public struct Interval
    {
        /// <summary>
        /// Lowest point of the interval.
        /// </summary>
        public float Min;

        /// <summary>
        /// Highest point of the interval.
        /// </summary>
        public float Max;

        public Interval(float min, float max)
        {
            Min = min;
            Max = max;
        }
        
        /// <summary>
        /// Checks if this Interval contains a value.
        /// </summary>
        public bool Contains(float value)
        {
            return (value > Min && value < Max);
        }

        public bool Equals(Interval obj)
        {
            return (Min == obj.Min && Max == obj.Max);
        }

        /// <summary>
        /// Checks if this Interval overlaps with another.
        /// </summary>
        public bool Overlaps(Interval other)
        {
            if (Equals(other))
                return true;
            else if (other.Contains(Min))
                return true;
            else if (other.Contains(Max))
                return true;
            else if (Contains(other.Min))
                return true;
            else if (Contains(other.Max))
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Gets the minimum translation that moves this Interval away from another.
        /// </summary>
        public float GetMinimumTranslation(Interval other)
        {
            var a = other.Max - Min;
            var b = Max - other.Min;

            if (a < b) return a;
            else return -b;
        }
    }
}
