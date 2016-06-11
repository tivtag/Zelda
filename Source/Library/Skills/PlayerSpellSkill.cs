// <copyright file="PlayerSpellSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.PlayerSpellSkill{TTalent, TSpell} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills
{
    using Zelda.Casting;
    using Zelda.Talents;

    /// <summary>
    /// Represents a <see cref="PlayerSkill"/> that internally casts an <see cref="Spell"/>
    /// when the Skill is fired.
    /// </summary>
    /// <typeparam name="TTalent">
    /// The exact type of the <see cref="SkillTalent"/>.
    /// </typeparam>
    /// <typeparam name="TSpell">
    /// The exact type of the <see cref="Spell"/>.
    /// </typeparam>
    internal abstract class PlayerSpellSkill<TTalent, TSpell> : PlayerAttackSkill<TTalent, TSpell>
        where TTalent : SkillTalent
        where TSpell : PlayerSpell
    {
        /// <summary>
        /// Gets or sets the Spell that is cast by this PlayerSpellSkill{TTalent, TSpell}.
        /// </summary>
        protected TSpell Spell
        {
            get
            {
                return this.Attack;
            }

            set
            {
                this.Attack = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PlayerSpellSkill{TTalent, TSpell} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Talent"/> that descripes the power of the new PlayerSpellSkill{TTalent, TSpell}.
        /// </param>
        /// <param name="cooldownTime">
        /// The number of second the new PlayerSpellSkill{TTalent, TSpell} has to cooldown after it has been used.
        /// </param>
        protected PlayerSpellSkill( TTalent talent, float cooldownTime )
            : base( talent, cooldownTime )
        {
        }

        /// <summary>
        /// Fires this PlayerSpellSkill{TTalent, TSpell}.
        /// </summary>
        /// <returns>
        /// True if the Spell was cast;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            return this.Attack.Cast();
        }
    }
}
