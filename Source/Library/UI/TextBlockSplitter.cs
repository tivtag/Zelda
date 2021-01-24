// <copyright file="TextBlockSplitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.UI.TextBlockSplitter class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Atom.Diagnostics.Contracts;

    /// <summary>
    /// Defines a <see cref="Atom.Xna.UI.ITextBlockSplitter"/> that splits text 
    /// so that it stay inside a specific area.
    /// </summary>
    public sealed class TextBlockSplitter : Atom.Xna.UI.ITextBlockSplitter
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockSplitter"/> class.
        /// </summary>
        /// <param name="font">The font which is later used to render the text.</param>
        /// <param name="maximumAllowedWidth">
        /// The maximum allowed width one text block row is allowed to have.
        /// </param>
        public TextBlockSplitter( IFont font, float maximumAllowedWidth )
        {
            Contract.Requires<ArgumentNullException>( font != null );

            this.font = font;
            this.maximumAllowedWidth = maximumAllowedWidth;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Splits the specified text string.
        /// </summary>
        /// <param name="text">
        /// The text to split. Can be null.
        /// </param>
        /// <returns>
        /// The split text block; or null.
        /// </returns>
        public string[] Split( string text )
        {
            if( text == null )
                return null;

            List<StringBuilder> lines = SplitIntoLines( text );
            string[] finalBlocks = new string[lines.Count];

            for( int i = 0; i < finalBlocks.Length; ++i )
            {
                finalBlocks[i] = lines[i].ToString();
            }

            return finalBlocks;
        }

        /// <summary>
        /// Splits the specified text string into lines.
        /// </summary>
        /// <param name="text">
        /// The text to split. Is not null.
        /// </param>
        /// <returns>
        /// The list of lines.
        /// </returns>
        private List<StringBuilder> SplitIntoLines( string text )
        {
            float offsetX = 0.0f;
            int wordIndex = 0;
            int lineIndex = 0;

            var lines = new List<StringBuilder>();
            var words = text.Split( delimeters, StringSplitOptions.None );

            if( words.Length > 0 )
            {
                lines.Add( new StringBuilder() );
            }

            for( int i = 0; i < words.Length; )
            {
                string word = words[i];
                bool lineEnded = false;

                if( word == "\r\n" )
                {
                    lineEnded = true;
                    ++i;
                }
                else
                {
                    if( wordIndex != 0 )
                        word = ' ' + word;

                    float wordWidth = this.font.MeasureStringWidth( word );
                    float newOffsetX = offsetX + wordWidth;

                    if( word.Length == 0 )
                    {
                        // words with no length are new lines.
                        newOffsetX = this.maximumAllowedWidth;
                    }

                    if( newOffsetX <= this.maximumAllowedWidth || wordWidth > this.maximumAllowedWidth )
                    {
                        lines[lineIndex].Append( word );

                        offsetX = newOffsetX;
                        ++wordIndex;
                        ++i;
                    }
                    else
                    {
                        // Can't fit current word into line,
                        // start a new one and retry the enter the word
                        lineEnded = true;
                    }
                }

                if( lineEnded )
                {
                    lines.Add( new StringBuilder() );

                    ++lineIndex;
                    offsetX = 0;
                    wordIndex = 0;
                }
            }

            return lines;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The maximum allowed width one text block row is allowed to have.
        /// </summary>
        private readonly float maximumAllowedWidth;

        /// <summary>
        /// The font to use to find out how long a single word is.
        /// </summary>
        private readonly IFont font;

        /// <summary>
        /// Additional delimeters which can be used to split the text further.
        /// </summary>
        private readonly string[] delimeters = new string[1] { " " };

        #endregion
    }
}
