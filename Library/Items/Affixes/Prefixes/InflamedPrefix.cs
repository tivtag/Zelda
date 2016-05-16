// <copyright file="InflamedPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.InflamedPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The Inflamed prefix adds '+120% of item-level Fire Spell Power' to a weapon.
    /// </summary>
    internal sealed class InflamedPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.Inflamed;
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
            int extraFireDamage = (int)(1 + (item.Level * 1.2f));

            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<SpellPowerEffect>(
               ( x ) => x.SpellSchool == ElementalSchool.Fire && x.ManipulationType == StatusManipType.Fixed
            );

            // Apply change:
            if( effect != null )
            {
                effect.Value += extraFireDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellPowerEffect( extraFireDamage, StatusManipType.Fixed, ElementalSchool.Fire )
                );
            }

            equipment.MultiplyColor( 1.0f, 0.35f, 0.35f );
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
            if( equipment == null )
                return false;

            return equipment.Slot == EquipmentSlot.WeaponHand ||
                   equipment.Slot == EquipmentSlot.Staff ||
                   equipment.Slot == EquipmentSlot.Relic;
        }
    }
}
