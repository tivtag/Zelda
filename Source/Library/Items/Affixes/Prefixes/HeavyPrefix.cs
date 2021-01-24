// <copyright file="HeavyPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.HeavyPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Rusty prefix adds 35% armor of an equipment, but also makes it 25% harder to wear.
    /// </summary>
    internal sealed class HeavyPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Heavy;
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

            equipment.Armor += (int)(2 + (0.35f * equipment.Armor) );

            const float StatMultiplier = 1.3f;
            equipment.RequiredLevel        = 1 + (int)(equipment.RequiredLevel * 1.1f);
            equipment.RequiredStrength     = (int)(equipment.RequiredStrength * StatMultiplier);
            equipment.RequiredDexterity    = (int)(equipment.RequiredDexterity * StatMultiplier);
            equipment.RequiredAgility      = (int)(equipment.RequiredAgility * StatMultiplier);
            equipment.RequiredVitality     = (int)(equipment.RequiredVitality * StatMultiplier);
            equipment.RequiredIntelligence = (int)(equipment.RequiredIntelligence * StatMultiplier);
            equipment.RequiredLuck         = (int)(equipment.RequiredLuck * StatMultiplier);

            // Make the item more gray/blue:
            item.MultiplyColor( 0.5f, 0.65f, 0.5f );
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
            return equipment != null && equipment.Armor > 0;
        }
    }
}
