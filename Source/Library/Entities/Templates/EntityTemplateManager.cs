// <copyright file="EntityTemplateManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityTemplateManager class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Zelda.Difficulties;
    
    /// <summary>
    /// The EntityTemplateManager is responsible for loading and caching of Entity Templates.
    /// This class can't be inherited.
    /// </summary>
    public sealed class EntityTemplateManager
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTemplateManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public EntityTemplateManager( IZeldaServiceProvider serviceProvider )
        {
#if DEBUG
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );
#endif
            this.serviceProvider = serviceProvider;
            this.entityReaderWriterManager = serviceProvider.EntityReaderWriterManager;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets an instance of the EntityTemplate that has the given <paramref name="templateName"/>.
        /// </summary>
        /// <param name="templateName">
        /// The name of the template.
        /// </param>
        /// <returns>
        /// An instance of the template.
        /// </returns>
        public ZeldaEntity GetEntity( string templateName )
        {
            var template = this.GetTemplate( templateName );
            return template.CreateInstance();
        }

        /// <summary>
        /// Gets the template with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the template.
        /// </param>
        /// <returns>
        /// The template.
        /// </returns>
        public IEntityTemplate GetTemplate( string name )
        {
            IEntityTemplate template;

            if( templates.TryGetValue( name, out template ) )
                return template;

            template = new EntityTemplate( this.LoadBareboneEntity( name ), this.serviceProvider );
            templates.Add( name, template );

            return template;
        }

        /// <summary>
        /// Loads the ZeldaEntity with the given <paramref name="name"/> directly from the hard-disc.
        /// </summary>
        /// <param name="name">
        /// The name of the entity.
        /// </param>
        /// <returns>
        /// The directly loaded entity. Should not be used as a template.
        /// </returns>
        public ZeldaEntity LoadEntity( string name )
        {
            var entity = this.LoadBareboneEntity( name );

            GameDifficulty.ApplyOn( entity );

            return entity;
        }

        /// <summary>
        /// Loads the template with the given <paramref name="name"/> from the hard-disc.
        /// </summary>
        /// <remarks>
        /// This method doesn't add the template to the dictionary.
        /// </remarks>
        /// <param name="name">
        /// The name of the template.
        /// </param>
        /// <returns>
        /// The entity that serves as a template.
        /// </returns>
        private ZeldaEntity LoadBareboneEntity( string name )
        {
            string path = "Content/Objects/" + name;

            using( var reader = new BinaryReader( File.OpenRead( path ) ) )
            {
                string typeName = reader.ReadString();
                Type type     = Type.GetType( typeName );

                var readerWriter = this.entityReaderWriterManager.Get( type );

                // Create instance.
                var entity = readerWriter.Create( string.Empty );

                // Deserialize data.
                readerWriter.Deserialize( entity, this.GetDeserializationContext( reader ) );

                var setupable = entity as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( this.serviceProvider );

                return entity;
            }
        }

        /// <summary>
        /// Gets the deserialization context that should be used for the given BinaryReader.
        /// </summary>
        /// <param name="reader">
        /// The BinaryReader used to deserialize an entity template.
        /// </param>
        /// <returns>
        /// The IZeldaDeserializationContext to use.
        /// </returns>
        private Zelda.Saving.IZeldaDeserializationContext GetDeserializationContext( BinaryReader reader )
        {
            return new Zelda.Saving.DeserializationContext( reader, this.serviceProvider );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dictionary of templates, sorted by entity name.
        /// </summary>
        private readonly Dictionary<string, IEntityTemplate> templates = new Dictionary<string, IEntityTemplate>( 15 );

        /// <summary>
        /// Identifies the <see cref="EntityReaderWriterManager"/> object.
        /// </summary>
        private readonly EntityReaderWriterManager entityReaderWriterManager;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
