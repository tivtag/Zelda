// <copyright file="BaseWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.BaseWeather class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    using System.Diagnostics;

    /// <summary>
    /// Defines the abstract base class of all <see cref="IWeather"/> objects.
    /// </summary>
    internal abstract class BaseWeather : IWeather
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new BaseWeather.
        /// </param>
        protected BaseWeather( IWeatherMachine weatherMachine )
        {
            Debug.Assert( weatherMachine != null );

            this.scene  = weatherMachine.Scene;
            this.weatherMachine = weatherMachine;
        }        

        /// <summary>
        /// Setups this BaseWeather.
        /// </summary>
        /// <param name="totalTime">
        /// The total time this IWeather should last.
        /// </param>
        protected void Setup( float totalTime )
        {
            this.totalTime = totalTime;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the time (in seconds) left until this IWeather stopps.
        /// </summary>
        public float TimeLeft
        {
            get 
            { 
                return this.time;
            }
        }

        /// <summary>
        /// Gets the total time (in seconds) this IWeather lasts.
        /// </summary>
        public float TotalTime
        {
            get 
            {
                return this.totalTime;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> this IWeather is part of.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            { 
                return this.scene;
            }
        }

        /// <summary>
        /// Gets the IWeatherMachine that owns this BaseWeather.
        /// </summary>
        protected IWeatherMachine WeatherMachine
        {
            get
            {
                return this.weatherMachine;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this IWeather.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            this.time -= updateContext.FrameTime;

            if( this.time <= 0.0f )
            {
                this.Stop( true );
            }
        }

        /// <summary>
        /// Starts this IWeather.
        /// </summary>
        public virtual void Start()
        {
            this.Reset();
        }

        /// <summary>
        /// Stops this IWeather.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        public virtual void Stop( bool informWeatherMachine )
        {
            this.time = 0.0f;

            if( informWeatherMachine )
            {
                this.weatherMachine.OnWeatherStopped( this );
            }
        }

        /// <summary>
        /// Resets the state of this IWeather.
        /// </summary>
        public virtual void Reset()
        {
            this.time = this.totalTime;
        }

        #endregion
        
        #region [ Fields ]

        /// <summary>
        /// States the total time this IWeather lasts.
        /// </summary>
        private float totalTime;

        /// <summary>
        /// States the time this IWeather has left until it gets removed.
        /// </summary>
        private float time;

        /// <summary>
        /// Identifies the ZeldaScene that owns this IWeather.
        /// </summary>
        private readonly ZeldaScene scene;

        /// <summary>
        /// Identifies the weather machine that has triggered this IWeather.
        /// </summary>
        private readonly IWeatherMachine weatherMachine;

        #endregion
    }
}
