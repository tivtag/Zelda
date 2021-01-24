// <copyright file="OfFeathersSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfFeathersSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Feathers' suffix adds 1 + 20% of item-level Movement Speed Rating to an Item.
    /// </summary>
    internal sealed class OfFeathersSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.OfFeathers;
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
            int rating = 1 + (int)(item.Level * 0.20f);
            var equipment = (Equipment)item;

            // Movement Speed:
            var effect = equipment.AdditionalEffectsAura.GetEffect<MovementSpeedEffect>( StatusManipType.Rating );

            if( effect != null )
            {
                effect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new MovementSpeedEffect( rating, StatusManipType.Rating )
                );
            }

            // Make the item more red/blue:
            equipment.MultiplyColor( 0.9f, 0.9f, 0.9f );
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
            return equipment != null && equipment.Slot == EquipmentSlot.Boots;
        }
    }
}
