// <copyright file="IStateSaveable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Saving.IStateSaveable interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Saving
{    
    /// <summary>
    /// Provides a mechanism to write and read the state of the object
    /// that implements the interface.
    /// </summary>
    public interface IStateSaveable
    {
        /// <summary>
        /// Serializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void SerializeState( Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context );
    }
}
