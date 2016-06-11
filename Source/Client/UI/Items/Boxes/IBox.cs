// <copyright file="IBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.IBox interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;

    /// <summary>
    /// IBoxes are the building blocks of BlockModels.
    /// </summary>
    internal interface IBox
    {
        /// <summary>
        /// Gets the area this IBox takes up.
        /// </summary>
        /// <returns>
        /// The area this IBox cowers.
        /// </returns>
        Point2 Size
        {
            get;
        }

        /// <summary>
        /// Draws the content of this IBox.
        /// </summary>
        /// <param name="verticalPosition">
        /// The position on the y-axis at which this IBox should be drawn at.
        /// </param>
        /// <param name="totalSize">
        /// The total size of the BoxModel this IBox is part of.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void Draw( int verticalPosition, Point2 totalSize, ZeldaDrawContext drawContext );
    }
}
