// <copyright file="TintedAndScaledOneDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.TintedAndScaledOneDirAnimDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws
    /// the same Animated Sprite for all actions and directions.
    /// </summary>
    /// <remarks>
    /// SpriteGroup format, where X is the SpriteGroup:
    /// 'X'
    /// </remarks>
    public sealed class TintedAndScaledOneDirAnimDrawDataAndStrategy : TintedDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="TintedAndScaledOneDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        public SpriteAnimation Animation
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the scaling factor applied when drawing the SpriteAnimation.
        /// </summary>
        public Atom.Math.Vector2 Scale
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TintedAndScaledOneDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="TintedAndScaledOneDirAnimDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public TintedAndScaledOneDirAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.Scale  = Vector2.One;
            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the TintedAndScaledOneDirAnimDrawDataAndStrategy class.
        /// </summary>
        internal TintedAndScaledOneDirAnimDrawDataAndStrategy()
        {
        }

        #endregion

        #region [ Methods ]

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
            if( this.Animation != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.Animation.Draw( drawPosition, this.FinalColor, 0.0f, Vector2.Zero, this.Scale, drawContext.Batch );
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
            var clone = new TintedAndScaledOneDirAnimDrawDataAndStrategy( newOwner ) {
                Scale       = this.Scale,
                Animation   = this.Animation != null ? this.Animation.Clone() : null
            };

            this.SetupClone( clone );
            return clone;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            context.Write( this.Scale );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            this.Scale = context.ReadVector2();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        #endregion
    }
}
