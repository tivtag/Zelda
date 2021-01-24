// <copyright file="PiercingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.PiercingPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The Piercing prefix adds ',1 + ItemLevel, Piercing Chance Rating' to an bow.
    /// </summary>
    internal sealed class PiercingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Piercing;
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
            int piercingRating = 1 + item.Level;
           
            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>(
                (x) => x.StatusType == ChanceToStatus.Pierce && 
                       x.ManipulationType == StatusManipType.Rating
            );

            if( effect != null )
            {
                effect.Value += piercingRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( piercingRating, StatusManipType.Rating, ChanceToStatus.Pierce )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 0.95f, 0.95f, 0.75f );
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
            return weapon != null && weapon.WeaponType.IsRanged();
        }
    }
}
