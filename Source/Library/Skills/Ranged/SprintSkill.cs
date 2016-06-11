// <copyright file="SprintSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.SprintSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Status;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// The SprintSkill increases the movement speed 
    /// of the Player for a fixed amount of time.
    /// </summary>
    internal sealed class SprintSkill : PlayerBuffSkill<SprintTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="SprintSkill"/> is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        /// <value>Always returns true.</value>
        public override bool IsOnlyLimitedByCooldown
        {
            get
            {
                return true; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SprintSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        internal SprintSkill( SprintTalent talent )
            : base( talent, SprintTalent.Cooldown )
        {
            this.effect = new MovementSpeedEffect( 0.0f, StatusManipType.Percental );            
            this.Aura   = new TimedAura( SprintTalent.Duration, effect ) {
                DescriptionProvider = talent,
                Name      = this.LocalizedName,
                IsVisible = true,
                Symbol    = talent.Symbol
            };
        }               
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this SprintSkill.
        /// </summary>
        protected override void RefreshAuraEffect()
        {
            this.effect.Value = this.Talent.MovementSpeedIncrease;
        }

        /// <summary>
        /// The MovementSpeedEffect this SprintSkill applies.
        /// </summary>
        private readonly MovementSpeedEffect effect;
    }
}
