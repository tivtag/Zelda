// <copyright file="OfImmediacySuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfImmediacySuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Immediacy' suffix adds '+X Armor Ignore and +Y Hit Rating' to an item.
    /// </summary>
    internal sealed class OfImmediacySuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            { 
                return AffixResources.OfImmediacy; 
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
            int armorIgnore = (int)(2.75f * item.Level);
            int hitRating   = -(2 + (item.Level / 3));

            ApplyArmorIgnore( armorIgnore, item );
            ApplyHitRating( hitRating, item );

            // Make the item darker:
            item.MultiplyColor( 0.55f, 0.25f, 0.75f );
        }

        /// <summary>
        /// Applies the armor ignore effect to the given Item.
        /// </summary>
        /// <param name="armorIgnore">
        /// The amount of armor ignore to apply.
        /// </param>
        /// <param name="item">
        /// The item to modify.
        /// </param>
        private static void ApplyArmorIgnore( int armorIgnore, Item item )
        {
            var equipment = (Equipment)item;
            var armorIgnoreEffect = equipment.AdditionalEffectsAura.GetEffect<ArmorIgnoreEffect>( StatusManipType.Fixed );

            if( armorIgnoreEffect != null )
            {
                armorIgnoreEffect.Value += armorIgnore;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ArmorIgnoreEffect( armorIgnore, StatusManipType.Fixed )
                );
            }
        }

        /// <summary>
        /// Applies the hit rating effect to the given Item.
        /// </summary>
        /// <param name="hitRating">
        /// The amount of hit rating to apply.
        /// </param>
        /// <param name="item">The item to modify.</param>
        private static void ApplyHitRating( int hitRating, Item item )
        {
            var equipment = (Equipment)item;
            var hitEffect = equipment.AdditionalEffectsAura.GetEffect<ChanceToStatusEffect>(
                ( x ) => x.StatusType == ChanceToStatus.Miss && x.ManipulationType == StatusManipType.Rating
            );

            if( hitEffect != null )
            {
                hitEffect.Value += hitRating;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new ChanceToStatusEffect( hitRating, StatusManipType.Rating, ChanceToStatus.Miss )
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
            if( baseItem.Level < 18 )
                return false;

            if( baseItem is Weapon )
                return true;

            var equipment = baseItem as Equipment;
            if( equipment == null )
                return false;
            
            return equipment.Slot == EquipmentSlot.Gloves || ItemExtensions.IsOffensiveShieldHand( equipment );
        }
    }
}
