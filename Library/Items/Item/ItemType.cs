// <copyright file="ItemType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the different types of items.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// No type has been specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a simple item.
        /// </summary>
        Item,

        /// <summary>
        /// Represents an item that can be equipped.
        /// </summary>
        Equipment,

        /// <summary>
        /// Represents an item that can be equipped in a weapon slot.
        /// </summary>
        Weapon,

        /// <summary>
        /// Represents an item that can be socketed into another item.
        /// </summary>
        Gem,

        /// <summary>
        /// Represents an item that can be equipped, and has been 'enhanced' with affixes.
        /// </summary>
        AffixedEquipment,

        /// <summary>
        /// Represents an item that can be equipped in a weapon slot, and has been 'enhanced' with affixes.
        /// </summary>
        AffixedWeapon
    }
}
