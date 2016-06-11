// <copyright file="WeaponType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.WeaponType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the different types of weapons.
    /// </summary>
    public enum WeaponType
    {
        /// <summary>
        /// Indicates that no specific type has been set.
        /// </summary>
        None,

        /// <summary>
        /// An one-handed axe. The favourite weapon of Berserkers.
        /// </summary>
        OneHandedAxe,

        /// <summary>
        /// An one-handed mace. The favourite weapon of Monks.
        /// </summary>
        OneHandedMace,

        /// <summary>
        /// An one-handed sword. The favourite weapon of Knights.
        /// </summary>
        OneHandedSword,

        /// <summary> 
        /// A two-handed axe. The favourite weapon of Berserkers.
        /// </summary>
        TwoHandedAxe,

        /// <summary> 
        /// A two-handed mace. The favourite weapon of Paladins.
        /// </summary>
        TwoHandedMace,

        /// <summary>
        /// A two-handed sword. The favourite weapon of Knights.
        /// </summary>
        TwoHandedSword,

        /// <summary>
        /// A dagger. The favourite weapon of Assasins and Hunters.
        /// </summary>
        Dagger,

        /// <summary>
        /// A staff is hold with two hands. The favourite weapon of Mages and Wizards.
        /// </summary>
        Staff,

        /// <summary>
        /// A rod is hold with one hand. The favourite weapon of Priests. 
        /// </summary>
        Rod,

        /// <summary>
        /// A bow. This is a ranged weapon. The favourite weapon of Archers/Hunters.
        /// </summary>
        Bow,

        /// <summary>
        /// A crossbow. This is a ranged weapon. The favourite ranged weapon of Knights.
        /// </summary>
        Crossbow
    }
}
