// <copyright file="Predicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Predicate{T} class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates
{
    using Zelda.Saving;

    /// <summary>
    /// Represents a yes/no question against a specific value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the Predicate{T} acts on.
    /// </typeparam>
    public abstract class Predicate<T> : IPredicate<T>
    {
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
        }

        /// <summary>
        /// Gets a value indicating whether this IPredicate{T}
        /// holds on the specified value.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        /// <returns>
        /// true if this IPredicate{T} holds on the specified value;
        /// -or- otherwise false.
        /// </returns>
        public abstract bool Holds( T value );
    }
}
