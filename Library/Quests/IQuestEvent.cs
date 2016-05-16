// <copyright file="IQuestEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.IQuestEvent interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests
{
    /// <summary>
    /// Represents the interface an object can implement
    /// to allow notification when a quest has been accomplished.
    /// </summary>
    public interface IQuestEvent : Saving.ISaveable
    {
        /// <summary>
        /// Executes this <see cref="IQuestEvent"/>.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        void Execute( Quest quest );
    }
}
