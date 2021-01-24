// <copyright file="OfCourageSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfCourageSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Courage' suffix adds Life and Life Regeneration to an Item.
    /// </summary>
    /// <remarks>
    /// This suffix is based on the idea that the goddess of Courage, Farore,
    /// is the source of all life that exists within the world of Hyrule.
    /// </remarks>
    internal sealed class OfCourageSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfCourage;
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

            // Increase Life:
            int value = 10 + (3 * item.Level);

            var lifeEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaEffect>(
                ( x ) => x.PowerType == LifeMana.Life && x.ManipulationType == StatusManipType.Fixed
            );

            if( lifeEffect != null )
            {
                lifeEffect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaEffect( value, StatusManipType.Fixed, LifeMana.Life )
                );
            }

            // Increase life regeneration:
            int regenValue = 1 + (int)(item.Level * 0.1f);

            var lifeRegenEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaRegenEffect>(
                ( x ) => x.PowerType == LifeMana.Life && x.ManipulationType == StatusManipType.Fixed
            );

            if( lifeRegenEffect != null )
            {
                lifeRegenEffect.Value += regenValue;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaRegenEffect( regenValue, StatusManipType.Fixed, LifeMana.Life )
                );
            }

            // Make the item more green:
            equipment.MultiplyColor( 0.85f, 1.0f, 0.65f );
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
