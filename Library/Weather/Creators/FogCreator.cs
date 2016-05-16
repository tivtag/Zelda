// <copyright file="FogCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.FogCreator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates
    /// foggy weather.
    /// </summary>
    public class FogCreator : LinearEffectWeatherCreator, IContentLoadable
    {
        /// <summary>
        /// States the name of the texture that is loaded by default by this FogCreator.
        /// </summary>
        private const string DefaultTextureName = "CloudsA";

        /// <summary>
        /// Gets or sets the texture that is used to visualize
        /// the fog.
        /// </summary>
        public Texture2D Texture
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FogCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public FogCreator( IZeldaServiceProvider serviceProvider )
            : this( DefaultTextureName, serviceProvider )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FogCreator"/> class.
        /// </summary>
        /// <param name="textureAssetName">
        /// The name of the fog texture.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public FogCreator( string textureAssetName, IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            // Set default settings:
            this.MinimumTime = 10000.0f;
            this.MaximumTime = 21000.0f;

            this.MinimumSpeed = new Vector2( 2.0f / 3.0f, -2.0f / 3.0f );
            this.MaximumSpeed = new Vector2( 3.0f / 3.0f, -1.0f / 3.0f );

            this.MaximumDensityStartFactor = 0.8f;
            this.MaximumDensityEndFactor   = 0.2f;

            this.MinimumDensity = 0.5f;
            this.MaximumDensity = 0.8f;
            this.textureAssetName = textureAssetName;
        }

        public void LoadContent()
        {
            this.Texture = this.ServiceProvider.TextureLoader.Load( this.textureAssetName );
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
            FoggyWeather fog = new FoggyWeather( weatherMachine );
            float time = this.GetTime( weatherMachine );

            Vector2 scroll = new Vector2(
                this.Rand.RandomRange( -500.0f, 500.0f ),
                this.Rand.RandomRange( -500.0f, 500.0f )
            );

            fog.Setup(
                this.Texture,
                scroll,
                this.GetSpeed(),
                time,
                this.GetDensity(),
                time * this.MaximumDensityStartFactor,
                time * this.MaximumDensityEndFactor
            );
            fog.Color = GetFogColor()
                .WithAlpha( 0 );

            return new IWeather[1] { fog };
        }

        protected virtual Microsoft.Xna.Framework.Color GetFogColor()
        {
            return Microsoft.Xna.Framework.Color.White;
        }
        
        private readonly string textureAssetName;
    }
}
