// <copyright file="Timer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Timing.Timer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Timing
{
    using System;
    using System.Diagnostics.Contracts;
    using Atom;
    using Zelda.Saving;

    /// <summary>
    /// Represents an ITimer that raises an event after a specified time.
    /// </summary>
    public class Timer : ITimer
    {
        /// <summary>
        /// Raised when this Timer has ended.
        /// </summary>
        public event SimpleEventHandler<ITimer> Ended;

        /// <summary>
        /// Gets or sets the time until this Timer ends.
        /// </summary>
        public float Time
        {
            get
            {
                return this.timeLeft;
            }

            set
            {
                Contract.Requires<ArgumentException>( value >= 0.0f );

                this.timeLeft = value;
            }
        }

        /// <summary>
        /// Updates this Timer.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.timeLeft > 0.0f )
            {
                this.timeLeft -= updateContext.FrameTime;
            }

            if( this.timeLeft <= 0.0f )
            {
                this.Ended.Raise( this );
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.timeLeft );
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

            this.timeLeft = context.ReadSingle();
        }
        
        /// <summary>
        /// The time left until this Timer ends.
        /// </summary>
        private float timeLeft;
    }
}
