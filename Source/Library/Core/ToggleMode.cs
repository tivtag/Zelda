// <copyright file="ToggleMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.ToggleMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core
{
    /// <summary>
    /// Enumerates the different ways something can be toggled.
    /// </summary>
    public enum ToggleMode
    {
        /// <summary>
        /// Toggles from on to off, or from off to on.
        /// </summary>
        Invert = 0,

        /// <summary>
        /// Toggles on.
        /// </summary>
        On,

        /// <summary>
        /// Toggles off.
        /// </summary>
        Off
    }
}
