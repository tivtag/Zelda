// <copyright file="PlayerDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.PlayerDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Items;

    /// <summary>
    /// Stores the data and provides the strategy to
    /// render the <see cref="PlayerEntity"/>. 
    /// This is a sealed class.
    /// </summary>
    /// <remarks>
    /// This class is quite a -quirks-. It should be rewritten.
    /// </remarks>
    public sealed class PlayerDrawDataAndStrategy : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the sprite group used for this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        public string SpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the sprite that represents the front of the player.
        /// </summary>
        public Sprite FrontSprite
        {
            get
            {
                return sprites.StandDown;
            }
        }

        /// <summary>
        /// Gets the current <see cref="SpriteAnimation"/>, if any.
        /// </summary>
        public SpriteAnimation CurrentAnimation
        {
            get
            {
                return this.currentAnim;
            }
        }

        /// <summary>
        /// Gets or sets what <see cref="PlayerSpecialAnimation"/> is currently shown.
        /// </summary>
        public PlayerSpecialAnimation SpecialAnimation
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="player">The player that is drawn using the new IDrawDataAndStrategy.</param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PlayerDrawDataAndStrategy( PlayerEntity player, IZeldaServiceProvider serviceProvider )
        {
            this.player = player;
            this.castBar = player.Castable.CastBar;
            this.moveable = player.Moveable;
            this.statable = player.Statable;
            this.transform = player.Transform;
            this.rand = serviceProvider.Rand;

            moveable.SpeedChanged += this.OnPlayerSpeedChanged;

            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 50.0f, 2.0f ) );
            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 56.0f, 2.05f ) );
            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 60.0f, 2.6f ) );
            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 70.0f, 3.35f ) );
            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 80.0f, 4.50f ) );
            movementAnimationSpeedCurve.Keys.Add( new CurveKey( 90.0f, 6.00f ) );
            movementAnimationSpeedCurve.ComputeTangents( CurveTangent.Flat );
        }

        /// <summary>
        /// Loads this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Load( IZeldaServiceProvider serviceProvider )
        {
            this.sprites = serviceProvider.GetService<LinkSprites>();
        }

        private void OnPlayerSpeedChanged( Components.Moveable sender )
        {
            RefreshMovementAnimationSpeed();
        }

        #endregion

        #region [ Methods ]

        #region > Updating <

        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( transform.OldDirection != transform.Direction ||
                moveable.WasStanding != moveable.IsStanding )
            {
                if( currentAnim != null )
                    currentAnim.Reset();
            }

            // Reset DrawOffset:
            drawOffset.X = 0;
            drawOffset.Y = 0;

            if( castBar.IsCasting )
            {
                UpdateSprites_Casting();
            }
            else if( statable.IsDead )
            {
                current = currentAnim = sprites.Dieing;
            }
            else
            {
                switch( this.SpecialAnimation )
                {
                    case PlayerSpecialAnimation.AttackMelee:
                        this.UpdateSprites_AttackMelee();
                        break;

                    case PlayerSpecialAnimation.AttackRanged:
                        this.UpdateSprites_AttackRanged();
                        break;

                    case PlayerSpecialAnimation.AttackWhirlwind:
                        this.UpdateSprites_AttackWhirlwind();
                        break;

                    case PlayerSpecialAnimation.AttackBladestorm:
                        this.UpdateSprites_AttackBladestorm();
                        break;

                    case PlayerSpecialAnimation.PlayOcarina:
                        this.UpdateSprites_PlayOcarina();
                        break;

                    default:
                        if( moveable.IsStanding )
                            this.UpdateSprites_Standing( updateContext.FrameTime );
                        else
                            this.UpdateSprites_Moving();
                        break;
                }
            }

            var updateable = current as Atom.IUpdateable;

            if( updateable != null )
                updateable.Update( updateContext );
        }

        /// <summary>
        /// Updates the sprite to show link attacking with the default melee attack.
        /// </summary>
        private void UpdateSprites_AttackMelee()
        {
            switch( transform.Direction )
            {
                case Direction4.Left:
                    currentAnim = sprites.MeleeLeft;
                    break;
                case Direction4.Right:
                    currentAnim = sprites.MeleeRight;
                    break;
                case Direction4.Up:
                    currentAnim = sprites.MeleeUp;
                    break;
                default:
                case Direction4.Down:
                    currentAnim = sprites.MeleeDown;
                    break;
            }

            if( currentAnim.Time >= currentAnim.TotalTime )
                this.SpecialAnimation = PlayerSpecialAnimation.None;

            current = currentAnim;
        }

        /// <summary>
        /// Updates the sprite to show link attacking with the default ranged attack.
        /// </summary>
        private void UpdateSprites_AttackRanged()
        {
            switch( transform.Direction )
            {
                case Direction4.Left:
                    currentAnim = sprites.RangedLeft;
                    break;
                case Direction4.Right:
                    currentAnim = sprites.RangedRight;
                    break;
                case Direction4.Up:
                    currentAnim = sprites.RangedUp;
                    break;
                default:
                case Direction4.Down:
                    currentAnim = sprites.RangedDown;
                    break;
            }

            if( currentAnim.Time >= currentAnim.TotalTime )
            {
                this.SpecialAnimation = PlayerSpecialAnimation.None;
            }

            this.current = currentAnim;
        }

        /// <summary>
        /// Updates the sprite to show link attacking with the whirlwind attack.
        /// </summary>
        private void UpdateSprites_AttackWhirlwind()
        {
            currentAnim = sprites.AtkWhirlwind;

            if( currentAnim.Time >= currentAnim.TotalTime )
                this.SpecialAnimation = PlayerSpecialAnimation.None;

            current = currentAnim;
        }

        /// <summary>
        /// Updates the sprite to show link attacking with the Bladestorm attack.
        /// </summary>
        private void UpdateSprites_AttackBladestorm()
        {
            this.currentAnim = this.sprites.AtkBladestorm;

            if( this.currentAnim.Time >= this.currentAnim.TotalTime )
                this.SpecialAnimation = PlayerSpecialAnimation.None;

            this.current = this.currentAnim;
        }

        /// <summary>
        /// Updates the sprite to show link casting.
        /// </summary>
        private void UpdateSprites_Casting()
        {
            switch( transform.Direction )
            {
                case Direction4.Left:
                    currentAnim = sprites.CastLeft;
                    break;
                case Direction4.Right:
                    currentAnim = sprites.CastRight;
                    break;
                case Direction4.Up:
                    currentAnim = sprites.CastUp;
                    break;
                default:
                case Direction4.Down:
                    currentAnim = sprites.CastDown;
                    break;
            }

            if( currentAnim != null )
                currentAnim.FrameIndex = this.castBar.CastIndex;

            current = currentAnim;
        }

        /// <summary>
        /// Updates the sprite to show link standing.
        /// </summary>
        /// <param name="frameTime">
        /// The time the last frame took (in seconds).
        /// </param>
        private void UpdateSprites_Standing( float frameTime )
        {
            if( moveable.IsSwimming )
            {
                switch( transform.Direction )
                {
                    case Direction4.Left:
                        currentAnim = sprites.SwimStandLeft;
                        break;
                    case Direction4.Right:
                        currentAnim = sprites.SwimStandRight;
                        break;
                    case Direction4.Up:
                        currentAnim = sprites.SwimStandUp;
                        break;
                    default:
                    case Direction4.Down:
                        currentAnim = sprites.SwimStandDown;
                        break;
                }

                drawOffset.Y = 2;
                current = currentAnim;
            }
            else
            {
                currentAnim = null;
                eyeClosingTick += 1000.0f * frameTime;

                if( updateEyeExtraTicks )
                {
                    eyeExtraTicks = rand.RandomRange( -512.0f, 5096.0f );
                    updateEyeExtraTicks = false;
                }

                switch( transform.Direction )
                {
                    case Direction4.Up:
                        current = sprites.StandUp;
                        break;

                    case Direction4.Down:
                        if( eyeClosingTick < 1200 + eyeExtraTicks )
                            current = sprites.StandDown;
                        else if( eyeClosingTick < 1450 + eyeExtraTicks )
                            current = sprites.ClosingEyesDown;
                        else if( eyeClosingTick < 1600 + eyeExtraTicks )
                            current = sprites.ClosedEyesDown;
                        else
                        {
                            current = sprites.StandDown;
                            updateEyeExtraTicks = true;
                            eyeClosingTick = 0;
                        }

                        break;

                    case Direction4.Left:
                        if( eyeClosingTick < 1200 + eyeExtraTicks )
                        {
                            current = sprites.StandLeft;
                        }
                        else if( eyeClosingTick < 1450 + eyeExtraTicks )
                        {
                            current = sprites.ClosingEyesLeft;
                            drawOffset.X = -1;
                        }
                        else if( eyeClosingTick < 1600 + eyeExtraTicks )
                        {
                            current = sprites.ClosedEyesLeft;
                            drawOffset.X = -1;
                        }
                        else
                        {
                            updateEyeExtraTicks = true;
                            current = sprites.StandLeft;
                            eyeClosingTick = 0;
                        }

                        break;

                    case Direction4.Right:
                        if( eyeClosingTick < 1200 + eyeExtraTicks )
                            current = sprites.StandRight;
                        else if( eyeClosingTick < 1450 + eyeExtraTicks )
                            current = sprites.ClosingEyesRight;
                        else if( eyeClosingTick < 1600 + eyeExtraTicks )
                            current = sprites.ClosedEyesRight;
                        else
                        {
                            updateEyeExtraTicks = true;
                            current = sprites.StandRight;
                            eyeClosingTick = 0;
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the sprite to show link moving.
        /// </summary>
        private void UpdateSprites_Moving()
        {
            if( moveable.IsSwimming )
            {
                switch( transform.Direction )
                {
                    case Direction4.Left:
                        currentAnim = sprites.SwimLeft;
                        drawOffset.Y = 4;
                        break;
                    case Direction4.Right:
                        currentAnim = sprites.SwimRight;
                        drawOffset.X = -4;
                        drawOffset.Y = 4;
                        break;
                    case Direction4.Up:
                        currentAnim = sprites.SwimUp;
                        break;
                    case Direction4.Down:
                        currentAnim = sprites.SwimDown;
                        drawOffset.Y = 5;
                        break;

                    default:
                        currentAnim = null;
                        break;
                }
            }
            else
            {
                switch( transform.Direction )
                {
                    case Direction4.Up:
                        currentAnim = sprites.MoveUp;
                        break;
                    case Direction4.Down:
                        currentAnim = sprites.MoveDown;
                        break;
                    case Direction4.Left:
                        currentAnim = sprites.MoveLeft;
                        break;
                    case Direction4.Right:
                        currentAnim = sprites.MoveRight;
                        break;
                    default:
                        currentAnim = null;
                        break;
                }
            }

            current = currentAnim;
        }

        /// <summary>
        /// Updates the current sprite to show link playing his ocarina.
        /// </summary>
        private void UpdateSprites_PlayOcarina()
        {
            this.currentAnim = null;
            this.current = sprites.PlayOcarinaStand;
        }

        #endregion

        #region > Drawing <

        public Vector2 DrawPosition
        {
            get
            {
                Vector2 playerPosition = transform.Position;
                return new Vector2( (int)playerPosition.X + drawOffset.X, (int)playerPosition.Y + drawOffset.Y );
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
            if( current != null )
            {
                var batch = drawContext.Batch;
                Vector2 drawPosition = this.DrawPosition;

                this.current.Draw( drawPosition, batch );
                this.DrawExtended_SpecialAnimationShown( drawPosition, batch );
            }
        }

        /// <summary>
        /// Draws anything related to the current <see cref="SpecialAnimation"/>.
        /// </summary>
        /// <param name="drawPosition">The draw position.</param>
        /// <param name="batch">The XNA SpriteBatch object.</param>
        private void DrawExtended_SpecialAnimationShown( Vector2 drawPosition, ISpriteBatch batch )
        {
            if( this.player.IsCasting )
            {
                this.DrawEquipment( player.Equipment.Staff, drawPosition, batch );
            }
            else
            {
                switch( this.SpecialAnimation )
                {
                    case PlayerSpecialAnimation.AttackMelee:
                        this.DrawEquipment( player.Equipment.WeaponHand, drawPosition, batch );
                        break;

                    case PlayerSpecialAnimation.AttackRanged:
                        this.DrawEquipment( player.Equipment.Ranged, drawPosition, batch );
                        break;

                    case PlayerSpecialAnimation.AttackWhirlwind:
                        this.DrawEquipment( player.Equipment.WeaponHand, this.sprites.SwordWhirlwind, drawPosition, batch );
                        break;
                                                
                    case PlayerSpecialAnimation.AttackBladestorm:
                        this.DrawEquipment( player.Equipment.WeaponHand, this.sprites.SwordBladestorm, drawPosition, batch );
                        break;

                    default:
                        break;
                }
            }
        }

        private void DrawEquipment( EquipmentInstance equipmentInstance, AnimatedSprite spritesAnimation, Vector2 drawPosition, ISpriteBatch batch )
        {
            if( equipmentInstance == null )
                return;

            Equipment equipment = equipmentInstance.Equipment;
            if( equipment == null )
                return;

            AnimatedSpriteFrame frame = GetActiveEquipmentFrame( spritesAnimation );

            if( frame != null && frame.Sprite != null )
            {
                frame.Sprite.Draw(
                    new Vector2( drawPosition.X + frame.Offset.X, drawPosition.Y + frame.Offset.Y ),
                    equipment.IngameColor,
                    batch
                );
            }
        }

        /// <summary>
        /// Utility method that draws the given equipment (on the player).
        /// </summary>
        /// <param name="equipmentInstance">The instance of the equipment to draw.</param>
        /// <param name="drawPosition">The draw position.</param>
        /// <param name="batch">The XNA SpriteBatch object.</param>
        private void DrawEquipment( EquipmentInstance equipmentInstance, Vector2 drawPosition, ISpriteBatch batch )
        {
            if( equipmentInstance == null )
                return;

            Equipment equipment = equipmentInstance.Equipment;
            if( equipment == null )
                return;

            AnimatedSpriteFrame frame = GetActiveEquipmentFrame( equipment );

            if( frame != null && frame.Sprite != null )
            {
                frame.Sprite.Draw(
                    new Vector2( drawPosition.X + frame.Offset.X, drawPosition.Y + frame.Offset.Y ),
                    equipment.IngameColor,
                    batch
                );
            }
        }

        public AnimatedSpriteFrame GetActiveEquipmentFrame( Equipment equipment )
        {
            if( equipment.HasIngameAnimations )
            {
                AnimatedSprite equipAnim = equipment.GetIngameAnimation( transform.Direction );
                return GetActiveEquipmentFrame( equipAnim );
            }

            return null;
        }

        private AnimatedSpriteFrame GetActiveEquipmentFrame( AnimatedSprite equipAnim )
        {
            if( equipAnim != null && currentAnim != null )
            {
                if( currentAnim.FrameIndex < equipAnim.FrameCount )
                {
                    AnimatedSpriteFrame frame = equipAnim[currentAnim.FrameIndex];
                    return frame;
                }
            }

            return null;
        }

        #endregion

        #region > Special Animation <

        /// <summary>
        /// Sets the animationspeed of the given <see cref="PlayerSpecialAnimation"/>.
        /// </summary>
        /// <param name="animation">
        /// The PlayerSpecialAnimation to modify.
        /// </param>
        /// <param name="animationSpeed">
        /// The new speed of the animation.
        /// </param>
        internal void SetSpecialAnimationSpeed( PlayerSpecialAnimation spriteAnimation, float animationSpeed )
        {
            switch( spriteAnimation )
            {
                case PlayerSpecialAnimation.AttackWhirlwind:
                    this.sprites.AtkWhirlwind.AnimationSpeed = animationSpeed;
                    break;

                case PlayerSpecialAnimation.AttackBladestorm:
                    this.sprites.AtkBladestorm.AnimationSpeed = animationSpeed;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Shows the given <see cref="PlayerSpecialAnimation"/>.
        /// </summary>
        /// <param name="specialAnimation">
        /// The PlayerSpecialAnimation to show.
        /// </param>
        internal void ShowSpecialAnimation( PlayerSpecialAnimation specialAnimation )
        {
            this.ShowSpecialAnimation( specialAnimation, this.transform.Direction );
        }

        /// <summary>
        /// Shows the given <see cref="PlayerSpecialAnimation"/> for the given Direction4.
        /// </summary>
        /// <param name="specialAnimation">
        /// The PlayerSpecialAnimation to show.
        /// </param>
        /// <param name="direction">
        /// The initial direction.
        /// </param>
        private void ShowSpecialAnimation( PlayerSpecialAnimation specialAnimation, Direction4 direction )
        {
            if( specialAnimation == PlayerSpecialAnimation.AttackMelee )
            {
                this.lastMeleeAnimation = this.GetSpecialAnimation( PlayerSpecialAnimation.AttackMelee, direction );
            }
            else if( this.IsCurrentAnimation( PlayerSpecialAnimation.AttackMelee ) )
            {
                this.lastMeleeAnimation = this.currentAnim;
            }

            //else if( specialAnimation == PlayerSpecialAnimation.AttackRanged )
            //{
            //    this.lastRangedAnimation = this.GetSpecialAnimation( PlayerSpecialAnimation.AttackRanged, direction );
            //}
            //else if( this.IsCurrentAnimation( PlayerSpecialAnimation.AttackRanged ) )
            //{
            //    this.lastRangedAnimation = this.currentAnim;
            //}            
            this.SpecialAnimation = specialAnimation;
            this.ResetSpecialAnimation( specialAnimation, player.Transform.Direction );
        }

        /// <summary>
        /// Disables the currently used special animation.
        /// </summary>
        internal void DisableCurrentSpecialAnimation()
        {
            SpriteAnimation animation = null;

            if( this.SpecialAnimation == PlayerSpecialAnimation.AttackMelee )
            {
                animation = this.GetCurrentOrLastMeleeAnimation();
            }
            else
            {
                animation = this.CurrentAnimation;
            }
            // else if( this.SpecialAnimation == PlayerSpecialAnimation.AttackRanged )
            //{
            //    animation= this.GetCurrentOrLastRangedAnimation();
            //}

            if( animation != null )
            {
                animation.Reset();
            }

            this.SpecialAnimation = PlayerSpecialAnimation.None;
        }

        /// <summary>
        /// Gets a value indicating whether the Melee Attack Animation has been completed sprites.ating.
        /// </summary>
        /// <param name="timeFactor">
        /// The time factor to apply to the total time of the animation.
        /// </param>
        /// <returns>
        /// Whether the Melee Animation has been completed.
        /// </returns>
        internal bool IsMeleeAttackAnimationDone( float timeFactor )
        {
            var animation = this.GetCurrentOrLastMeleeAnimation();
            if( animation == null )
                return true;

            return animation.Time >= animation.TotalTime * timeFactor;
        }

        /// <summary>
        /// Helpers method that returns the current animationif it is a Melee Attack animation;
        /// otherwise it returns the lastMeleeAnimation.
        /// </summary>
        /// <returns>
        /// An SpriteAnimation instance; might be null.
        /// </returns>
        private SpriteAnimation GetCurrentOrLastMeleeAnimation()
        {
            if( this.IsCurrentAnimation( PlayerSpecialAnimation.AttackMelee ) )
            {
                return this.currentAnim;
            }
            else
            {
                return this.lastMeleeAnimation;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CurrentAnimation is the given PlayerSpecialAnimation.
        /// </summary>
        /// <param name="animation">
        /// The PlayerSpecialAnimation the currentAnim should be compared to.
        /// </param>
        /// <returns>
        /// true if the specified PlayerSpecialAnimation is currently shown;
        /// otherwise false.
        /// </returns>
        private bool IsCurrentAnimation( PlayerSpecialAnimation animation )
        {
            switch( animation )
            {
                case PlayerSpecialAnimation.AttackMelee:
                    return
                        this.currentAnim == this.sprites.MeleeLeft ||
                        this.currentAnim == this.sprites.MeleeRight ||
                        this.currentAnim == this.sprites.MeleeUp ||
                        this.currentAnim == this.sprites.MeleeDown;

                case PlayerSpecialAnimation.AttackRanged:
                    return
                        this.currentAnim == this.sprites.RangedLeft ||
                        this.currentAnim == this.sprites.RangedRight ||
                        this.currentAnim == this.sprites.RangedUp ||
                        this.currentAnim == this.sprites.RangedDown;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Resets the dieing animation.
        /// </summary>
        internal void ResetAnimationDead()
        {
            this.sprites.Dieing.Reset();
            this.SpecialAnimation = PlayerSpecialAnimation.None;
        }

        /// <summary>
        /// Resets the given <see cref="PlayerSpecialAnimation"/> for no specific Direction.
        /// </summary>
        /// <param name="animation">
        /// States what animationto reset.
        /// </param>
        internal void ResetSpecialAnimation( PlayerSpecialAnimation animation )
        {
            this.ResetSpecialAnimation( animation, Direction4.None );
        }

        /// <summary>
        /// Resets the given <see cref="PlayerSpecialAnimation"/>.
        /// </summary>
        /// <param name="animation">
        /// States what animationto reset.
        /// </param>
        /// <param name="direction">
        /// The direction of the animationto reset.
        /// </param>
        internal void ResetSpecialAnimation( PlayerSpecialAnimation animation, Direction4 direction )
        {
            switch( animation )
            {
                case PlayerSpecialAnimation.AttackMelee:
                    switch( direction )
                    {
                        case Direction4.Left:
                            sprites.MeleeLeft.Reset();
                            break;

                        case Direction4.Right:
                            sprites.MeleeRight.Reset();
                            break;

                        case Direction4.Up:
                            sprites.MeleeUp.Reset();
                            break;

                        case Direction4.Down:
                            sprites.MeleeDown.Reset();
                            break;

                        default:
                            break;
                    }

                    break;

                case PlayerSpecialAnimation.AttackRanged:
                    switch( direction )
                    {
                        case Direction4.Left:
                            sprites.RangedLeft.Reset();
                            break;

                        case Direction4.Right:
                            sprites.RangedRight.Reset();
                            break;

                        case Direction4.Up:
                            sprites.RangedUp.Reset();
                            break;

                        case Direction4.Down:
                            sprites.RangedDown.Reset();
                            break;

                        default:
                            break;
                    }

                    break;

                case PlayerSpecialAnimation.AttackWhirlwind:
                    sprites.AtkWhirlwind.Reset();
                    break;

                case PlayerSpecialAnimation.AttackBladestorm:
                    sprites.AtkBladestorm.Reset();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets whether the given <see cref="PlayerSpecialAnimation"/> is done sprites.ating.
        /// </summary>
        /// <param name="animation">
        /// The related animation.
        /// </param>
        /// <returns>
        /// true if the given animationis done;
        /// otherwise false.
        /// </returns>
        internal bool IsSpecialAnimationDone( PlayerSpecialAnimation animation)
        {
            switch( animation)
            {
                case PlayerSpecialAnimation.AttackWhirlwind:
                    return sprites.AtkWhirlwind.Time >= sprites.AtkWhirlwind.TotalTime;

                case PlayerSpecialAnimation.AttackBladestorm:
                    return sprites.AtkBladestorm.Time >= sprites.AtkBladestorm.TotalTime;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Receives the current FrameIndex of the given <see cref="PlayerSpecialAnimation"/>.
        /// </summary>
        /// <param name="animation">
        /// The animationto receive the current frame index for.
        /// </param>
        /// <returns>The requested frame index.</returns>
        internal int GetSpecialAnimationFrameIndex( PlayerSpecialAnimation animation)
        {
            switch( animation)
            {
                case PlayerSpecialAnimation.AttackWhirlwind:
                    return sprites.AtkWhirlwind.FrameIndex;

                case PlayerSpecialAnimation.AttackBladestorm:
                    return sprites.AtkBladestorm.FrameIndex;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the given PlayerSpecialAnimation for the given Direction.
        /// </summary>
        /// <param name="animation">The animationtype to receive.</param>
        /// <param name="direction">The direction of the animationto receive.</param>
        /// <returns>The requested animation.</returns>
        private SpriteAnimation GetSpecialAnimation( PlayerSpecialAnimation animation, Direction4 direction )
        {
            switch( animation )
            {
                case PlayerSpecialAnimation.AttackMelee:
                    switch( direction )
                    {
                        case Direction4.Left:
                            return sprites.MeleeLeft;

                        case Direction4.Right:
                            return sprites.MeleeRight;

                        case Direction4.Up:
                            return sprites.MeleeUp;

                        case Direction4.Down:
                            return sprites.MeleeDown;

                        default:
                            break;
                    }

                    break;

                case PlayerSpecialAnimation.AttackRanged:
                    switch( direction )
                    {
                        case Direction4.Left:
                            return sprites.RangedLeft;

                        case Direction4.Right:
                            return sprites.RangedRight;

                        case Direction4.Up:
                            return sprites.RangedUp;

                        case Direction4.Down:
                            return sprites.RangedDown;

                        default:
                            break;
                    }

                    break;

                case PlayerSpecialAnimation.AttackWhirlwind:
                    return sprites.AtkWhirlwind;

                case PlayerSpecialAnimation.AttackBladestorm:
                    return sprites.AtkBladestorm;

                default:
                    break;
            }

            throw new NotImplementedException();
        }
        
        internal float GetSpecialAnimationTotalTime( PlayerSpecialAnimation specialAnimation )
        {
            switch( specialAnimation )
            {
                case PlayerSpecialAnimation.AttackWhirlwind:
                    return sprites.AtkWhirlwind.TotalTime;

                case PlayerSpecialAnimation.AttackBladestorm:
                    return sprites.AtkBladestorm.TotalTime;

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region > Unsupported Methods <

        /// <summary>
        /// This operation is not supported by this IDrawDataAndStrategy.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="newOwner">This parameter is unused.</param>
        /// <returns>This function will never return.</returns>
        IDrawDataAndStrategy IDrawDataAndStrategy.Clone( ZeldaEntity newOwner )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            throw new NotSupportedException();
        }

        #endregion

        private void RefreshMovementAnimationSpeed()
        {
            float factor = movementAnimationSpeedCurve.Evaluate( moveable.Speed );
            float animationSpeed = 1000.0f + (factor * moveable.Speed);
            sprites.SetMoveAnimSpeed( animationSpeed );
        }

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// The currently displayed <see cref="ISprite"/>.
        /// </summary>
        private ISprite current;

        /// <summary>
        /// The currently displayed <see cref="SpriteAnimation"/>, if any.
        /// </summary>
        private SpriteAnimation currentAnim;

        /// <summary>
        /// Stores the last-used attack animation.
        /// </summary>
        private SpriteAnimation lastMeleeAnimation;

        /// <summary>
        /// Holds the sprites and sprite animations used to draw the player.
        /// </summary>
        private LinkSprites sprites;

        /// <summary>
        /// The draw offset applied to the currently drawn sprite, if any.
        /// </summary>
        private Point2 drawOffset;

        /// <summary>
        /// Used to calculate the animationspeed factor based on movement speed.
        /// </summary>
        private readonly Curve movementAnimationSpeedCurve = new Curve();

        /// <summary>
        /// The PlayerEntity this PlayerDrawDataAndStrategy helps to draw.
        /// </summary>
        private readonly PlayerEntity player;

        /// <summary>
        /// Identifies the CastBar of the player.
        /// </summary>
        private readonly Zelda.Casting.CastBar castBar;

        /// <summary>
        /// Identifies the ExtendedStatable component of the player.
        /// </summary>
        private readonly Zelda.Status.ExtendedStatable statable;

        /// <summary>
        /// Identifies the moveable component of the player.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        /// <summary>
        /// Identifies the transform component of the player.
        /// </summary>
        private readonly Zelda.Entities.Components.ZeldaTransform transform;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;
        
        /// <summary>
        /// Idendicates the current eye animationtick.
        /// </summary>
        private float eyeClosingTick;

        /// <summary>
        /// The current random extra tick value.
        /// </summary>
        private float eyeExtraTicks;

        /// <summary>
        /// Indicates whether to update the eyeExtraTicks value.
        /// </summary>
        private bool updateEyeExtraTicks = true;

        #endregion
    }

    /// <summary>
    /// Enumerates the different Special Animations of the PlayerDrawDataAndStrategy.
    /// </summary>
    public enum PlayerSpecialAnimation
    {
        /// <summary>
        /// No specific special animation.
        /// </summary>
        None,

        /// <summary>
        /// The default melee attack.
        /// </summary>
        AttackMelee,

        /// <summary>
        /// The default ranged attack.
        /// </summary>
        AttackRanged,

        /// <summary>
        /// The Wirlwind attack.
        /// </summary>
        AttackWhirlwind,

        /// <summary>
        /// The Bladestorm attack.
        /// </summary>
        AttackBladestorm,

        /// <summary>
        /// Shows link playing the ocarina.
        /// </summary>
        PlayOcarina
    }
}

/*      /// <summary>
        /// Gets a value indicating whether the Ranged Attack Animation has been completed sprites.ating.
        /// </summary>
        /// <param name="timeFactor">
        /// The time factor to apply to the total time of the animation.
        /// </param>
        /// <returns>
        /// Whether the Melee Animation has been completed.
        /// </returns>
        internal bool IsRangedAttackAnimationDone( float timeFactor )
        {
            var animation= this.GetCurrentOrLastRangedAnimation();
            if( animation== null )
                return true;

            return animation.Time >= animation.TotalTime * timeFactor;
        }

        /// <summary>
        /// <summary>
        /// Helpers method that returns the current animationif it is a Ranged Attack animation;
        /// otherwise it returns the lastRangedAnimation.
        /// </summary>
        /// <returns>
        /// An SpriteAnimation instance; might be null.
        /// </returns>
        private SpriteAnimation GetCurrentOrLastRangedAnimation()
        {
            if( this.IsCurrentAnimation( PlayerSpecialAnimation.AttackRanged ) )
            {
                return this.currentAnim;
            }
            else
            {
                return this.lastRangedAnimation;
            }
        }
*/