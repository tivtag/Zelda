// <copyright file="WeaponAttackType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.WeaponAttackType enumeration.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the differen attack-types of <see cref="Weapon"/>s.
    /// </summary>
    public enum WeaponAttackType
    {
        /// <summary>
        /// Indicates that no type has been selected.
        /// </summary>
        None,

        /// <summary>
        /// A melee weapon.
        /// </summary>
        Melee,

        /// <summary>
        /// A ranged weapon.
        /// </summary>
        Ranged
    }
}
