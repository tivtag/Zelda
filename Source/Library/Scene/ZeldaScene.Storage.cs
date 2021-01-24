// <copyright file="ZeldaScene.Storage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the storage mechanisms Zelda.ZeldaScene class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Atom;
    using Atom.Events;
    using Atom.Scene.Tiles;
    using Zelda.Entities;
    using Zelda.Saving;
    using Zelda.Waypoints;

    /// <content>
    /// This partial class implementation contains all
    /// code related to saving and loading a ZeldaScene.
    /// </content>
    public sealed partial class ZeldaScene
    {
        #region - Save -

        /// <summary>
        /// Saves this <see cref="ZeldaScene"/>.
        /// </summary>
        public void Save()
        {
            if( string.IsNullOrEmpty( this.Name ) )
                throw new InvalidOperationException( Resources.Error_CantSaveSceneNameNotSet );

            using( var memoryStream = new MemoryStream() )
            {
                var writer = new BinaryWriter( memoryStream );
                var context = new SceneSerializationContext( this, new BinaryWriter( memoryStream ), this.serviceProvider );
                this.Write( context );

                this.WriteToFile( memoryStream );
            }
        }

        /// <summary>
        /// Writes the specified stream of scene data to the hard-disc.
        /// </summary>
        /// <param name="memoryStream">
        /// The memory stream to copy over into a file.
        /// </param>
        private void WriteToFile( MemoryStream memoryStream )
        {
            const string FolderPath = "Content/Scenes/";
            Directory.CreateDirectory( FolderPath );

            string filePath = FolderPath + this.Name + Extension;
            using( FileStream fileStream = File.Create( filePath ) )
            {
                Atom.IOUtilities.CopyStream( memoryStream, fileStream );
                fileStream.Flush();
            }
        }

        /// <summary>
        /// Write this <see cref="ZeldaScene"/> to the given <see cref="System.IO.BinaryWriter"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void Write( IZeldaSerializationContext context )
        {
            // Write Header
            const int FileVersion = 9;
            context.Write( FileVersion );

            this.WriteSettings( context );
            TileMap.Save( this.map, context );

            this.WriteWaypoints( context );

            // Find the entities, events and triggers that need to be saved.
            var entitiesStorage = new SceneEntitiesStorage( this.serviceProvider );

            var objectsToSave = entitiesStorage.FindToSave( this.entities );
            var eventsToSave = eventManager.GetEventsToSave();
            var triggersToSave = eventManager.GetTriggersToSave();

            var eventWriter = new Atom.Events.EventManager.ReaderWriter( new TypeActivator() );
            var eventContext = new ZeldaEventSerializationContext( this.eventManager, context.Writer, serviceProvider );

            // Write the header data, which contains enough information
            // to create the entities.
            entitiesStorage.WriteHeader( objectsToSave, context );
            WriteEventsHeader( eventsToSave, triggersToSave, eventWriter, eventContext );
            this.storyboard.SerializeHeader( context );

            // Now write the actual data.
            entitiesStorage.Write( objectsToSave, context );
            WriteEvents( eventsToSave, triggersToSave, eventWriter, eventContext );
            this.storyboard.SerializeBody( context );
        }

        /// <summary>
        /// Writes the Waypoints, Path Segments and Paths of this Scene using the given IZeldaSerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void WriteWaypoints( IZeldaSerializationContext context )
        {
            var writer = new ZeldaWaypointMap.ReaderWriter();
            writer.Serialize( this.waypointMap, context );
        }

        /// <summary>
        /// Writes the Scene's settings using the given IZeldaSerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void WriteSettings( IZeldaSerializationContext context )
        {
            this.settings.Serialize( context );
        }

        /// <summary>
        /// Writes the event header data junk that contains
        /// the types and names of all events and triggers in the scene.
        /// </summary>
        /// <param name="eventsToSave">
        /// The events that are required to be saved.
        /// </param>
        /// <param name="triggersToSave">
        /// The triggers that are required to be saved.
        /// </param>
        /// <param name="writer">
        /// The writer to use when writing the event data.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// </param>
        private static void WriteEventsHeader(
            IList<Event> eventsToSave,
            IList<EventTrigger> triggersToSave,
            EventManager.ReaderWriter writer,
            IEventSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Whee.
            writer.WriteHeader( eventsToSave, triggersToSave, context );
        }

        /// <summary>
        /// Writes the Events and EventTriggers of the Scene using the given writer and context.
        /// </summary>
        /// <param name="eventsToSave">
        /// The events that are required to be saved.
        /// </param>
        /// <param name="triggersToSave">
        /// The triggers that are required to be saved.
        /// </param>
        /// <param name="writer">
        /// The writer to use when writing the event data.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// </param>
        private static void WriteEvents(
            IList<Event> eventsToSave,
            IList<EventTrigger> triggersToSave,
            EventManager.ReaderWriter writer,
            IEventSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            writer.WriteBody( eventsToSave, triggersToSave, context );
        }

        #endregion

        #region - Load -

        /// <summary>
        /// Loads the scene with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the scene to load.
        /// </param>
        /// <param name="worldStatus">
        /// Stores the status of the game world. Can be null.
        /// </param>
        public void Load( string name, WorldStatus worldStatus )
        {
            string path = "Content/Scenes/" + name + Extension;

            using( var stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.None ) )
            {
                var context = new SceneDeserializationContext( this, worldStatus, new BinaryReader( stream ), this.serviceProvider );
                this.Read( context );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe a complete scene.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void Read( SceneDeserializationContext context )
        {
            this.ClearAll();

            // Read Header
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, 6, 9, this.GetType() );

            if( version >= 9 )
            {
                context.Version = version;
            }

            // Read Data.
            this.ReadSettings( context );
            this.ReadMap( context );

            if( version >= 7 )
            {
                this.ReadWaypoints( context );
            }

            // Read the header data, which contains enough information
            // to create the entities.
            var entitiesStorage = new SceneEntitiesStorage( this.serviceProvider );
            var sortedEntities = entitiesStorage.ReadHeader( context );

            foreach( var group in sortedEntities )
            {
                foreach( var entity in group.Value )
                {
                    entity.AddToScene( this );
                }                
            }

            var eventReader = new Atom.Events.EventManager.ReaderWriter( ZeldaEventTypeActivator.Instance );
            var eventContext = new ZeldaEventDeserializationContext( this.eventManager, context.Reader, serviceProvider );
            ReadEventsHeader( eventReader, eventContext );

            if( version >= 8 )
            {
                this.storyboard.DeserializeHeader( context );
            }

            // Now read the actual data.
            entitiesStorage.Read( sortedEntities, context );
            this.ReadEvents( eventReader, eventContext );

            if( version >= 8 )
            {
                this.storyboard.DeserializeBody( context );
            }

            // Finalize.
            this.FinalizeRead();
        }

        /// <summary>
        /// Finalizes reading of the ZeldaScene.
        /// </summary>
        private void FinalizeRead()
        {
            this.RemoveRemovedPersistantEntities();

            if( this.status != null )
            {
                this.status.InitializeScene( this );
            }

            this.NotifyVisabilityUpdateNeeded();
            this.UpdateVisible();
            this.SortVisible();
        }

        /// <summary>
        /// Reads the Waypoints, Path Segments and Paths of this Scene using the given IZeldaDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadWaypoints( IZeldaDeserializationContext context )
        {
            var writer = new ZeldaWaypointMap.ReaderWriter();
            writer.Deserialize( this.waypointMap, context );
        }

        /// <summary>
        /// Reads the Settings of the Scene from the given BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadSettings( IZeldaDeserializationContext context )
        {
            this.settings.Deserialize( context );
        }

        /// <summary>
        /// Reads the Map data of the Scene from the given BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadMap( ISceneDeserializationContext context )
        {
            var reader = new TileMap.ReaderWriter( new TileMapFloor.ReaderWriter( ZeldaTypeActivator.Instance ) );

            reader.Deserialize( this.map, context );

            // Setup what we got so far.
            this.SetupStatus( context.WorldStatus );
            this.SetupMap();
        }    

        /// <summary>
        /// Reads the entity header and returns the used EventDocument.
        /// </summary>
        /// <param name="eventReader">
        /// Provides a mechanism for reading the event header.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private static void ReadEventsHeader( EventManager.ReaderWriter eventReader, IEventDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaScene (EventsHeader)" );

            eventReader.ReadHeader( context );
        }

        /// <summary>
        /// Reads the Events and EventTriggers of the Scene from the given BinaryReader.
        /// </summary>
        /// <param name="eventReader">
        /// The EventDocument that has been used to read the header.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadEvents( EventManager.ReaderWriter eventReader, IEventDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaScene (Events)" );

            eventReader.ReadBody( context );
            this.eventManager.Setup( this.serviceProvider );
        }       

        #endregion
    }
}