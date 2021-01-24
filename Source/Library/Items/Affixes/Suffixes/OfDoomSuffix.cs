// <copyright file="OfDoomSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfDoomSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Doom' suffix adds '0.1 + 2.0% of item-level' Crit Chance to an Item.
    /// </summary>
    internal sealed class OfDoomSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.OfDoom; 
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
            int critRating = 2 + (item.Level / 2);

            // Apply:
            var effect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>( 
                (x) => x.StatusType == ChanceToStatus.Crit && x.ManipulationType == StatusManipType.Rating
            );

            if( effect != null )
            {
                effect.Value += critRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( critRating, StatusManipType.Rating, ChanceToStatus.Crit )
                );
            }

            // Make the item more red:
            equipment.MultiplyColor( 1.0f, 0.5f, 0.5f );
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
            return baseItem is Equipment;
        }
    }
}
