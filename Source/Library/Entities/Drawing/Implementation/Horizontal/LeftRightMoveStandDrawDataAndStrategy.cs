// <copyright file="LeftRightMoveStandDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.LeftRightMoveStandDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
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
    public sealed class LeftRightMoveStandDrawDataAndStrategy : IDrawDataAndStrategy
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
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="LeftRightMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently moving left.
        /// </summary>
        public SpriteAnimation MovingAnimationLeft
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="LeftRightMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently moving right.
        /// </summary>
        public SpriteAnimation MovingAnimationRight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Sprite"/> displayed by this <see cref="LeftRightMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently standing.
        /// </summary>
        public Sprite StandingSpriteLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Sprite"/> displayed by this <see cref="LeftRightMoveStandDrawDataAndStrategy"/>
        /// if the entity is currently standing.
        /// </summary>
        public Sprite StandingSpriteRight
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftRightMoveStandDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="LeftRightMoveStandDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public LeftRightMoveStandDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );
            
            this.entity = entity;
            this.moveable = entity.Components.Get<Components.Moveable>();
            Atom.ThrowHelper.IfComponentNull( this.moveable );
        }

        /// <summary>
        /// Initializes a new instance of the LeftRightMoveStandDrawDataAndStrategy class.
        /// </summary>
        internal LeftRightMoveStandDrawDataAndStrategy()
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
            Direction4 direction = entity.Transform.Direction;

            if( direction == Direction4.Left )
                this.lastHoriDir = HorizontalDirection.Left;
            else if( direction == Direction4.Right )
                this.lastHoriDir = HorizontalDirection.Right;

            if( !this.moveable.IsStanding )
            {
                switch( direction )
                {
                    case Direction4.Left:
                        this.currentSprite = this.MovingAnimationLeft;
                        break;

                    case Direction4.Right:
                        this.currentSprite = this.MovingAnimationRight;
                        break;

                    case Direction4.Up:
                    case Direction4.Down:
                        switch( this.lastHoriDir )
                        {
                            case HorizontalDirection.Left:
                                this.currentSprite = this.MovingAnimationLeft;
                                break;
                            case HorizontalDirection.Right:
                                this.currentSprite = this.MovingAnimationRight;
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        this.currentSprite = null;
                        break;
                }
            }
            else
            {
                switch( direction )
                {
                    case Direction4.Left:
                        this.currentSprite = this.StandingSpriteLeft;
                        break;

                    case Direction4.Right:
                        this.currentSprite = this.StandingSpriteRight;
                        break;

                    case Direction4.Up:
                    case Direction4.Down:
                        switch( this.lastHoriDir )
                        {
                            case HorizontalDirection.Left:
                                this.currentSprite = this.StandingSpriteLeft;
                                break;
                            case HorizontalDirection.Right:
                                this.currentSprite = this.StandingSpriteRight;
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        this.currentSprite = null;
                        break;
                }
            }

            var updateable = this.currentSprite as Atom.IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
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
                this.StandingSpriteLeft   = null;
                this.StandingSpriteRight  = null;
                this.MovingAnimationLeft  = null;
                this.MovingAnimationRight = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                this.StandingSpriteLeft = spriteLoader.LoadSprite( this.SpriteGroup + "_Stand_Left" );
                this.StandingSpriteRight = spriteLoader.LoadSprite( this.SpriteGroup + "_Stand_Right" );
                this.MovingAnimationLeft = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Left" ).CreateInstance();
                this.MovingAnimationRight = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Right" ).CreateInstance();
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
            return new LeftRightMoveStandDrawDataAndStrategy( newOwner ) {
                SpriteGroup = this.SpriteGroup,
                StandingSpriteLeft = this.StandingSpriteLeft,
                StandingSpriteRight  = this.StandingSpriteRight,
                MovingAnimationLeft  = this.MovingAnimationLeft != null ? this.MovingAnimationLeft.Clone() : null,
                MovingAnimationRight = this.MovingAnimationRight != null ? this.MovingAnimationRight.Clone() : null
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
        /// Stores the last horizontal direction of the object.
        /// </summary>
        private HorizontalDirection lastHoriDir = HorizontalDirection.Left;

        /// <summary>
        /// Stores the currently drawn sprite.
        /// </summary>
        private ISprite currentSprite;

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        #endregion
    }
}
