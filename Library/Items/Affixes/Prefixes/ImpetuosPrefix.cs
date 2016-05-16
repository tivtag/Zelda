// <copyright file="ImpetuosPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.ImpetuosPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'Impetuous' prefix adds '3 + 55% item-level' attack speed and cast haste rating to an Item.
    /// </summary>
    internal sealed class ImpetuousPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Impetuous; 
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
            int rating = 3 + (int)(0.55f * item.Level);

            // Apply:
            var attackEffect = equipment.AdditionalEffectsAura.GetEffect<AttackSpeedEffect>(
                x => x.ManipulationType == StatusManipType.Rating && 
                     x.AttackType == Zelda.Attacks.AttackType.All
            );

            if( attackEffect != null )
            {
                attackEffect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new AttackSpeedEffect( Zelda.Attacks.AttackType.All, rating, StatusManipType.Rating )
                );
            }

            var hasteEffect = equipment.AdditionalEffectsAura.GetEffect<SpellHasteEffect>( StatusManipType.Rating );

            if (hasteEffect != null)
            {
                hasteEffect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellHasteEffect(rating, StatusManipType.Rating)
                );
            }

            // Make the item more blue:
            equipment.MultiplyColor( 0.5f, 0.5f, 1.0f );
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
                (equipment.Slot == EquipmentSlot.Ring ||
                 equipment.Slot == EquipmentSlot.Trinket ||
                 equipment.Slot == EquipmentSlot.Relic ||
                 equipment.Slot == EquipmentSlot.ShieldHand ||
                 equipment.Slot == EquipmentSlot.Gloves ||
                 equipment.Slot == EquipmentSlot.Cloak || 
                 equipment.Slot == EquipmentSlot.WeaponHand);
        }
    }
}
