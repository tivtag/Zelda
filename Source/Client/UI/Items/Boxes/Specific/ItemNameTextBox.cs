// <copyright file="ItemNameTextBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.ItemNameTextBox class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements an IBox that contains the name of an Item.
    /// </summary>
    internal sealed class ItemNameTextBox : IBox
    {
        /// <summary>
        /// States the maximum number of pixels the Item Name is allowed to
        /// have to use the normal font. If it is greater an alternative smaller font is used.
        /// </summary>
        private const int MaximimWidthForNormalFont = 250;

        /// <summary>
        /// The additional horizontal pixel size the ItemNameTextBox has.
        /// </summary>
        private const int AdditionalWidth = 40;

        /// <summary>
        /// Gets the area this IBox takes up.
        /// </summary>
        /// <returns>
        /// The area this IBox cowers.
        /// </returns>
        public Point2 Size
        {
            get
            {
                return this.size;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemNameTextBox class.
        /// </summary>
        /// <param name="itemName">
        /// The name of the item that should be fit into the new ItemNameTextBox.
        /// </param>
        /// <param name="spriteFont">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <param name="spriteFontSmall">
        /// The IFont that should be used to draw the specified text;
        /// incase the itemName is too large.
        /// </param>
        /// <param name="color">
        /// The color of the itenName.
        /// </param>
        public ItemNameTextBox( 
            string itemName,
            IFont spriteFont,
            IFont spriteFontSmall,
            Xna.Color color )
        {
            this.itemName = itemName;
            this.color = color;

            // Cache font size.
            var size = spriteFont.MeasureString( this.itemName );

            if( size.X > MaximimWidthForNormalFont )
            {
                spriteFont = spriteFontSmall;
                size = spriteFontSmall.MeasureString( this.itemName );
            }

            this.spriteFont = spriteFont;
            this.textSize = new Point2( (int)size.X, (int)size.Y );
            this.size = new Point2( this.textSize.X + AdditionalWidth, this.textSize.Y );
        }

        /// <summary>
        /// Draws the content of this MultiLineTextBox.
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
        public void Draw( int verticalPosition, Point2 totalSize, ZeldaDrawContext drawContext )
        {
            this.spriteFont.Draw(
                this.itemName,
                this.GetDrawPosition( verticalPosition, totalSize ),
                this.color,
                drawContext
            );
        }

        /// <summary>
        /// Gets the position at which the text should be drawn.
        /// </summary>
        /// <param name="verticalPosition">
        /// The position on the y-axis at which this IBox should be drawn at.
        /// </param>
        /// <param name="totalSize">
        /// The total size of the BoxModel this IBox is part of.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        private Vector2 GetDrawPosition( int verticalPosition, Point2 totalSize )
        {
            return new Vector2( 
                (totalSize.X / 2) - (this.textSize.X / 2), 
                verticalPosition
            );
        }

        private readonly string itemName;
        private readonly IFont spriteFont;
        private readonly Xna.Color color;
        private readonly Point2 textSize;
        private readonly Point2 size;
    }
}
