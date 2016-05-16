// <copyright file="RiskyPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.RiskyPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Risky prefix decreases the minimum damage and increases the maximum damage of a weapon. 
    /// </summary>
    internal sealed class RiskyPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Risky; 
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
            var weapon = (Weapon)item;

            // Apply change:
            int change = (int)(weapon.DamageMinimum * 0.65f);

            weapon.DamageMinimum -= change;
            weapon.DamageMaximum += (int)(change * 1.05f);

            weapon.MultiplyColor( 0.25f, 0.9f, 0.9f );
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
            return baseItem is Weapon;
        }
    }
}
