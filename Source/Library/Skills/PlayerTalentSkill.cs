// <copyright file="PlayerTalentSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.PlayerTalentSkill{TTalent} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills
{
    using Zelda.Talents;

    /// <summary>
    /// Represents a <see cref="PlayerSkill"/> whose power is descriped by a <see cref="Talent"/>.
    /// </summary>
    /// <typeparam name="TTalent">
    /// The exact type of the <see cref="Talent"/>.
    /// </typeparam>
    internal abstract class PlayerTalentSkill<TTalent> : PlayerSkill
        where TTalent : Talent
    {
        /// <summary>
        /// Gets the <see cref="Talent"/> that descripes the power of this PlayerTalentSkill{TTalent}.
        /// </summary>
        public TTalent Talent
        {
            get
            {
                return this.talent;
            }
        }

        /// <summary>
        /// Gets the (localized) description of this PlayerTalentSkill{TTalent}.
        /// </summary>
        public override string Description
        {
            get
            {
                return this.talent.Description;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PlayerTalentSkill{TTalent} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Talent"/> that descripes the power of the new PlayerTalentSkill{TTalent}.
        /// </param>
        /// <param name="cooldownTime">
        /// The number of second the new PlayerTalentSkill{TTalent} has to cooldown after it has been used.
        /// </param>
        protected PlayerTalentSkill( TTalent talent, float cooldownTime )
            : this( talent, new Cooldown( cooldownTime ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlayerTalentSkill{TTalent} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Talent"/> that descripes the power of the new PlayerTalentSkill{TTalent,}.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown of the new PlayerTalentSkill{TTalent}.
        /// </param>
        protected PlayerTalentSkill( TTalent talent, Cooldown cooldown )
            : base( talent.LocalizedName, cooldown, talent.Symbol, talent.Owner )
        {
            this.talent = talent;
        }

        /// <summary>
        /// The <see cref="Talent"/> that descripes the power of this PlayerTalentSkill{TTalent}.
        /// </summary>
        private readonly TTalent talent;
    }
}
