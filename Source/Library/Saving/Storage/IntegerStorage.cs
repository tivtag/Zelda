// <copyright file="IntegerStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.IntegerStorage class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving.Storage
{
    /// <summary>
    /// Provides a place to store a single <see cref="System.Int32"/> value.
    /// This class can't be inherited.
    /// </summary>
    public sealed class IntegerStorage : IValueStorage<int>
    {
        /// <summary>
        /// Gets or sets the value stored in this IntegerStorage.
        /// </summary>
        public int Value
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes the data required to descripe this IStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeStorage( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.Value );
        }

        /// <summary>
        /// Deserializes the data required to descripe this IStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeStorage( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.Value = context.ReadInt32();
        }
    }
}
