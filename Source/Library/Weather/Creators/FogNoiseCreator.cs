// <copyright file="FogNoiseCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.FogNoiseCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom.Xna;

    /// <summary>
    /// Represents an <see cref="IWeatherCreator"/> that creates foggy and
    /// noisy IWeather at the same time. 
    /// This class can't be inherited
    /// </summary>
    public sealed class FogNoiseCreator : MultiWeatherCreator, IContentLoadable
    {
        /// <summary>
        /// Gets the <see cref="FogCreator"/> that creates the foggy IWeather of this FogNoiseCreator.
        /// </summary>
        public FogCreator Fog
        {
            get { return this.fogCreator; }
        }

        /// <summary>
        /// Gets the <see cref="NoiseCreator"/> that creates the noisy IWeather of this FogNoiseCreator.
        /// </summary>
        public NoiseCreator Noise
        {
            get { return this.noiseCreator; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FogNoiseCreator"/> class.
        /// </summary>   
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public FogNoiseCreator( IZeldaServiceProvider serviceProvider )
            : base( 2, serviceProvider )
        {
            this.fogCreator = new FogCreator( serviceProvider );
            this.AddCreator( fogCreator );

            this.noiseCreator = new NoiseCreator( serviceProvider );
            this.AddCreator( noiseCreator );
            
            this.fogCreator.MinimumDensity = 0.4f;
            this.fogCreator.MaximumDensity = 0.7f;
        }

        public void LoadContent()
        {
            this.fogCreator.LoadContent();
        }

        /// <summary>
        /// The <see cref="FogCreator"/> that creates the foggy IWeather of this FogNoiseCreator.
        /// </summary>
        private readonly FogCreator fogCreator;

        /// <summary>
        /// The <see cref="NoiseCreator"/> that creates the noisy IWeather of this FogNoiseCreator.
        /// </summary>
        private readonly NoiseCreator noiseCreator;
    }
}