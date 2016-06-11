// <copyright file="TintedOneDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.TintedOneDirAnimDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws
    /// the same Animated Sprite for all actions and directions.
    /// </summary>
    /// <remarks>
    /// SpriteGroup format, where X is the SpriteGroup:
    /// 'X'
    /// </remarks>
    public sealed class TintedOneDirAnimDrawDataAndStrategy : TintedDrawDataAndStrategy, IAnimatedDrawDataAndStrategy
    {
        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="TintedOneDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        public SpriteAnimation Animation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the currently shown SpriteAnimation.
        /// </summary>
        public SpriteAnimation CurrentAnimation
        {
            get
            {
                return this.Animation;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TintedOneDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="TintedOneDirAnimDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public TintedOneDirAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the TintedOneDirAnimDrawDataAndStrategy class.
        /// </summary>
        internal TintedOneDirAnimDrawDataAndStrategy()
        {
        }

        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.Animation != null )
                this.Animation.Animate( updateContext.FrameTime );

            base.Update( updateContext );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            Draw( this.FinalColor, drawContext );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="color">
        /// The final color tint to use.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( Microsoft.Xna.Framework.Color color, ZeldaDrawContext drawContext )
        {
            if( this.Animation != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.Animation.Draw( drawPosition, color, drawContext.Batch );
            }
        }

        /// <summary>
        /// Loads the assets needed by this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Load( IZeldaServiceProvider serviceProvider )
        {
            if( string.IsNullOrEmpty( this.SpriteGroup ) )
            {
                this.Animation = null;
            }
            else
            {
                this.Animation = serviceProvider.SpriteLoader.LoadAnimatedSprite( this.SpriteGroup ).CreateInstance();
            }
        }

        /// <summary>
        /// Clones this <see cref="IDrawDataAndStrategy"/> for use by the specified object.
        /// </summary>
        /// <param name="newOwner">
        /// The new owner of the cloned <see cref="IDrawDataAndStrategy"/>.
        /// </param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="newOwner"/> is null.
        /// </exception>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new TintedOneDirAnimDrawDataAndStrategy( newOwner ) {
                Animation   = this.Animation != null ? this.Animation.Clone() : null
            };

            this.SetupClone( clone );
            return clone;
        }
        
        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
