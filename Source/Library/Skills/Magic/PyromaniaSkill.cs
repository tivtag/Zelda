// <copyright file="PyromaniaSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Magic.PyromaniaSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Magic
{
    using System.Collections.Generic;
    using Zelda.Status;
    using Zelda.Talents.Magic;
    
    /// <summary>
    /// Reduces the cooldown of all offensive Fire spells
    /// by 10/20/30/40/50% for 15 seconds.
    /// </summary>
    internal sealed class PyromaniaSkill : PlayerBuffSkill<PyromaniaTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this PyromaniaSkill is only limited by its own cooldown
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
        /// Initializes a new instance of the PyromaniaSkill class.
        /// </summary>
        /// <param name="talent">
        /// The PyromaniaTalent that modifies the strength of the new PyromaniaSkill.
        /// </param>
        public PyromaniaSkill( PyromaniaTalent talent )
            : base( talent, PyromaniaTalent.Cooldown )
        {
            this.Aura = this.CreateAura();
        }

        /// <summary>
        /// Creates the aura that gets enabled when this
        /// PyromaniaSkill is used.
        /// </summary>
        /// <returns>
        /// The newly created Aura.
        /// </returns>
        private TimedAura CreateAura()
        {
            return new TimedAura( PyromaniaTalent.Duration, this.CreateEffects() ) {
                IsVisible = true,
                Symbol = this.Symbol,
                DescriptionProvider = this.Talent
            };
        }

        /// <summary>
        /// Creates the StatusEffects that get enabled when this
        /// PyromaniaSkill is used.
        /// </summary>
        /// <returns>
        /// The newly created StatusEffects.
        /// </returns>
        private StatusEffect[] CreateEffects()
        {
            var effects = new List<StatusEffect>();

            foreach( var affectedTalent in this.Talent.GetAffectedSkillTalents() )
            {
                var effect = new SkillCooldownEffect( StatusManipType.Percental, affectedTalent );
                effects.Add( effect );
            }

            return effects.ToArray();
        }
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this PlayerBuffSkill{TTalent}.
        /// </summary>
        protected override void RefreshAuraEffect()
        {         
            float cooldownChangeInPercent = -this.Talent.CooldownReductionInPercent;

            foreach( var effect in this.Aura.Effects )
            {
                ((SkillCooldownEffect)effect).Value = cooldownChangeInPercent;
            }
        }
    }
}
