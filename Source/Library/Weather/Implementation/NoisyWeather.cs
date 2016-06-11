// <copyright file="NoisyWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.NoisyWeather class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    using System;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Overlays;
    
    /// <summary>
    /// Represents foggy <see cref="IWeather"/> that uses a NoiseOverlay.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class NoisyWeather : LinearEffectWeather, IDisposable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="NoisyWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new NoisyWeather.
        /// </param>
        public NoisyWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this NoisyWeather.
        /// </summary>
        /// <param name="effectLoader">
        /// Provides a mechanism that allows loading of effect asserts.
        /// </param>
        /// <param name="deviceService">
        /// Provides access to the Microsoft.Xna.Framework.Graphics.GraphicsDevice.
        /// </param>
        /// <param name="scrollSpeed">
        /// The fog movement speed of this NoisyWeather. 
        /// </param>
        /// <param name="scrollDirection">
        /// The direction the noise is scrolling.
        /// </param>
        /// <param name="totalTime">
        /// The total this NoisyWeather should last.
        /// </param>
        /// <param name="maxDensityEffect">
        /// The effect value at the peak of the NoisyWeather.
        /// </param>
        /// <param name="startMaxDensityTime">
        /// The time (in seconds) on the time table of this NoisyWeather the <paramref name="maxDensityEffect"/>
        /// is set as the current effect value.
        /// </param>
        /// <param name="endMaxDensityTime">
        /// The time (in seconds) on the time table of this NoisyWeather the <paramref name="maxDensityEffect"/>
        /// is not set as the current effect value anymore.
        /// </param>
        public void Setup(
                Atom.Xna.Effects.IEffectLoader effectLoader,
                IGraphicsDeviceService deviceService,
                Vector2 scrollSpeed,
                Vector2 scrollDirection,
                float totalTime,
                float maxDensityEffect,
                float startMaxDensityTime,
                float endMaxDensityTime
            )
        {
            base.Setup( totalTime, 0.0f, maxDensityEffect, 0.0f, startMaxDensityTime, endMaxDensityTime );

            this.noiseOverlay = new NoiseOverlay( effectLoader, deviceService );

            var noise = this.noiseOverlay.Noise;
            noise.Alpha     = 0.0f;
            noise.MoveSpeed = scrollSpeed;
            noise.MoveDirection = scrollDirection;
            noise.Overcast = 1.5f;
            noise.BaseColor = new Microsoft.Xna.Framework.Color( 0, 0, 0, 255 );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this NoisyWeather.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            this.noiseOverlay.Noise.Alpha = GetLinearEffect();
        }

        /// <summary>
        /// Starts this NoisyWeather.
        /// </summary>
        public override void Start()
        {
            this.Scene.Add( this.noiseOverlay );
            base.Start();
        }

        /// <summary>
        /// Stops this NoisyWeather.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        public override void Stop( bool informWeatherMachine )
        {
            this.Scene.Remove( this.noiseOverlay );
            base.Stop( informWeatherMachine );
        }
        
        /// <summary>
        /// Disposes this NoisyWeather.
        /// </summary>
        public void Dispose()
        {
            if( this.noiseOverlay != null )
            {
                this.Stop( true );

                this.noiseOverlay.Dispose();
                this.noiseOverlay = null;
            }

            GC.SuppressFinalize( this );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The NoiseOverlay that is used to visualize this NoisyWeather.
        /// </summary>
        private NoiseOverlay noiseOverlay;

        #endregion
    }
}
