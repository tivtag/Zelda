// <copyright file="ISceneProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ISceneProvider interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    /// <summary>
    /// Provides a mechanism that allows one
    /// to receive a ZeldaScene instance.
    /// </summary>
    public interface ISceneProvider
    {
        /// <summary>
        /// Gets the ZeldaScene that this ISceneProvider provides.
        /// </summary>
        ZeldaScene Scene
        {
            get;
        }
    }
}
