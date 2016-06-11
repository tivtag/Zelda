// <copyright file="ItemBoxModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines theZelda.UI.Items.Boxes.ItemBoxModel class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Items.Boxes
{
    using System.Collections.Generic;
    using Atom.Math;

    /// <summary>
    /// Represents a BoxModel that descripes the properties of an Item.
    /// </summary>
    internal sealed class ItemBoxModel : BoxModel
    {
        /// <summary>
        /// The width of the border on the left and right side of the ItemBoxModel.
        /// </summary>
        public const int VerticalBorderWidth = 4;

        /// <summary>
        /// The height of the border on the top and bottom side of the ItemBoxModel
        /// </summary>
        public const int HorizontalBorderHeight = 2;

        /// <summary>
        /// Gets the total size this ItemBoxModel takes up;
        /// including the border.
        /// </summary>
        public Point2 TotalSize
        {
            get
            {
                return this.Size + new Point2( 2 * VerticalBorderWidth, 2 * HorizontalBorderHeight );
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemBoxModel class.
        /// </summary>
        /// <param name="boxes">
        /// The boxes that make up the new ItemBoxModel.
        /// </param>
        public ItemBoxModel( IList<IBox> boxes )
            : base( boxes )
        {
        }
    }
}
