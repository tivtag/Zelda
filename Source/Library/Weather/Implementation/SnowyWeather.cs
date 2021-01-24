// <copyright file="SnowyWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.SnowyWeather class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    using System;
    using Zelda.Graphics.Particles;
    using Zelda.Overlays;

    /// <summary>
    /// Represents snowy <see cref="IWeather"/>.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class SnowyWeather : LinearEffectWeather, IPauseableWeather, IReloadable
    {
        /// <summary>
        /// The name of the snow sound sample.
        /// </summary>
        private const string SnowSampleName = "Snow.wav";

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowyWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new SnowyWeather.
        /// </param>
        public SnowyWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this SnowyWeather.
        /// </summary>
        /// <param name="totalTime">
        /// The total this SnowyWeather should last.
        /// </param>
        /// <param name="maxDensityEffect">
        /// The effect value at the peak of the SnowyWeather.
        /// </param>
        /// <param name="startMaxDensityTime">
        /// The time (in seconds) on the time table of this SnowyWeather the <paramref name="maxDensityEffect"/>
        /// is set as the current effect value.
        /// </param>
        /// <param name="endMaxDensityTime">
        /// The time (in seconds) on the time table of this SnowyWeather the <paramref name="maxDensityEffect"/>
        /// is not set as the current effect value anymore.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup(
                float totalTime,
                float maxDensityEffect,
                float startMaxDensityTime,
                float endMaxDensityTime,
                IZeldaServiceProvider serviceProvider
            )
        {
            base.Setup( totalTime, 0.0f, maxDensityEffect, 0.0f, startMaxDensityTime, endMaxDensityTime );

            this.effect = RainSnowParticleEffect.CreateSnow( 
                this.WeatherMachine.Settings.SnowSettings, 
                serviceProvider
            );

            this.effectOverlay = new ScrollingParticleEffectOverlay( this.effect );
            this.LoadSnowSample( serviceProvider.AudioSystem );
        }

        /// <summary>
        /// Loads the SnowSample that is playing in the background 
        /// while this SnowyyWeather is active.
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem that should be used to load the sample.
        /// </param>
        private void LoadSnowSample( Atom.Fmod.AudioSystem audioSystem )
        {
            this.snowSound = audioSystem.GetSample( SnowSampleName );

            if( this.snowSound != null )
            {
                this.snowSound.LoadAsSample( true );
            }
        }
        
        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            var emitter = this.effect.Emitters[0];
            emitter.ParticleTexture = serviceProvider.TextureLoader.Load( emitter.ParticleTexture.Name );
        }

        /// <summary>
        /// Updates this SnowyWeather.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( updateContext.IsMainUpdate )
            {
                float effect = this.GetLinearEffect();

                this.effect.SetDensity( effect );
                this.UpdateSnowSample( effect );
            }
        }

        /// <summary>
        /// Updates the volume of the snow sample.
        /// </summary>
        /// <param name="linearEffect">
        /// The current strength of this SnowyWeather.
        /// </param>
        private void UpdateSnowSample( float linearEffect )
        {
            if( this.snowChannel != null )
            {
                this.snowChannel.Volume = Math.Min( linearEffect * 2.5f, 1.0f );
            }
        }

        /// <summary>
        /// Starts this SnowyWeather.
        /// </summary>
        public override void Start()
        {
            this.StartPlayingSnowSample();

            this.Scene.Add( this.effectOverlay );
            base.Start();
        }

        /// <summary>
        /// Stops this SnowyWeather.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        public override void Stop( bool informWeatherMachine )
        {
            this.StopPlayingSnowSample();

            this.Scene.Remove( this.effectOverlay );
            base.Stop( informWeatherMachine );
        }

        /// <summary>
        /// Starts playing the snow sample.
        /// </summary>
        private void StartPlayingSnowSample()
        {
            if( this.snowSound != null )
            {
                this.snowChannel = this.snowSound.Play( true );

                this.snowChannel.Priority = 25; // Quite important.
                this.snowChannel.Volume = 0.0f;
                this.snowChannel.Unpause();
            }
        }

        /// <summary>
        /// Stops playing the snow sample.
        /// </summary>
        private void StopPlayingSnowSample()
        {
            if( this.snowChannel != null )
            {
                this.snowChannel.Stop();
                this.snowChannel = null;
            }
        }

        /// <summary>
        /// Pauses this SnowyWeather.
        /// </summary>
        public void Pause()
        {
            this.StopPlayingSnowSample();
        }

        /// <summary>
        /// Unpauses this SnowyWeather.
        /// </summary>
        public void Unpause()
        {
            this.StartPlayingSnowSample();
        }
        
        /// <summary>
        /// The ParticleEffect that spawns the individual Particles.
        /// </summary>
        private RainSnowParticleEffect effect;

        /// <summary>
        /// The ScrollingParticleEffectOverlay that is used to visualize this SnowyWeather.
        /// </summary>
        private ScrollingParticleEffectOverlay effectOverlay;

        /// <summary>
        /// The sound sample that gets played in the background while snowy weather is active.
        /// </summary>
        private Atom.Fmod.Sound snowSound;

        /// <summary>
        /// The sound channel that is playing the snowSound.
        /// </summary>
        private Atom.Fmod.Channel snowChannel;
    }
}
