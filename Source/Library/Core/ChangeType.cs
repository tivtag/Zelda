// <copyright file="ChangeType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ChangeType ernumation.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Enumerates the different ways an object can gain or lose focus.
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// The current scene has changed to a different scene.
        /// </summary>
        Away,

        /// <summary>
        /// The current scene has changed to this scene.
        /// </summary>
        To
    }
}
