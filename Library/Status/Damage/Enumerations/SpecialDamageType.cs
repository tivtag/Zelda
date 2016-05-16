// <copyright file="SpecialDamageType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.SpecialDamageType enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Enumerates various special kinds of damage.
    /// </summary>
    /// <remarks>
    /// StatusEffects and Containers use actual elements of the enumeration; not combined.
    /// The <see cref="DamageTypeInfo"/> on the other hand might have combined SpecialDamageType flags.
    /// </remarks>
    [System.Flags]
    public enum SpecialDamageType
    {
        /// <summary>
        /// Represents no specific special damage type.
        /// </summary>
        None,

        /// <summary>
        /// Damage Over Time StatusEffects are applied over a longer period of time.
        /// </summary>
        DamagerOverTime = 0x01,

        /// <summary>
        /// Poisons are usually damage-over-time nature damage.
        /// </summary>
        Poison = 0x02,

        /// <summary>
        /// Bleeds are usually damage-over-time physical damage.
        /// </summary>
        Bleed = 0x03
    }
}
