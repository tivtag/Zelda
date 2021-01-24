// <copyright file="RainCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.RainCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates
    /// rainy weather.
    /// </summary>
    public sealed class RainCreator : LinearEffectWeatherCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RainCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public RainCreator( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            // Set default settings:
            this.MinimumTime = 12000.0f;
            this.MaximumTime = 25000.0f;

            this.MaximumDensityStartFactor = 0.6f;
            this.MaximumDensityEndFactor   = 0.2f;

            this.MinimumDensity = 0.45f;
            this.MaximumDensity = 0.8f;
        }

        /// <summary>
        /// Creates a new instance of the IWeather this IWeatherCreator creates.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine for which the IWeather should be created for.
        /// </param>
        /// <returns> 
        /// The <see cref="IWeather"/> instances that make up the weather.
        /// </returns>
        public override IWeather[] CreateWeather( IWeatherMachine weatherMachine )
        {
            RainyWeather rain = new RainyWeather( weatherMachine );

            float time = this.GetTime( weatherMachine );

            rain.TimeBetweenThunders = new Atom.Math.FloatRange( 10.0f, 25.0f );

            rain.Setup(
                time,
                this.GetDensity(),
                time * this.MaximumDensityStartFactor,
                time * this.MaximumDensityEndFactor,
                this.ServiceProvider
            );

            return new IWeather[1] { rain };
        }
    }
}
