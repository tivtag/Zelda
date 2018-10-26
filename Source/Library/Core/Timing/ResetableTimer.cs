// <copyright file="ResetableTimer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Timing.ResetableTimer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Timing
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Zelda.Saving;

    /// <summary>
    /// Represents a Timer that can be reset to be run again with the same timing values.
    /// </summary>
    public class ResetableTimer : ITimer
    {
        /// <summary>
        /// Raised when this Timer has ended.
        /// </summary>
        public event SimpleEventHandler<ITimer> Ended;

        /// <summary>
        /// Gets or sets the time this <see cref="ResetableTimer"/> takes until it ends.
        /// </summary>
        public float Time
        {
            get
            {
                return this.time;
            }

            set
            {
                Contract.Requires<ArgumentException>( value >= 0.0f );

                this.time = value;
            }
        }

        /// <summary>
        /// Gets or sets the time until this ITimer ends.
        /// </summary>
        public float TimeLeft
        {
            get
            {
                return this.timeLeft;
            }

            set
            {
                this.timeLeft = value;
            }
        }

        /// <summary>
        /// Gets the current ratio between time left and time.
        /// </summary>
        public float Ratio
        {
            get
            {
                return this.timeLeft / this.time;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ITimer has ended.
        /// </summary>
        public bool HasEnded
        {
            get
            {
                return this.timeLeft <= 0.0f;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ITimer has not ended.
        /// </summary>
        public bool HasNotEnded
        {
            get
            {
                return !this.HasEnded;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ResetableTimer class.
        /// </summary>
        public ResetableTimer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResetableTimer class.
        /// </summary>
        /// <param name="time">
        /// The time in seconds the action is not allowed to be executed for.
        /// </param>
        public ResetableTimer( float time )
        {
            this.time = time;
            this.timeLeft = time;
        }

        /// <summary>
        /// Resets the time left until the action is useable again.
        /// </summary>
        public void Reset()
        {
            this.timeLeft = this.time;
        }

        /// <summary>
        /// Updates this ITimer.
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

            context.Write( this.time );
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

            this.time = context.ReadSingle();
        }

        /// <summary>
        /// The time this ITimer takes.
        /// </summary>
        private float time;

        /// <summary>
        /// The time left until this ITimer ends.
        /// </summary>
        private float timeLeft;
    }
}
