// <copyright file="ISpawnPoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.ISpawnPoint interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{  
    /// <summary>
    /// A spawn point is a location in the Scene where Entities can spawn.
    /// </summary>
    public interface ISpawnPoint : IZeldaEntity
    {
        /// <summary>
        /// Spawns the given <see cref="ZeldaEntity"/> at this <see cref="ISpawnPoint"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to spawn.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        void Spawn( ZeldaEntity entity );
    }
}
