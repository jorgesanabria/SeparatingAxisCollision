#region using

using System;
using Starry.Math;

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
        Vector2D GetPosition();

        /// <summary>
        ///     Sets the position of the axial center of the IPolygon.
        /// </summary>
        void SetPosition(Vector2D position);

        /// <summary>
        ///     Gets the position of this IPolygon around its axial center.
        /// </summary>
        Double GetRotation();

        /// <summary>
        ///     Sets the position of this IPolygon around its axial center.
        /// </summary>
        void SetRotation(Double rotation);

        /// <summary>
        ///     Gets the scale of this IPolygon from its axial center.
        /// </summary>
        Double GetScale();

        /// <summary>
        ///     Sets the scale of this IPolygon from its axial center.
        /// </summary>
        void SetScale(Double scale);

        /// <summary>
        ///     Gets the points of an IPolygon.
        /// </summary>
        Vector2D[] GetPoints();

        /// <summary>
        ///     Returns an interval that represents the projection of this IPolygon onto passed axis.
        /// </summary>
        Interval GetProjection(Vector2D axis);

        /// <summary>
        ///     Gets the normals of this IPolygon to be tested against in collision.
        /// </summary>
        Vector2D[] GetNormals(IPolygon other);

        /// <summary>
        ///     Gets the unscaled, untranslated rotation-less bounding square of this IPolygon.
        /// </summary>
        RectD GetBoundingSquareUnit();

        /// <summary>
        ///     Gets the rotation-less bounding square of this IPolygon.
        /// </summary>
        RectD GetBoundingSquare();

        /// <summary>
        ///     Gets the unscaled, untranslated bounding box of this IPolygon.
        /// </summary>
        RectD GetBoundingBoxUnit();

        /// <summary>
        ///     Gets the minimum bounding box of this IPolygon.
        /// </summary>
        RectD GetBoundingBox();
    }
}
