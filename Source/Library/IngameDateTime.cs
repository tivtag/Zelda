// <copyright file="IngameDateTime.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IngameDateTime class.<
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;

    /// <summary>
    /// Encapsulates the flow of date and time within the game.
    /// This is a sealed class.
    /// </summary>
    public sealed class IngameDateTime
    {
        /// <summary>
        /// Gets or sets the current <see cref="DateTime"/>.
        /// </summary>
        public DateTime Current
        {
            get 
            {
                return this.current;
            }

            set
            {
                this.current = value;
            }
        }

        /// <summary>
        /// Gets or sets the number ingame-seconds that are added in one second.
        /// </summary>
        public float TickSpeed
        {
            get 
            {
                return this.tickSpeed; 
            }

            set
            {
                this.tickSpeed = value;
                this.invTickSpeed = this.tickSpeed != 0.0f ? (1.0f / this.tickSpeed) : 0.0f;
            }
        }

        /// <summary>
        /// Gets the number ingame-seconds that are added in one second; inversed.
        /// </summary>
        /// <remarks>
        /// This value is cached for improved perfomance.
        /// </remarks>
        public float InverseTickSpeed
        {
            get
            { 
                return this.invTickSpeed;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IngameDateTime"/> class,
        /// initialized with default settings.
        /// </summary>
        public IngameDateTime()
        {
            this.TickSpeed = 96.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IngameDateTime"/> class.
        /// </summary>
        /// <param name="startDateTime"> The start date/time. </param>
        /// <param name="tickSpeed"> The number ingame-seconds that are added in one second. </param>
        public IngameDateTime( DateTime startDateTime, float tickSpeed )
        {
            this.current   = startDateTime;
            this.TickSpeed = tickSpeed;
        }
        
        /// <summary>
        /// Updates the current time of the <see cref="IngameDateTime"/> object.
        /// Usually called once per frame.
        /// </summary>
        /// <param name="updateContext"> 
        /// The current IUpdateContext. 
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.current = this.current.AddSeconds( this.tickSpeed * updateContext.FrameTime );
        }

        /// <summary>
        /// Adds the given number of hours to the current IngameDateTime.
        /// </summary>
        /// <param name="hours">
        /// The number of hours to add.
        /// </param>
        internal void AddHours( int hours )
        {
            this.current = this.current.AddHours( hours );
        }

        /// <summary>
        /// Converts the current ingame time into a short string.
        /// </summary>
        /// <returns>
        /// A string that contains the short time string representation of the current System.DateTime object.
        /// </returns>
        public string ToShortTimeString()
        {
            return this.current.ToString( "t", System.Globalization.DateTimeFormatInfo.InvariantInfo );
        }
        
        /// <summary>
        /// The current date/time.
        /// </summary>
        private DateTime current;

        /// <summary>
        /// The number of ingame-seconds that shall be added in one second.
        /// </summary>
        private float tickSpeed;

        /// <summary>
        /// Inversed tick speed value. Cached to reduce overhead.
        /// </summary>
        private float invTickSpeed;
    }
}
