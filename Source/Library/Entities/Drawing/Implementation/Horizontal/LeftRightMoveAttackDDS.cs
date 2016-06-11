// <copyright file="LeftRightMoveAttackDDS.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.LeftRightMoveAttackDDS class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains the data and logic to draw an ZeldaEntity that is moving/attack LEFT and RIGHT.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X_Move_Left, X_Move_Right, X_Attack_Left, X_Attack_Right
    /// </para>
    /// <para>
    /// The ZeldaEntity is required to implement the <see cref="Zelda.Entities.IAttackingEntity"/> interface.
    /// </para>
    /// </remarks>
    public sealed class LeftRightMoveAttackDDS : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the set Sprite Group of the <see cref="IDrawDataAndStrategy"/>.
        /// Is null until initialized.
        /// </summary>
        public string SpriteGroup
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftRightMoveAttackDDS"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given ZeldaEntity doesn't implement the <see cref="Zelda.Entities.IAttackingEntity"/> interface.
        /// </exception>
        public LeftRightMoveAttackDDS( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity;
            this.attackingEntity = entity as IAttackingEntity;

            if( this.attackingEntity == null )
            {
                throw new ArgumentException(
                    string.Format(
                       System.Globalization.CultureInfo.CurrentCulture,
                       Atom.ErrorStrings.ObjectXDoesntImplementY,
                       "entity",
                       "Zelda.Entities.IAttackingEntity"
                   ),
                   "entity"
               );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftRightMoveAttackDDS"/> class.
        /// </summary>
        internal LeftRightMoveAttackDDS()
        {
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
                animMoveLeft = animMoveRight = animAttackLeft = animAttackRight = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                this.animMoveLeft    = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Move_Left" ).CreateInstance();
                this.animMoveRight   = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Move_Right" ).CreateInstance();
                this.animAttackLeft  = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Attack_Left" ).CreateInstance();
                this.animAttackRight = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Attack_Right" ).CreateInstance();
            }
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
            bool     isAttacking = attackingEntity.IsAttacking;

            if( direction == Direction4.Left )
                lastHoriDir = HorizontalDirection.Left;
            else if( direction == Direction4.Right )
                lastHoriDir = HorizontalDirection.Right;

            switch( direction )
            {
                case Direction4.Left:
                    current = isAttacking ? animAttackLeft : animMoveLeft;
                    break;

                case Direction4.Right:
                    current = isAttacking ? animAttackRight : animMoveRight;
                    break;

                case Direction4.Up:
                    switch( lastHoriDir )
                    {
                        case HorizontalDirection.Left:
                            current = isAttacking ? animAttackLeft : animMoveLeft;
                            break;
                        case HorizontalDirection.Right:
                            current = isAttacking ? animAttackRight : animMoveRight;
                            break;
                        default:
                            break;
                    }

                    break;

                case Direction4.Down:
                    switch( lastHoriDir )
                    {
                        case HorizontalDirection.Left:
                            current = isAttacking ? animAttackLeft : animMoveLeft;
                            break;
                        case HorizontalDirection.Right:
                            current = isAttacking ? animAttackRight : animMoveRight;
                            break;
                        default:
                            break;
                    }

                    break;

                default:
                    current = null;
                    break;
            }

            if( current != null )
                current.Animate( updateContext.FrameTime );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( current != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( drawPosition, drawContext.Batch );
            }
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="LeftRightMoveAttackDDS"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            return new LeftRightMoveAttackDDS( newOwner )
            {
                SpriteGroup     = this.SpriteGroup,

                animMoveLeft    = GetSpriteAnimationClone( this.animMoveLeft ),
                animMoveRight   = GetSpriteAnimationClone( this.animMoveRight ),
                animAttackLeft  = GetSpriteAnimationClone( this.animAttackLeft ),
                animAttackRight = GetSpriteAnimationClone( this.animAttackRight )
            };
        }

        /// <summary>
        /// Helper function that creates a clone of the given SpriteAnimation.
        /// </summary>
        /// <param name="animation">
        /// The SpriteAnimation to clone. Can be null.
        /// </param>
        /// <returns>
        /// The cloned SpriteAnimation. Might be null.
        /// </returns>
        private static Atom.Xna.SpriteAnimation GetSpriteAnimationClone( Atom.Xna.SpriteAnimation animation )
        {
            if( animation == null )
                return null;

            return animation.Clone();
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
        /// The object that gets visualized by the <see cref="LeftRightMoveAttackDDS"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Provides a mechanism to receive a value that indicates whether
        /// the ZeldaEntity is currently attacking.
        /// </summary>
        private readonly IAttackingEntity attackingEntity;

        /// <summary>
        /// The movement animations.
        /// </summary>
        private SpriteAnimation animMoveLeft, animMoveRight;

        /// <summary>
        /// The attack animations.
        /// </summary>
        private SpriteAnimation animAttackLeft, animAttackRight;

        /// <summary>
        /// The currently selected <see cref="SpriteAnimation"/>.
        /// </summary>
        private SpriteAnimation current;

        #endregion
    }
}
