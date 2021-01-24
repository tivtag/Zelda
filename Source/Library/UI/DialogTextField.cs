// <copyright file="DialogTextField.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.DialogTextField class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;

    /// <summary>
    /// Defines the <see cref="TextField"/> UI control
    /// that is used to display a simple one-way dialog.
    /// </summary>
    public class DialogTextField : SpriteTextField
    {
        /// <summary>
        /// Gets or sets the sprite shown when the <see cref="TextField.Text"/>
        /// has reached the end of the current block.
        /// </summary>
        public Sprite SpriteReachedBlockEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sprite shown when the <see cref="TextField.Text"/>
        /// has reached the end.
        /// </summary>
        public Sprite SpriteReachedEnd
        {
            get;
            set;
        }
                
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogTextField"/> class.
        /// </summary>
        public DialogTextField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogTextField"/> class.
        /// </summary>
        /// <param name="name">The name of the new <see cref="DialogTextField"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is null.
        /// </exception>
        public DialogTextField( string name )
            : base( name )
        {
        }
        
        /// <summary>
        /// Called when this DialogTextField is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            base.OnDraw( drawContext );
            
            var text = this.Text as AnimatedTypeWriterText;
            if( text == null )
                return;

            if( text.HasReachedEnd )
            {
                this.DrawTextReachedEnd( drawContext );
            }
            else if( text.HasReachedEndOfBlock )
            {
                this.DrawReachedEndOfBlock( drawContext );
            }
        }

        /// <summary>
        /// Draws an indicator that the text-field has reached the end.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawTextReachedEnd( ISpriteDrawContext drawContext )
        {
            if( this.SpriteReachedEnd != null )
            {
                var position = this.Position + this.SpriteOffset +
                    new Vector2(
                        (this.Sprite.Width / 2.0f) - (this.SpriteReachedEnd.Width / 2.0f),
                        this.Sprite.Height - (SpriteReachedEnd.Height / 2.0f)
                    );

                // Clamp to remove blur effect
                position.X = (int)position.X;
                position.Y = (int)position.Y;

                this.SpriteReachedEnd.Draw( position, 0.75f, drawContext.Batch );
            }
        }

        /// <summary>
        /// Draws an indicator that the text-field has reached the end of a text block.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawReachedEndOfBlock( ISpriteDrawContext drawContext )
        {
            if( this.SpriteReachedBlockEnd != null )
            {
                var position = this.Position + this.SpriteOffset +
                    new Vector2(
                        (this.Sprite.Width / 2.0f) - (this.SpriteReachedBlockEnd.Width / 2.0f),
                        this.Sprite.Height - (SpriteReachedBlockEnd.Height / 2.0f)
                    );

                // Clamp to remove blur effect
                position.X = (int)position.X;
                position.Y = (int)position.Y;

                this.SpriteReachedBlockEnd.Draw( position, 0.75f, drawContext.Batch );
            }
        }
    }
}
