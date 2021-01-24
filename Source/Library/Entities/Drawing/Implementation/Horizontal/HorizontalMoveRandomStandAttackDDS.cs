// <copyright file="HorizontalMoveRandomStandAttackDDS.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.HorizontalMoveRandomStandAttackDDS class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Entities.Components;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains the data and logic to draw an ZeldaEntity that is moving/attack LEFT and RIGHT.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X__Left, X_Right, X_Stand_Left, X_Stand_Right
    /// </para>
    /// <para>
    /// The ZeldaEntity is required to implement the <see cref="Zelda.Entities.IAttackingEntity"/> interface.
    /// </para>
    /// </remarks>
    public sealed class HorizontalMoveRandomStandAttackDDS : TintedDrawDataAndStrategy
    {
        #region [ Properties ]
  
        /// <summary>
        /// Gets or sets the animation shown when the ZeldaEntity is standing around.
        /// </summary>
        public SpriteAnimation StandingAnimationLeft
        {
            get 
            {
                return this._standingAnimationLeft;
            }

            set
            {
                if( this.StandingAnimationLeft != null )
                    this.StandingAnimationLeft.ReachedEnd -= this.OnStandingAnimation_ReachedEnd;

                this._standingAnimationLeft = value;

                if( this.StandingAnimationLeft != null )
                    this.StandingAnimationLeft.ReachedEnd += this.OnStandingAnimation_ReachedEnd;
            }
        }

        /// <summary>
        /// Gets or sets the animation shown when the ZeldaEntity is standing around.
        /// </summary>
        public SpriteAnimation StandingAnimationRight
        {
            get 
            { 
                return this._standingAnimationRight; 
            }

            set
            {
                if( this.StandingAnimationRight != null )
                    this.StandingAnimationRight.ReachedEnd -= this.OnStandingAnimation_ReachedEnd;

                this._standingAnimationRight = value;

                if( this.StandingAnimationRight != null )
                    this.StandingAnimationRight.ReachedEnd += this.OnStandingAnimation_ReachedEnd;
            }
        }

        /// <summary>
        /// Gets or sets the animation shown when the ZeldaEntity is moving around.
        /// </summary>
        public SpriteAnimation MovingAnimationLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the animation shown when the ZeldaEntity is moving around.
        /// </summary>
        public SpriteAnimation MovingAnimationRight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the animation shown when the ZeldaEntity is attacking.
        /// </summary>
        public SpriteAnimation AttackingAnimationLeft
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the animation shown when the ZeldaEntity is attacking.
        /// </summary>
        public SpriteAnimation AttackingAnimationRight
        {
            get;
            private set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalMoveRandomStandAttackDDS"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the given ZeldaEntity doesn't implement the <see cref="Zelda.Entities.IAttackingEntity"/> interface.
        /// </exception>
        private HorizontalMoveRandomStandAttackDDS( ZeldaEntity entity, RandMT rand )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );
            if( rand == null )
                throw new ArgumentNullException( "rand" );

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

            this.moveable = entity.Components.Get<Moveable>();
            this.transform = entity.Transform;
            this.rand = rand;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalMoveRandomStandAttackDDS"/> class.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        internal HorizontalMoveRandomStandAttackDDS( RandMT rand )
        {
            this.rand = rand;
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
                this.StandingAnimationLeft  = this.StandingAnimationRight = null;
                this.MovingAnimationLeft    = this.MovingAnimationRight = null;
                this.AttackingAnimationLeft = this.AttackingAnimationRight = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                this.MovingAnimationLeft = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Left" ).CreateInstance();
                this.MovingAnimationRight = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Right" ).CreateInstance();
                this.StandingAnimationLeft = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Stand_Left" ).CreateInstance();
                this.StandingAnimationRight = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Stand_Right" ).CreateInstance();

                this.AttackingAnimationLeft = this.MovingAnimationLeft;
                this.AttackingAnimationRight = this.MovingAnimationRight;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( this.current != null )
            {
                var drawPosition = this.transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( 
                    drawPosition, 
                    this.FinalColor, 
                    this.transform.Rotation,
                    this.transform.Origin,
                    this.transform.Scale, 
                    drawContext.Batch
                );
            }
        }

        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            bool isAttacking = this.UpdateAttacking( updateContext );
            this.SelectCurrentAnimation( isAttacking );

            if( this.current != null )
                this.current.Animate( updateContext.FrameTime );

            base.Update( updateContext );
        }
        
        /// <summary>
        /// Selects the animation to draw in the current frame.
        /// </summary>
        /// <param name="isAttacking">
        /// States whether the entity is currently attacking.
        /// </param>
        private void SelectCurrentAnimation( bool isAttacking )
        {
            var direction = transform.Direction;
            var isStanding = moveable.IsStanding;

            if( direction == Direction4.Left )
                this.lastHoriDir = HorizontalDirection.Left;
            else if( direction == Direction4.Right )
                this.lastHoriDir = HorizontalDirection.Right;

            if( isStanding )
            {
                switch( direction )
                {
                    case Direction4.Left:
                        this.SelectStandingAnimation( isAttacking, HorizontalDirection.Left );
                        break;

                    case Direction4.Right:
                        this.SelectStandingAnimation( isAttacking, HorizontalDirection.Right );
                        break;

                    case Direction4.Up:
                    case Direction4.Down:
                        this.SelectStandingAnimation( isAttacking, this.lastHoriDir );
                        break;

                    default:
                        this.current = null;
                        break;
                }
            }
            else
            {
                switch( direction )
                {
                    case Direction4.Left:
                        this.SelectMovingAnimation( isAttacking, HorizontalDirection.Left );
                        break;

                    case Direction4.Right:
                        this.SelectMovingAnimation( isAttacking, HorizontalDirection.Right );
                        break;

                    case Direction4.Up:
                    case Direction4.Down:
                        this.SelectMovingAnimation( isAttacking, this.lastHoriDir );
                        break;

                    default:
                        this.current = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets whether the attacking animation should be shown
        /// by updating the currently known attacking state.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        /// <returns>
        /// Whether the attack animation should be shown.
        /// </returns>
        private bool UpdateAttacking( ZeldaUpdateContext updateContext )
        {
            bool isAttacking = attackingEntity.IsAttacking;

            if( !isAttacking && wasAttacking )
                this.attackExtensionTick = MaximumAttackTick;

            if( this.attackExtensionTick > 0.0f )
            {
                this.attackExtensionTick -= updateContext.FrameTime;
                isAttacking = true;
            }

            this.wasAttacking = isAttacking;
            return isAttacking;
        }

        /// <summary>
        /// Selects the correct SpriteAnimation for the current Sprite.
        /// </summary>
        /// <param name="showAttackAnimation">
        /// States whether the attacking animation should be shown.
        /// </param>
        /// <param name="direction">
        /// The direction the entity is facing.
        /// </param>
        private void SelectStandingAnimation( bool showAttackAnimation, HorizontalDirection direction )
        {
            if( showAttackAnimation )
            {
                this.current = direction == HorizontalDirection.Left ? 
                    this.AttackingAnimationLeft : this.AttackingAnimationRight;
            }
            else
            {
                this.current = direction == HorizontalDirection.Left ? 
                    this.StandingAnimationLeft : this.StandingAnimationRight;
            }
        }

        /// <summary>
        /// Selects the correct SpriteAnimation for the current Sprite.
        /// </summary>
        /// <param name="showAttackAnimation">
        /// States whether the attacking animation should be shown.
        /// </param>
        /// <param name="direction">
        /// The direction the entity is facing.
        /// </param>
        private void SelectMovingAnimation( bool showAttackAnimation, HorizontalDirection direction )
        {
            if( showAttackAnimation )
            {
                this.current = direction == HorizontalDirection.Left ? 
                    this.AttackingAnimationLeft : this.AttackingAnimationRight;
            }
            else
            {
                this.current = direction == HorizontalDirection.Left ? 
                    this.MovingAnimationLeft : this.MovingAnimationRight;
            }
        }

        /// <summary>
        /// Called when the standing animation has ended.
        /// </summary>
        /// <param name="animation">
        /// The sender of the event.
        /// </param>
        private void OnStandingAnimation_ReachedEnd( SpriteAnimation animation )
        {
            var animatedSprite = animation.AnimatedSprite;
            float animSpeed = animatedSprite.DefaultAnimationSpeed;

            // Randomize the speed of the next animation loop.
            animation.AnimationSpeed = rand.RandomRange( 0.5f * animSpeed, 2.0f * animSpeed );
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="LeftRightMoveAttackDDS"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new HorizontalMoveRandomStandAttackDDS( newOwner, this.rand ) {
                MovingAnimationLeft = this.MovingAnimationLeft,
                MovingAnimationRight = this.MovingAnimationRight,
                StandingAnimationLeft  = this.StandingAnimationLeft,
                StandingAnimationRight = this.StandingAnimationRight,
                AttackingAnimationLeft = this.AttackingAnimationLeft,
                AttackingAnimationRight = this.AttackingAnimationRight
            };

            this.SetupClone( clone );
            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the last horizontal direction of the object.
        /// </summary>
        private HorizontalDirection lastHoriDir = HorizontalDirection.Left;

        /// <summary>
        /// The currently selected <see cref="SpriteAnimation"/>.
        /// </summary>
        private SpriteAnimation current;

        /// <summary>
        /// The time for which the attack animation will be shown
        /// for, even after the entity has stopped attacking.
        /// </summary>
        private const float MaximumAttackTick = 1.0f;

        /// <summary>
        /// The time left for which the attack animation will be shown
        /// for, even after the entity has stopped attacking.
        /// </summary>
        /// <seealso cref="MaximumAttackTick"/>
        private float attackExtensionTick;

        /// <summary>
        /// States whether the entity was attacking last frame.
        /// </summary>
        private bool wasAttacking;

        /// <summary>
        /// Provides a mechanism to receive a value that indicates whether
        /// the ZeldaEntity is currently attacking.
        /// </summary>
        private readonly IAttackingEntity attackingEntity;

        /// <summary>
        /// The <see cref="ZeldaTransform"/> component of the ZeldaEntity.
        /// </summary>
        private readonly ZeldaTransform transform;

        /// <summary>
        /// The <see cref="Moveable"/> component of the ZeldaEntity.
        /// </summary>
        private readonly Moveable moveable;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;
        
        /// <summary>
        /// The storage field of the <see cref="StandingAnimationLeft"/> property.
        /// </summary>
        private SpriteAnimation _standingAnimationLeft;

        /// <summary>
        /// The storage field of the <see cref="StandingAnimationRight"/> property.
        /// </summary>
        private SpriteAnimation _standingAnimationRight;

        #endregion
    }
}