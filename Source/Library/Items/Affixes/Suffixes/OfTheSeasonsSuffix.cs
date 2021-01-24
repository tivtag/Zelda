// <copyright file="OfTheSeasonsSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfTheSeasonsSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of the Seasons' suffix adds '2.5% life and mana potion effectiviness' to an item.
    /// </summary>
    internal sealed class OfTheSeasonsSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfTheSeasons; 
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
            const float EffectivinessIncrease = 2.5f;

            // Life Potion Effectiveness
            var lifePotionEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaPotionEffectivenessEffect>(
                ( x ) => x.PowerType == LifeMana.Life
            );

            if( lifePotionEffect != null )
            {
                lifePotionEffect.Value += EffectivinessIncrease;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaPotionEffectivenessEffect( EffectivinessIncrease, LifeMana.Life )
                );
            }

            // Mana Potion Effectiveness
            var manaPotionEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaPotionEffectivenessEffect>(
               ( x ) => x.PowerType == LifeMana.Mana
            );

            if( manaPotionEffect != null )
            {
                manaPotionEffect.Value += EffectivinessIncrease;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaPotionEffectivenessEffect( EffectivinessIncrease,  LifeMana.Mana )
                );
            }

            // Make the item more red/blue-ish:
            equipment.MultiplyColor( 1.0f, 0.75f, 1.00f );
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
            return equipment != null && equipment.Slot.IsJewelry();
        }
    }
}
