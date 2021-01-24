// <copyright file="OfProtectionSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfProtectionSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Protection' suffix adds '80% of item-level' block value and (1 + 7.5%) armor to an Item.
    /// </summary>
    internal sealed class OfProtectionSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfProtection; 
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
            var equipment = (Equipment)item;

            // Calculate:
            int blockValueRating = item.Level;
            int increasedArmor = (int)(1 + (equipment.Armor * 0.075f));

            // Apply:
            equipment.Armor += increasedArmor;

            var effect = equipment.AdditionalEffectsAura.GetEffect<BlockValueEffect>( StatusManipType.Fixed );

            if( effect != null )
            {
                effect.Value += blockValueRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new BlockValueEffect( blockValueRating, StatusManipType.Fixed )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 0.75f, 0.75f, 0.75f );
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
            if( equipment == null )
                return false;

            return ItemExtensions.IsProtectiveShield( equipment ) || equipment.Slot == EquipmentSlot.Chest;                
        }
    }
}
