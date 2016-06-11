// <copyright file="OfFortuneSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfFortuneSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Fortune' suffix adds 1 + 12.5% of item-level Magic Find to an Item.
    /// </summary>
    internal sealed class OfFortuneSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfFortune;
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
            int extraMagicFind = 1 + (int)(item.Level * 0.125f);
            
            // Apply:
            var effect = equipment.AdditionalEffectsAura.GetEffect<MagicFindEffect>( StatusManipType.Fixed );

            if( effect != null )
            {
                effect.Value += extraMagicFind;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new MagicFindEffect( extraMagicFind, StatusManipType.Fixed )
                );
            }

            // Make the item more yellow:
            equipment.MultiplyColor( 1.0f, 1.0f, 0.7f );
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
