// <copyright file="FlyingText.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.FlyingText class and FlyingTextEvulator delegate.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using Atom.Collections.Pooling;
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;
      
    /// <summary>
    /// A 'FlyingText' is a text that is displayed ingame, 
    /// above all other game objects, but below the the User Interface.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// This includes things like Damage Done, Damage Taken 
    /// and Information Texts (such as the 'Level Up' text).
    /// </remarks>
    public sealed class FlyingText
    {
        #region [ Properties ]

        /// <summary>
        /// The color of the FlyingText.
        /// </summary>
        public Xna.Color Color;

        /// <summary>
        /// The font to use for rendering.
        /// </summary>
        public IFont Font;

        /// <summary>
        /// The text that is displayed by the FlyingText.
        /// </summary>
        public string Text;

        /// <summary>
        /// The initial time the <see cref="FlyingText"/> last.
        /// </summary>
        public float Time;
        
        /// <summary>
        /// The movement direction of the FlyingText. This should be a normalized vector.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The position of the <see cref="FlyingText"/>.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The scaling factor of the text.
        /// </summary>
        public float Scale = 1.0f;

        /// <summary>
        /// Gets the time the <see cref="FlyingText"/> will last.
        /// </summary>
        public float TimeLeft
        {
            get 
            {
                return this.timeLeft; 
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Resets the time left value to the initial time value.
        /// </summary>
        public void ResetTime()
        {
            this.timeLeft = this.Time;
        }

        /// <summary>
        /// Updates this <see cref="FlyingText"/>.
        /// </summary>
        /// <param name="frameTime">
        /// The time the last frame took (in seconds).
        /// </param>
        public void Update( float frameTime )
        {
            this.timeLeft -= frameTime;

            if( this.timeLeft <= 0.0f )
            {
                this.Manager.Return( this );
                return;
            }
            
            this.Position += this.Velocity * frameTime;
        }
        
        /// <summary>
        /// Draws the <see cref="FlyingText"/> object.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            this.Font.Draw(
                this.Text,
                new Vector2( (int)this.Position.X, (int)this.Position.Y ),
                this.Color,
                0.0f,
                Vector2.Zero,
                this.Scale, 
                SpriteEffects.None, 
                0.0f,
                drawContext
            );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Reference to the <see cref="FlyingTextManager"/> that manages the <see cref="FlyingText"/>.
        /// </summary>
        internal FlyingTextManager Manager;

        /// <summary>
        /// The pool node of the FlyingText.
        /// </summary>
        internal PoolNode<FlyingText> PoolNode;

        /// <summary> 
        /// Specifies the time (in seconds) this <see cref="FlyingText"/> will last.
        /// </summary>
        private float timeLeft;

        #endregion
    }
}
