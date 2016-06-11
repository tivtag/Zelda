// <copyright file="OfAssassinationSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfAssassinationSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Assassination' suffix adds '1 + 75% item-level' dodge rating
    /// and '1 + 15% item-level' dagger damage to an item.
    /// </summary>
    internal sealed class OfAssassinationSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfAssassination;
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
            // Calculate
            float dodgeRating = (int)(1 + (0.75f * (item.Level / 2.0f)));
            int daggerDamage = (int)(1 + (0.20f * item.Level));
            
            // Dodge rating.
            var equipment = (Equipment)item;
            var dodgeEffect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>(
                ( x ) => x.StatusType == ChanceToStatus.Dodge && x.ManipulationType == StatusManipType.Rating
            );

            if( dodgeEffect != null )
            {
                dodgeEffect.Value += dodgeRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( dodgeRating, StatusManipType.Rating, ChanceToStatus.Dodge )
                );
            }            

            // Dagger damage.
            var daggerEffect = equipment.AdditionalEffectsAura.GetEffect<WeaponDamageTypeBasedEffect>(
                ( x ) => x.WeaponType == WeaponType.Dagger && x.ManipulationType == StatusManipType.Fixed
            );

            if( daggerEffect != null )
            {
                daggerEffect.Value += daggerDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new WeaponDamageTypeBasedEffect( daggerDamage, StatusManipType.Fixed, WeaponType.Dagger )
                );
            }

            equipment.MultiplyColor( 0.4f, 0.9f, 0.8f );
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
            return equipment != null && !equipment.Slot.IsWeaponOrShield() && !equipment.Slot.IsJewelry();
        }
    }
}
