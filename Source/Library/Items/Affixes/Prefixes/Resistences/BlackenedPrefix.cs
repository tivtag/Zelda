// <copyright file="BlackenedPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.BlackenedPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    
    /// <summary>
    /// The Blackened prefix adds shadow resistance to an Item.
    /// </summary>
    internal sealed class BlackenedPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            { 
                return AffixResources.Blackened; 
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
            int extraResistance = (int)(1 + (1.7f * item.Level));

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<ChanceToResistEffect>(
                (x) => x.ElementalSchool == ElementalSchool.Shadow && x.ManipulationType == StatusManipType.Rating
            );

            if( effect != null )
            {
                effect.Value += extraResistance;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToResistEffect( extraResistance, StatusManipType.Rating, ElementalSchool.Shadow )
                );
            }

            // Make the item more red:
            equipment.MultiplyColor( 0.25f, 0.25f, 0.25f );
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
            return equipment != null && equipment.Slot.IsJewelry();
        }
    }
}
