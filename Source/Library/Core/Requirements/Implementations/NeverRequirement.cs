// <copyright file="NeverRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Requirements.NeverRequirement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Requirements
{
    using Zelda.Saving;

    /// <summary>
    /// Implements an <see cref="IRequirement"/> that always returns false.
    /// </summary>
    public sealed class NeverRequirement : IRequirement
    {
        /// <summary>
        /// Gets a value indicating whether the given PlayerEntity
        /// fulfills the requirements as specified by this IItemDropRequirement.
        /// </summary>
        /// <param name="player">
        /// The realted PlayerEntity.
        /// </param>
        /// <returns>
        /// Returns true if the given PlayerEntity fulfills the specified requirement;
        /// or otherwise false.
        /// </returns>
        public bool IsFulfilledBy(Zelda.Entities.PlayerEntity player)
        {
            return false;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize(IZeldaSerializationContext context)
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
        public void Deserialize(IZeldaDeserializationContext context)
        {
            context.ReadDefaultHeader(this.GetType());
        }
    }
}
