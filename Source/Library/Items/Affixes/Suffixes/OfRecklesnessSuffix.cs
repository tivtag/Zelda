// <copyright file="OfRecklesnessSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfRecklesnessSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The 'of Recklesness' suffix adds '2.0% damage done and 2.0% damage taken' to an item.
    /// </summary>
    internal sealed class OfRecklesnessSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfRecklessness;
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
            float value = 2.0f;

            // Apply damage done increase:
            var damageDoneEffect = equipment.AdditionalEffectsAura.GetEffect<DamageDoneWithSchoolEffect>(
                ( x ) => x.DamageSchool == DamageSchool.All && x.ManipulationType == StatusManipType.Percental
            );

            if( damageDoneEffect != null )
            {
                damageDoneEffect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageDoneWithSchoolEffect() {
                        DamageSchool = DamageSchool.All,
                        ManipulationType = StatusManipType.Percental,
                        Value = value
                    }
                );
            }

            // Apply damage taken increase:
            var damageTakenEffect = equipment.AdditionalEffectsAura.GetEffect<DamageTakenFromSchoolEffect>(
                ( x ) => x.DamageSchool == DamageSchool.All && x.ManipulationType == StatusManipType.Percental
            );

            if( damageTakenEffect != null )
            {
                damageTakenEffect.Value += value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageTakenFromSchoolEffect() { 
                        DamageSchool = DamageSchool.All,
                        ManipulationType = StatusManipType.Percental,
                        Value = value
                    }
                );
            }

            // Make the item more red-ish:
            equipment.MultiplyColor( 1.0f, 0.25f, 0.25f );
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
            return equipment != null && !equipment.Slot.IsJewelry();
        }
    }
}
