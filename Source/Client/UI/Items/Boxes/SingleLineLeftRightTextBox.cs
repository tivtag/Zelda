// <copyright file="SingleLineLeftRightTextBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.SingleLineLeftRightTextBox class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements an IBox that contains a left-aligned and a right-aligned text on
    /// the same line.
    /// </summary>
    internal sealed class SingleLineLeftRightTextBox : IBox
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
        /// Initializes a new instance of the SingleLineLeftRightTextBox class.
        /// </summary>
        /// <param name="leftText">
        /// The left-aligned text.
        /// </param>
        /// <param name="rightText">
        /// The right-aligned text.
        /// </param>
        /// <param name="minimumGap">
        /// The minimum pixel gap that should be between the two texts.
        /// </param>
        /// <param name="additionalVerticalSize">
        /// The additional height of the new SingleLineLeftRightTextBox.
        /// </param>
        /// <param name="font">
        /// The font that should be used to draw the text.
        /// </param>
        /// <param name="color">
        /// The color of the text.
        /// </param>
        public SingleLineLeftRightTextBox( string leftText, string rightText, int minimumGap, int additionalVerticalSize, IFont font, Xna.Color color )
            : this( leftText, rightText, minimumGap, additionalVerticalSize, font, color, color )
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingleLineLeftRightTextBox class.
        /// </summary>
        /// <param name="leftText">
        /// The left-aligned text.
        /// </param>
        /// <param name="rightText">
        /// The right-aligned text.
        /// </param>
        /// <param name="minimumGap">
        /// The minimum pixel gap that should be between the two texts.
        /// </param>
        /// <param name="additionalVerticalSize">
        /// The additional height of the new SingleLineLeftRightTextBox.
        /// </param>
        /// <param name="font">
        /// The font that should be used to draw the text.
        /// </param>
        /// <param name="leftColor">
        /// The color of the left text.
        /// </param>
        /// <param name="rightColor">
        /// The color of the left text.
        /// </param>
        public SingleLineLeftRightTextBox( string leftText, string rightText, int minimumGap, int additionalVerticalSize, IFont font, Xna.Color leftColor, Xna.Color rightColor )
        {
            this.leftText = leftText;
            this.rightText = rightText;
            this.minimumGap = minimumGap;
            this.font = font;
            this.leftColor = leftColor;
            this.rightColor = rightColor;

            // Cache font sizes.
            int leftTextWidth = (int)this.font.MeasureStringWidth( leftText );
            this.rightTextWidth = (int)this.font.MeasureStringWidth( rightText );

            this.size = new Point2( 
                leftTextWidth + this.minimumGap + this.rightTextWidth,
                this.font.LineSpacing + additionalVerticalSize
            );
        }
        
        /// <summary>
        /// Draws the content of this LeftRightTextBox.
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
            // Left String:
            this.font.Draw(
                this.leftText,
                new Vector2( 0, verticalPosition ),
                this.leftColor,
                drawContext
            );

            // Right String:           
            this.font.Draw(
                this.rightText,
                new Vector2( totalSize.X - this.rightTextWidth, verticalPosition ),
                this.rightColor,
                drawContext
            );
        }

        private readonly string leftText, rightText;
        private readonly int minimumGap;
        private readonly IFont font;
        private readonly Xna.Color leftColor, rightColor;

        private readonly int rightTextWidth;
        private readonly Point2 size;
    }
}
