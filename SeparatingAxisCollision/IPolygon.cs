using Microsoft.Xna.Framework;
using System.Drawing;

namespace SeparatingAxisCollision
{
    public interface IPolygon
    {
        /// <summary>
        /// Checks for collision between this IPolygon and another, and returns a suggested Minimum Translation Vector for projection-based resolution.
        /// Vector2.Zero means there was no collision.
        /// </summary>
        Vector2 CheckCollisionAndRespond(IPolygon other);

        /// <summary>
        /// Checks for collision between this IPolygon and another. Returns true if they collide.
        /// </summary>
        bool CheckCollision(IPolygon other);

        /// <summary>
        /// Returns an interval that represents the projection of this IPolygon onto passed axis.
        /// </summary>
        Interval GetProjection(Vector2 axis);

        /// <summary>
        /// Gets the position of the axial center of the IPolygon.
        /// </summary>
        Vector2 GetPosition();

        /// <summary>
        /// Gets the rotation-less bounding square of this IPolygon.
        /// </summary>
        RectangleF GetBoundingSquare();

        /// <summary>
        /// Gets the unscaled, untranslated rotation-less bounding square of this IPolygon.
        /// </summary>
        RectangleF GetBoundingSquareUnit();

        /// <summary>
        /// Gets the minimum bounding box of this IPolygon.
        /// </summary>
        RectangleF GetBoundingBox();

        /// <summary>
        /// Gets the unscaled, untranslated bounding box of this IPolygon.
        /// </summary>
        RectangleF GetBoundingBoxUnit();
    }
}
