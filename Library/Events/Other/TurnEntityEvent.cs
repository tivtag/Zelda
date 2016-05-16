// <copyright file="TurnEntityEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.TurnEntityEvent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Events
{
    using System;
    using Atom;
    using Atom.Events;
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Represents a LongTermEvent that when executed
    /// turns a ZeldaEntity N-times into a specific direction by
    /// changing its Transform.Direction.
    /// This class can't be inherited.
    /// </summary>
    public sealed class TurnEntityEvent : LongTermEvent
    {
        #region [ Events ]

        /// <summary>
        /// Called when this TurnEntityEvent has completed a 360° turn of the Entity.
        /// </summary>
        public event SimpleEventHandler<TurnEntityEvent> TurnCompleted;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the ZeldaEntity turned by this TurnEntityEvent.
        /// </summary>
        public ZeldaEntity Entity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of complete turns that should be executed.
        /// </summary>
        public int TurnCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time between individual direction changes.
        /// </summary>
        public float TimeBetweenDirectionChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reduction of the time between two direction changes
        /// applied after a turn has completed in %.
        /// </summary>
        public float TurnSpeedIncreasePerTurn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direction the Entity is turned.
        /// </summary>
        public TurnDirection TurnDirection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating how many 360° turns this TurnEntityEvent has completed.
        /// </summary>
        public int TurnsCompleted
        {
            get
            {
                return this.turnsCompleted;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the TurnEntityEvent class.
        /// </summary>
        public TurnEntityEvent()
        {
            this.TurnCount = 1;
            this.TurnDirection = TurnDirection.Clockwise;
            this.TimeBetweenDirectionChanges = 0.2f;
            this.TurnSpeedIncreasePerTurn = 0.2f;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this TurnEntityEvent is triggered.
        /// </summary>
        /// <param name="obj">
        /// The object that wants to trigger this TurnEntityEvent.
        /// </param>
        /// <returns>
        /// Whether this TurnEntityEvent should be triggering.
        /// </returns>
        protected override bool Triggering( object obj )
        {
            this.Reset();                        
            return base.Triggering( obj );
        }

        /// <summary>
        /// Resets this TurnEntityEvent.
        /// </summary>
        private void Reset()
        {
            this.directionChangeCount = 0;
            this.turnsCompleted = 0;
            this.time = this.TimeBetweenDirectionChanges;
            this.timeLeft = this.time;
        }

        /// <summary>
        /// Updates this TurnEntityEvent.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            this.timeLeft -= updateContext.FrameTime;

            if( this.timeLeft <= 0.0f )
            {
                this.timeLeft = this.time;
                this.ChangeDirection();
            }
        }

        /// <summary>
        /// Changes the direction of the <see cref="Entity"/>.
        /// </summary>
        private void ChangeDirection()
        {
            if( this.Entity != null )
            {
                var transform = this.Entity.Transform;
                transform.Direction = transform.Direction.GetNext( this.TurnDirection );
            }

            ++this.directionChangeCount;

            if( this.directionChangeCount >= 4 )
            {
                this.OnTurnCompleted();
            }
        }

        /// <summary>
        /// Called when this TurnEntityEvent has completed one 360° turn of the Entity.
        /// </summary>
        private void OnTurnCompleted()
        {
            ++this.turnsCompleted;
            this.directionChangeCount = 0;
            this.time -= (this.time * TurnSpeedIncreasePerTurn);

            if( this.turnsCompleted >= this.TurnCount )
            {
                this.Stop();
            }

            if( this.TurnCompleted != null )
            {
                this.TurnCompleted( this );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The time left (in seconds) until the Entity changes direction again.
        /// </summary>
        private float timeLeft;

        /// <summary>
        /// The time between two direction changes.
        /// </summary>
        private float time;

        /// <summary>
        /// The number of direction changes applied to the Entity.
        /// </summary>
        private int directionChangeCount;

        /// <summary>
        /// The number of total turns applied to the Entity.
        /// </summary>
        private int turnsCompleted;

        #endregion
    }
}
