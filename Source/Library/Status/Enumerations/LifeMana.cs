// <copyright file="LifeMana.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.LifeMana enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumerates the two power resources in the game.
    /// </summary>
    public enum LifeMana
    {
        /// <summary>
        /// The health points.
        /// </summary>
        Life = 0,

        /// <summary>
        /// The mana points.
        /// </summary>
        Mana = 1,

        /// <summary>
        /// Represents both life and mana at the same time.
        /// </summary>
        Both = 100
    }
}
