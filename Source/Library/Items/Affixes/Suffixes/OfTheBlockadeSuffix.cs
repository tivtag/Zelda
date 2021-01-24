// <copyright file="OfTheBlockadeSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfTheBlockadeSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of the Blockade' suffix adds '2 + 75% item-level Block Value' to a shield.
    /// </summary>
    internal sealed class OfTheBlockadeSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfTheBlockade;
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
            float blockValueIncrease = (int)(2 + (0.75f * item.Level));

            // Life Potion Effectiveness
            var effect = equipment.AdditionalEffectsAura.GetEffect<BlockValueEffect>( StatusManipType.Fixed );

            if( effect != null )
            {
                effect.Value += blockValueIncrease;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new BlockValueEffect( blockValueIncrease, StatusManipType.Fixed )
                );
            }

            // Make the item more blue-ish:
            equipment.MultiplyColor( 0.85f, 0.75f, 1.00f );
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
            return equipment != null && equipment.Slot == EquipmentSlot.ShieldHand && equipment.Armor > 0;
        }
    }
}
