// <copyright file="ZeldaTimeline.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Story.ZeldaTimeline class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Story
{
    using Atom.Story;
    using Zelda.Saving;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a single independent row of Incidents.
    /// </summary>
    public sealed class ZeldaTimeline : Timeline, ISaveable
    {
        /// <summary>
        /// Gets the incidents this ZeldaTimeline contains.
        /// </summary>
        public new IEnumerable<ZeldaIncident> Incidents
        {
            get
            {
                return base.Incidents.Cast<ZeldaIncident>();
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Saving.IZeldaSerializationContext context )
        {
            var incidents = this.Incidents.ToArray();

            context.WriteDefaultHeader();
            context.Write( this.IsActive );
            context.WriteUnsigned( this.StartOffset.Value );

            context.Write( incidents.Length );
            
            foreach( var incident in incidents )
            {

            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
        }
    }
}
