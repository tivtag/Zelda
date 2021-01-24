// <copyright file="RetardingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.RetardingPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The Retarding prefix reduces movement speed by 2% 
    /// and increases armor of the item by (1 + 5%).
    /// </summary>
    internal sealed class RetardingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Retarding;
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
            const float MovementSpeedReduction = -2.0f;
            var equipment = (Equipment)item;

            // Armor.
            equipment.Armor = (int)((equipment.Armor * 1.05f) + 1);

            // Movement Speed:
            var effect = equipment.AdditionalEffectsAura.GetEffect<MovementSpeedEffect>( StatusManipType.Percental );

            if( effect != null )
            {
                effect.Value += MovementSpeedReduction;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new MovementSpeedEffect( MovementSpeedReduction, StatusManipType.Percental )
                );
            }

            // Make the item more red:
            item.MultiplyColor( 0.45f, 0.75f, 0.75f );
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
