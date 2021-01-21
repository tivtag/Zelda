// <copyright file="BladestormAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Melee.BladestormAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Melee
{
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    using Zelda.Talents.Melee;

    /// <summary>
    /// The player goes nuts after using Whirlwind, 
    /// turning for another X times, dealing (MeleeDamage x Y%) 
    /// non-parry nor dodgeable damage.
    /// Compared to Whirlwind movement is allowed with a speed penality of Z%.
    /// </summary>
    internal sealed class BladestormAttack : PlayerMeleeAttack
    {
        /// <summary>
        /// Sets the number of times the player uses 'Whirlwind'
        /// while the Bladestorm attack is active.
        /// </summary>
        public int MaximumTurns
        {
            set 
            { 
                this.maximumTurns = value; 
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this BladestormAttack is currently active.
        /// </summary>
        public bool IsActive
        {
            get
            { 
                return this.isActive;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BladestormAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new BladestormAttack.
        /// </param>
        /// <param name="method">
        /// The IAttackDamageMethod that is used to calculate the damage done by the new BladestormAttack.
        /// </param>
        /// <param name="cooldown">
        /// The time the new BladestormAttack needs to useable again.
        /// </param>
        internal BladestormAttack( PlayerEntity player, AttackDamageMethod method, Cooldown cooldown )
            : base( player, method, cooldown )
        {
            this.IsPushing           = true;
            this.PushingPowerMinimum = 25.0f;
            this.PushingPowerMaximum = 45.0f;
        }
        
        /// <summary>
        /// Fires this <see cref="BladestormAttack"/>.
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// true if this BladestormAttack has been used;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.turns = 0;
                this.isActive = true;

                this.DrawStrategy.SetSpecialAnimationSpeed(PlayerSpecialAnimation.AttackBladestorm, BladestormTalent.GetAnimationSpeed(player));
                this.DrawStrategy.SpecialAnimation = PlayerSpecialAnimation.AttackBladestorm;
                this.DrawStrategy.ResetSpecialAnimation( PlayerSpecialAnimation.AttackBladestorm );

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
            if( !IsUseable || player.IsCasting )
                return false;

            var drawStrategy = this.player.DrawDataAndStrategy;

            // Don't allow attacking if any other special animation is currently shown.
            if( drawStrategy.SpecialAnimation != PlayerSpecialAnimation.None &&
                drawStrategy.SpecialAnimation != PlayerSpecialAnimation.AttackBladestorm )
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

            if( isActive )
            {
                var drawStrategy = this.DrawStrategy;

                if( drawStrategy.IsSpecialAnimationDone( PlayerSpecialAnimation.AttackBladestorm ) )
                {
                    ++this.turns;

                    if( this.turns > this.maximumTurns )
                    {
                        this.isActive = false;
                        return;
                    }
                    else
                    {
                        this.lastFrameIndex = -1;
                        drawStrategy.ResetSpecialAnimation( PlayerSpecialAnimation.AttackBladestorm );
                    }
                }
                else if( drawStrategy.SpecialAnimation != PlayerSpecialAnimation.AttackBladestorm )
                {
                    this.isActive = false;
                    return;
                }

                int frameIndex = drawStrategy.GetSpecialAnimationFrameIndex( PlayerSpecialAnimation.AttackBladestorm );
                if( frameIndex == this.lastFrameIndex )
                    return;

                RectangleF attackRectangle;
                GetAttackRectangle(frameIndex, out attackRectangle);
                this.HandleAttack(ref attackRectangle);
                this.lastFrameIndex = frameIndex;
            }
        }

        /// <summary>
        /// Gets the attack collision rectangle for the current frame.
        /// </summary>
        /// <param name="frameIndex">
        /// The index of the current frame.
        /// </param>
        /// <param name="attackRectangle">
        /// Will contain attack collision rectangle.
        /// </param>
        private void GetAttackRectangle( int frameIndex, out RectangleF attackRectangle )
        {
            // This functions is hardcoded as it makes it easier to implement
            // and because we will never ever have different values
            float x = this.Transform.X;
            float y = this.Transform.Y;

            switch( frameIndex )
            {
                // the preparation phase
                default:
                    attackRectangle.X = 0;
                    attackRectangle.Y = 0;
                    attackRectangle.Width = 0;
                    attackRectangle.Height = 0;
                    break;

                case 0:
                    attackRectangle.X = x - 16;
                    attackRectangle.Y = y + 3;
                    attackRectangle.Width = 18;
                    attackRectangle.Height = 8;
                    break;

                case 1:
                    attackRectangle.X = x - 13;
                    attackRectangle.Y = y + 15;
                    attackRectangle.Width = 14;
                    attackRectangle.Height = 8;
                    break;

                case 2:
                    attackRectangle.X = x + 9;
                    attackRectangle.Y = y + 19;
                    attackRectangle.Width = 9;
                    attackRectangle.Height = 12;
                    break;

                case 3:
                    attackRectangle.X = x + 16;
                    attackRectangle.Y = y + 13;
                    attackRectangle.Width = 13;
                    attackRectangle.Height = 9;
                    break;

                case 4:
                    attackRectangle.X = x + 16;
                    attackRectangle.Y = y + 7;
                    attackRectangle.Width = 14;
                    attackRectangle.Height = 8;
                    break;

                case 5:
                    attackRectangle.X = x + 10;
                    attackRectangle.Y = y - 7;
                    attackRectangle.Width = 8;
                    attackRectangle.Height = 14;
                    break;

                case 6:
                    attackRectangle.X = x - 5;
                    attackRectangle.Y = y - 5;
                    attackRectangle.Width = 9;
                    attackRectangle.Height = 14;
                    break;
            }
        }
        
        /// <summary>
        /// The number of whirlwinds that have been executed.
        /// </summary>
        private int turns;

        /// <summary>
        /// The maximum number of turns to be executed.
        /// </summary>
        private int maximumTurns;

        /// <summary>
        /// States whether the Bladestorm is currently going on.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The last frame-index.
        /// </summary>
        private int lastFrameIndex = -1;
    }
}
