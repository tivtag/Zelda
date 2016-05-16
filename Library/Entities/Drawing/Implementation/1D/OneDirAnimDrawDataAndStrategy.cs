// <copyright file="OneDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.OneDirAnimDrawDataAndStrategy class.
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
    public sealed class OneDirAnimDrawDataAndStrategy : IAnimatedDrawDataAndStrategy
    {
        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        public string SpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="OneDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        public SpriteAnimation Animation
        {
            get;
            set;
        }

        public SpriteAnimation CurrentAnimation
        {
            get 
            {
                return this.Animation;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="OneDirAnimDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public OneDirAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the OneDirAnimDrawDataAndStrategy class.
        /// </summary>
        public OneDirAnimDrawDataAndStrategy()
        {
        }
        
        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.Animation != null )
                this.Animation.Animate( updateContext.FrameTime );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.Animation != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.Animation.Draw( drawPosition, drawContext.Batch );
            }
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="color">
        /// The color tint that hsould be used instead of the default tint of each sprite.
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
        public void Load( IZeldaServiceProvider serviceProvider )
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
        public IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            return new OneDirAnimDrawDataAndStrategy( newOwner ) {
                SpriteGroup = this.SpriteGroup,
                Animation   = this.Animation != null ? this.Animation.Clone() : null
            };
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.SpriteGroup ?? string.Empty );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.SpriteGroup = context.ReadString();
        }
        
        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
