// <copyright file="MultiLineTextBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.MultiLineTextBox class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Items.Boxes
{
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents an IBox that takes multiple lines of text
    /// as its input. This class can't be inherited.
    /// </summary>
    internal sealed class MultiLineTextBox : IBox
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
        /// Initializes a new instance of the MultiLineTextBox class.
        /// </summary>
        /// <param name="splitText">
        /// The text to visualize; split by line.
        /// </param>
        /// <param name="additionalHorizontalSize">
        /// The additional horizontal pixel size.
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
        public MultiLineTextBox( string[] splitText, int additionalHorizontalSize, IFont font, Xna.Color color, TextAlign textAlign )
        {
            this.splitText = splitText;
            this.font = font;
            this.color = color;
            this.textAlign = textAlign;
            this.size = CalculateSize( splitText, additionalHorizontalSize, font );
        }

        /// <summary>
        /// Calculates the size of a MultiLineTextBox.
        /// </summary>
        /// <param name="splitText">
        /// The text to visualize; split by line.
        /// </param>
        /// <param name="additionalHorizontalSize">
        /// The additional horizontal pixel size.
        /// </param>
        /// <param name="font">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <returns></returns>
        private static Point2 CalculateSize( string[] splitText, int additionalHorizontalSize, IFont font )
        {
            Point2 size = new Point2();

            int lastIndex = splitText.Length - 1;

            for( int i = 0; i < splitText.Length; ++i )
            {
                string text = splitText[i];
                var textSize = font.MeasureString( text );

                // Y
                size.Y += (int)textSize.Y;

                if( i == lastIndex && (textSize.X <= 5 || text.Length == 1) )
                {
                    size.Y -= 3;
                }

                // X
                textSize.X += additionalHorizontalSize;

                if( textSize.X > size.X )
                {
                    size.X = (int)textSize.X;
                }
            }

            return size;
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
            this.font.DrawBlock(
                this.splitText,
                GetDrawPosition( verticalPosition, totalSize ),
                this.textAlign,
                this.color,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                1.0f,
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
                case TextAlign.Center:
                    return new Vector2( totalSize.X / 2, verticalPosition );

                default:
                    throw new System.NotImplementedException();
            }
        }

        private readonly string[] splitText;
        private readonly IFont font;
        private readonly Xna.Color color;
        private readonly TextAlign textAlign;
        private readonly Point2 size;
    }
}
