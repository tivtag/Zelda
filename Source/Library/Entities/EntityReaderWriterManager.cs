// <copyright file="EntityReaderWriterManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityReaderWriterManager class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages all <see cref="IEntityReaderWriter"/> the gme knows about.
    /// This class can't be inherited.
    /// </summary>
    public sealed class EntityReaderWriterManager
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityReaderWriterManager"/> class.
        /// </summary>
        /// <remarks>
        /// All <see cref="IEntityReaderWriter"/> that are supported by default are added.
        /// </remarks>
        public EntityReaderWriterManager()
        {
            this.dict = new Dictionary<Type, IEntityReaderWriter>( 19 );
        }
        
        /// <summary>
        /// Adds any known default Object Reader Writers to the <see cref="EntityReaderWriterManager"/>´.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void LoadDefaults( IZeldaServiceProvider serviceProvider )
        {
            this.Register( new Light.ReaderWriter( serviceProvider ) );
            this.Register( new Enemy.ReaderWriter( serviceProvider ) );
            this.Register( new Plant.ReaderWriter( serviceProvider ) );
            this.Register( new FriendlyNpc.ReaderWriter( serviceProvider ) );
            this.Register( new Decoration.ReaderWriter( serviceProvider ) );
            this.Register( new UseableActionEntity.ReaderWriter( serviceProvider ) );

            this.Register( new Spawning.SpawnPoint.ReaderWriter( serviceProvider ) );
            this.Register( new Spawning.PlayerSpawnPoint.ReaderWriter( serviceProvider ) );
            this.Register( new Spawning.EnemySpawnPoint.ReaderWriter( serviceProvider ) );

            this.Register( new Spawning.EntitySpawn.ReaderWriter( serviceProvider ) );
            this.Register( new Spawning.RespawnableEntitySpawn.ReaderWriter( serviceProvider ) );
            this.Register( new Spawning.RazorEntitySpawn.ReaderWriter( serviceProvider ) );

            this.Register( new MapItem.ReaderWriter( serviceProvider ) );
            this.Register( new PersistentMapItem.ReaderWriter( serviceProvider ) );
            this.Register( new BlockTrigger.ReaderWriter( serviceProvider ) );
            this.Register( new RedBlueBlockTrigger.ReaderWriter( serviceProvider ) );
            this.Register( new FirePlace.ReaderWriter( serviceProvider ) );
            this.Register( new TileBlock.ReaderWriter( serviceProvider ) );
            this.Register( new UnlockableDoorTileBlock.ReaderWriter( serviceProvider ) );

            this.Register( new PositionalSoundEmitter.ReaderWriter( serviceProvider ) );
            this.Register( new PositionalShotSoundEmitter.ReaderWriter( serviceProvider ) );
            this.Register( new AreaSoundEmitter.ReaderWriter( serviceProvider ) );
            this.Register( new WantedSignPost.ReaderWriter( serviceProvider ) );

            this.Register( new Trading.Merchant.ReaderWriter( serviceProvider ) );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the <see cref="IEntityReaderWriter"/> that handles 
        /// objects of the given <paramref name="type"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If there is no reader-writer for the given object type.
        /// </exception>
        /// <param name="type">
        /// The type of the object to receive the IEntityReaderWriter for.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityReaderWriter"/> object.
        /// </returns>
        public IEntityReaderWriter Get( Type type )
        {
            IEntityReaderWriter readerWriter;

            if( !this.dict.TryGetValue( type, out readerWriter ) )
            {
                throw new ArgumentException(
                    string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_CouldntFindEntityReaderWriterForTypeX,
                        type.ToString() 
                    ) 
                );
            }

            return readerWriter;
        }

        /// <summary>
        /// Gets the <see cref="IEntityReaderWriter"/> that handles 
        /// objects of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the object to receive the IEntityReaderWriter for.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityReaderWriter"/> object; -or- null if no reader-writer exists for the object type.
        /// </returns>
        public IEntityReaderWriter TryGet( Type type )
        {
            IEntityReaderWriter readerWriter;
            this.dict.TryGetValue( type, out readerWriter );
            return readerWriter;
        }

        /// <summary>
        /// Gets whether the <see cref="EntityReaderWriterManager"/> contains
        /// an <see cref="IEntityReaderWriter"/> that handles the objects
        /// with the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>
        /// true if this ObjectReaderWriterManager has an <see cref="IEntityReaderWriter"/> for the given Type;
        /// otherwise false.
        /// </returns>
        public bool Contains( Type type )
        {
            return this.dict.ContainsKey( type );
        }

        /// <summary>
        /// Registers the given IEntityReaderWriter at this ObjectReaderWriterManager.
        /// </summary>
        /// <param name="readerWriter">
        /// The IEntityReaderWriter to register.
        /// </param>
        private void Register( IEntityReaderWriter readerWriter )
        {
            this.dict.Add( readerWriter.EntityType, readerWriter );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dictionary of <see cref="IEntityReaderWriter"/>s.
        /// </summary>
        private readonly Dictionary<Type, IEntityReaderWriter> dict;

        #endregion
    }
}
