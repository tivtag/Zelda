// <copyright file="NoiseCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.NoiseCreator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom;
    using Atom.Math;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates noisy weather.
    /// </summary>
    public sealed class NoiseCreator : LinearEffectWeatherCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoiseCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public NoiseCreator( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.deviceService = serviceProvider.GetService<Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService>();
            this.effectLoader = serviceProvider.GetService<Atom.Xna.Effects.IEffectLoader>();

            // Set default settings:
            this.MinimumTime = 10000.0f;
            this.MaximumTime = 25000.0f;

            this.MinimumSpeed = new Vector2( 0.005f, 0.005f );
            this.MaximumSpeed = new Vector2( 0.018f, 0.018f );

            this.MaximumDensityStartFactor = 0.8f;
            this.MaximumDensityEndFactor   = 0.2f;

            this.MinimumDensity = 0.07f;
            this.MaximumDensity = 0.095f;
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
            NoisyWeather noise = new NoisyWeather( weatherMachine );

            float time = this.GetTime( weatherMachine );
            Vector2 noiseDir = new Vector2(
                this.Rand.RandomRange( -1.0f, 1.0f ),
                this.Rand.RandomRange( -1.0f, 1.0f )
            );
            noiseDir.Normalize();

            noise.Setup(
                this.effectLoader,
                this.deviceService,
                this.GetSpeed(),
                noiseDir,
                time,
                this.GetDensity(),
                time * this.MaximumDensityStartFactor,
                time * this.MaximumDensityEndFactor
            );

            return new IWeather[1] { noise };
        }
        
        /// <summary>
        /// Provides access to the Microsoft.Xna.Framework.Graphics.GraphicsDevice.
        /// </summary>
        private readonly Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService deviceService;

        /// <summary>
        /// Provides a mechanism that allows loading of effect asserts.
        /// </summary>
        private readonly Atom.Xna.Effects.IEffectLoader effectLoader;
    }
}
