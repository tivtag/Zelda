// <copyright file="EnragingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.EnragingPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The Enraging prefix adds '+Melee Damage' to an item.
    /// </summary>
    internal sealed class EnragingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Enraging;
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
            int value = 1 + (int)(item.Level * 0.125f);

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<DamageDoneWithSourceEffect>(
                (x) => x.DamageSource == DamageSource.Melee &&
                       x.ManipulationType == StatusManipType.Fixed
             );

            if( effect != null )
            {
                effect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageDoneWithSourceEffect( value, StatusManipType.Fixed, DamageSource.Melee )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 0.85f, 0.95f, 0.70f );
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
            return equipment != null && (!equipment.Slot.IsJewelry() && !equipment.Slot.IsWeaponOrShield());
        }
    }
}
