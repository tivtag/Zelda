// <copyright file="SkillTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.SkillTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents
{
    using System;
    using Zelda.Skills;

    /// <summary>
    /// Defines a <see cref="Talent"/> that provides
    /// the player with a new <see cref="Skill"/>.
    /// </summary>
    internal abstract class SkillTalent : Talent, ISkillProvider
    {
        /// <summary>
        /// Gets the skill that is provided through the SkillTalent.
        /// </summary>
        /// <value>
        /// Is null until the SkillTalent is actully learned.
        /// </value>
        public Skill Skill
        {
            get 
            { 
                return this.skill; 
            }
        }

        /// <summary>
        /// Gets the Type of the Skill learned by this SkillTalent.
        /// </summary>
        public abstract Type SkillType
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTalent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="maximumLevel">
        /// The maximum number of TalentPoints the Player can invest into the talent.
        /// </param>
        /// <param name="tree">
        /// The TalentTree that 'owns' the new Talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected internal SkillTalent(
            string                localizedName,
            Atom.Xna.Sprite       symbol,
            int                   maximumLevel,
            TalentTree            tree,
            IZeldaServiceProvider serviceProvider
        )
            : base( localizedName, symbol, maximumLevel, tree, serviceProvider )
        {
        }

        /// <summary>
        /// Creates the <see cref="Skill"/> object of this SkillTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected abstract Skills.Skill CreateSkill();

        /// <summary>
        /// Initializes this SkillTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.skill = this.CreateSkill();
            this.skill.Initialize();

            this.Tree.Owner.Skills.Learn( this.skill );
        }

        /// <summary>
        /// Uninitializes this SkillTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            if( this.skill != null )
            {
                this.skill.Uninitialize();
                this.Tree.Owner.Skills.Unlearn( this.skill );
                this.skill = null;
            }
        }

        /// <summary>
        /// Refreshes the strength of this SkillTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.RefreshSkill();
        }

        /// <summary>
        /// Refreshes the power of the <see cref="Skill"/> that is related to this SkillTalent.
        /// </summary>
        public virtual void RefreshSkill()
        {
            if( this.skill != null )
            {
                this.skill.RefreshDataFromTalents();
            }
        }

        /// <summary>
        /// The skill that is provided through this SkillTalent.
        /// </summary>
        private Skill skill;
    }
}
