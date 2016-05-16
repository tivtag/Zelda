// <copyright file="AttackReceiveType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.AttackReceiveType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks
{
    /// <summary>
    /// Enumerates the different possible types
    /// of how an attack can be received.
    /// </summary>
    public enum AttackReceiveType
    {
        /// <summary>
        /// Indicates that no specific type has been selected.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the attack was a direct hit.
        /// </summary>
        Hit,

        /// <summary>
        /// Indicates that the attack was a critical hit, and as such did extra damage.
        /// </summary>
        Crit,

        /// <summary> 
        /// Indicates that the melee or ranged attack has missed its target.
        /// </summary>
        Miss,

        /// <summary> 
        /// Indicates that the melee or ranged attack was dodged.
        /// </summary>
        Dodge,

        /// <summary> 
        /// Indicates that the melee attack was parried.
        /// </summary>
        Parry,

        /// <summary> 
        /// Indicates that the attack was resisted.
        /// </summary>
        Resisted,

        /// <summary> 
        /// Indicates that the attack was partialy resisted.
        /// </summary>
        PartialResisted
    }
}
