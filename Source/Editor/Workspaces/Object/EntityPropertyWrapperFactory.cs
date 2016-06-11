// <copyright file="EntityPropertyWrapperFactory.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Object.EntityPropertyWrapperFactory class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object
{
    using System;
    using Atom.Design;

    /// <summary>
    /// Is responsible of creating <see cref="IObjectPropertyWrapper"/>s.
    /// This is a sealed class.
    /// </summary>
    internal sealed class EntityPropertyWrapperFactory : ObjectPropertyWrapperFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyWrapperFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public EntityPropertyWrapperFactory( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            RegisterWrapper( new PropertyWrappers.LightPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.MapItemPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.PersistentMapItemPropertyWrapper( serviceProvider ) );

            RegisterWrapper( new PropertyWrappers.DecorationPropertyWrapper() );
            RegisterWrapper( new PropertyWrappers.UseableActionEntityPropertyWrapper( serviceProvider ) );

            RegisterWrapper( new PropertyWrappers.SpawnPointPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.PlayerSpawnPointPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.EnemySpawnPointPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.EntitySpawnPropertyWrapper() );
            RegisterWrapper( new PropertyWrappers.RespawnableEntitySpawnPropertyWrapper() );
            RegisterWrapper( new PropertyWrappers.RazorEntitySpawnPropertyWrapper() );
            
            RegisterWrapper( new PropertyWrappers.BlockTriggerPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.RedBlueBlockTriggerPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.TileBlockPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.UnlockableDoorTileBlockPropertyWrapper( serviceProvider ) );
            
            RegisterWrapper( new PropertyWrappers.FirePlacePropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.PositionalSoundEmitterPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.PositionalShotSoundEmitterPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.AreaSoundEmitterPropertyWrapper( serviceProvider ) );
            RegisterWrapper( new PropertyWrappers.WantedSignPostPropertyWrapper( serviceProvider ) );
        }
    }
}
