// <copyright file="ScintillantPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.ScintillantPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The Scintillant prefix adds Spell Power to an item.
    /// </summary>
    internal sealed class ScintillantPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Scintillant; 
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
            int spellPower = (int)(1 + (0.1f * item.Level) + ((item.Level / 10) * (item.Level / 20.0)));

            // Apply:
            var equipment = (Equipment)item;
            var effect = equipment.AdditionalEffectsAura.GetEffect<SpellPowerEffect>(
                ( x ) => x.SpellSchool == ElementalSchool.All && x.ManipulationType == StatusManipType.Fixed
            );

            if( effect != null )
            {
                effect.Value += spellPower;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellPowerEffect( spellPower, StatusManipType.Fixed, ElementalSchool.All )
                );
            }

            // Make the item more golden:
            equipment.MultiplyColor( 1.0f, 0.843f, 0.2f );
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

            return equipment.Slot.IsWeaponOrShield() ||
                   equipment.Slot == EquipmentSlot.Head ||
                   equipment.Slot == EquipmentSlot.Gloves;
        }
    }
}
