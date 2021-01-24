// <copyright file="OfBerserkerRageSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfBerserkerRageSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Berserker Rage' suffix adds '1 + 15% of item-level' damage done with Two-handed Swords.
    /// </summary>
    internal sealed class OfBerserkerRageSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            { 
                return AffixResources.OfBerserkerRage; 
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
            int extraDamage = 1 + (int)(item.Level * 0.25f);

            // 2H Weapon Damage
            var effect = equipment.AdditionalEffectsAura.GetEffect<WeaponDamageTypeBasedEffect>(
                ( x ) => x.WeaponType == WeaponType.TwoHandedSword && x.ManipulationType == StatusManipType.Fixed
            );

            if( effect != null )
            {
                effect.Value += extraDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new WeaponDamageTypeBasedEffect( extraDamage, StatusManipType.Fixed, WeaponType.TwoHandedSword )
                );
            }

            // Make the item more blue-ish:
            equipment.MultiplyColor( 0.85f, 0.9f, 0.55f );
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
                (equipment.Slot == EquipmentSlot.Head || 
                 equipment.Slot == EquipmentSlot.Chest ||
                 equipment.Slot == EquipmentSlot.Boots);
        }
    }
}
