// <copyright file="EquipmentStatusSlot.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.EquipmentStatusSlot enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the equipment slots that are available in the <see cref="EquipmentStatus"/>.
    /// </summary>
    /// <seealso cref="EquipmentSlot"/>
    /// <seealso cref="EquipmentStatus"/>
    public enum EquipmentStatusSlot
    {
        /// <summary>
        /// Indicates that no specific slot has been set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The WeaponHand slot.
        /// </summary>
        WeaponHand,

        /// <summary>
        /// The ShieldHand slot.
        /// </summary>
        ShieldHand,

        /// <summary>
        /// The RangedWeapon slot.
        /// </summary>
        Ranged,

        /// <summary>
        /// The Chest slot.
        /// </summary>,
        Chest,

        /// <summary>
        /// The Head slot.
        /// </summary>
        Head,

        /// <summary>
        /// The Boots slot.
        /// </summary>
        Boots,

        /// <summary>
        /// The first Ring slot.
        /// </summary>
        Ring1,

        /// <summary>
        /// The second Ring slot.
        /// </summary>
        Ring2,

        /// <summary>
        /// The first Necklace slot.
        /// </summary>
        Necklace1,

        /// <summary>
        /// The second Necklace slot.
        /// </summary>
        Necklace2,

        /// <summary>
        /// The first Trinket slot.
        /// </summary>
        Trinket1,

        /// <summary>
        /// The second Trinket slot.
        /// </summary>
        Trinket2,

        /// <summary>
        /// The Belt slot.
        /// </summary>
        Belt,

        /// <summary>
        /// The Staff/Wand slot.
        /// </summary>
        Staff,

        /// <summary>
        /// The first relic slot.
        /// </summary>
        Relic1,

        /// <summary>
        /// The second relic slot.
        /// </summary>
        Relic2,

        /// <summary>
        /// The gloves slot.
        /// </summary>
        Gloves,

        /// <summary>
        /// The cloak slot.
        /// </summary>
        Cloak,

        /// <summary>
        /// The first bag slot.
        /// </summary>
        Bag1,

        /// <summary>
        /// The second bag slot.
        /// </summary>
        Bag2,

        /// <summary>
        /// Represents the number of EquipmentStatusSlot enum value.
        /// </summary>
        _EnumCount
    }
}
