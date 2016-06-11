// <copyright file="CemeteryPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.CemeteryPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The Cemetery prefix adds '+damage done to undeads' to an Item.
    /// </summary>
    internal sealed class CemeteryPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.Cemetery;
            }
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
            int extraDamage = (int)(1 + (0.2f * item.Level) );

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<DamageDoneAgainstRaceEffect>(
                ( x ) => x.Race == RaceType.Undead && x.ManipulationType == StatusManipType.Fixed
            );

            if( effect != null )
            {
                effect.Value += extraDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageDoneAgainstRaceEffect( RaceType.Undead, extraDamage )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 0.35f, 0.35f, 0.25f );
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

            return equipment != null && 
                (equipment.Slot == EquipmentSlot.Ring || 
                 equipment.Slot == EquipmentSlot.Necklace ||
                 equipment.Slot == EquipmentSlot.Relic || 
                 equipment.Slot == EquipmentSlot.Trinket || 
                 equipment.Slot == EquipmentSlot.Staff || 
                 equipment.Slot == EquipmentSlot.Gloves || 
                 equipment.Slot == EquipmentSlot.Cloak );
        }
    }
}
