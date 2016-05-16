// <copyright file="ISkillProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Skills.ISkillProvider and Zelda.Skills.IISkillProvider{TSkill} interfaces.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Skills
{
    /// <summary>
    /// Provides a mechanism that allows to receive a <see cref="Skill"/> instance.
    /// </summary>
    public interface ISkillProvider
    {
        /// <summary>
        /// Gets the <see cref="Skill"/> this ISkillProvider provides.
        /// </summary>
        Skill Skill
        {
            get;
        }
    }

    /// <summary>
    /// Provides a mechanism that allows to receive a <see cref="Skill"/> instance.
    /// </summary>
    /// <typeparam name="TSkill">
    /// The type of the Skill provided by the ISkillProvider.
    /// </typeparam>
    public interface ISkillProvider<TSkill> : ISkillProvider
        where TSkill : Skill
    {
        /// <summary>
        /// Gets the <see cref="Skill"/> this ISkillProvider provides.
        /// </summary>
        new TSkill Skill
        {
            get;
        }
    }
}
