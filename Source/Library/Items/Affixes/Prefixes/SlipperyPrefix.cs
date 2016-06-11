// <copyright file="SlipperyPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.SlipperyPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    
    /// <summary>
    /// The Slippery prefix adds '+X Dodge Rating' to an item.
    /// </summary>
    internal sealed class SlipperyPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.Slippery;
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
            float value = (int)(2 + (item.Level / 2));

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>(
                ( x ) => x.StatusType == ChanceToStatus.Dodge && x.ManipulationType == StatusManipType.Rating
            );

            if( effect != null )
            {
                effect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( value, StatusManipType.Rating, ChanceToStatus.Dodge )
                );
            }

            // Make the item more yellow/green:
            equipment.MultiplyColor( 0.75f, 0.95f, 0.65f );
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
            return equipment != null && !equipment.Slot.IsWeaponOrShield() && !equipment.Slot.IsJewelry();
        }
    }
}
