// <copyright file="AmbientRainStormCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.AmbientRainStormCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates
    /// stormy rainy weather (sound only).
    /// </summary>
    public sealed class AmbientRainStormCreator : LinearEffectWeatherCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientRainStormCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public AmbientRainStormCreator( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            // Set default settings:
            this.MinimumTime = 8500.0f;
            this.MaximumTime = 18000.0f;

            this.MaximumDensityStartFactor = 0.8f;
            this.MaximumDensityEndFactor = 0.2f;

            this.MinimumDensity = 0.8f;
            this.MaximumDensity = 1.15f;
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
            AmbientRainWeather rain = new AmbientRainWeather( weatherMachine );

            float time = this.GetTime( weatherMachine );
            rain.TimeBetweenThunders = new Atom.Math.FloatRange( 6.0f, 25.0f );

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
