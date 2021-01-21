// <copyright file="BubbleText.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.BubbleText class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.UI;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Represents a text that shows up within the gameworld as a bubble over an object.
    /// </summary>
    public sealed class BubbleText
    {
        private const float BlendOutTime = 0.8f;
        private const float AlphaFactor = 0.65f;
        private static readonly Xna.Color ColorBackground = Xna.Color.Black;

        /// <summary>
        /// Gets or sets the <see cref="EventHandler"/> that is called when this BubbleText stops to be shown.
        /// </summary>
        public EventHandler Ended
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the area which this <see cref="BubbleText"/> covers.
        /// </summary>
        public RectangleF Area
        {
            get
            {
                Vector2 sizeF = this.Text.TextBlockSize;
                Point2 size = new Point2( (int)sizeF.X + 4, (int)sizeF.Y );
                int halfWidth = (int)(size.X / 2);

                Vector2 position = this.Entity.Transform.Position + new Vector2( this.Entity.Collision.Offset.X + this.Entity.Collision.Width / 2.0f - halfWidth, -size.Y - 1 );
                position.X = (float)Math.Floor( position.X );

                return new RectangleF( position, size );
            }
        }

        /// <summary>
        /// Gets the position where the text of this <see cref="BubbleText"/> is drawn.
        /// </summary>
        public Vector2 TextPosition
        {
            get
            {
                Vector2 size = this.Text.TextBlockSize;
                Vector2 position = this.Entity.Transform.Position + new Vector2( this.Entity.Collision.Offset.X + this.Entity.Collision.Width / 2.0f, -size.Y - 1 );
                position.X = (float)Math.Floor( position.X );

                return position;
            }
        }

        /// <summary>
        /// Gets or sets the parent entity over which this <see cref="BubbleText"/> hovers.
        /// </summary>
        public ZeldaEntity Entity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text which is displayed by this <see cref="BubbleText"/>.
        /// </summary>
        public Text Text
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the time left in seconds for which this <see cref="BubbleText"/> will stay visible.
        /// </summary>
        public float TimeLeft
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Draws this <see cref="BubbleText"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect( this.Area, this.effect.Apply( ColorBackground ).MultiplyBy( 1.0f, 1.0f, 1.0f, AlphaFactor ) );
            this.Text.Draw( this.TextPosition, drawContext );
        }

        /// <summary>
        /// Sets the duration for which this <see cref="BubbleText"/> shoudl be shown.
        /// </summary>
        /// <param name="duration">
        /// The duration in seconds.
        /// </param>
        public void SetDuration( float duration )
        {
            this.TimeLeft = duration;
            
            if( this.effect != null )
            {
                this.Text.RemoveColorEffect( effect );
            }

            this.effect = new AlphaBlendInOutColorEffect( duration, 2, duration - BlendOutTime );
            this.Text.Color = Xna.Color.White.WithAlpha( 0 );

            this.Text.AddColorEffect( effect );
        }

        /// <summary>
        /// Forces this BubbleText to blend out, even if there is still time left.
        /// </summary>
        public void ForceBlendOut()
        {
            if( !this.effect.IsBlendingOut )
            {
                this.effect.Time = this.effect.EndMaxAlphaTime;
                this.TimeLeft = BlendOutTime;
            }
        }

        private Atom.Xna.Effects.AlphaBlendInOutColorEffect effect;
    }
}
