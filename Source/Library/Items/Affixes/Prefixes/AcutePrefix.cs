// <copyright file="AcutePrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.AcutePrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The Acute prefix adds '+damage done with damage over time effects' to an Item.
    /// </summary>
    internal sealed class AcutePrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Acute;
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
        public void Apply(Item item, Item baseItem)
        {
            int extraDamage = (int)(5 + item.Level);

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<SpecialDamageDoneEffect>(
                (x) => x.DamageType == SpecialDamageType.DamagerOverTime && x.ManipulationType == StatusManipType.Fixed
            );

            if (effect != null)
            {
                effect.Value += extraDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpecialDamageDoneEffect(extraDamage, StatusManipType.Fixed, SpecialDamageType.DamagerOverTime)
                );
            }

            // Make the item darker:
            equipment.MultiplyColor(0.85f, 0.65f, 0.85f);
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
        public bool IsApplyable(Item baseItem)
        {
            if( baseItem.Level < 25 )
            {
                return false;
            }

            var equipment = baseItem as Equipment;
            
            return equipment != null &&
                (equipment.Slot == EquipmentSlot.Ring ||
                 equipment.Slot == EquipmentSlot.Necklace ||
                 equipment.Slot == EquipmentSlot.Relic ||
                 equipment.Slot == EquipmentSlot.Trinket);
        }
    }
}
