#region using

using System;
using System.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InconsistentNaming

#endregion

namespace Starry.Math {
    /// <inheritdoc cref="IEquatable{T}" />
    /// <summary>
    ///     Stores the location and size of a rectangular region.
    /// </summary>
    [Serializable]
    public struct RectD : IEquatable<RectD> {
        /// <summary>
        ///     Initializes a new instance of the <see cref='RectD' />
        ///     class.
        /// </summary>
        public static readonly RectD Empty = new RectD();

        #region Do not refactor (binary serialization)

        private Double x;
        private Double y;
        private Double width;
        private Double height;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref='RectD' />
        ///     class with the specified location and size.
        /// </summary>
        public RectD(Double x, Double y, Double width, Double height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref='RectD' />
        ///     class with the specified location
        ///     and size.
        /// </summary>
        public RectD(Vector2D location, Vector2D size) {
            x = location.X;
            y = location.Y;
            width = size.X;
            height = size.Y;
        }

        /// <summary>
        ///     Creates a new <see cref='RectD' /> with
        ///     the specified location and size.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static RectD FromLTRB(Double left, Double top, Double right, Double bottom) {
            return new RectD(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///     Gets or sets the coordinates of the upper-left corner of
        ///     the rectangular region represented by this <see cref='RectD' />.
        /// </summary>
        [Browsable(false)]
        public Vector2D Location {
            get => new Vector2D(X, Y);
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     Gets or sets the size of this <see cref='RectD' />.
        /// </summary>
        [Browsable(false)]
        public Vector2D Size {
            get => new Vector2D(Width, Height);
            set {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>
        ///     Gets or sets the x-coordinate of the upper-left corner
        ///     of the rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        public Double X {
            get => x;
            set => x = value;
        }

        /// <summary>
        ///     Gets or sets the y-coordinate of the upper-left corner
        ///     of the rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        public Double Y {
            get => y;
            set => y = value;
        }

        /// <summary>
        ///     Gets or sets the width of the rectangular
        ///     region defined by this <see cref='RectD' />.
        /// </summary>
        public Double Width {
            get => width;
            set => width = value;
        }

        /// <summary>
        ///     Gets or sets the region on the X axis of the
        ///     rectangular region defined by this <see cref="RectD" />
        /// </summary>
        [Browsable(false)]
        public Interval IntervalX {
            get => new Interval(x, x + width);
            set {
                x = value.Min;
                width = value.Max - value.Min;
            }
        }

        /// <summary>
        ///     Gets or sets the height of the
        ///     rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        public Double Height {
            get => height;
            set => height = value;
        }

        /// <summary>
        ///     Gets or sets the region on the Y axis of the
        ///     rectangular region defined by this <see cref="RectD" />
        /// </summary>
        [Browsable(false)]
        public Interval IntervalY {
            get => new Interval(y, y + height);
            set {
                y = value.Min;
                height = value.Max - value.Min;
            }
        }

        /// <summary>
        ///     Gets the x-coordinate of the upper-left corner of the
        ///     rectangular region defined by this <see cref='RectD' /> .
        /// </summary>
        [Browsable(false)]
        public Double Left {
            get => X;
        }

        /// <summary>
        ///     Gets the y-coordinate of the upper-left corner of the
        ///     rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        [Browsable(false)]
        public Double Top {
            get => Y;
        }

        /// <summary>
        ///     Gets the x-coordinate of the lower-right corner of the
        ///     rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        [Browsable(false)]
        public Double Right {
            get => X + Width;
        }

        /// <summary>
        ///     Gets the y-coordinate of the lower-right corner of the
        ///     rectangular region defined by this <see cref='RectD' />.
        /// </summary>
        [Browsable(false)]
        public Double Bottom {
            get => Y + Height;
        }

        /// <summary>
        ///     Tests whether this <see cref='RectD' /> has a <see cref='Width' /> or a
        ///     <see cref='Height' /> of 0.
        /// </summary>
        [Browsable(false)]
        public Boolean IsEmpty {
            get => Width <= 0 || Height <= 0;
        }

        /// <summary>
        ///     Tests whether <paramref name="obj" /> is a <see cref='RectD' />
        ///     with the same location and size of this <see cref='RectD' />.
        /// </summary>
        public override Boolean Equals(Object obj) {
            return obj is RectD rect && Equals(rect);
        }

        public Boolean Equals(RectD other) {
            return this == other;
        }

        /// <summary>
        ///     Tests whether two <see cref='RectD' />
        ///     objects have equal location and size.
        /// </summary>
        public static Boolean operator ==(RectD left, RectD right) {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }

        /// <summary>
        ///     Tests whether two <see cref='RectD' />
        ///     objects differ in location or size.
        /// </summary>
        public static Boolean operator !=(RectD left, RectD right) {
            return !(left == right);
        }

        /// <summary>
        ///     Determines if the specified point is contained within the
        ///     rectangular region defined by this <see cref='RectD' /> .
        /// </summary>
        public Boolean Contains(Double argX, Double argY) {
            return X <= argX && argX < X + Width && Y <= argY && argY < Y + Height;
        }

        /// <summary>
        ///     Determines if the specified point is contained within the
        ///     rectangular region defined by this <see cref='RectD' /> .
        /// </summary>
        public Boolean Contains(Vector2D pt) {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        ///     Determines if the rectangular region represented by <paramref name="rect" />
        ///     is entirely contained within the rectangular region represented by
        ///     this <see cref='RectD' /> .
        /// </summary>
        public Boolean Contains(RectD rect) {
            return X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y && rect.Y + rect.Height <= Y + Height;
        }

        /// <summary>
        ///     Gets the hash code for this <see cref='RectD' />.
        /// </summary>
        [Obsolete]
        public override Int32 GetHashCode() {
            return -1; //TODO: Get hash code.
            /*
            return HashHelpers.Combine(
                HashHelpers.Combine(HashHelpers.Combine(X.GetHashCode(), Y.GetHashCode()), Width.GetHashCode()),
                Height.GetHashCode());
            */
        }

        /// <summary>
        ///     Inflates this <see cref='RectD' />
        ///     by the specified amount.
        /// </summary>
        public void Inflate(Double inflateX, Double inflateY) {
            X -= inflateX;
            Y -= inflateY;
            Width += 2 * inflateX;
            Height += 2 * inflateY;
        }

        /// <summary>
        ///     Inflates this <see cref='RectD' /> by the specified amount.
        /// </summary>
        public void Inflate(Vector2D size) {
            Inflate(size.X, size.Y);
        }

        /// <summary>
        ///     Creates a <see cref='RectD' />
        ///     that is inflated by the specified amount.
        /// </summary>
        public static RectD Inflate(RectD rect, Double x, Double y) {
            RectD r = rect;
            r.Inflate(x, y);
            return r;
        }

        /// <summary>
        ///     Creates a Rectangle that represents the intersection between this Rectangle and rect.
        /// </summary>
        public void Intersect(RectD rect) {
            RectD result = Intersect(rect, this);

            X = result.X;
            Y = result.Y;
            Width = result.Width;
            Height = result.Height;
        }

        /// <summary>
        ///     Returns a copy of this rect, scaled from a specifiable origin point.
        /// </summary>
        /// <returns></returns>
        public RectD ScaledPositionFromOrigin(Double scale, Vector2D? origin = null) {
            origin = origin ?? Vector2D.Zero;
            RectD copy = new RectD(Location - origin.Value, Size);
            Double centerX = copy.Left + (copy.Right - copy.Left) / 2;
            Double centerY = copy.Top + (copy.Bottom - copy.Top) / 2;
            copy.X = X + origin.Value.X + (centerX * scale - centerX);
            copy.Y = Y + origin.Value.Y + (centerY * scale - centerY);
            return copy;
        }

        /// <summary>
        ///     Returns a copy of this rect scaled from its center.
        /// </summary>
        public RectD ScaledFromCenter(Double scale) {
            RectD copy = new RectD(Location, Size);
            copy.Width *= scale;
            copy.Height *= scale;
            copy.X -= (copy.Width - Width) / 2;
            copy.Y -= (copy.Height - Height) / 2;
            return copy;
        }

        /// <summary>
        ///     Creates a rectangle that represents the intersection between a and
        ///     b. If there is no intersection, null is returned.
        /// </summary>
        public static RectD Intersect(RectD a, RectD b) {
            Double x1 = MathD.Max(a.X, b.X);
            Double x2 = MathD.Min(a.X + a.Width, b.X + b.Width);
            Double y1 = MathD.Max(a.Y, b.Y);
            Double y2 = MathD.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
                return new RectD(x1, y1, x2 - x1, y2 - y1);

            return Empty;
        }

        /// <summary>
        ///     Determines if this rectangle intersects with rect.
        /// </summary>
        public Boolean IntersectsWith(RectD rect) {
            return rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height && Y < rect.Y + rect.Height;
        }

        /// <summary>
        ///     Determines if this rectangle overlaps another rect.
        /// </summary>
        public Boolean Overlaps(RectD rect) {
            Boolean x = IntervalX.Overlaps(rect.IntervalX);
            Boolean y = IntervalY.Overlaps(rect.IntervalY);

            return x && y;
        }

        /// <summary>
        ///     Creates a rectangle that represents the union between a and b.
        /// </summary>
        public static RectD Union(RectD a, RectD b) {
            Double x1 = MathD.Min(a.X, b.X);
            Double x2 = MathD.Max(a.X + a.Width, b.X + b.Width);
            Double y1 = MathD.Min(a.Y, b.Y);
            Double y2 = MathD.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectD(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>
        ///     Adjusts the location of this rectangle by the specified amount.
        /// </summary>
        public void Offset(Vector2D pos) {
            Offset(pos.X, pos.Y);
        }

        /// <summary>
        ///     Adjusts the location of this rectangle by the specified amount.
        /// </summary>
        public void Offset(Double offsetX, Double offsetY) {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        ///     Converts the <see cref='Location' /> and <see cref='Size' /> of this
        ///     <see cref='RectD' /> to a human-readable string.
        /// </summary>
        public override String ToString() {
            return "{X="
                   + X
                   + ",Y="
                   + Y
                   + ",Width="
                   + Width
                   + ",Height="
                   + Height
                   + "}";
        }
    }
}
