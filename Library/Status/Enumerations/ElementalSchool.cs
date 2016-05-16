// <copyright file="ElementalSchool.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ElementalSchool enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumerates the different schools of elemental magic.
    /// </summary>
    public enum ElementalSchool
    {
        /// <summary>
        /// No element.
        /// </summary>
        None = 0,

        /// <summary>
        /// The light element.
        /// </summary>
        Light,

        /// <summary>
        /// The shadow element.
        /// </summary>
        Shadow,

        /// <summary>
        /// The fire element.
        /// </summary>
        Fire,

        /// <summary>
        /// The water element.
        /// </summary>
        Water,

        /// <summary>
        /// The nature element; includes poisons.
        /// </summary>
        Nature,

        /// <summary>
        /// Represents all elemental schools at the same time.
        /// </summary>
        All = System.Byte.MaxValue
    }
}
