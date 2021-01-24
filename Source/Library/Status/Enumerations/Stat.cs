// <copyright file="Stat.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Stat enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumerates the different (main) Stats used by the Status system.
    /// </summary>
    public enum Stat
    {
        /// <summary> No specific stat. </summary>
        None = 0,

        /// <summary>
        /// Strength increases Melee Attack Power (MAP),
        /// and to a small amount how much damage may be blocked by the shield.
        /// </summary>
        Strength = 1,

        /// <summary> 
        /// Dexterity increases Ranged Attack Power (RAP),
        /// the hit chance, to a small part also Melee Attack Power,
        /// and reduces the cast-time of spell.
        /// </summary>
        Dexterity = 2,

        /// <summary> 
        /// Agility increases Chance To Dodge, Melee and Ranged Attack Speed
        /// and to a small part Armor.
        /// </summary>
        Agility = 3,

        /// <summary> 
        /// Vitality increases Life Points, the Life Point Regeneration
        /// and also increases the chance to resist status changing StatusEffects.
        /// </summary>
        Vitality = 4,

        /// <summary> 
        /// Intelligence increases Magical Attack Power (MagAP), the amount of Mana, and Mana Regeneration.
        /// </summary>
        Intelligence = 5,

        /// <summary> 
        /// Luck increases the chance to get a Critical Attack and the chance to find rare items.
        /// </summary>
        Luck = 6
    }
}
