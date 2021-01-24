// <copyright file="OfTheHolySpiritSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfTheHolySpiritSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The 'of the Holy Spirit' suffix adds 
    /// '1 + 15% of item-level Damage done against Undead' and 
    /// '1 + 10% of item-level Mana Regeneration' to an item.
    /// </summary>
    internal sealed class OfTheHolySpiritSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfTheHolySpirit;
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
            int extraDamage = 1 + (int)(item.Level * 0.15f);
            int regenValue  = 1 + (int)(item.Level * 0.1f);

            // Damage Against Undeads
            var effect = equipment.AdditionalEffectsAura.GetEffect<DamageDoneAgainstRaceEffect>(
                ( x ) => x.Race == RaceType.Undead && x.ManipulationType == StatusManipType.Fixed
            );

            if( effect != null )
            {
                effect.Value += extraDamage;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new DamageDoneAgainstRaceEffect( RaceType.Undead, extraDamage )
                );
            }

            // Increase mana regeneration:
            var lifeRegenEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaRegenEffect>(
                ( x ) => x.PowerType == LifeMana.Mana && x.ManipulationType == StatusManipType.Fixed
            );

            if( lifeRegenEffect != null )
            {
                lifeRegenEffect.Value += regenValue;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaRegenEffect( regenValue, StatusManipType.Fixed, LifeMana.Mana )
                );
            }

            // Make the item more blue-ish:
            equipment.MultiplyColor( 1.0f, 1.0f, 0.55f );
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
                 equipment.Slot == EquipmentSlot.Staff ||
                 equipment.Slot == EquipmentSlot.Chest ||
                 equipment.Slot == EquipmentSlot.Ring ||
                 equipment.Slot == EquipmentSlot.Necklace );
        }
    }
}
