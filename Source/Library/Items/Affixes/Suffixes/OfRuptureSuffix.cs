// <copyright file="OfRuptureSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfRuptureSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Rupture' suffix adds '+X Armor Ignore' to an item.
    /// </summary>
    internal sealed class OfRuptureSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfRupture; 
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
            int armorIgnore = (int)(4.5f * item.Level);
            ApplyArmorIgnore( armorIgnore, item );

            // Make the item darker:
            item.MultiplyColor( 0.55f, 0.70f, 0.35f );
        }

        /// <summary>
        /// Applies the armor ignore effect to the given Item.
        /// </summary>
        /// <param name="armorIgnore">
        /// The amount of armor ignore to apply.
        /// </param>
        /// <param name="item">
        /// The item to modify.
        /// </param>
        private static void ApplyArmorIgnore( int armorIgnore, Item item )
        {
            var equipment = (Equipment)item;
            var armorIgnoreEffect = equipment.AdditionalEffectsAura.GetEffect<ArmorIgnoreEffect>( StatusManipType.Fixed );

            if( armorIgnoreEffect != null )
            {
                armorIgnoreEffect.Value += armorIgnore;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ArmorIgnoreEffect( armorIgnore, StatusManipType.Fixed )
                );
            }
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
            if( baseItem.Level < 20 )
                return false;

            if( baseItem is Weapon )
                return true;

            var equipment = baseItem as Equipment;
            if( equipment == null )
                return false;

            return equipment.Slot == EquipmentSlot.Cloak || ItemExtensions.IsOffensiveShieldHand( equipment );
        }
    }
}
