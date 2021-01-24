// <copyright file="OfRecoverySuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfRecoverySuffix class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Recovery' suffix adds '1 + 10% of item-level life/mana regeneration' to an Item.
    /// </summary>
    internal sealed class OfRecoverySuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfRecovery;
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
            int value = 1 + (int)(item.Level * 0.1f);
            var equipment = (Equipment)item;

            var lifeManaEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaRegenEffect>(
                ( x ) => x.PowerType == LifeMana.Both && x.ManipulationType == StatusManipType.Fixed
            );

            if( lifeManaEffect != null )
            {
                lifeManaEffect.Value += value;
            }
            else
            {
                var lifeEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaRegenEffect>(
                    ( x ) => x.PowerType == LifeMana.Life && x.ManipulationType == StatusManipType.Fixed
                );

                var manaEffect = equipment.AdditionalEffectsAura.GetEffect<LifeManaRegenEffect>(
                    ( x ) => x.PowerType == LifeMana.Mana && x.ManipulationType == StatusManipType.Fixed
                );

                if( lifeEffect == null && manaEffect == null )
                {
                    // Increase life and mana regeneration:
                    equipment.AdditionalEffectsAura.AddEffect(
                        new LifeManaRegenEffect( value, StatusManipType.Fixed, LifeMana.Both )
                    );
                }
                else
                {
                    // Increase life regeneration individually:
                    if( lifeEffect != null )
                    {
                        lifeEffect.Value += value;
                    }
                    else
                    {
                        equipment.AdditionalEffectsAura.AddEffect(
                            new LifeManaRegenEffect( value, StatusManipType.Fixed, LifeMana.Life )
                        );
                    }

                    // Increase mana regeneration individually:
                    if( manaEffect != null )
                    {
                        manaEffect.Value += value;
                    }
                    else
                    {
                        equipment.AdditionalEffectsAura.AddEffect(
                            new LifeManaRegenEffect( value, StatusManipType.Fixed, LifeMana.Mana )
                        );
                    }
                }
            }

            // Make the item more yellow:
            equipment.MultiplyColor( 0.85f, 0.85f, 0.5f );
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
