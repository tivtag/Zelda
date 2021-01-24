// <copyright file="OneDirAnimMoveStandDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.OneDirAnimMoveStandDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws
    /// an Animated Sprite if the entity is moving;
    /// and a normal Sprite when standing.
    /// </summary>
    /// <remarks>
    /// SpriteGroup format, where X is the SpriteGroup:
    /// 'X'
    /// 'X_Standing'
    /// </remarks>
    public sealed class OneDirAnimMoveStandDrawDataAndStrategy : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        public string SpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="OneDirAnimMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently moving.
        /// </summary>
        public SpriteAnimation MovingAnimation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Sprite"/> displayed by this <see cref="OneDirAnimMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently standing.
        /// </summary>
        public Sprite StandingSprite
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDirAnimMoveStandDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="OneDirAnimMoveStandDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public OneDirAnimMoveStandDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.moveable = entity.Components.Get<Components.Moveable>();

            if( this.moveable == null )
            {
                throw new ArgumentException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Atom.ErrorStrings.EntityIsRequiredToHaveComponentType,
                        "Zelda.Entities.Components.Moveable"
                    )
                );
            }

            this.entity = entity;
            this.attackingEntity = entity as IAttackingEntity;
        }

        /// <summary>
        /// Initializes a new instance of the OneDirAnimMoveStandDrawDataAndStrategy class.
        /// </summary>
        internal OneDirAnimMoveStandDrawDataAndStrategy()
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
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.moveable.IsStanding || (this.attackingEntity != null && this.attackingEntity.IsAttacking) )
            {
                if( this.MovingAnimation != null )
                    this.MovingAnimation.Animate( updateContext.FrameTime );

                this.currentSprite = this.MovingAnimation;
            }
            else
            {
                this.currentSprite = this.StandingSprite;
            }        
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.currentSprite != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.currentSprite.Draw( drawPosition, drawContext.Batch );
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
                this.MovingAnimation = null;
                this.StandingSprite  = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;
                this.StandingSprite  = spriteLoader.LoadSprite( this.SpriteGroup + "_Standing" );
                this.MovingAnimation = spriteLoader.LoadAnimatedSprite( this.SpriteGroup ).CreateInstance();
            }
        }

        #region > Cloning <

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
            return new OneDirAnimMoveStandDrawDataAndStrategy( newOwner ) {
                SpriteGroup     = this.SpriteGroup,
                MovingAnimation = this.MovingAnimation != null ? this.MovingAnimation.Clone() : null,
                StandingSprite  = this.StandingSprite
            };
        }

        #endregion

        #region > Storage <

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

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the currently drawn sprite.
        /// </summary>
        private ISprite currentSprite;

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Provides a mechanism to receive a value that is indicating whether
        /// the ZeldaEntity is currently attacking.
        /// </summary>
        private readonly IAttackingEntity attackingEntity;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        #endregion
    }
}
