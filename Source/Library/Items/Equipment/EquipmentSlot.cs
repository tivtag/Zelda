// <copyright file="EquipmentSlot.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.EquipmentSlot enumeration.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the various slots an Equipment may occupy.
    /// </summary>
    public enum EquipmentSlot
    {
        /// <summary>
        /// Indicates that no specific slot has been set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Weapon slot.
        /// </summary>
        WeaponHand,

        /// <summary>
        /// The Shield slot.
        /// </summary>
        ShieldHand,

        /// <summary>
        /// The Ranged Weapon slot.
        /// </summary>
        Ranged,

        /// <summary>
        /// The Chest slot.
        /// </summary>
        Chest,

        /// <summary>
        /// The Boots slot.
        /// </summary>
        Boots,

        /// <summary>
        /// The Head slot.
        /// </summary>
        Head,

        /// <summary> 
        /// The Necklace slot; the player has two of these.
        /// </summary>
        Necklace,

        /// <summary>
        /// The Ring slot; the player has two of these.
        /// </summary>
        Ring,

        /// <summary>
        /// The Trinket slot; the player has two of these.
        /// </summary>
        Trinket,

        /// <summary>
        /// The Belt slot.
        /// </summary>
        Belt,

        /// <summary>
        /// The Staff/Wand slot.
        /// </summary>
        Staff,

        /// <summary>
        /// The relic slot.
        /// </summary>
        Relic,

        /// <summary>
        /// The gloves slot.
        /// </summary>
        Gloves,

        /// <summary>
        /// The cloak slot.
        /// </summary>
        Cloak,

        /// <summary>
        /// The bag slot.
        /// </summary>
        Bag
    }
}
