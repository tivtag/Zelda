// <copyright file="EdgedPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.EdgedPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Edged prefix adds 10% to the minumum damage of a weapon.
    /// </summary>
    internal sealed class EdgedPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Edged;
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
            weapon.DamageMinimum = (int)(weapon.DamageMinimum * 1.1f);
            if( weapon.DamageMaximum < weapon.DamageMinimum )
                weapon.DamageMaximum = weapon.DamageMinimum;

            weapon.MultiplyColor( 0.9f, 0.9f, 0.9f );
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
            var weapon = baseItem as Weapon;

            if( weapon != null )
            {
                return weapon.WeaponType.IsRanged();
            }

            return false;
        }
    }
}
