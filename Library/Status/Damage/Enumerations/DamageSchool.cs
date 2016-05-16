// <copyright file="DamageSchool.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageSchool enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Enumerates the different schools of damage.
    /// </summary>
    public enum DamageSchool : byte
    {
        /// <summary>
        /// Represents no specific damage school.
        /// </summary>
        None,

        /// <summary>
        /// Represents the physical damage school;
        /// used by most melee and ranged attacks.
        /// Is mitigated by armor.
        /// </summary>
        Physical,

        /// <summary>
        /// Represents the magical damage school;
        /// used by most spell attacks.
        /// Isn't mitigated by armor, bt can be resisted.
        /// </summary>
        Magical,

        /// <summary>
        /// Represents all damage schools together.
        /// </summary>
        All = System.Byte.MaxValue
    }
}
