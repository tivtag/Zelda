// <copyright file="WeaponTypeGroup.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.WeaponTypeGroup enumeration.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the different groups a weapon of a specific
    /// <see cref="WeaponType"/> can be grouped into.
    /// </summary>
    public enum WeaponTypeGroup
    {
        /// <summary>
        /// One-Handed Weapons allow the player to block with a shield;
        /// includes:
        /// <para>
        ///     <see cref="WeaponType.OneHandedAxe"/>
        ///     <see cref="WeaponType.OneHandedMace"/>
        ///     <see cref="WeaponType.OneHandedSword"/>
        ///     <see cref="WeaponType.Dagger"/>
        /// </para>
        /// </summary>
        OneHanded,

        /// <summary>
        /// Two-Handed Weapons don't allow the player to block with a shield;
        /// includes:
        /// <para>
        ///     <see cref="WeaponType.TwoHandedAxe"/>
        ///     <see cref="WeaponType.TwoHandedMace"/>
        ///     <see cref="WeaponType.TwoHandedSword"/>
        /// </para>
        /// </summary>
        TwoHanded,

        /// <summary>
        /// Ranged Weapon allow the player to fire projectiles.
        /// <para>
        ///     <see cref="WeaponType.Bow"/>
        ///     <see cref="WeaponType.Crossbow"/>
        /// </para>
        /// </summary>
        Ranged
    }
}
