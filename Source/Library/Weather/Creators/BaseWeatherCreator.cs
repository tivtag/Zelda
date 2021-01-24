// <copyright file="BaseWeatherCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.BaseWeatherCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;

    /// <summary>
    /// Defines the abstract base class of all <see cref="IWeatherCreator"/> objects.
    /// </summary>
    public abstract class BaseWeatherCreator : IWeatherCreator
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWeatherCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        protected BaseWeatherCreator( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        protected Atom.Math.RandMT Rand
        {
            get
            {
                return this.serviceProvider.Rand;
            }
        }

        /// <summary>
        /// Gets an object that provides fast access to game-related services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get
            { 
                return this.serviceProvider; 
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates a new instance of the IWeather this IWeatherCreator creates.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine for which the IWeather should be created for.
        /// </param>
        /// <returns> 
        /// The <see cref="IWeather"/> instances that make up the weather.
        /// </returns>
        public abstract IWeather[] CreateWeather( IWeatherMachine weatherMachine );

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
