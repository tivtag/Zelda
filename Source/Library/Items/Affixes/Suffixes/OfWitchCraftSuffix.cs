// <copyright file="OfWitchCraftSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfWitchCraftSuffix class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The 'of Witch Craft' suffix adds 
    /// '1 + 10% of item-level Intelligence' and 
    /// '1 + 85% of item-level poison damage' to an item.
    /// </summary>
    internal sealed class OfWitchCraftSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfWitchCraft;
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
            int intelligenceIncrease = 1 + (int)(item.Level * 0.1f);
            int poisonDamageIncrease = 1 + (int)(item.Level * 0.85f);
            
            // Iitelligence:
            var equipment = (Equipment)item;
            equipment.Intelligence += intelligenceIncrease;

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

            equipment.MultiplyColor( 0.15f, 1.0f, 0.45f );
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
            return equipment != null &&
                (equipment.Slot == EquipmentSlot.Relic || 
                 equipment.Slot == EquipmentSlot.Staff || 
                 equipment.Slot == EquipmentSlot.Gloves);
        }
    }
}
