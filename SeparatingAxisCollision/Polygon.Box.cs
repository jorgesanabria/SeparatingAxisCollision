using Microsoft.Xna.Framework;

namespace SeparatingAxisCollision
{
    public partial class Polygon
    {
        /// <summary>
        /// Creates a box.
        /// </summary>
        /// <param name="offset">The offset between the center of the box and the axial center.</param>
        public static Polygon CreateBox(float halfWidth, float halfHeight, Vector2? offset = null, Vector2? pos = null)
        {
            return new Polygon(CreateBoxShape(halfWidth, halfHeight, offset), 1.0f, pos: pos);
        }

        #region Private Methods
        private static Shape CreateBoxShape(float halfWidth, float halfHeight, Vector2? offset = null)
        {
            var o = offset ?? Vector2.Zero;
            return new Shape(
                new Vector2(-halfWidth, halfHeight) + o,
                new Vector2(halfWidth, halfHeight) + o,
                new Vector2(halfWidth, -halfHeight) + o,
                new Vector2(-halfWidth, -halfHeight) + o);
        }
        #endregion
    }
}
