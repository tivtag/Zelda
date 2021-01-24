// <copyright file="OfSorcerySuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfSorcerySuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Sorcery' suffix adds '10 + 4 x item-level' mana to an Item.
    /// </summary>
    internal sealed class OfSorcerySuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfSorcery;
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
            int extraMana = 10 + (int)(3.75f * item.Level);

            // Apply:
            var effect = equipment.AdditionalEffectsAura.GetEffect<LifeManaEffect>(
                (x) => (x.ManipulationType == StatusManipType.Fixed && x.PowerType == LifeMana.Mana)
            );

            if( effect != null )
            {
                effect.Value += extraMana;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new LifeManaEffect( extraMana, StatusManipType.Fixed, LifeMana.Mana )
                );
            }

            // Make the item more blue:
            equipment.MultiplyColor( 0.9f, 0.9f, 1.0f );
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
            return baseItem is Equipment;
        }
    }
}
