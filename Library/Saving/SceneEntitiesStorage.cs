namespace Zelda.Saving
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom;
    using Zelda.Entities;

    /// <summary>
    /// Encapsulates the serialization and deserialization of a list of entities.
    /// </summary>
    public sealed class SceneEntitiesStorage
    {
        public SceneEntitiesStorage( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Helper method that filters out the entities that must be saved.
        /// </summary>
        /// <param name="entities">
        /// All entities that might want to be saved.
        /// </param>
        /// <returns>A new dictionary that maps entity type to a list of entities.</returns>
        public Dictionary<Type, List<ZeldaEntity>> FindToSave( IEnumerable<ZeldaEntity> entities )
        {
            // Get the EntityReaderWriterManager, which manages and
            // stores the IEntityReaderWriter instances:
            var readerWriterManager = this.serviceProvider.EntityReaderWriterManager;

            // We now sort the entities that want to be saved by Type:
            var sortedEntities = new Dictionary<Type, List<ZeldaEntity>>( 10 );

            // :D
            foreach( var entity in entities )
            {
                var savebleSwitch = entity as ISavedState;
                if( savebleSwitch != null )
                {
                    if( !savebleSwitch.IsSaved )
                        continue;
                }

                Type entityType = entity.GetType();
                IEntityReaderWriter readerWriter = readerWriterManager.TryGet( entityType );

                // Only add it if there actually exists an EntityReaderWriter for it:
                if( readerWriter != null )
                {
                    // And if the reader-writer permits it
                    if( readerWriter.ShouldSave( entity ) )
                    {
                        List<ZeldaEntity> list;

                        if( !sortedEntities.TryGetValue( entityType, out list ) )
                        {
                            list = new List<ZeldaEntity>( 10 );
                            sortedEntities.Add( entityType, list );
                        }

                        list.Add( entity );
                    }
                }
                else
                {
                    this.serviceProvider.Log.WriteLine(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Resources.Error_CouldntFindEntityReaderWriterForTypeX,
                            entity.GetType()
                        )
                    );
                }
            }

            return sortedEntities;
        }

        /// <summary>
        /// Writes the entities header data junk that contains
        /// the types and names of all entities in the scene.
        /// </summary>
        /// <param name="sortedEntities">
        /// The entities that are required to be saved.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void WriteHeader( IDictionary<Type, List<ZeldaEntity>> sortedEntities, IZeldaSerializationContext context )
        {
            const int FileVersion = 1;
            context.Write( FileVersion );

            // Write how many different entity types we have:
            context.Write( sortedEntities.Count );

            // Now for each entity type ..
            foreach( var entitiesWithType in sortedEntities )
            {
                var entities = entitiesWithType.Value;

                // Let's first write the TypeName:
                context.Write( entitiesWithType.Key.GetTypeName() );

                // Write the number of entities:
                context.Write( entities.Count );

                // And now for each entity ..
                foreach( var entity in entities )
                {
                    // Write the name that uniquely identifies it.
                    context.Write( entity.Name ?? string.Empty );
                }
            }
        }

        /// <summary>
        /// Writes the objects of the Scene using the given BinaryWriter.
        /// </summary>
        /// <param name="sortedEntities">
        /// The entities that are required to be saved.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Write( Dictionary<Type, List<ZeldaEntity>> sortedEntities, IZeldaSerializationContext context )
        {
            // Get the ObjectReaderWriterManager, which manages and stores 
            // the IEntityReaderWriter instances:
            var readerWriterManager = this.serviceProvider.EntityReaderWriterManager;

            // First write the header:
            const int FileVersion = 2;
            context.Write( FileVersion );

            // Write how many different entity types we have:
            context.Write( sortedEntities.Count );

            // Now for each object type ..
            foreach( var entitiesWithType in sortedEntities )
            {
                // Let's get the IEntityReaderWriter that is needed todo the actual
                // serialization:
                var readerWriter = readerWriterManager.Get( entitiesWithType.Key );
                var entities = entitiesWithType.Value;

                // Let's first write the TypeName:
                context.Write( entitiesWithType.Key.GetTypeName() );

                // Write the number of entities:
                context.Write( entities.Count );

                // And now let's finally serialize each entity :))
                foreach( var obj in entities )
                {
                    readerWriter.Serialize( obj, context );
                }
            }

            // Done! :D
        }

        /// <summary>
        /// Reads the entity header and returns the sorted list of entities.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The list of entities, sorted by type.
        /// </returns>
        public IDictionary<Type, List<ZeldaEntity>> ReadHeader( IZeldaDeserializationContext context )
        {
            var entityReaderWriterManager = this.serviceProvider.EntityReaderWriterManager;

            // Write Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, "ZeldaScene (EntitiesHeader)" );

            // Write Data.
            int typeCount = context.ReadInt32();
            var sortedEntities = new Dictionary<Type, List<ZeldaEntity>>( typeCount );

            // For each entity type ..
            for( int typeIndex = 0; typeIndex < typeCount; ++typeIndex )
            {
                string typeName = context.ReadString();
                Type type = Type.GetType( typeName );

                if( type == null )
                {
                    throw new NotFoundException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "Could not find the type that corresponds to the type name '{0}'.",
                            typeName
                        )
                    );
                }

                // And get the needed IEntityReaderWriter!
                var readerWriter = entityReaderWriterManager.Get( type );

                int objectCount = context.ReadInt32();
                var entityList = new List<ZeldaEntity>( objectCount );

                // For each entity ..
                for( int objectIndex = 0; objectIndex < objectCount; ++objectIndex )
                {
                    // Read the name that uniquely identifies it.
                    string name = context.ReadString();

                    // Create..
                    var entity = readerWriter.Create( name );

                    // And add the uninitialized entity :)
                    entityList.Add( entity );
                }

                sortedEntities.Add( type, entityList );
            }

            return sortedEntities;
        }

        /// <summary>
        /// Reads the entities of the Scene from the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="sortedEntities">
        /// The list of entities the scene contains, as readen from the entities header, sorted by type.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Read( IDictionary<Type, List<ZeldaEntity>> sortedEntities, IZeldaDeserializationContext context )
        {
            var readerWriterManager = this.serviceProvider.EntityReaderWriterManager;

            // Read Header:
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read how many different entity types we have:
            int entityTypeCount = context.ReadInt32();

            // For each type ..
            for( int typeIndex = 0; typeIndex < entityTypeCount; ++typeIndex )
            {
                // Let's read the TypeName:
                var typeName = context.ReadString();
                var type = Type.GetType( typeName );

                // And get the needed IEntityReaderWriter!
                var readerWriter = readerWriterManager.Get( type );
                var entityList = sortedEntities[type];

                // Now read how many entities there are
                // that have the same TypeName:
                int entityCount = context.ReadInt32();

                // Finally - read the entities:
                for( int entityIndex = 0; entityIndex < entityCount; ++entityIndex )
                {
                    var entity = entityList[entityIndex];
                    readerWriter.Deserialize( entity, context );

                    // Setup the object.
                    var setupable = entity as IZeldaSetupable;
                    if( setupable != null )
                        setupable.Setup( serviceProvider );

                    Debug.Assert( entity.GetType() == type );
                }

                // Repeat for all TypeNames.
            }
        }

        private readonly IZeldaServiceProvider serviceProvider;
    }
}
