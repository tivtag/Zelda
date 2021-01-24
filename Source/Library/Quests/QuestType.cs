// <copyright file="QuestType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.QuestType enumeration.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests
{
    /// <summary>
    /// Enumerates the different types of <see cref="Quest"/>s.
    /// </summary>
    public enum QuestType
    {
        /// <summary>
        /// Represents a quest that drives the main story of the game.
        /// </summary>
        Main = 0,

        /// <summary>
        /// Represents an optional sub quest.
        /// </summary>
        Sub = 1
    }
}
