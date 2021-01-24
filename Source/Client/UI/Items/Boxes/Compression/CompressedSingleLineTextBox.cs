// <copyright file="CompressedSingleLineTextBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.CompressedSingleLineTextBox class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;

    /// <summary>
    /// Represents an IBox that compresses two <see cref="SingleLineTextBox"/>es into one line.
    /// </summary>
    /// <remarks>
    /// This is used when compressing a BoxModel for space.
    /// </remarks>
    internal sealed class CompressedSingleLineTextBox : IBox
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
        /// Initializes a new instance of the CompressedSingleLineTextBox class.
        /// </summary>
        /// <param name="leftBox">
        /// The box to be displayed first.
        /// </param>
        /// <param name="rightBox">
        /// The box to be displayed on the right next to the leftBox.
        /// </param>
        public CompressedSingleLineTextBox( SingleLineTextBox leftBox, SingleLineTextBox rightBox )
        {
            this.leftBox = leftBox;
            this.rightBox = rightBox;

            this.leftString = leftBox.Text + ", ";
            this.leftStringWidth = (int)leftBox.Font.MeasureStringWidth( this.leftString );

            this.size = new Point2( leftStringWidth + leftBox.HorizontalOffset + rightBox.Size.X, leftBox.Size.Y );
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
        public void Draw( int verticalPosition, Point2 totalSize, ZeldaDrawContext drawContext )
        {
            this.leftBox.Font.Draw(
                this.leftString,
                new Vector2( this.leftBox.HorizontalOffset, verticalPosition ),
                this.leftBox.Color,
                drawContext
            );

            this.rightBox.Font.Draw(
                this.rightBox.Text,
                new Vector2( this.leftBox.HorizontalOffset + leftStringWidth, verticalPosition ),
                this.rightBox.Color,
                drawContext
            );
        }

        private readonly string leftString;
        private readonly int leftStringWidth;
        private readonly SingleLineTextBox leftBox;
        private readonly SingleLineTextBox rightBox;
        private readonly Point2 size;
    }
}
