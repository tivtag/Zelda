// <copyright file="EntityReaderWriter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityReaderWriter{T} class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;

    /// <summary>
    /// Represents an <see cref="IEntityReaderWriter"/> that implements
    /// the basic default behaviour of an IEntityReaderWriter.
    /// </summary>
    /// <remarks>
    /// This class is used to reduce the amount of duplicate code.
    /// </remarks>
    /// <typeparam name="TEntity">
    /// The entity type the BasicEntityReaderWriter is supposed to handle.
    /// </typeparam>
    internal abstract class EntityReaderWriter<TEntity> : IEntityReaderWriter
        where TEntity : ZeldaEntity, new()
    {      
        /// <summary>
        /// Gets the entity-type handled by this <see cref="EntityReaderWriter{TEntity}"/>.
        /// </summary>
        public Type EntityType
        {
            get
            {
                return typeof( TEntity );
            }
        }

        /// <summary>
        /// Initializes a new instance of the EntityReaderWriter{TEntity} class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected EntityReaderWriter( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new ZeldaEntity of the <see cref="EntityType"/> this IEntityReaderWriter supports.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the new ZeldaEntity.
        /// </param>
        /// <returns>
        /// The newly created ZeldaEntity.
        /// </returns>
        public ZeldaEntity Create( string name )
        {
            return new TEntity() { Name = name };
        }

        /// <summary>
        /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// If the type of the given entity is invalid for the <see cref="IEntityReaderWriter"/>.
        /// </exception>
        public void Serialize( ZeldaEntity entity, Saving.IZeldaSerializationContext context )
        {
            this.Serialize( (TEntity)entity, context );
        }

        /// <summary>
        /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
        /// the given ZeldaEntity.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// If the type of the given entity is invalid for the <see cref="IEntityReaderWriter"/>.
        /// </exception>
        /// <param name="entity">
        /// The ZeldaEntity to initialize.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( ZeldaEntity entity, Saving.IZeldaDeserializationContext context )
        {
            this.Deserialize( (TEntity)entity, context );
        }

        /// <summary>
        /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void Serialize( TEntity entity, Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
        /// the given ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The ZeldaEntity to initialize.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void Deserialize( TEntity entity, Zelda.Saving.IZeldaDeserializationContext context );
        
        /// <summary>
        /// Gets a value indicating whether the given entity should be serialized.
        /// </summary>
        /// <param name="entity">
        /// The entity to check.
        /// </param>
        /// <returns>
        /// True if it should be serialized; 
        /// -or- otherwise false if the entity should be abondend -
        /// e.g. when the entity is in an unrecoverable save state.
        /// </returns>
        public bool ShouldSave( ZeldaEntity entity )
        {
            return this.ShouldSave( (TEntity)entity );
        }

        /// <summary>
        /// Gets a value indicating whether the given entity should be serialized.
        /// </summary>
        /// <param name="entity">
        /// The entity to check.
        /// </param>
        /// <returns>
        /// True if it should be serialized; 
        /// -or- otherwise false if the entity should be abondend -
        /// e.g. when the entity is in an unrecoverable save state.
        /// </returns>
        public virtual bool ShouldSave( TEntity entity )
        {
            return true;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        protected readonly IZeldaServiceProvider serviceProvider;
    }
}
