// <copyright file="SceneStatus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.SceneStatus class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;
    using System.Linq;
    using Zelda.Entities;
    using Zelda.Saving.Storage;

    /// <summary>
    /// Provides a storage place for the current status of a ZeldaScene.
    /// </summary>
    public sealed class SceneStatus : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Gets the name of the Scene whose status
        /// is descriped by the SceneStatus.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets an IEnumerable over the persistant objects
        /// that have been removed from the scene. Can be null.
        /// </summary>
        public IEnumerable<string> RemovedPersistantEntities
        {
            get
            {
                return this.removedEntities;
            }
        }

        /// <summary>
        /// Gets the storage place for arabitary data
        /// that is related to this SceneStatus.
        /// </summary>
        public DataStore DataStore
        {
            get
            {
                return this.dataStore;
            }
        }

        /// <summary>
        /// Gets the storage place for per-entity data that is related to this SceneStatus.
        /// </summary>
        public EntityDataStore EntityDataStore
        {
            get
            {
                return this.entityDataStore;
            }
        }

        /// <summary>
        /// Gets the fog-of-war status of the scene descriped by this SceneStatus.
        /// </summary>
        public FogOfWarStatus FogOfWar
        {
            get
            {
                return this.fogOfWar;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneStatus"/> class.
        /// </summary>
        internal SceneStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneStatus"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the Scene.
        /// </param>
        internal SceneStatus( string name )
        {
            this.name = name;
        }

        public void AddEntity( ISceneStatusStorableEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            entity.Storeable.Stored = true;
            this.addedEntities.Add( (ZeldaEntity)entity );
        }

        public bool RemoveEntity( ISceneStatusStorableEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            if( this.addedEntities.Remove( (ZeldaEntity)entity ) )
            {
                entity.Storeable.Stored = false;
                return true;
            }
            else
            {
                return false;
            }        
        }

        /// <summary>
        /// Adds the specified <see cref="IPersistentEntity"/>
        /// to the list of IPersistentEntities that have been removed from the Scene.
        /// </summary>
        /// <param name="persistentEntity">
        /// The IPersistentEntity to remove.
        /// </param>
        internal void RemovePersistantEntity( IPersistentEntity persistentEntity )
        {
            if( persistentEntity == null )
                throw new ArgumentNullException( "persistentEntity" );

            this.RemovePersistantEntity( persistentEntity.Name );
        }

        /// <summary>
        /// Adds the <see cref="IPersistentEntity"/> with the specified name
        /// to the list of IPersistentEntities that have been removed from the Scene.
        /// </summary>
        /// <param name="name">
        /// The name of the IPersistentEntity to remove.
        /// </param>
        internal void RemovePersistantEntity( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            if( this.removedEntities == null )
                this.removedEntities = new List<string>();

            if( !this.removedEntities.Contains( name ) )
                this.removedEntities.Add( name );
        }

        /// <summary>
        /// Gets whether the persistant object with 
        /// the given <paramref name="name"/> is removed from the Scene.
        /// </summary>
        /// <param name="name">
        /// The name of the persistant object.
        /// </param>
        /// <returns>
        /// True if the object has been removed; otherwise false.
        /// </returns>
        public bool HasPersistantEntityBeenRemoved( string name )
        {
            if( removedEntities == null )
                return false;

            return this.removedEntities.Contains( name );
        }

        /// <summary>
        /// Initializes the specified ZeldaScene based on the data stored in this SceneStatus.
        /// </summary>
        public void InitializeScene( ZeldaScene scene )
        {
            Contract.Requires<ArgumentNullException>( scene != null );
            Contract.Requires<ArgumentException>( scene.Name == this.Name );

            this.entityDataStore.InitializeEntities( scene.Entities.ToArray() );

            foreach( ZeldaEntity entity in this.addedEntities )
            {
                if( entity.Scene != null )
                {
                    entity.RemoveFromScene();
                }

                entity.AddToScene( scene );
            }
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
            const int CurrentVersion = 5;
            context.Write( CurrentVersion );
            context.Write( this.name );

            this.SerializeRemovedEntities( context );

            this.fogOfWar.Serialize( context );
            this.dataStore.Serialize( context );
            this.entityDataStore.Serialize( context );
            this.SerializeAddedEntities( context );
        }

        /// <summary>
        /// Serializes the list of entities that have been
        /// removed from the ZeldaScene.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void SerializeRemovedEntities( Zelda.Saving.IZeldaSerializationContext context )
        {
            if( this.removedEntities == null )
            {
                context.Write( 0 );
            }
            else
            {
                context.Write( this.removedEntities.Count );

                foreach( string entityName in this.removedEntities )
                {
                    context.Write( entityName );
                }
            }
        }

        private void SerializeAddedEntities( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            var entitiesStorage = new SceneEntitiesStorage( context.ServiceProvider );
            var toSave = entitiesStorage.FindToSave( this.addedEntities );
            entitiesStorage.WriteHeader( toSave, context );
            entitiesStorage.Write( toSave, context );
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
            const int CurrentVersion = 5;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            this.name = context.ReadString();
            this.DeserializeRemovedEntities( context );

            // New in Version 2
            if( version >= 2 )
            {
                this.fogOfWar.Deserialize( context );
            }

            // New in Version 3
            if( version >= 3 )
            {
                this.dataStore.Deserialize( context );
            }

            // New in Version 4
            if( version >= 4 )
            {
                this.entityDataStore.Deserialize( context );
            }

            // New in Version 5
            if( version >= 5 )
            {
                this.DeserializeAddedEntities( context );
            }
        }

        /// <summary>
        /// Deserializes the list of entities that have been
        /// removed from the ZeldaScene.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void DeserializeRemovedEntities( Zelda.Saving.IZeldaDeserializationContext context )
        {
            int removedPersistentObjectCount = context.ReadInt32();
            if( removedPersistentObjectCount == 0 )
            {
                this.removedEntities = null;
            }
            else
            {
                if( this.removedEntities == null )
                    this.removedEntities = new List<string>( removedPersistentObjectCount );
                else
                    this.removedEntities.Capacity = removedPersistentObjectCount;

                for( int i = 0; i < removedPersistentObjectCount; ++i )
                {
                    this.removedEntities.Add( context.ReadString() );
                }
            }
        }

        private void DeserializeAddedEntities( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( "SceneStatus.AddedEntities" );

            var entitiesStorage = new SceneEntitiesStorage( context.ServiceProvider );
            var sortedEntities = entitiesStorage.ReadHeader( context );
            
            foreach( var type in sortedEntities )
            {
                this.addedEntities.AddRange( type.Value );
            }

            entitiesStorage.Read( sortedEntities, context );
        }

        /// <summary>
        /// The name of the Scene whose status is descriped
        /// by the SceneStatus.
        /// </summary>
        private string name;

        /// <summary>
        /// Stores the names of persistant entities that have been removed from the scene.
        /// </summary>
        private List<string> removedEntities;

        /// <summary>
        /// Lists the additional SceneStatusStorable entities that have been added to the map.
        /// </summary>
        private List<ZeldaEntity> addedEntities = new List<ZeldaEntity>();

        /// <summary>
        /// Provides a storage place for arabitary data
        /// that is related to this SceneStatus.
        /// </summary>
        private readonly DataStore dataStore = new DataStore();

        /// <summary>
        /// Provides a storage place for per-entity data that is related to this SceneStatus.
        /// </summary>
        private readonly EntityDataStore entityDataStore = new EntityDataStore();

        /// <summary>
        /// Stores the fog-of-war status of the scene descripes by this SceneStatus.
        /// </summary>
        private readonly FogOfWarStatus fogOfWar = new FogOfWarStatus();
    }
}