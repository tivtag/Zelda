// <copyright file="ChanceToStatus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ChanceToStatus enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumerates the different chance-to status values.
    /// </summary>
    public enum ChanceToStatus
    {
        /// <summary>
        /// No specific ChanceToStatus.
        /// </summary>
        None = 0,

        /// <summary>
        /// A critical strike gives bonus damage.
        /// </summary>
        Crit,

        /// <summary>
        /// A critical heal provides bonus healing.
        /// </summary>
        CritHeal,

        /// <summary>
        /// Melee and Ranged Attacks can be doged.
        /// </summary>
        Dodge,

        /// <summary>
        /// Melee and Ranged Attacks can miss.
        /// </summary>
        Miss,

        /// <summary>
        /// Melee and Ranged attacks can be blocked.
        /// </summary>
        Block,

        /// <summary>
        /// Melee attacks can be parried.
        /// </summary>
        Parry,

        /// <summary>
        /// Ranged attacks can pierce their target.
        /// </summary>
        Pierce,

        /// <summary>
        /// A critical block provides additional block value.
        /// </summary>
        CritBlock,
    }
}
