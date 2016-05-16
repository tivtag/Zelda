// <copyright file="PoisonedPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.PoisonedPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;
    
    /// <summary>
    /// The Poisoned prefix adds '+108% of item-level poison damage' to a weapon.
    /// </summary>
    internal sealed class PoisonedPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Poisoned;
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
            int poisonDamageIncrease = 1 + (int)(item.Level * 1.08f);
            var equipment = (Equipment)item;

            // Poison Damage:
            var effect = equipment.AdditionalEffectsAura.GetEffect<SpecialDamageDoneEffect>(
               ( x ) => x.DamageType == SpecialDamageType.Poison && x.ManipulationType == StatusManipType.Fixed
            );

            if( effect != null )
            {
                effect.Value += poisonDamageIncrease;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpecialDamageDoneEffect( poisonDamageIncrease, StatusManipType.Fixed, SpecialDamageType.Poison )
                );
            }

            equipment.MultiplyColor( 0.15f, 1.0f, 0.15f );
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
