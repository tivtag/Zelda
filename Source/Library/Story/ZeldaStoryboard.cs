// <copyright file="ZeldaStoryboard.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Story.ZeldaStoryboard class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Story
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom.Story;
    using Zelda.Saving;

    /// <summary>
    /// Combines multiple independent ZeldaTimelines to lie on a single master timeline.
    /// </summary>
    public sealed class ZeldaStoryboard : Storyboard
    {
        /// <summary>
        /// Gets the ZeldaTimeline that this ZeldaStoryboard contains.
        /// </summary>
        public new IEnumerable<ZeldaTimeline> Timelines
        {
            get
            {
                return base.Timelines.Cast<ZeldaTimeline>();
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeHeader( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.TimelineCount );

            foreach( var timeline in this.Timelines )
            {
                context.Write( timeline.Name ?? string.Empty );
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeBody( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.TimelineCount );

            foreach( var timeline in this.Timelines )
            {
                timeline.Serialize( context );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeHeader( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            
            int timelineCount = context.ReadInt32();

            for( int i = 0; i < timelineCount; ++i )
            {
                var timeline = new ZeldaTimeline() { 
                    Name = context.ReadString()
                };

                this.AddTimeline( timeline );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeBody( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            int timelineCount = context.ReadInt32();

            if( timelineCount != this.TimelineCount )
            {
                throw new InvalidOperationException(
                    @"The Storyboard doesn't contain the expected number of Timelines.
                    Don't add or remove Timelines between calling DeserializeHeader and DeserializeBody." 
                );
            }

            for( int i = 0; i < timelineCount; ++i )
            {
                var timeline = this.GetTimelineAt( i );
                timeline.Deserialize( context );
            }
        }
        
        /// <summary>
        /// Gets the timeline at the specified zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Timeline to get.
        /// </param>
        /// <returns>
        /// The requested Timeline.
        /// </returns>
        public new ZeldaTimeline GetTimelineAt( int index )
        {
            return (ZeldaTimeline)base.GetTimelineAt( index );
        }
    }
}
