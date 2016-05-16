// <copyright file="DebuffFlags.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.DebuffFlags enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumates the different types of debuffs.
    /// </summary>
    [System.Flags]
    public enum DebuffFlags
    {
        /// <summary>
        /// Indicates that there is no debuff.
        /// </summary>
        None = 0,

        /// <summary>
        /// Movement is no possible under this effect.
        /// </summary>
        Immobilize = 2,

        /// <summary>
        /// Movement is slowed down under this effect.
        /// </summary>
        Slow = 4,

        /// <summary>
        /// A disease, may get worse over time.
        /// </summary>
        Disease = 8,

        /// <summary>
        /// An evil curse.
        /// </summary>
        Curse = 16,

        /// <summary>
        /// A poison, damaging over time.
        /// </summary>
        Poisoned = 32,

        /// <summary>
        /// Burns, damaging over time.
        /// </summary>
        Burning = 64
    }
}
