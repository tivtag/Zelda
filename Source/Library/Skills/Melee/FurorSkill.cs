// <copyright file="FurorSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.FurorSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary>
    /// Melee increases melee attack speed by 15%/30%/45% for 10 seconds.
    /// </summary>
    internal sealed class FurorSkill : PlayerBuffSkill<FurorTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        public override bool IsOnlyLimitedByCooldown
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the FurorSkill class.
        /// </summary>
        /// <param name="talent">
        /// The FurorTalent that modifies the power of the new FurorSkill.
        /// </param>
        internal FurorSkill( FurorTalent talent )
            : base( talent, FurorTalent.Cooldown )
        {
            this.effect = new AttackSpeedEffect( Zelda.Attacks.AttackType.Melee, 0.0f, StatusManipType.Percental );

            this.Aura = new TimedAura( FurorTalent.Duration, effect ) {
                DescriptionProvider = talent,
                Name      = this.LocalizedName,
                IsVisible = true,
                Symbol    = talent.Symbol,
            };
        }

        /// <summary>
        /// Refreshes the strength of the individual buff effect of this PlayerBuffSkill{TTalent}.
        /// </summary>
        protected override void RefreshAuraEffect()
        { 
            this.effect.Value = -this.Talent.SpeedIncrease;
        }
        
        /// <summary>
        /// The AttackSpeedEffect that is applied by this FurorSkill.
        /// </summary>
        private readonly AttackSpeedEffect effect;
    }
}
