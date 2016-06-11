// <copyright file="IStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.IStorage interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving.Storage
{
    /// <summary>
    /// Provides a place to store arabitary data.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Serializes the data required to descripe this IStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void SerializeStorage( Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes the data required to descripe this IStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void DeserializeStorage( Zelda.Saving.IZeldaDeserializationContext context );
    }
}
