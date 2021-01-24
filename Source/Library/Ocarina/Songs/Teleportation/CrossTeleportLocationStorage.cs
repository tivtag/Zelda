// <copyright file="CrossTeleportLocationStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Teleportation.CrossTeleportLocationStorage class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    using Atom.Math;
    using Zelda.Saving;
    using Zelda.Saving.Storage;

    /// <summary>
    /// Stores world-presistant data relevant to the <see cref="MusicCrossSong"/>.
    /// </summary>
    public sealed class CrossTeleportLocationStorage : IStorage
    {
        /// <summary>
        /// The identifier string that uniquely identifies the unique CrossTeleportLocationStorage.
        /// </summary>
        public const string Identifier = "cross-teleport-location";

        /// <summary>
        /// Gets or sets the name of the scene at which the cross has been used.
        /// </summary>
        public string SceneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position in the scene at which the cross has been used.
        /// </summary>
        public Vector2 Position 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the number of the floor in the scene at which the cross has been used.
        /// </summary>
        public int FloorNumber
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
        public void SerializeStorage( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.SceneName ?? string.Empty );
            context.Write( this.Position );
            context.Write( this.FloorNumber );
        }
        
        /// <summary>
        /// Deserializes the data required to descripe this IStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeStorage( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.SceneName = context.ReadString();
            this.Position = context.ReadVector2();
            this.FloorNumber = context.ReadInt32();
        }
    }
}
