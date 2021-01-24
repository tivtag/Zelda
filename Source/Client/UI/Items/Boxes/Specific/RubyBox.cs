// <copyright file="RubyBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//      Defines the Zelda.UI.Items.BoxModel.RubyBox class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;

    /// <summary>
    /// Represents an IBox that is used to draw how much an Item
    /// is worth in rubees.
    /// </summary>
    internal sealed class RubyBox : IBox
    {
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
        /// Initializes a new instance of the RubyBox class.
        /// </summary>
        /// <param name="boxHeight">
        /// The height of the new RubyBox in pixels.
        /// </param>
        /// <param name="rubyString">
        /// The string representing how many rubees the item is worth.
        /// </param>
        /// <param name="rubySprite">
        /// The sprite used to signalize what the string represents.
        /// </param>
        /// <param name="font">
        /// The font used to draw the rubyString.
        /// </param>
        public RubyBox( int boxHeight, string rubyString, Sprite rubySprite, IFont font )
        {
            this.rubyString = rubyString;
            this.rubySprite = rubySprite;
            this.font = font;

            float stringWidth = this.font.MeasureStringWidth( this.rubyString );
            this.size = new Point2( (int)stringWidth + 2 + this.rubySprite.Width, boxHeight );
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
            var batch = drawContext.Batch;

            // String:
            float stringWidth = font.MeasureStringWidth( this.rubyString );

            this.font.Draw(
                this.rubyString,
                new Vector2(
                    totalSize.X - this.rubySprite.Width - 2 - stringWidth,
                    totalSize.Y - this.rubySprite.Height - 2
                ),
                Microsoft.Xna.Framework.Color.White,
                drawContext
            );

            // Sprite:
            this.rubySprite.Draw(
                new Vector2( totalSize.X - this.rubySprite.Width, totalSize.Y - this.rubySprite.Height ),
                batch
            );
        }

        private readonly Point2 size;
        private readonly string rubyString;
        private readonly Sprite rubySprite;
        private readonly IFont font;
    }
}
