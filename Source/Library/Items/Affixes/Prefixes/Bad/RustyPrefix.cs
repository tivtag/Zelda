// <copyright file="RustyPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.RustyPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Rusty prefix removes 25% armor of an equipment, or 10% min/max damage of a weapon.
    /// </summary>
    internal sealed class RustyPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Rusty;
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
            var weapon = item as Weapon;
            if( weapon != null )
            {
                weapon.DamageMinimum -= (int)(1 + (0.1f * weapon.DamageMinimum));
                weapon.DamageMaximum -= (int)(1 + (0.1f * weapon.DamageMaximum));
            }
            else
            {
                var equipment = (Equipment)item;
                equipment.Armor -= (int)(1 + (0.25f * equipment.Armor));
            }

            // Make the item more red:
            item.MultiplyColor( 1.0f, 0.85f, 0.75f );
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
                return !weapon.WeaponType.IsRanged();

            var equipment = baseItem as Equipment;
            if( equipment != null )
                return equipment.Armor > 0;
            return false;
        }
    }
}
