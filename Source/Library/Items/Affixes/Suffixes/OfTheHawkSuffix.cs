// <copyright file="OfTheHawkSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfTheHawkSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    /// <summary>
    /// The 'of the Hawk' suffix adds 1 + 9% of item-level dexterity and agility.
    /// </summary>
    internal sealed class OfTheHawkSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get { return AffixResources.OfTheHawk; }
        }

        /// <summary>
        /// Applies this IAffix to an Item.
        /// </summary>
        /// <param name="item">
        /// The Item that gets directly modified by this IAffix.
        /// </param>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        public void Apply( Item item, Item baseItem )
        {
            var equipment = (Equipment)item;

            int statIncrease = (int)(1 + (0.09f * item.Level));
            
            // Apply:
            equipment.Dexterity += statIncrease;
            equipment.Agility += statIncrease;

            // Make the item more blue:
            equipment.MultiplyColor( 0.9f, 0.8f, 1.0f );
        }

        /// <summary>
        /// Gets a value indicating whether this IAffix could
        /// possibly applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The item this IAffix is supposed to be applied to.
        /// </param>
        /// <returns>
        /// True if this IAffix could possible applied to the given <paramref name="baseItem"/>;
        /// otherwise false.
        /// </returns>
        public bool IsApplyable( Item baseItem )
        {
            var equipment = baseItem as Equipment;

            if( equipment.Slot == EquipmentSlot.ShieldHand )
                return equipment.Armor == 0;

            return equipment.Slot == EquipmentSlot.Head || 
                   equipment.Slot == EquipmentSlot.Gloves ||
                   equipment.Slot == EquipmentSlot.Cloak;
        }
    }
}
