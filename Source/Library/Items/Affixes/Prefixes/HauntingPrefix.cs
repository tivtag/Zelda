// <copyright file="HauntingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.HauntingPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The Enraging prefix adds '+X Chance To Be Missed Rating' to an item.
    /// </summary>
    internal sealed class HauntingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Haunting;
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
            int value = -(2 + (int)(item.Level * 0.15f));

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<ChanceToBeStatusEffect>(
                ( x ) => x.StatusType == ChanceToStatus.Miss && x.ManipulationType == StatusManipType.Rating
             );

            if( effect != null )
            {
                effect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToBeStatusEffect( value, StatusManipType.Rating, ChanceToStatus.Miss )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 1.0f, 1.0f, 1.0f, 0.5f );
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

            return equipment.Slot.IsJewelry() || equipment.Slot == EquipmentSlot.Staff;
        }
    }
}
