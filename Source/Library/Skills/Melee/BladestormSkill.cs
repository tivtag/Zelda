// <copyright file="BladestormSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.BladestormSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks.Melee;
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary>
    /// The player goes nuts after using Whirlwind, 
    /// turning for another X times, dealing (MeleeDamage x Y%) 
    /// non-parry nor dodgeable damage.
    /// Compared to Whirlwind movement is allowed with a speed penality of Z%.
    /// </summary>
    internal sealed class BladestormSkill : PlayerAttackSkill<BladestormTalent, BladestormAttack>
    {        
        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is currently inactive;
        /// and as such unuseable.
        /// </summary>
        public override bool IsInactive
        {
            get 
            {
                return !this.IsInTimeframeAfterWhirlwind(); 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BladestormSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The BladestormTalent that 'learns' the player the new BladestormSkill.
        /// </param>
        /// <param name="whirlwindSkill">
        /// The WhirlwindSkill that must be on cooldown for Bladestorm to be useable.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BladestormSkill( BladestormTalent talent, WhirlwindSkill whirlwindSkill, IZeldaServiceProvider serviceProvider )
            : base( talent, BladestormTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( BladestormTalent.ManaNeededPoBM );
            this.whirlwindSkill  = whirlwindSkill;

            this.method = new WhirlwindDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new BladestormAttack( talent.Owner, method, this.Cooldown );
            this.Attack.Setup( serviceProvider );

            this.speedEffect = new MovementSpeedEffect( BladestormTalent.MovementSpeedReduction, StatusManipType.Percental );
            this.aura        = new PermanentAura( this.speedEffect );
        }

        /// <summary>
        /// Refreshes the power of this BladestormSkill based on the Talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Attack.MaximumTurns = this.Talent.Turns;
            this.method.SetValues( this.Talent.DamageMultiplier );
        }
        
        /// <summary>
        /// Called when this BladestormSkill has been succesfully fired
        /// </summary>
        protected override void OnFired()
        {
            this.AuraList.Add( this.aura );
        }

        /// <summary>
        /// Updates this BladestormSkill.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.Attack.Update( updateContext );

            // Remove the movement speed penality.
            if( !this.Attack.IsActive && this.aura.AuraList != null )
            {
                this.aura.AuraList.Remove( this.aura );                
            }
        }

        /// <summary>
        /// Gets a value indicating whether the player just used Whirlwind,
        /// and is sitll allowed to use Bladestorm.
        /// </summary>
        /// <seealso cref="BladestormTalent.TimeUseableAfterWhirlwind"/>
        /// <returns>
        /// Whether Bladestorm is useable regarding the current timeframe related to Whirlwind.
        /// </returns>
        private bool IsInTimeframeAfterWhirlwind()
        {
            var whirlwindAttack = whirlwindSkill.Attack;

            if( whirlwindAttack.HasEnded )
            {
                var whirlwindCooldown = whirlwindSkill.Cooldown;

                float time = whirlwindCooldown.TotalTime - whirlwindCooldown.TimeLeft;
                return time > 0.0f && time < BladestormTalent.TimeUseableAfterWhirlwind;
            }

            return false;
        }
        
        /// <summary>
        /// Identifies the WhirlwindSkill, that must be on cooldown for Bladestorm to be useable.
        /// </summary>
        private readonly WhirlwindSkill whirlwindSkill;

        /// <summary>
        /// The damage method that is used to calculate the damage of this BladestormSkill.
        /// </summary>
        private readonly WhirlwindDamageMethod method;
        
        /// <summary>
        /// The movement speed penality applied when the Player uses the Bladestorm skill.
        /// </summary>
        private readonly MovementSpeedEffect speedEffect;

        /// <summary>
        /// The effect aura that gets applied when the Player uses the Bladestorm skill.
        /// </summary>
        private readonly PermanentAura aura;
    }
}
