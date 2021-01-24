// <copyright file="StrondPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.StrondPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Strond prefix removes 25% of all basic stats from an equipment.
    /// </summary>
    internal sealed class StrondPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Strond; 
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

            equipment.Strength     -= (int)(equipment.Strength * 0.25f);
            equipment.Dexterity    -= (int)(equipment.Dexterity * 0.25f);
            equipment.Vitality     -= (int)(equipment.Vitality * 0.25f);
            equipment.Agility      -= (int)(equipment.Agility * 0.25f);
            equipment.Intelligence -= (int)(equipment.Intelligence * 0.25f);
            equipment.Luck         -= (int)(equipment.Luck * 0.25f);

            equipment.Armor        -= (int)(equipment.Armor * 0.25f);
            equipment.RubiesWorth  -= (int)(equipment.RubiesWorth * 0.25f);

            // Make the item more green:
            item.MultiplyColor( 0.75f, 1.00f, 0.75f );
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
            return equipment != null &&
                (equipment.Strength > 0 || equipment.Dexterity > 0 ||
                 equipment.Agility > 0 || equipment.Vitality > 0 ||
                 equipment.Intelligence > 0 || equipment.Luck > 0 );
        }
    }
}
