// <copyright file="RapidFireSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.RapidFireSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Status;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// Rapid Fire increases chance to pierce and ranged attack speed by 10%/20%/30% for 24 seconds.
    /// </summary>
    internal sealed class RapidFireSkill : PlayerBuffSkill<RapidFireTalent>
    {        
        /// <summary>
        /// Initializes a new instance of the RapidFireSkill class.
        /// </summary>
        /// <param name="talent">
        /// The RapidFireTalent that modifies the power of the new RapidFireSkill.
        /// </param>
        internal RapidFireSkill( RapidFireTalent talent )
            : base( talent, RapidFireTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( RapidFireTalent.ManaNeededPoBM );

            this.speedEffect = new AttackSpeedEffect( Zelda.Attacks.AttackType.Ranged, 0.0f, StatusManipType.Percental );
            this.piercingEffect = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Pierce );

            this.Aura = new TimedAura( RapidFireTalent.Duration, new StatusEffect[] { speedEffect, piercingEffect } ) {
                DescriptionProvider = talent,
                Name      = this.LocalizedName,
                IsVisible = true,
                Symbol    = talent.Symbol              
            };
        }        
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this PlayerBuffSkill{TTalent}.
        /// </summary>
        protected override void RefreshAuraEffect()
        {
            this.speedEffect.Value = -this.Talent.SpeedChanceIncrease;
            this.piercingEffect.Value = this.Talent.PiercingChanceIncrease;
        }

        /// <summary>
        /// The AttackSpeedEffect that is applied by this RapidFireSkill.
        /// </summary>
        private readonly AttackSpeedEffect speedEffect;

        /// <summary>
        /// The ChanceToStatusEffect that is applied by this RapidFireSkill.
        /// </summary>
        private readonly ChanceToStatusEffect piercingEffect;
    }
}
