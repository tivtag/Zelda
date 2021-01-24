// <copyright file="TickingAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.TickingAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Auras
{
    using System;
    using Atom;

    /// <summary>
    /// Represents a <see cref="TimedAura"/> that ticks in a fixed interval.
    /// </summary>
    public class TickingAura : TimedAura
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this TickingAura has ticked.
        /// </summary>
        public event SimpleEventHandler<TickingAura> Ticked;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the time a single tick of this TIckingAura lasts.
        /// </summary>
        public float TickTime
        {
            get
            {
                return this.tickTime;
            }

            set
            {
                if( value <= 0.0f || value >= this.Duration )
                    throw new ArgumentOutOfRangeException( "value" );

                this.tickTime = value;
                this.timeLeftUntilNextTick = value;
            }
        }

        /// <summary>
        /// Gets the number of ticks this TickingAura will have.
        /// </summary>
        public int TickCount
        {
            get
            {
                return (int)(Math.Ceiling( this.Duration / this.TickTime ));
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TickingAura"/> class.
        /// </summary>
        /// <param name="totalTime">
        /// The total time (in seconds) the new TickingAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks of the new TickingAura.
        /// </param>
        public TickingAura( float totalTime, float tickTime )
            : base( totalTime )
        {
            this.Initialize( tickTime );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TickingAura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"> 
        /// If <paramref name="effects"/> is null.
        /// </exception>
        /// <param name="totalTime">
        /// The total time (in seconds) the new TickingAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks of the new TickingAura.
        /// </param>
        /// <param name="effects">
        /// The list of <see cref="StatusEffect"/>s of the new TickingAura.
        /// </param>
        public TickingAura( float totalTime, float tickTime, StatusEffect[] effects )
            : base( totalTime, effects )
        {
            this.Initialize( tickTime );
        }

        /// <summary>
        /// Initializes this TickingAura.
        /// </summary>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks of the TickingAura.
        /// </param>
        private void Initialize( float tickTime )
        {
            this.TickTime = tickTime;
            this.Cooldown.Reset();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this TickingAura.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.timeLeftUntilNextTick -= updateContext.FrameTime;

            if( this.timeLeftUntilNextTick <= 0.0f )
            {
                this.OnTickedPrivate();
                this.timeLeftUntilNextTick = this.tickTime;
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Resets the time until the TickingAura ticks again.
        /// </summary>
        public void ResetTick()
        {
            this.timeLeftUntilNextTick = this.tickTime;
        }

        /// <summary>
        /// Raises the Ticked event.
        /// </summary>
        private void OnTickedPrivate()
        {
            if( this.Ticked != null )
            {
                this.Ticked( this );
            }

            this.OnTicked();
        }

        /// <summary>
        /// Called when this TickingAura has ticked.
        /// </summary>
        protected virtual void OnTicked()
        {
        }
        
        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="TickingAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            var clone = new TickingAura( this.Duration, this.tickTime, this.GetClonedEffects() );

            this.SetupClone( clone );

            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The time left until this TickingAura ticks again.
        /// </summary>
        private float timeLeftUntilNextTick;

        /// <summary>
        /// The storage field of the <see cref="TickTime"/> property.
        /// </summary>
        private float tickTime;

        #endregion
    }
}