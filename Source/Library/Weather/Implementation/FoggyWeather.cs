// <copyright file="FoggyWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.FoggyWeather class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Overlays;

    /// <summary>
    /// Represents foggy <see cref="IWeather"/>.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class FoggyWeather : LinearEffectWeather
    {
        /// <summary>
        /// Gets or sets the color of the fog effect.
        /// </summary>
        public Microsoft.Xna.Framework.Color Color
        {
            get
            {
                return this.fogOverlay.Color;
            }

            set
            {
                this.fogOverlay.Color = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoggyWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The WeatherMachine that has triggered the new FoggyWeather.
        /// </param>
        public FoggyWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this FoggyWeather.
        /// </summary>
        /// <param name="texture">
        /// The texture that contains the scrollable fog image.
        /// </param>
        /// <param name="scroll">
        /// The starting scroll value of the overlay.
        /// </param>
        /// <param name="scrollSpeed">
        /// The fog movement speed of this FoggyWeather. 
        /// </param>
        /// <param name="totalTime">
        /// The total this FoggyWeather should last.
        /// </param>
        /// <param name="maxDensityEffect">
        /// The effect value at the peak of the FoggyWeather.
        /// </param>
        /// <param name="startMaxDensityTime">
        /// The time (in seconds) on the time table of this FoggyWeather the <paramref name="maxDensityEffect"/>
        /// is set as the current effect value.
        /// </param>
        /// <param name="endMaxDensityTime">
        /// The time (in seconds) on the time table of this FoggyWeather the <paramref name="maxDensityEffect"/>
        /// is not set as the current effect value anymore.
        /// </param>
        public void Setup(
                Texture2D texture,
                Vector2   scroll,
                Vector2   scrollSpeed,
                float     totalTime,
                float     maxDensityEffect,
                float     startMaxDensityTime,
                float     endMaxDensityTime
            )
        {
            base.Setup( totalTime, 0.0f, maxDensityEffect, 0.0f, startMaxDensityTime, endMaxDensityTime );

            fogOverlay = new ScrollingTextureOverlay( texture );

            fogOverlay.Scroll      = scroll;
            fogOverlay.ScrollSpeed = scrollSpeed;
            fogOverlay.Alpha       = 0.0f;
        }
        
        /// <summary>
        /// Updates this FoggyWeather.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            this.fogOverlay.Alpha = this.GetLinearEffect();
        }

        /// <summary>
        /// Starts this FoggyWeather.
        /// </summary>
        public override void Start()
        {
            this.Scene.Add( this.fogOverlay );
            base.Start();
        }

        /// <summary>
        /// Stops this FoggyWeather.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        public override void Stop( bool informWeatherMachine )
        {
            this.Scene.Remove( this.fogOverlay );
            base.Stop( informWeatherMachine );
        }

        /// <summary>
        /// The ScrollingTextureOverlay that is used to visualize this FoggyWeather.
        /// </summary>
        private ScrollingTextureOverlay fogOverlay;
    }
}
