// <copyright file="ZeldaEventDeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ZeldaEventDeserializationContext class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    using System.IO;
    using Atom.Events;
    using Zelda.Events;

    /// <summary>
    /// Provides access to the objects required during deserialization of event data.
    /// </summary>
    public sealed class ZeldaEventDeserializationContext : DeserializationContext, IZeldaEventDeserializationContext
    {      
        /// <summary>
        /// Gets the <Desee cref="ZeldaEventManager"/> that is currently deserialized.
        /// </summary>
        /// <value>
        /// Is null when deserializing an event that is not attached to an EventManager.
        /// </value>
        public ZeldaEventManager EventManager
        {
            get
            {
                return this.eventManager;
            }
        }

        /// <summary>
        /// Gets the <Desee cref="EventManager"/> that is currently deserialized.
        /// </summary>
        /// <value>
        /// Is null when deserializing an event that is not attached to an EventManager.
        /// </value>
        EventManager IEventStorageContext.EventManager
        {
            get
            {
                return this.eventManager;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ZeldaEventDeserializationContext class.
        /// </summary>
        /// <param name="eventManager">
        /// The ZeldaEventManager that is going to be deserialized.
        /// </param>
        /// <param name="reader">
        /// The BinaryReader that is going to be read with.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public ZeldaEventDeserializationContext( 
            ZeldaEventManager eventManager,
            BinaryReader reader, 
            IZeldaServiceProvider serviceProvider )
            : base( reader, serviceProvider )
        {
            this.eventManager = eventManager;
        }

        /// <summary>
        /// Gets the <Desee cref="Event"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the Event to reveive.
        /// </param>
        /// <returns>
        /// The requested Event;
        /// or null.
        /// </returns>
        public Event GetEvent( string name )
        {
            return this.eventManager.GetEvent( name );
        }

        /// <summary>
        /// Gets the <Desee cref="EventTrigger"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the EventTrigger to reveive.
        /// </param>
        /// <returns>
        /// The requested EventTrigger;
        /// or null.
        /// </returns>
        public EventTrigger GetTrigger( string name )
        {
            return this.eventManager.GetTrigger( name );
        }

        /// <summary>
        /// The <see cref="ZeldaEventManager"/> that is currently deserialized.
        /// </summary>
        private readonly ZeldaEventManager eventManager;
    }
}
