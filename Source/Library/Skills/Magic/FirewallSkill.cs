// <copyright file="FirewallSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Magic.FirewallSkill class.
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
    /// Each Firewall consists of 3 individual pillars that deal ..
    /// 
    /// </summary>
    internal sealed class FirewallSkill : PlayerSpellSkill<FirewallTalent, FirewallSpell>, ICooldownDependant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new FirewallSkill.
        /// </param>
        /// <param name="piercingFireTalent">
        /// Identifies the PiercingFireTalent that improves the cance for Firewall to crit.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirewallSkill( FirewallTalent talent, PiercingFireTalent piercingFireTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, FirewallTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBaseAndTotal( FirewallTalent.ManaCostOfBaseMana, FirewallTalent.ManaCostOfTotalMana );

            this.piercingFireTalent = piercingFireTalent;

            this.method = new FirepillarDamageMethod();
            this.method.Setup( serviceProvider );

            this.Spell = new FirewallSpell( this.Player, FirewallTalent.CastTime, this.method, serviceProvider ) {
                Limiter = new FreelyFireNtimesThenTimedAttackLimiter( 
                    FirewallTalent.TimesCastableBeforeCooldown,
                    this.Cooldown
                )
            };
        }

        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            var damageModifierRange = new FloatRange( this.Talent.MinumumDamageModifier, this.Talent.MaximumDamageModifier );
            this.method.SetValues( damageModifierRange, this.piercingFireTalent.CritChanceIncrease );
        }

        /// <summary>
        /// Refreshes the cooldown of this FirewallSkill.
        /// </summary>
        public void RefreshCooldown()
        {
            float fixedValue, multiplierValue;
            this.AuraList.GetEffectValues(
                SkillCooldownEffect.GetIdentifier<FirewallSkill>(),
                out fixedValue,
                out multiplierValue
            );

            this.Cooldown.TotalTime = (FirewallTalent.Cooldown + fixedValue) * multiplierValue;
        }

        /// <summary>
        /// Identifies the PiercingFireTalent that increases the chance to crit with this FirewallSkill.
        /// </summary>
        private readonly PiercingFireTalent piercingFireTalent;
        
        /// <summary>
        /// The FireSpellDamageMethod that calculates how much damage an arrow launched by the FirewallSkill does.
        /// </summary>
        private readonly FirepillarDamageMethod method;
    }
}