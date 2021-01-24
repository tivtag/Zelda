// <copyright file="IQuestReward.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.IQuestReward interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests
{
    using Zelda.Entities;

    /// <summary>
    /// Represents a single reward of a <see cref="Quest"/>. 
    /// </summary>
    /// <remarks>
    /// The rewards of a <see cref="Quest"/> are rewarded to the player
    /// once he has accomplished all <see cref="IQuestGoal"/>s.
    /// </remarks>
    public interface IQuestReward : Saving.ISaveable
    {
        /// <summary>
        /// Rewards this IQuestReward to the player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        void Reward( PlayerEntity player );
    }
}
