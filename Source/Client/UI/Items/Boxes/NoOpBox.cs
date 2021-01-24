// <copyright file="NoOpBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.NoOpBox class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items.Boxes
{
    /// <summary>
    /// Represents an IBox that does absolutely nothing.
    /// </summary>
    internal class NoOpBox : IBox
    {
        /// <summary>
        /// Gets the area this IBox takes up.
        /// </summary>
        /// <returns>
        /// The area this IBox cowers.
        /// </returns>
        public Atom.Math.Point2 Size
        {
            get
            {
                return Atom.Math.Point2.Zero;
            }
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
        public void Draw( int verticalPosition, Atom.Math.Point2 totalSize, ZeldaDrawContext drawContext )
        {
            // no op.
        }
    }
}
