// <copyright file="OfKingsSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfKingsSuffix class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// The 'of Kinds' suffix increases every stat by 10%, and adds 1 to the highest stat.
    /// </summary>
    internal sealed class OfKingsSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return AffixResources.OfKings; 
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

            Stat stat = GetGreatestStat( equipment );
            equipment.SetStat( 2 + equipment.GetStat( stat ), stat );

            // Apply stat change:
            const float Multiplier = 0.1f;
            equipment.Strength     += (int)Math.Round( equipment.Strength     * Multiplier );
            equipment.Dexterity    += (int)Math.Round( equipment.Dexterity    * Multiplier );
            equipment.Agility      += (int)Math.Round( equipment.Agility      * Multiplier );
            equipment.Vitality     += (int)Math.Round( equipment.Vitality     * Multiplier );
            equipment.Intelligence += (int)Math.Round( equipment.Intelligence * Multiplier );
            equipment.Luck         += (int)Math.Round( equipment.Luck         * Multiplier );
                        
            // Make the item more golden:
            equipment.MultiplyColor( 1.0f, 0.843f, 0.0f );
        }

        /// <summary>
        /// Gets the greatest Stat of the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment.
        /// </param>
        /// <returns>
        /// The greatest Stat.
        /// </returns>
        private static Stat GetGreatestStat( Equipment equipment )
        {
            Stat stat = Stat.Luck;
            int value = equipment.Luck;
            
            if( equipment.Strength > value )
            {
                stat  = Stat.Strength;
                value = equipment.Strength;
            }

            if( equipment.Dexterity > value )
            {
                stat  = Stat.Dexterity;
                value = equipment.Dexterity;
            }

            if( equipment.Agility > value )
            {
                stat  = Stat.Agility;
                value = equipment.Agility;
            }

            if( equipment.Vitality > value )
            {
                stat  = Stat.Vitality;
                value = equipment.Vitality;
            }

            if( equipment.Intelligence > value )
            {
                stat  = Stat.Intelligence;

                // Not needed.
                // value = equipment.Intelligence;
            }

            return stat;
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
                (equipment.Slot != EquipmentSlot.WeaponHand && 
                equipment.Slot != EquipmentSlot.Ranged &&
                equipment.Slot != EquipmentSlot.Gloves);
        }
    }
}
