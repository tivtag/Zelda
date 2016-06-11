// <copyright file="EntityDataStore.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.EntityDataStore class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Atom.Components;
    using Zelda.Entities;

    /// <summary>
    /// Represents a store that stores per-entity data.
    /// </summary>
    /// <remarks>
    /// Multiple different <see cref="IEntityDataStorage"/>s are supported per entity.
    /// </remarks>
    public sealed class EntityDataStore : ISaveable
    {
        /// <summary>
        /// Gets or creates and then updates the TEntityData for the specified IEntity.
        /// </summary>
        /// <typeparam name="TEntityData">
        /// The exact type of the entity data.
        /// </typeparam>
        /// <param name="entity">
        /// The entity for which the data storage should be received.
        /// </param>
        /// <returns>
        /// The requested TEntityData.
        /// </returns>
        public TEntityData GetOrCreateAndUpdate<TEntityData>( IEntity entity )
            where TEntityData : class, IEntityDataStorage, new()
        {
            Contract.Requires<ArgumentNullException>( entity != null );

            var data = this.GetOrCreate<TEntityData>( entity.Name );
            data.ReceiveFrom( entity );

            return data;
        }

        /// <summary>
        /// Gets or creates the TEntityData for the specified IEntity.
        /// </summary>
        /// <typeparam name="TEntityData">
        /// The exact type of the entity data.
        /// </typeparam>
        /// <param name="entity">
        /// The entity for which the data storage should be received.
        /// </param>
        /// <returns>
        /// The requested TEntityData.
        /// </returns>
        public TEntityData GetOrCreate<TEntityData>( IEntity entity )
            where TEntityData : class, IEntityDataStorage, new()
        {
            Contract.Requires<ArgumentNullException>( entity != null );

            return this.GetOrCreate<TEntityData>( entity.Name );;
        }

        /// <summary>
        /// Gets or creates the TEntityData for the entity with given name.
        /// </summary>
        /// <typeparam name="TEntityData">
        /// The exact type of the entity data.
        /// </typeparam>
        /// <param name="entityName">
        /// The name that uniquely identifies the entity.
        /// </param>
        /// <returns>
        /// The requested TEntityData.
        /// </returns>
        public TEntityData GetOrCreate<TEntityData>( string entityName )
            where TEntityData : class, IEntityDataStorage, new()
        {
            Contract.Requires<ArgumentException>( !string.IsNullOrEmpty( entityName ) );

            DataStore store;

            if( !this.dataStores.TryGetValue( entityName, out store ) )
            {
                store = new DataStore();
                this.dataStores.Add( entityName, store );
            }

            return store.GetOrCreate<TEntityData>( GetIdentifier<TEntityData>() );
        }

        /// <summary>
        /// Initializes the specified entities by applying the data that is stored about
        /// them in this EntityDataStore.
        /// </summary>
        /// <param name="entities">
        /// The entities to initialize.
        /// </param>
        public void InitializeEntities( IEnumerable<ZeldaEntity> entities )
        {
            foreach( var entity in entities )
            {
                DataStore store;

                if( this.dataStores.TryGetValue( entity.Name, out store ) )
                {
                    foreach( IEntityDataStorage entry in store.Entries )
                    {
                        entry.ApplyOn( entity );
                    }
                }                
            }
        }

        /// <summary>
        /// Gets the identifier that uniquely identifies the IEntityDataStorage of the specified type.
        /// </summary>
        /// <typeparam name="TEntityData">
        /// The exact type of the IEntityDataStorage.
        /// </typeparam>
        /// <returns>
        /// The string that uniquely identifies the entity.
        /// </returns>
        private static string GetIdentifier<TEntityData>()
            where TEntityData : class, IEntityDataStorage
        {
            return typeof( TEntityData ).Name;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.dataStores.Count );

            foreach( var entry in this.dataStores )
            {
                context.Write( entry.Key );
                entry.Value.Serialize( context );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            int storeCount = context.ReadInt32();

            for( int i = 0; i < storeCount; ++i )
            {
                string entityName = context.ReadString();

                DataStore store = new DataStore();
                store.Deserialize( context );

                this.dataStores.Add( entityName, store );
            }
        }

        /// <summary>
        /// The dictionary that maps entity names onto their individual DataStores.
        /// </summary>
        private readonly Dictionary<string, DataStore> dataStores = new Dictionary<string, DataStore>();
    }
}
