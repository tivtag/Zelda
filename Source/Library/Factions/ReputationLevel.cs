// <copyright file="ReputationLevel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Factions.ReputationLevel enumeration.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Factions
{
    /// <summary>
    /// Enumerates the different levels on the reputation table of a faction.
    /// </summary>
    /// <remarks>
    /// This enumeration may not be changed as the exact intager values of it are used.
    /// The possible reputation levels are set into stone for now. (more than enough anyway)
    /// </remarks>
    public enum ReputationLevel
    {
        /// <summary>
        /// Represents no specific reputation level.
        /// </summary>
        None = 0,

        /// <summary>
        /// The lowest possible reputation level.
        /// </summary>
        /// <remarks>
        /// The player won't get any quests from a hated Faction.
        /// He must build up his reputation somewhere else.
        /// </remarks>
        Hated = 1,

        /// <summary>
        /// The second lowest possible reputation level.
        /// </summary>
        /// <remarks>
        /// The player will only get a few quests from a hostile Faction.
        /// He must build up his reputation mainly somewhere else.
        /// </remarks>
        Hostile = 2,

        /// <summary>
        /// The level below Neutral.
        /// </summary>
        Unfriendly = 3,

        /// <summary>
        /// The default reputation level of most factions.
        /// </summary>
        Neutral = 4,

        /// <summary>
        /// The level above Neutral. Provides additional quests.
        /// </summary>
        Friendly = 5,

        /// <summary>
        /// The level above Friendly. Provides additional quests.
        /// </summary>
        Honored = 6,

        /// <summary>
        /// The second highest reputation level.
        /// </summary>
        Revered = 7,

        /// <summary>
        /// The highest reputation level.
        /// </summary>
        /// <remarks>
        /// The exalted level is pretty hard to archive,
        /// but gives the ultimate rewards of the faction.
        /// </remarks>
        Exalted = 8
    }
}
