// <copyright file="AttackType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.AttackType enumeration.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks
{
    /// <summary>
    /// Enumerates the different attack types.
    /// </summary>
    public enum AttackType
    {
        /// <summary>
        /// Represents no specific attack.
        /// </summary>
        None = -1,

        /// <summary>
        /// The attack is a melee attack.
        /// </summary>
        Melee = 0,

        /// <summary>
        /// The attack is a ranged attack.
        /// </summary>
        Ranged = 1,

        /// <summary>
        /// The attack is a magic attack.
        /// </summary>
        Spell = 2,

        /// <summary>
        /// All attack types in one.
        /// </summary>
        All = 3,
    }
}
