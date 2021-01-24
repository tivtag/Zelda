// <copyright file="ZeldaIncident.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Story.ZeldaIncident class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Story
{
    using Atom;
    using Atom.Story;
    using Zelda.Saving;

    /// <summary>
    /// Represents a single 'event' that is placed on a ZeldaTimeline.
    /// </summary>
    public sealed class ZeldaIncident : Incident, ISaveable
    {
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

            context.WriteUnsigned( this.RelativeTick.Value );
            context.WriteStoreObject( this.Action );
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

            this.RelativeTick = new TimeTick( context.ReadUInt32() );
            this.Action = context.ReadStoreObject<IAction>();
        }
    }
}
