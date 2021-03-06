﻿#region using

using System;
using Starry.Math;

#endregion

namespace SeparatingAxisCollision {
    // Features needed:
    // - IPolygon cloning without knowing base class.
    // - Translate(Vector2D) method for easier work.
    // - (In Collision) A way to "preview" polygons at a certain state without actually setting it to that state.

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

        /// <summary>
        ///     Returns an exact copy of this IPolygon.
        /// </summary>
        IPolygon Clone();

        /// <summary>
        ///     Adds the translation to the position of this IPolygon.
        /// </summary>
        void Translate(Vector2D translation);

        /// <summary>
        ///     Rotates the IPolygon by the specified angle.
        /// </summary>
        void Rotate(Double angle);

        /// <summary>
        ///     Additively modifies the scale of this IPolygon by the specified amount.
        /// </summary>
        void AddScale(Double amount);

        /// <summary>
        ///     Multiplies the scale of this IPolygon by the specified factor.
        /// </summary>
        void MultiplyScale(Double factor);
    }
}
