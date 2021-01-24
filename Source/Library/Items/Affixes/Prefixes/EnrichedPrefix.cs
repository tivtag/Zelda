// <copyright file="EnrichedPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.EnrichedPrefix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'Enriched' prefix adds LUCK and SPELL PENETRATION RATING to
    /// an item.
    /// </summary>
    public sealed class EnrichedPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.Enriched;
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
            int spellPenRating = (int)(1 + (0.35f * item.Level));
            int luck = (int)(1 + (0.11f * item.Level));
            
            // Apply:
            var equipment = (Equipment)item;
            equipment.Luck += luck;
            
            var spellPenetrationEffect = equipment.AdditionalEffectsAura.GetEffect<SpellPenetrationEffect>( StatusManipType.Rating );

            if( spellPenetrationEffect != null )
            {
                spellPenetrationEffect.Value += spellPenRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new SpellPenetrationEffect( spellPenRating, StatusManipType.Rating )
                );
            }

            // Make the item more golden:
            equipment.MultiplyColor( 0.75f, 0.843f, 0.2f );
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

            return equipment.Slot != EquipmentSlot.Cloak &&
                   !equipment.SpecialType.IsMetalOrChain();
        }
    }
}
