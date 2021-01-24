// <copyright file="DamageSource.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageSource enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Enumerates the different sources of damage.
    /// </summary>
    public enum DamageSource : byte
    {
        /// <summary>
        /// Represents no damage source.
        /// </summary>
        None,

        /// <summary>
        /// States that a melee attack was the source of the damage.
        /// </summary>
        Melee,

        /// <summary>
        /// States that a ranged attack was the source of the damage.
        /// </summary>
        Ranged,

        /// <summary>
        /// States that a spell was the source of the damage.
        /// </summary>
        Spell,

        /// <summary>
        /// Represents all damage sources together.
        /// </summary>
        All = System.Byte.MaxValue
    }
}
