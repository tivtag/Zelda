// <copyright file="TintedLeftRightMoveAttackDDS.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.TintedLeftRightMoveAttackDDS class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="TintedDrawDataAndStrategy"/>
    /// which contains data for moving/attack LEFT and RIGHT.
    /// </summary>
    /// <remarks>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X_Move_Left, X_Move_Right, X_Attack_Left, X_Attack_Right
    /// </remarks>
    public sealed class TintedLeftRightMoveAttackDDS : TintedDrawDataAndStrategy
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TintedLeftRightMoveAttackDDS"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public TintedLeftRightMoveAttackDDS( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = (Enemy)entity;
        }

        /// <summary>
        /// Initializes a new instance of the TintedLeftRightMoveAttackDDS class.
        /// </summary>
        internal TintedLeftRightMoveAttackDDS()
        {
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
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.SelectCurrentSprite();

            if( this.current != null )
                this.current.Animate( updateContext.FrameTime );

            base.Update( updateContext );
        }

        /// <summary>
        /// Selects the <see cref="current"/> SpriteAnimation.
        /// </summary>
        private void SelectCurrentSprite()
        {
            Direction4 direction = entity.Transform.Direction;
            bool isAttacking = entity.IsAttacking;

            if( direction == Direction4.Left )
                this.lastHoriDir = HorizontalDirection.Left;
            else if( direction == Direction4.Right )
                this.lastHoriDir = HorizontalDirection.Right;

            switch( direction )
            {
                case Direction4.Left:
                    this.current = isAttacking ? animAttackLeft : animMoveLeft;
                    break;

                case Direction4.Right:
                    this.current = isAttacking ? animAttackRight : animMoveRight;
                    break;

                case Direction4.Up:
                    switch( this.lastHoriDir )
                    {
                        case HorizontalDirection.Left:
                            this.current = isAttacking ? animAttackLeft : animMoveLeft;
                            break;
                        case HorizontalDirection.Right:
                            this.current = isAttacking ? animAttackRight : animMoveRight;
                            break;
                        default:
                            break;
                    }

                    break;

                case Direction4.Down:
                    switch( this.lastHoriDir )
                    {
                        case HorizontalDirection.Left:
                            this.current = isAttacking ? animAttackLeft : animMoveLeft;
                            break;
                        case HorizontalDirection.Right:
                            this.current = isAttacking ? animAttackRight : animMoveRight;
                            break;
                        default:
                            break;
                    }

                    break;

                default:
                    this.current = null;
                    break;
            }
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( current != null )
            {
                var transform = entity.Transform;
                var drawPosition = transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( drawPosition, this.FinalColor, transform.Rotation, transform.Origin, transform.Scale, drawContext.Batch );
            }
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="TintedLeftRightMoveAttackDDS"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var color = new TintedLeftRightMoveAttackDDS( newOwner ) {
                animMoveLeft    = GetSpriteAnimationClone( this.animMoveLeft ),
                animMoveRight   = GetSpriteAnimationClone( this.animMoveRight ),
                animAttackLeft  = GetSpriteAnimationClone( this.animAttackLeft ),
                animAttackRight = GetSpriteAnimationClone( this.animAttackRight )
            };

            this.SetupClone( color );
            return color;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the last horizontal direction of the object.
        /// </summary>
        private HorizontalDirection lastHoriDir = HorizontalDirection.Left;

        /// <summary>
        /// The object that gets visualized by the <see cref="TintedLeftRightMoveAttackDDS"/>.
        /// </summary>
        private readonly Enemy entity;

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
