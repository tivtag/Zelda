// <copyright file="UnsavedTimer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Timing.UnsavedTimer class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Timing
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents a wrapper around an <see cref="ITimer"/> that by 
    /// implementing the <see cref="IMaybeSaved"/> interface tells
    /// consumers that it doesn't want to be saved/serializes.
    /// This class can't be inherited.
    /// </summary>
    public sealed class UnsavedTimer : ITimer, IMaybeSaved
    {
        /// <summary>
        /// Raised when the <see cref="Timer"/> has ended.
        /// </summary>
        public event Atom.SimpleEventHandler<ITimer> Ended
        {
            add
            {
                this.timer.Ended += value;
            }

            remove
            {
                this.timer.Ended -= value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITimer"/> this UnsavedTimer wraps around.
        /// </summary>
        public ITimer Timer
        {
            get
            {
                return this.timer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the UnsavedTimer class.
        /// </summary>
        /// <param name="timer">
        /// The <see cref="ITimer"/> the new UnsavedTimer wraps around.
        /// </param>
        public UnsavedTimer( ITimer timer )
        {
            Contract.Requires<ArgumentNullException>( timer != null );

            this.timer = timer;
        }

        /// <summary>
        /// Gets a value indicating whether this UnsavedTimer
        /// should be saved.
        /// </summary>
        /// <returns>
        /// Always returns false.
        /// </returns>
        public bool ShouldSerialize()
        {
            return false;
        }

        /// <summary>
        /// Updates this UnsavedTimer.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.timer.Update( updateContext );
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
            this.timer.Serialize( context );
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
            this.timer.Deserialize( context );
        }

        /// <summary>
        /// Identifies the <see cref="ITimer"/> this UnsavedTimer wraps around.
        /// </summary>
        private readonly ITimer timer;
    }
}
