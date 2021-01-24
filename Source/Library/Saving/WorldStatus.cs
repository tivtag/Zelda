// <copyright file="WorldStatus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.WorldStatus class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    using System;
    using System.Collections.Generic;
    using Zelda.Saving.Storage;

    /// <summary>
    /// Descripes the full status of the game world.
    /// This class can't be inherited.
    /// </summary>
    public sealed class WorldStatus : IStateSaveable, IZeldaUpdateable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a store for arabitary world-wide data.
        /// </summary>
        public DataStore DataStore
        {
            get
            {
                return this.dataStore;
            }
        }

        /// <summary>
        /// Gets a store for arabitary world-wide data that is not persisted.
        /// </summary>
        public DataStore TempDataStore
        {
            get
            {
                return this.tempDataStore;
            }
        }

        /// <summary>
        /// Gets the object that stores the ingame date and time.
        /// </summary>
        public IngameDateTime IngameDateTime
        {
            get 
            { 
                return this.ingameDateTime;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaScenesCache"/> that provides a mechanism to cache previous <see cref="ZeldaScene"/>s.
        /// </summary>
        public ZeldaScenesCache ScenesCache
        {
            get
            { 
                return this.scenesCache;
            }
        }

        /// <summary>
        /// Gets the map of ITimers that are world-wide active.
        /// </summary>
        public Zelda.Timing.TimerMap WorldWideTimers
        {
            get
            {
                return this.worldWideTimers;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this WorldStatus.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.worldWideTimers.Update( updateContext );
        }

        /// <summary>
        /// Tries to get the <see cref="SceneStatus"/>
        /// of the scene with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the scene to get.
        /// </param>
        /// <returns>
        /// The status of the scene with the given name; or null.
        /// </returns>
        public SceneStatus GetSceneStatus( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            for( int i = 0; i < scenes.Count; ++i )
            {
                var status = scenes[i];
                if( name.Equals( status.Name, StringComparison.Ordinal ) )
                {
                    return status;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the specified <see cref="SceneStatus"/>
        /// to the list of known SceneStatuses of the <see cref="WorldStatus"/>.
        /// </summary>
        /// <param name="status">
        /// The SceneStatus to add.
        /// </param>
        public void AddSceneStatus( SceneStatus status )
        {
            if( this.ContainsSceneStatus( status ) )
                throw new ArgumentException( Resources.Error_SceneStatusAlreadyExists, "status" );
            
            this.scenes.Add( status );
        }
        
        /// <summary>
        /// Gets a value indicating whether this WorldStatus contains the specified SceneStatus. 
        /// </summary>
        /// <param name="status">
        /// The SceneStatus to search for.
        /// </param>
        /// <returns>
        /// true if it contains the specified SceneStatus or another SceneStatus for the same scene;
        /// otherwise false.
        /// </returns>
        private bool ContainsSceneStatus( SceneStatus status )
        {
            foreach( SceneStatus sceneStatus in this.scenes )
            {
                if( status == sceneStatus || status.Name.Equals( sceneStatus.Name, StringComparison.Ordinal ) )
                {
                    return true;
                }
            }

            return false;
        }

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe the state of this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 3;
            context.Write( CurrentVersion );

            DateTime dateTime = this.ingameDateTime.Current;
            context.Write( dateTime.Year );
            context.Write( dateTime.Month );
            context.Write( dateTime.Day );
            context.Write( dateTime.Hour );
            context.Write( dateTime.Minute );
            context.Write( dateTime.Second );

            context.Write( this.scenes.Count );

            foreach( SceneStatus scene in this.scenes )
            {
                scene.Serialize( context );
            }

            this.worldWideTimers.Serialize( context ); // New in V2.
            this.dataStore.Serialize( context ); // New in V3.
        }

        /// <summary>
        /// Deserializes the current state of the World.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 3;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            // Read Game Time.
            int year   = context.ReadInt32();
            int month  = context.ReadInt32();
            int day    = context.ReadInt32();
            int hour   = context.ReadInt32();
            int minute = context.ReadInt32();
            int second = context.ReadInt32();
            ingameDateTime.Current = new DateTime( year, month, day, hour, minute, second );

            // Read SceneStatuses.
            int sceneCount = context.ReadInt32();

            this.scenes.Clear();
            this.scenes.Capacity = sceneCount;

            for( int i = 0; i < sceneCount; ++i )
            {
                SceneStatus scene = new SceneStatus();

                scene.Deserialize( context );
                this.scenes.Add( scene );
            }

            if( version >= 2 )
            {
                this.worldWideTimers.Deserialize( context );
            }

            if( version >= 3 )
            {
                this.dataStore.Deserialize( context );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents a store for arabitary world-wide data.
        /// </summary>
        private readonly DataStore dataStore = new DataStore();
        
        /// <summary>
        /// Represents a store for arabitary world-wide data that is not persisted.
        /// </summary>
        private readonly DataStore tempDataStore = new DataStore();

        /// <summary>
        /// Stores the ingame date and time.
        /// </summary>
        private readonly IngameDateTime ingameDateTime = new IngameDateTime();

        /// <summary>
        /// Stores the status of the scenes in the world known to the player.
        /// </summary>
        private readonly List<SceneStatus> scenes = new List<SceneStatus>();

        /// <summary>
        /// Provides a mechanism to cache previous <see cref="ZeldaScene"/>s.
        /// </summary>
        private readonly ZeldaScenesCache scenesCache = new ZeldaScenesCache();

        /// <summary>
        /// The map of timers that are world-wide active.
        /// </summary>
        private readonly Zelda.Timing.TimerMap worldWideTimers = new Zelda.Timing.TimerMap();

        #endregion
    }
}
