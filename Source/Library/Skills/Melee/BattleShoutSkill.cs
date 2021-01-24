// <copyright file="BattleShoutSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.BattleShoutSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary> 
    /// Battle Shout increases the Strength of the player
    /// for 60 seconds. (cooldown 120 seconds)
    /// </summary>
    internal sealed class BattleShoutSkill : PlayerBuffSkill<BattleShoutTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this BattleShoutSkill is only limited by its own cooldown
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
        /// Initializes a new instance of the <see cref="BattleShoutSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new BattleShoutSkill.
        /// </param>
        internal BattleShoutSkill( BattleShoutTalent talent )
            : base( talent, BattleShoutTalent.Cooldown )
        {
            this.effectFixed = new StatEffect( 0.0f, StatusManipType.Fixed, Stat.Strength );
            this.effectMulti = new StatEffect( 0.0f, StatusManipType.Percental, Stat.Strength);

            this.Aura = new TimedAura( BattleShoutTalent.Duration, new StatusEffect[2] { effectFixed, effectMulti } ) {
                Name                = this.LocalizedName,
                IsVisible           = true,
                Symbol              = talent.Symbol,
                DescriptionProvider = this
            };
        }
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this PlayerBuffSkill{TTalent}.
        /// </summary>
        protected override void RefreshAuraEffect()
        {
            this.effectMulti.Value = this.Talent.StrengthMultiplier;
            this.effectFixed.Value = this.Talent.FixedStrengthIncrease;
        }

        /// <summary>
        /// The effect that is applied by this BattleShoutSkill.
        /// </summary>
        private readonly StatEffect effectFixed, effectMulti;
    }
}
