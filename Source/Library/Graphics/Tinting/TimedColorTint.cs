// <copyright file="TimedColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.TimedColorTint class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Tinting
{
    using System;
    using Atom;
    using Atom.Math;

    /// <summary>
    /// Represents an <see cref="IColorTint"/> that has its effect
    /// controlled by a single time value.
    /// </summary>
    public abstract class TimedColorTint : IColorTint
    {
        /// <summary>
        /// Raised when this TimedColorTint has reached its full effect.
        /// </summary>
        public event EventHandler ReachedFullEffect;

        /// <summary>
        /// Gets or sets the total time (in seconds) for this TimedColorTint
        /// to reach its full effect.
        /// </summary>
        public float TotalTime
        {
            get 
            { 
                return this.totalTime; 
            }

            set
            {
                this.totalTime = value;
                this.Reset();
            }
        }

        /// <summary>
        /// Gets the time left until this TimedColorTint has reached its full effect.
        /// </summary>
        protected float TimeLeft
        {
            get
            {
                return this.timeLeft;
            }
        }

        /// <summary>
        /// Gets the ratio between timeLeft and totalTime.
        /// </summary>
        /// <value>
        /// This TimedColorTint reaches its full effect when this value is 0.
        /// </value>
        protected float Factor
        {
            get
            {
                return this.timeLeft / this.totalTime;
            }
        }

        /// <summary>
        /// Applies this IColorTint to the given color.
        /// </summary>
        /// <param name="color">
        /// The input color.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        public abstract Vector4 Apply( Vector4 color );

        /// <summary>
        /// Updates this IZeldaUpdateable.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.hasReachedFullEffect )
                return;

            this.timeLeft -= updateContext.FrameTime;

            if( this.timeLeft < 0.0f )
            {
                this.timeLeft = 0.0f;

                this.hasReachedFullEffect = true;
                this.ReachedFullEffect.Raise( this );
            }
        }

        /// <summary>
        /// Resets the time until this BlendOutColorTint has reached its full effect.
        /// </summary>
        public void Reset()
        {
            this.timeLeft = this.totalTime;
            this.hasReachedFullEffect = false;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.TotalTime );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.TotalTime = context.ReadSingle();
        }

        /// <summary>
        /// The storage fields of the TotalTime property.
        /// </summary>
        private float totalTime;

        /// <summary>
        /// The time left until this TimedColorTint has reached its full effect.
        /// </summary>
        private float timeLeft;

        /// <summary>
        /// States whether this TimedColorTint has reached its full effect.
        /// </summary>
        private bool hasReachedFullEffect;
    }
}
