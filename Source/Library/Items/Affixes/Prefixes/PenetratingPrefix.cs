// <copyright file="PenetratingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.PenetratingPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The Penetrating prefix adds '+chance to pierce with ranged attacks' to a melee or ranged weapon.
    /// </summary>
    internal sealed class PenetratingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Penetrating;
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
            int armorIgnoreRating = 6 + (item.Level / 3);

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<ArmorIgnoreEffect>( StatusManipType.Rating );

            if( effect != null )
            {
                effect.Value += armorIgnoreRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ArmorIgnoreEffect( armorIgnoreRating, StatusManipType.Rating )
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
            if( baseItem is Weapon )
                return true;

            var equipment = baseItem as Equipment;
            return equipment != null && (equipment.Armor == 0 && equipment.Slot == EquipmentSlot.ShieldHand);
        }
    }
}
