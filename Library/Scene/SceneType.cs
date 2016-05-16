// <copyright file="SceneType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.SceneType enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    /// <summary>
    /// Enumerates the different types of <see cref="ZeldaScene"/>s.
    /// </summary>
    public enum SceneType
    {
        /// <summary>
        /// An outdoor scene has an active day/night cycle and weather system.
        /// </summary>
        Outdoor,

        /// <summary>
        /// An indoor-ambient scene has a static ambient color and no active weather system.
        /// </summary>
        IndoorAmbient,

        /// <summary>
        /// An outdoor-ambient scene has a static ambient color and an active weather system.
        /// </summary>
        OutdoorAmbient,

        /// <summary>
        /// An indoor scene has an active day/night cycle and no active weather system.
        /// </summary>
        Indoor
    }
}
