// <copyright file="DifficultyId.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.DifficultyId enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    /// <summary>
    /// Enumerates the difficulties of the game;
    /// assigning an unique id to each.
    /// </summary>
    public enum DifficultyId
    {
        /// <summary>
        /// Represents the default difficulty.
        /// </summary>
        Easy = 0,

        /// <summary>
        /// Represents the difficulty after the Easy difficulty.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents the difficulty after the Normal difficulty.
        /// </summary>
        Nightmare,

        /// <summary>
        /// Represents the difficulty after the Nightmare difficulty.
        /// </summary>
        Hell,

        /// <summary>
        /// Represents the difficulty after the Hell difficulty. Must be unlocked.
        /// </summary>
        Insane
    }
}
