// <copyright file="MinimumDifficultyRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Requirements.MinimumDifficultyRequirement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Requirements
{
    using Zelda.Difficulties;
    using Zelda.Saving;

    /// <summary>
    /// Represents an IRequirement that requires the player to have choosen a specific
    /// minimum difficutly for it to be fulfilled.
    /// </summary>
    public sealed class MinimumDifficultyRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the minimum difficulty that is required before
        /// this MinimumDifficultyRequirement is fulfilled.
        /// </summary>
        public DifficultyId MinimumDifficulty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this IRequirement is fulfilled.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity for which this IRequirement is checked against.
        /// </param>
        /// <returns>
        /// true if it is fulfilled;
        /// otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity player )
        {
            return (int)player.Profile.Difficulty >= (int)this.MinimumDifficulty;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( (byte)this.MinimumDifficulty );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.MinimumDifficulty = (DifficultyId)context.ReadByte();
        }
    }
}
