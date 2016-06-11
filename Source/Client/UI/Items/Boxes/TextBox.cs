// <copyright file="TextBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.TextBox class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements an IBox that contains some simple text.
    /// </summary>
    internal sealed class TextBox : IBox
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
        /// Initializes a new instance of the TextBox class.
        /// </summary>
        /// <param name="text">
        /// The text that should be fit into the new TextBox.
        /// </param>
        /// <param name="font">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <param name="color">
        /// The color of the text.
        /// </param>
        /// <param name="textAlign">
        /// The text alignment that should be used relative to the full box model.
        /// </param>
        public TextBox( string text, IFont font, Xna.Color color, TextAlign textAlign )
            : this( text, 0, 0, 0, font, color, textAlign )
        {
        }

         /// <summary>
        /// Initializes a new instance of the TextBox class.
        /// </summary>
        /// <param name="text">
        /// The text that should be fit into the new TextBox.
        /// </param>
        /// <param name="horizontalOffset">
        /// The additional horizontal pixel offset before the text.
        /// </param>
        /// <param name="additionalHorizontalSize">
        /// The additional size of the TextBox on the x-axis.
        /// </param>
        /// <param name="font">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <param name="color">
        /// The color of the text.
        /// </param>
        /// <param name="textAlign">
        /// The text alignment that should be used relative to the full box model.
        /// </param>
        public TextBox(
            string text,
            int horizontalOffset,
            int additionalHorizontalSize,
            IFont font,
            Xna.Color color,
            TextAlign textAlign )
            : this( text, horizontalOffset, additionalHorizontalSize, 0, font, color, textAlign )
        {
        }

        /// <summary>
        /// Initializes a new instance of the TextBox class.
        /// </summary>
        /// <param name="text">
        /// The text that should be fit into the new TextBox.
        /// </param>
        /// <param name="horizontalOffset">
        /// The additional horizontal pixel offset before the text.
        /// </param>
        /// <param name="additionalHorizontalSize">
        /// The additional size of the TextBox on the x-axis.
        /// </param>
        /// <param name="additionalVerticalSize">
        /// The additional size of the TextBox on the y-axis.
        /// </param>
        /// <param name="font">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <param name="color">
        /// The color of the text.
        /// </param>
        /// <param name="textAlign">
        /// The text alignment that should be used relative to the full box model.
        /// </param>
        public TextBox( 
            string text, 
            int horizontalOffset, 
            int additionalHorizontalSize,
            int additionalVerticalSize,
            IFont font, 
            Xna.Color color,
            TextAlign textAlign )
        {
            this.text = text;
            this.horizontalOffset = horizontalOffset;
            this.font = font;
            this.color = color;
            this.textAlign = textAlign;

            // Cache font size.
            var size = this.font.MeasureString( this.text );
            this.textSize = new Point2( (int)size.X, (int)size.Y );
            this.size = new Point2( 
                this.textSize.X + this.horizontalOffset + additionalHorizontalSize,
                this.textSize.Y + additionalVerticalSize
            );
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
            this.font.Draw(
                this.text,
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
            switch( this.textAlign )
            {
                default:
                case TextAlign.Left:
                    return new Vector2( this.horizontalOffset, verticalPosition );

                case TextAlign.Right:
                    return new Vector2( totalSize.X - this.textSize.X, verticalPosition );

                case TextAlign.Center:
                    return new Vector2( (totalSize.X / 2) - (this.textSize.X / 2), verticalPosition );
            }
        }

        private readonly string text;
        private readonly IFont font;
        private readonly Xna.Color color;
        private readonly TextAlign textAlign;
        private readonly Point2 textSize;
        private readonly int horizontalOffset;
        private readonly Point2 size;
    }
}
