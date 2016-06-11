// <copyright file="IEntityReaderWriter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IEntityReaderWriter interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    using System;

    /// <summary>
    /// Defines the interface for an object that encapsulates the process
    /// of serializing and deserializing the data of a specific ZeldaEntity type
    /// to and from a binary format.
    /// </summary>
    public interface IEntityReaderWriter
    {
        /// <summary>
        /// Gets the entity-type handled by the <see cref="IEntityReaderWriter"/>.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Creates a new ZeldaEntity of the <see cref="EntityType"/> this IEntityReaderWriter supports.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the new ZeldaEntity.
        /// </param>
        /// <returns>
        /// The newly created ZeldaEntity.
        /// </returns>
        ZeldaEntity Create( string name );

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
        void Serialize( ZeldaEntity entity, Zelda.Saving.IZeldaSerializationContext context  );

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
        void Deserialize( ZeldaEntity entity, Zelda.Saving.IZeldaDeserializationContext context );

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
        bool ShouldSave( ZeldaEntity entity );
    }
}
