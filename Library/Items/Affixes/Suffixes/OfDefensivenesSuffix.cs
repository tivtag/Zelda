// <copyright file="OfDefensivenesSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfDefensivenesSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;
    
    /// <summary>
    /// The 'of Defensivenes' suffix adds '-2.0% damage done and -2.0% damage taken' to an item.
    /// </summary>
    internal sealed class OfDefensivenesSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfDefensiveness;
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
            const float Value = -2.0f;
            var equipment = (Equipment)item;
            
            // Apply damage done reduction:
            var damageDoneEffect = equipment.AdditionalEffectsAura.GetEffect<DamageDoneWithSourceEffect>(
                ( x ) => x.DamageSource == DamageSource.All && x.ManipulationType == StatusManipType.Percental
            );

            if( damageDoneEffect != null )
            {
                damageDoneEffect.Value += Value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageDoneWithSourceEffect() {
                        DamageSource = DamageSource.All,
                        ManipulationType = StatusManipType.Percental,
                        Value = Value 
                    }
                );
            }

            // Apply damage done increase:
            var damageTakenEffect = equipment.AdditionalEffectsAura.GetEffect<DamageTakenFromSourceEffect>(
                ( x ) => x.DamageSource == DamageSource.All && x.ManipulationType == StatusManipType.Percental
            );

            if( damageTakenEffect != null )
            {
                damageTakenEffect.Value += Value;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageTakenFromSourceEffect() {
                        DamageSource = DamageSource.All,
                        ManipulationType = StatusManipType.Percental,
                        Value = Value
                    }
                );
            }

            // Make the item more greenish-ish:
            equipment.MultiplyColor( 0.25f, 1.0f, 0.25f );
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
