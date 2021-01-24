// <copyright file="OiledPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OiledPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Feathers' suffix adds 1 + 8% of item-level Movement Speed, Attack Speed and Attack Haste Rating to an Item.
    /// </summary>
    internal sealed class OiledPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Oiled;
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
            int rating = 1 + (int)(item.Level * 0.15f);
            var equipment = (Equipment)item;

            // Movement Speed:
            var movementEffect = equipment.AdditionalEffectsAura.GetEffect<MovementSpeedEffect>( StatusManipType.Rating );

            if( movementEffect != null )
            {
                movementEffect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new MovementSpeedEffect( rating, StatusManipType.Rating )
                );
            }

            // Attack Speed:
            var attackEffect = equipment.AdditionalEffectsAura.GetEffect<AttackSpeedEffect>( effect => effect.ManipulationType == StatusManipType.Rating && effect.AttackType == Attacks.AttackType.All );

            if( attackEffect != null )
            {
                attackEffect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new AttackSpeedEffect( Attacks.AttackType.All, rating, StatusManipType.Rating )
                );
            }

            // Spell Haste:
            var spellHasteEffect = equipment.AdditionalEffectsAura.GetEffect<SpellHasteEffect>( StatusManipType.Rating );

            if( spellHasteEffect != null )
            {
                spellHasteEffect.Value += rating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellHasteEffect( rating, StatusManipType.Rating )
                );
            }

            // Make the item darker:
            equipment.MultiplyColor( 0.65f, 0.15f, 0.15f );
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
            return equipment != null && (equipment.Slot == EquipmentSlot.Ring || equipment.Slot == EquipmentSlot.WeaponHand);
        }
    }
}
