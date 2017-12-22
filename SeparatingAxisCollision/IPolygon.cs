#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;

#endregion

namespace SeparatingAxisCollision {
    /* All IPolygons need:
    *  a readonly/implicit "origin" assigned at object creation.
    *  a get/set position of that origin
    *  a get/set rotation
    *  a get/set scale
    *  projectability onto an axis
    *  variable # of axes that need to be tested
    *  (poly = n axes. box = 2 axes, circle = 1 axis, ray = 2 axes, point = 0 axes)
    *  bounding boxes/squares
    */

    public interface IPolygon {
        /// <summary>
        ///     Gets the position of the axial center of the IPolygon.
        /// </summary>
        Vector2 GetPosition();

        /// <summary>
        ///     Sets the position of the axial center of the IPolygon.
        /// </summary>
        void SetPosition(Vector2 position);

        /// <summary>
        ///     Gets the position of this IPolygon around its axial center.
        /// </summary>
        Single GetRotation();

        /// <summary>
        ///     Sets the position of this IPolygon around its axial center.
        /// </summary>
        void SetRotation(Single rotation);

        /// <summary>
        ///     Gets the scale of this IPolygon from its axial center.
        /// </summary>
        Single GetScale();

        /// <summary>
        ///     Sets the scale of this IPolygon from its axial center.
        /// </summary>
        void SetScale(Single scale);

        /// <summary>
        ///     Gets the points of an IPolygon.
        /// </summary>
        Vector2[] GetPoints();

        /// <summary>
        ///     Returns an interval that represents the projection of this IPolygon onto passed axis.
        /// </summary>
        Interval GetProjection(Vector2 axis);

        /// <summary>
        ///     Gets the normals of this IPolygon to be tested against in collision.
        /// </summary>
        Vector2[] GetNormals(IPolygon other);

        /// <summary>
        ///     Gets the unscaled, untranslated rotation-less bounding square of this IPolygon.
        /// </summary>
        RectangleF GetBoundingSquareUnit();

        /// <summary>
        ///     Gets the rotation-less bounding square of this IPolygon.
        /// </summary>
        RectangleF GetBoundingSquare();

        /// <summary>
        ///     Gets the unscaled, untranslated bounding box of this IPolygon.
        /// </summary>
        RectangleF GetBoundingBoxUnit();

        /// <summary>
        ///     Gets the minimum bounding box of this IPolygon.
        /// </summary>
        RectangleF GetBoundingBox();
    }
}
