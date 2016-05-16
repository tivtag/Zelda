// <copyright file="OfEaseSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfEaseSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Ease' suffix makes an item 35% easier to wear.
    /// </summary>
    internal sealed class OfEaseSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfEase; 
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

            const float Multiplier = 0.7f;
            equipment.RequiredLevel        = (int)(equipment.RequiredLevel * Multiplier);
            equipment.RequiredStrength     = (int)(equipment.RequiredStrength * Multiplier);
            equipment.RequiredDexterity    = (int)(equipment.RequiredDexterity * Multiplier);
            equipment.RequiredAgility      = (int)(equipment.RequiredAgility * Multiplier);
            equipment.RequiredVitality     = (int)(equipment.RequiredVitality * Multiplier);
            equipment.RequiredIntelligence = (int)(equipment.RequiredIntelligence * Multiplier);
            equipment.RequiredLuck         = (int)(equipment.RequiredLuck * Multiplier);
            
            // Make the item more 'gray':
            equipment.MultiplyColor( 0.5f, 0.5f, 0.5f );
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
