// <copyright file="IWorldStatusProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IWorldStatusProvider interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides a mechanism to receive the current status of the world.
    /// </summary>
    public interface IWorldStatusProvider
    {
        /// <summary>
        /// Gets the <see cref="WorldStatus"/> object;
        /// responsible for holding the overal state of the game world.
        /// </summary>
        WorldStatus WorldStatus
        {
            get;
        }
    }
}
