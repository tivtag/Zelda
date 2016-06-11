// <copyright file="ResetableRangeTimer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Timing.ResetableRangeTimer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Timing
{
    using Atom;
    using Atom.Math;
    using Zelda.Saving;

    /// <summary>
    /// Represents a Timer that can be reset to be run again with the same timing values.
    /// </summary>
    public class ResetableRangeTimer : ITimer, IZeldaSetupable
    {
        /// <summary>
        /// Raised when this Timer has ended.
        /// </summary>
        public event SimpleEventHandler<ITimer> Ended;

        /// <summary>
        /// Gets or sets the time this <see cref="ResetableRangeTimer"/> takes until it ends.
        /// </summary>
        public FloatRange TimeRange
        {
            get;
            set;
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
                return this.timeLeft / this.lastTime;
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

        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Resets the time left until the action is useable again.
        /// </summary>
        public void Reset()
        {
            this.timeLeft = this.lastTime = this.TimeRange.GetRandomValue( this.rand );
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

            context.Write( this.TimeRange );
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

            this.TimeRange = context.ReadFloatRange();
            this.Reset();
        }

        /// <summary>
        /// The time this ITimer takes in the current pass.
        /// </summary>
        private float lastTime;

        /// <summary>
        /// The time left until this ITimer ends.
        /// </summary>
        private float timeLeft;
        
        private IRand rand;
    }
}
