// <copyright file="LightArrowSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.LightArrowSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Attacks.Limiter;
    using Zelda.Attacks.Ranged;
    using Zelda.Status;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// The LightArrowSkill is a instant ranged attack that does Light Damage.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class LightArrowSkill : PlayerAttackSkill<LightArrowTalent, LightArrowAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightArrowSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the LightArrowSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal LightArrowSkill( LightArrowTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, talent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( LightArrowTalent.ManaNeededPoBM );
            
            this.method = new LightArrowDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new LightArrowAttack( talent.Owner, method, serviceProvider ) {
                Limiter = new TimedAttackLimiter( this.Cooldown )
            };

            this.Attack.Setup( serviceProvider );
        }

        /// <summary>
        /// Refreshes the power of the LightArrowSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.method.SetValues( this.Talent.DamageMultiplier, this.Talent.FixedDamageIncrease );
            this.Cooldown.TotalTime = this.Talent.Cooldown;
        }
        
        /// <summary>
        /// The LightArrowDamageMethod that is responsible for calculating damage.
        /// </summary>
        private readonly LightArrowDamageMethod method;
    }
}
