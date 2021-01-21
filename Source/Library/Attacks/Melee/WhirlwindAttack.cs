// <copyright file="WhirlwindAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Melee.WhirlwindAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Melee
{
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    using Zelda.Talents.Melee;

    /// <summary>
    /// Whirlwind is a powerful attack that needs to be charged up.
    /// After the charge is complete the player turns like a Whirlwind,
    /// hitting and pushing all enemies very hard.
    /// </summary>
    internal sealed class WhirlwindAttack : PlayerMeleeAttack
    {
        /// <summary>
        /// Gets a value indicating whether the animation of the WhirlwindAttack
        /// has completed, and as such the attack in itself.
        /// </summary>
        public bool HasEnded
        {
            get
            {
                return this.DrawStrategy.IsSpecialAnimationDone( PlayerSpecialAnimation.AttackWhirlwind );
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WhirlwindAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new WhirlwindAttack.
        /// </param>
        /// <param name="method">
        /// The IAttackDamageMethod that is used to calculate the damage done by the new WhirlwindAttack.
        /// </param>
        /// <param name="cooldown">
        /// The time the new WhirlwindAttack needs to useable again.
        /// </param>
        internal WhirlwindAttack( PlayerEntity player, WhirlwindDamageMethod method, Cooldown cooldown )
            : base( player, method, cooldown )
        {
            this.IsPushing           = true;
            this.PushingPowerMinimum = 25.0f;
            this.PushingPowerMaximum = 85.0f;
        }

        /// <summary>
        /// Fires this <see cref="WhirlwindAttack"/>.
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// true if this WhirlwindAttack has been used;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.isActive = true;
                this.previousCanMoveState = player.Moveable.CanMove;
                this.player.Moveable.CanMove = false;

                this.DrawStrategy.SetSpecialAnimationSpeed( PlayerSpecialAnimation.AttackWhirlwind, WhirlwindTalent.GetAnimationSpeed( player ) );
                this.DrawStrategy.SpecialAnimation = PlayerSpecialAnimation.AttackWhirlwind;
                this.DrawStrategy.ResetSpecialAnimation( PlayerSpecialAnimation.AttackWhirlwind, this.Transform.Direction );

                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this Attack should fire.
        /// </summary>
        /// <returns>
        /// true if it can and should fire;
        /// otherwise false.
        /// </returns>
        protected override bool ShouldFire()
        {
            if( !this.IsUseable || this.player.IsCasting )
            {
                return false;
            }

            PlayerSpecialAnimation specialAnimation = this.DrawStrategy.SpecialAnimation;

            // Don't allow attacking if any other special animation is currently shown.
            if( specialAnimation != PlayerSpecialAnimation.None && 
                specialAnimation != PlayerSpecialAnimation.AttackWhirlwind )
            {
                return false;
            }

            return this.IsReady;
        }

        /// <summary>
        /// Updates this <see cref="WhirlwindAttack"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( this.isActive )
            {
                if( this.DrawStrategy.IsSpecialAnimationDone( PlayerSpecialAnimation.AttackWhirlwind ) )
                {
                    this.OnWhirlwindCompleted();
                    return;
                }

                int frameIndex = this.DrawStrategy.GetSpecialAnimationFrameIndex( PlayerSpecialAnimation.AttackWhirlwind );
                if( frameIndex == lastFrameIndex )
                    return;
                
                RectangleF attackRectangle;
                GetAttackRectangle(frameIndex, out attackRectangle);
                HandleAttack( ref attackRectangle );
                lastFrameIndex = frameIndex;
            }
        }

        /// <summary>
        /// Called when the Whirlwind animation has completed.
        /// </summary>
        private void OnWhirlwindCompleted()
        {
            this.isActive = false;
            this.lastFrameIndex = -1;
            this.player.Moveable.CanMove = this.previousCanMoveState;
            this.DrawStrategy.SpecialAnimation = PlayerSpecialAnimation.None;
        }

        /// <summary>
        /// Gets the attack collision rectangle for the current frame.
        /// </summary>
        /// <param name="frameIndex">
        /// The index of the current frame.
        /// </param>
        /// <param name="attackRectangle">
        /// The attack area for the given frame.
        /// </param>
        private void GetAttackRectangle(int frameIndex, out RectangleF attackRectangle )
        {
            // This functions is hardcoded as it makes it easier to implement
            // and because we will never ever have different values
            float x = this.Transform.X;
            float y = this.Transform.Y;

            switch( frameIndex )
            {
                // the preparation phase
                case 0:
                case 1:
                case 2:
                default:
                    attackRectangle.X = 0;
                    attackRectangle.Y = 0;
                    attackRectangle.Width = 0;
                    attackRectangle.Height = 0;
                    break;

                case 3:
                    attackRectangle.X = x - 7;
                    attackRectangle.Y = y - 8;
                    attackRectangle.Width = 13;
                    attackRectangle.Height = 13;
                    break;

                case 4:
                    attackRectangle.X = x - 16;
                    attackRectangle.Y = y + 3;
                    attackRectangle.Width = 18;
                    attackRectangle.Height = 8;
                    break;

                case 5:
                    attackRectangle.X = x - 13;
                    attackRectangle.Y = y + 15;
                    attackRectangle.Width = 14;
                    attackRectangle.Height = 8;
                    break;

                case 6:
                    attackRectangle.X = x + 9;
                    attackRectangle.Y = y + 19;
                    attackRectangle.Width = 9;
                    attackRectangle.Height = 12;
                    break;

                case 7:
                    attackRectangle.X = x + 16;
                    attackRectangle.Y = y + 13;
                    attackRectangle.Width = 13;
                    attackRectangle.Height = 9;
                    break;

                case 8:
                    attackRectangle.X = x + 16;
                    attackRectangle.Y = y + 7;
                    attackRectangle.Width = 14;
                    attackRectangle.Height = 8;
                    break;

                case 9:
                    attackRectangle.X = x + 10;
                    attackRectangle.Y = y - 7;
                    attackRectangle.Width = 8;
                    attackRectangle.Height = 14;
                    break;

                case 10:
                    attackRectangle.X = x - 5;
                    attackRectangle.Y = y - 5;
                    attackRectangle.Width = 9;
                    attackRectangle.Height = 14;
                    break;
            }
        }
        
        /// <summary>
        /// States whether the whirlwind is currently going on.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The CanMove state before the animation has started.
        /// </summary>
        private bool previousCanMoveState;

        /// <summary>
        /// The last frame-index.
        /// </summary>
        private int lastFrameIndex = -1;
    }
}