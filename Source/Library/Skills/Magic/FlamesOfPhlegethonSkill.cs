// <copyright file="FlamesOfPhlegethonSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Magic.FlamesOfPhlegethonSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Magic
{
    using Atom.Math;
    using Zelda.Attacks.Limiter;
    using Zelda.Casting.Spells;
    using Zelda.Status;
    using Zelda.Talents.Magic;

    /// <summary>
    /// Summons {0} wave(s) of fire directly from the underworld.
    /// Targets that are hit take {0}% to {1}% fire damage.
    /// {1} secs cooldown. {2} secs cast time.
    /// </summary>
    internal sealed class FlamesOfPhlegethonSkill : PlayerSpellSkill<FlamesOfPhlegethonTalent, FlamesOfPhlegethonSpell>, ICooldownDependant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlamesOfPhlegethonSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new FlamesOfPhlegethonSkill.
        /// </param>
        /// <param name="piercingFireTalent">
        /// Identifies the PiercingFireTalent that improves the cance for FlamesOfPhlegethon to crit.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FlamesOfPhlegethonSkill( FlamesOfPhlegethonTalent talent, PiercingFireTalent piercingFireTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, FlamesOfPhlegethonTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBaseAndTotal( FlamesOfPhlegethonTalent.ManaCostOfBaseMana, FlamesOfPhlegethonTalent.ManaCostOfTotalMana );

            this.piercingFireTalent = piercingFireTalent;

            this.method = new FirepillarDamageMethod();
            this.method.Setup( serviceProvider );

            this.Spell = new FlamesOfPhlegethonSpell( this.Player, FlamesOfPhlegethonTalent.CastTime, this.method, serviceProvider ) {
                Limiter = new TimedAttackLimiter( this.Cooldown )
            };
        }
        
        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Spell.MaximumWaveCount = this.Talent.WaveCount;

            var damageModifierRange = new FloatRange( this.Talent.MinumumDamageModifier, this.Talent.MaximumDamageModifier );
            this.method.SetValues( damageModifierRange, this.piercingFireTalent.CritChanceIncrease );
        }

        /// <summary>
        /// Refreshes the cooldown of this FlamesOfPhlegethonSkill.
        /// </summary>
        public void RefreshCooldown()
        {
            float fixedValue, multiplierValue;
            this.AuraList.GetEffectValues(
                SkillCooldownEffect.GetIdentifier<FlamesOfPhlegethonSkill>(),
                out fixedValue,
                out multiplierValue
            );

            this.Cooldown.TotalTime = (FlamesOfPhlegethonTalent.Cooldown + fixedValue) * multiplierValue;
        }

        /// <summary>
        /// Identifies the PiercingFireTalent that increases the chance to crit with this FlamesOfPhlegethonSkill.
        /// </summary>
        private readonly PiercingFireTalent piercingFireTalent;

        /// <summary>
        /// The AttackDamageMethod that calculates how much damage an arrow launched by the FlamesOfPhlegethonSkill does.
        /// </summary>
        private readonly FirepillarDamageMethod method;
    }
}