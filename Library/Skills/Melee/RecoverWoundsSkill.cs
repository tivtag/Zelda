// <copyright file="RecoverWoundsSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.RecoverWoundsSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Status;
    using Zelda.Talents.Melee;
    
    /// <summary>
    /// Recover Wounds increases Life Regeneration 
    /// by 100% per talent-level for a total of 300%.
    /// </summary>
    internal sealed class RecoverWoundsSkill : PlayerBuffSkill<RecoverWoundsTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this RecoverWoundsSkill is only limited by its own cooldown
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
        /// Initializes a new instance of the <see cref="RecoverWoundsSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        internal RecoverWoundsSkill( RecoverWoundsTalent talent )
            : base( talent, RecoverWoundsTalent.Cooldown )
        {
            this.effect = new LifeManaRegenEffect( 0.0f, StatusManipType.Percental, LifeMana.Life );
            this.Aura = new TimedAura( RecoverWoundsTalent.Duration, effect ) {
                Name                = talent.LocalizedName,
                DescriptionProvider = talent,
                IsVisible           = true,
                Symbol              = talent.Symbol
            };
        }
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this RecoverWoundsSkill.
        /// </summary>
        protected override void  RefreshAuraEffect()
        {
            this.effect.Value = this.Talent.LifeRegenerationIncrease;
        }

        /// <summary>
        /// The LifeManaRegenEffect that gets applied by this RecoverWoundsSkill.
        /// </summary>
        private readonly LifeManaRegenEffect effect;
    }
}
