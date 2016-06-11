// <copyright file="OfCorruptionSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfCorruptionSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Corruption' suffix removes X life/mana and adds Y magic damage to an item.
    /// </summary>
    internal sealed class OfCorruptionSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.OfCorruption;
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
            // Calculate:
            int lifeManaReduction        = -(3 + (2 * item.Level));
            int fixedMagicDamageIncrease = (int)(1 + (1.2f * item.Level));

            // Increase magic damage:
            var equipment = (Equipment)item;
            var magicDamageEffect = equipment.AdditionalEffectsAura.GetEffect<SpellPowerEffect>(
                ( x ) => x.SpellSchool == ElementalSchool.All && x.ManipulationType == StatusManipType.Fixed
            );

            if( magicDamageEffect != null )
            {
                magicDamageEffect.Value += fixedMagicDamageIncrease;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellPowerEffect( fixedMagicDamageIncrease, StatusManipType.Fixed, ElementalSchool.All )
                );
            }

            // Reduce total life:
            var lifeEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaEffect>(
                ( x ) => x.PowerType == LifeMana.Life && x.ManipulationType == StatusManipType.Fixed
            );

            if( lifeEffect != null )
            {
                lifeEffect.Value += lifeManaReduction;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaEffect( lifeManaReduction, StatusManipType.Fixed, LifeMana.Life )
                );
            }

            // Reduce total mana:
            var manaEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaEffect>(
                ( x ) => x.PowerType == LifeMana.Mana && x.ManipulationType == StatusManipType.Fixed
            );

            if( manaEffect != null )
            {
                manaEffect.Value += lifeManaReduction;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaEffect( lifeManaReduction, StatusManipType.Fixed, LifeMana.Mana )
                );
            }

            // Make the item more red:
            equipment.MultiplyColor( 1.0f, 0.9f, 0.8f );
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
            return equipment != null && !equipment.Slot.IsWeaponOrShield();
        }
    }
}
