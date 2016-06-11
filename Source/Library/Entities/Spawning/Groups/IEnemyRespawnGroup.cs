// <copyright file="IEnemyRespawnGroup.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.IEnemyRespawnGroup interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System.ComponentModel;
    using Zelda.Entities;

    /// <summary>
    /// Represents a group of enemies that implements respawning
    /// them when required.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IEnemyRespawnGroup : IZeldaUpdateable, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Creates and setups this IEnemyRespawnGroup.
        /// </summary>
        /// <param name="spawnPoint">
        /// The <see cref="ISpawnPoint"/> in which the Enemies of this IEnemyRespawnGroup should be spawned in.
        /// </param>
        /// <param name="templateManager">
        /// The <see cref="EntityTemplateManager"/> that should be used to load the Enemy template.
        /// </param>
        void Create( ISpawnPoint spawnPoint, EntityTemplateManager templateManager );
    }
}
