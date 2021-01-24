// <copyright file="OfRageSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfRageSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Rage' suffix adds '1 + 20% of item-level strength' and
    /// removes '1 + item-level / 3' hit rating.
    /// </summary>
    internal sealed class OfRageSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfRage;
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

            // Apply stat change:
            equipment.Strength += (int)(1 + (0.20f * item.Level));            
            ApplyMissRating( (1 + (item.Level / 3)), equipment );

            // Make the item more red/blue:
            equipment.MultiplyColor( 0.8f, 0.75f, 0.9f );
        }

        /// <summary>
        /// Applies the miss rating effect to the given Item.
        /// </summary>
        /// <param name="missRating">The rating to apply.</param>
        /// <param name="equipment">The item to modify.</param>
        private static void ApplyMissRating( int missRating, Equipment equipment )
        {
            var hitEffect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>(
                ( x ) => x.StatusType == ChanceToStatus.Miss && x.ManipulationType == StatusManipType.Rating
            );

            if( hitEffect != null )
            {
                hitEffect.Value += missRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( missRating, StatusManipType.Rating, ChanceToStatus.Miss )
                );
            }
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

            return equipment.Slot == EquipmentSlot.Belt ||
                   equipment.Slot == EquipmentSlot.Chest ||
                   equipment.Slot == EquipmentSlot.Head ||
                   equipment.Slot == EquipmentSlot.Boots;           
        }
    }
}
