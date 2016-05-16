// <copyright file="IQuestGoal.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.IQuestGoal interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Quests
{
    using System;
    using Zelda.Entities;
    
    /// <summary>
    /// Represents a single goal within a <see cref="Quest"/>.
    /// </summary>
    public interface IQuestGoal : Saving.ISaveable, Saving.IStateSaveable
    {
        /// <summary>
        /// Invoked when the state of this IQuestGoal has changed.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the state of this IQuestGoal in percentage.
        /// </summary>
        float State
        { 
            get;
        }

        /// <summary>
        /// Gets a short (localized) string that descripes
        /// the current of this IQuestGoal.
        /// </summary>
        string StateDescription
        { 
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the player has accomplished this IQuestGoal.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the given PlayerEntity has accomplished this IQuestGoal; 
        /// otherwise false.
        /// </returns>
        bool IsAccomplished( PlayerEntity player );

        /// <summary>
        /// Fired when the player has accepeted the quest this IQuestGoal is related to.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        void OnAccepted( PlayerEntity player );

        /// <summary>
        /// Fired when the player has accomplished all goals (including this!) of a <see cref="Quest"/>.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        void OnTurnIn( PlayerEntity player );
    }
}
