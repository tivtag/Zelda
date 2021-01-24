// <copyright file="BubbleTextManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.BubbleTextManager class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Zelda.Entities;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Manages the <see cref="BubbleText"/>-instances currently active within the game.
    /// </summary>
    public sealed class BubbleTextManager : IZeldaUpdateable
    {
        private static readonly Xna.Color ColorFont = Xna.Color.White;

        /// <summary>
        /// The number of active <see cref="BubbleText"/>-instances.
        /// </summary>
        public int ActiveTextCount => this.texts.Count;

        /// <summary>
        /// Loads the content of this BubbleTextManager.
        /// </summary>
        public void LoadContent()
        {
            this.splitter = new TextBlockSplitter( UIFonts.Tahoma10, 120.0f );
        }

        /// <summary>
        /// Shows a new BubbeText-Instance under the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="text">
        /// The text to display.
        /// </param>
        /// <param name="duration">
        /// The duration in seconds for which the BubbleText should shown.
        /// </param>
        /// <param name="speaker">
        /// The speaker under which the BubbleText should eb shown.
        /// </param>
        public void ShowText( string text, float duration, ZeldaEntity speaker )
        {
            if( speaker == null )
            {
                return;
            }

            BubbleText bubbleText = this.FindTextOf( speaker );

            if( bubbleText == null )
            {
                bubbleText = new BubbleText() {
                    Entity = speaker
                };

                this.texts.Add( bubbleText );
                this.SetupText( text, duration, bubbleText );
            }
            else
            {
                bubbleText.ForceBlendOut();
                bubbleText.Ended = (sender, e) => this.ShowText( text, duration, speaker );
            }
        }

        private void SetupText( string text, float duration, BubbleText bubbleText )
        {
            bubbleText.Text = new Text( UIFonts.Tahoma7, TextAlign.Center, ColorFont, this.splitter ) {
                TextString = text
            };
            bubbleText.SetDuration( duration );
        }

        /// <summary>
        /// Tries to find the text that is currently shown under the given <see cref="ZeldaEntity"/>.
        /// </summary>
        public BubbleText FindTextOf( ZeldaEntity speaker )
        {
            return this.texts.FirstOrDefault( t => t.Entity == speaker );
        }

        /// <summary>
        /// Updates this BubbleTextManager.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.texts.Count; ++i )
            {
                BubbleText text = this.texts[i];
                text.TimeLeft -= updateContext.FrameTime;

                if( text.TimeLeft <= 0.0f || text.Entity.Scene == null )
                {
                    this.texts.RemoveAt( i );
                    text.Ended.Raise( text );
                    --i;
                }
                else
                {
                    text.Text.Update( updateContext );
                }
            }
        }

        /// <summary>
        /// Draws this BubbleTextManager.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            for( int i = 0; i < this.texts.Count; ++i )
            {
                this.texts[i].Draw( drawContext );
            }
        }

        /// <summary>
        /// Removes all <see cref="BubbleText"/>s.
        /// </summary>
        public void Clear()
        {
            this.texts.Clear();
        }

        private readonly List<BubbleText> texts = new List<BubbleText>();

        private ITextBlockSplitter splitter;
    }
}
