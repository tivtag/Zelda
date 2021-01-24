// <copyright file="SkillTalent.TSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.SkillTalent{TSkill} class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents
{
    using System;
    using Zelda.Skills;

    /// <summary>
    /// Defines a <see cref="Talent"/> that provides
    /// the player with a new <see cref="Skill"/>.
    /// </summary>
    /// <typeparam name="TSkill">
    /// The exact type of the skill provided
    /// by this SkillTalent.
    /// </typeparam>
    internal abstract class SkillTalent<TSkill> : SkillTalent, ISkillProvider<TSkill>
        where TSkill : Skill
    {
        /// <summary>
        /// Gets the skill that is provided through the SkillTalent{TSkill}.
        /// </summary>
        public new TSkill Skill
        {
            get
            {
                return this.skill;
            }
        }

        /// <summary>
        /// Gets the Type of the Skill learned by this SkillTalent{TSkill}.
        /// </summary>
        public override Type SkillType
        {
            get
            {
                return typeof( TSkill );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTalent&lt;TSkill&gt;"/> class.
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
            string localizedName,
            Atom.Xna.Sprite symbol,
            int maximumLevel,
            TalentTree tree,
            IZeldaServiceProvider serviceProvider
        )
            : base( localizedName, symbol, maximumLevel, tree, serviceProvider )
        {
        }

        /// <summary>
        /// Initializes this SkillTalent{TSkill}.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.skill = (TSkill)base.Skill;
        }

        /// <summary>
        /// Uninitializes this SkillTalent{TSkill}.
        /// </summary>
        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.skill = null;
        }

        /// <summary>
        /// The skill that is provided through this SkillTalent{TSkill}.
        /// </summary>
        private TSkill skill;
    }
}
