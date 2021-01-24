// <copyright file="OfTheTitansSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfTheTitansSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;
    
    /// <summary>
    /// The 'of the Titans' suffix adds '2 + 20% of item-level' pushing power to an Item.
    /// </summary>
    /// <remarks>
    /// This suffix is capped at level 40.
    /// </remarks>
    internal sealed class OfTheTitansSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfTheTitans; 
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
            int extraPushPower = 2 + (int)(0.2f * item.Level);
            if( extraPushPower > 10 )
                extraPushPower = 10;

            // Apply:
            var effect = equipment.AdditionalEffectsAura.GetEffect<PushingForceEffect>( StatusManipType.Fixed );

            if( effect != null )
            {
                effect.Value += extraPushPower;
            }
            else
            {
                equipment.AdditionalEffectsAura.AddEffect(
                    new PushingForceEffect( extraPushPower, StatusManipType.Fixed )
                );
            }

            // Make the item more red:
            equipment.MultiplyColor( 0.75f, 0.5f, 0.65f );
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
                (equipment.Slot == EquipmentSlot.Gloves || 
                 equipment.Slot == EquipmentSlot.Chest || 
                 equipment.Slot == EquipmentSlot.Boots);
        }
    }
}
