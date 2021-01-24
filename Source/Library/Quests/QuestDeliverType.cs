// <copyright file="QuestDeliverType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.QuestDeliverType enumeration.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests
{
    /// <summary>
    /// Enumerates the different possible types 
    /// that a quest has to be delivered.
    /// </summary>
    public enum QuestDeliverType
    {
        /// <summary>
        /// The quest is instantly accomplished after the 
        /// player has fulfilled all <see cref="IQuestGoal"/>s.
        /// </summary>
        Instant,

        /// <summary>
        /// The quest has to be delivered at a Non Player Character
        /// to receive the rewards.
        /// </summary>
        Npc
    }
}
