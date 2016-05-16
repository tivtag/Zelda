// <copyright file="SnowCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.SnowCreator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather.Creators
{
    using System;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates
    /// <see cref="SnowyWeather"/>.
    /// </summary>
    public sealed class SnowCreator : LinearEffectWeatherCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnowCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public SnowCreator( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            // Set default settings:
            this.MinimumTime = 10000.0f;
            this.MaximumTime = 21000.0f;

            this.MaximumDensityStartFactor = 0.6f;
            this.MaximumDensityEndFactor   = 0.2f;

            this.MinimumDensity = 0.25f;
            this.MaximumDensity = 0.5f;
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
            SnowyWeather snow = new SnowyWeather( weatherMachine );

            float time = this.GetTime( weatherMachine );

            snow.Setup(
                time,
                this.GetDensity(),
                time * this.MaximumDensityStartFactor,
                time * this.MaximumDensityEndFactor,
                this.ServiceProvider
            );

            return new IWeather[1] { snow };
        }
    }
}
