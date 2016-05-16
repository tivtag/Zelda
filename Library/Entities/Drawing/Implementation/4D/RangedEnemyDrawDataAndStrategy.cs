// <copyright file="RangedEnemyDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.RangedEnemyDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Entities.Projectiles;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains data for moving LEFT, RIGHT, UP and DOWN.
    /// </summary>
    /// <remarks>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// <para>
    /// X_Left, X_Right, X_Up, X_Down
    /// X_Attack_Ranged_Left
    /// X_Attack_Ranged_Right,
    /// X_Attack_Ranged_Up
    /// X_Attack_Ranged_Down
    /// </para>
    /// They are required to be AnimatedSprites.
    /// </remarks>
    public sealed class RangedEnemyDrawDataAndStrategy : TintedDrawDataAndStrategy, IPostLoad
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangedEnemyDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <see cref="ZeldaEntity"/> doesn't own the <see cref="Zelda.Entities.Components.Moveable"/> component.
        /// </exception>
        public RangedEnemyDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );                     

            this.enemy = (Enemy)entity;
            this.moveable = this.enemy.Moveable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedEnemyDrawDataAndStrategy"/> class.
        /// </summary>
        internal RangedEnemyDrawDataAndStrategy()
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
                this.animMoveLeft = this.animMoveRight = this.animMoveUp = this.animMoveDown = null;
                this.animAttackRangedLeft = this.animAttackRangedRight = null;
                this.animAttackRangedUp = this.animAttackRangedDown = null;
            }
            else
            {
                string path = this.SpriteGroup;
                var spriteLoader = serviceProvider.SpriteLoader;

                this.animMoveLeft  = spriteLoader.LoadAnimatedSprite( path + "_Left" ).CreateInstance();
                this.animMoveRight = spriteLoader.LoadAnimatedSprite( path + "_Right" ).CreateInstance();
                this.animMoveUp    = spriteLoader.LoadAnimatedSprite( path + "_Up" ).CreateInstance();
                this.animMoveDown  = spriteLoader.LoadAnimatedSprite( path + "_Down" ).CreateInstance();

                path = this.SpriteGroup + "_Attack_Ranged_";
                this.animAttackRangedLeft  = spriteLoader.LoadAnimatedSprite( path + "Left" ).CreateInstance();
                this.animAttackRangedRight = spriteLoader.LoadAnimatedSprite( path + "Right" ).CreateInstance();
                this.animAttackRangedUp    = spriteLoader.LoadAnimatedSprite( path + "Up" ).CreateInstance();
                this.animAttackRangedDown  = spriteLoader.LoadAnimatedSprite( path + "Down" ).CreateInstance();
            }
        }

        public void PostLoad(IZeldaServiceProvider serviceProvider)
        {
            var rangedBehaviour = this.enemy.Behaveable.Behaviour as Zelda.Entities.Behaviours.IRangedEnemyBehaviour;
            if (rangedBehaviour == null)
                throw new ArgumentException(Resources.Error_RequiresIRangedEnemyBehaviour, "entity");

            var rangedAttack = rangedBehaviour.RangedAttack;
            if (rangedAttack != null)
            {
                rangedAttack.ProjectileFired += OnProjectileFired;
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
            this.SelectCurrentSprite();
            this.UpdateCurrent( updateContext );
            base.Update( updateContext );
        }

        /// <summary>
        /// Selects the current sprite to draw.
        /// </summary>
        private void SelectCurrentSprite()
        {
            if( this.showAttackAnimation )
                return;

            switch( enemy.Transform.Direction )
            {
                case Direction4.Left:
                    this.current = animMoveLeft;
                    break;
                case Direction4.Right:
                    this.current = animMoveRight;
                    break;
                case Direction4.Up:
                    this.current = animMoveUp;
                    break;
                case Direction4.Down:
                    this.current = animMoveDown;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Updates/Animates the current sprite.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateCurrent( ZeldaUpdateContext updateContext )
        {
            if( this.current != null )
            {
                if( this.moveable.IsStanding && !this.showAttackAnimation )
                {
                    this.current.Reset();
                }
                else
                {
                    this.current.Animate( updateContext.FrameTime );
                }
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
                var drawPosition = enemy.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( drawPosition, this.FinalColor, drawContext.Batch );
            }
        }

        /// <summary>
        /// Gets called when the enemy has fired a Projectile.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="projectile">The Projectile that has been fired.</param>
        private void OnProjectileFired( object sender, Projectile projectile )
        {
            this.showAttackAnimation = true;
            this.current = this.GetRangedAttackAnimation();
            this.current.Reset();
        }

        /// <summary>
        /// Gets called when the current Ranged Attack animation has ended.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        private void OnRangedAttackAnimationHasReachedEnd( SpriteAnimation sender )
        {
            this.current = null;
            this.showAttackAnimation = false;
            this.SelectCurrentSprite();
        }
        
        /// <summary>
        /// Gets the SpriteAnimation that should be used to visualize a ranged attack.
        /// </summary>
        /// <returns></returns>
        private SpriteAnimation GetRangedAttackAnimation()
        {
            switch( this.enemy.Transform.Direction )
            {
                default:
                case Direction4.Down:
                    return this.animAttackRangedDown;

                case Direction4.Up:
                    return this.animAttackRangedUp;

                case Direction4.Left:
                    return this.animAttackRangedLeft;

                case Direction4.Right:
                    return this.animAttackRangedRight;
            }
        }
        
        /// <summary>
        /// Creates a clone of this <see cref="RangedEnemyDrawDataAndStrategy"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new RangedEnemyDrawDataAndStrategy( newOwner ) {
                animMoveLeft  = this.animMoveLeft  == null ? null : this.animMoveLeft.Clone(),
                animMoveRight = this.animMoveRight == null ? null : this.animMoveRight.Clone(),
                animMoveUp    = this.animMoveUp    == null ? null : this.animMoveUp.Clone(),
                animMoveDown  = this.animMoveDown  == null ? null : this.animMoveDown.Clone(),

                animAttackRangedLeft  = this.animAttackRangedLeft  == null ? null : this.animAttackRangedLeft.Clone(),
                animAttackRangedRight = this.animAttackRangedRight == null ? null : this.animAttackRangedRight.Clone(),
                animAttackRangedUp    = this.animAttackRangedUp    == null ? null : this.animAttackRangedUp.Clone(),
                animAttackRangedDown  = this.animAttackRangedDown  == null ? null : this.animAttackRangedDown.Clone()
            };

            clone.HookSpriteEvents();
            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Hooks-up the sprite related events.
        /// </summary>
        private void HookSpriteEvents()
        {
            if( this.animAttackRangedLeft != null )
                this.animAttackRangedLeft.ReachedEnd += this.OnRangedAttackAnimationHasReachedEnd;

            if( this.animAttackRangedRight != null )
                this.animAttackRangedRight.ReachedEnd += this.OnRangedAttackAnimationHasReachedEnd;

            if( this.animAttackRangedUp != null )
                this.animAttackRangedUp.ReachedEnd += this.OnRangedAttackAnimationHasReachedEnd;

            if( this.animAttackRangedDown != null )
                this.animAttackRangedDown.ReachedEnd += this.OnRangedAttackAnimationHasReachedEnd;
        }

        /// <summary>
        /// The current <see cref="SpriteAnimation"/>.
        /// </summary>
        private SpriteAnimation current;

        /// <summary>
        /// States whether the attack animation should currently be shown.
        /// </summary>
        private bool showAttackAnimation;

        /// <summary>
        /// The movement animations.
        /// </summary>
        private SpriteAnimation animMoveLeft, animMoveRight, animMoveUp, animMoveDown;

        /// <summary>
        /// The ranged attack animations.
        /// </summary>
        private SpriteAnimation animAttackRangedLeft, animAttackRangedRight, animAttackRangedUp, animAttackRangedDown;

        /// <summary>
        /// The entity that gets visualized by thise <see cref="RangedEnemyDrawDataAndStrategy"/>.
        /// </summary>
        private readonly Enemy enemy;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;
    }
}
