// <copyright file="RainyWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.RainyWeather class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    using Zelda.Graphics.Particles;
    using Zelda.Overlays;

    /// <summary>
    /// Represents rainy <see cref="IWeather"/>.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class RainyWeather : AmbientRainWeather, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RainyWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new RainyWeather.
        /// </param>
        public RainyWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this RainyWeather.
        /// </summary>
        /// <param name="totalTime">
        /// The total this RainyWeather should last.
        /// </param>
        /// <param name="maxDensityEffect">
        /// The effect value at the peak of the RainyWeather.
        /// </param>
        /// <param name="startMaxDensityTime">
        /// The time (in seconds) on the time table of this RainyWeather the <paramref name="maxDensityEffect"/>
        /// is set as the current effect value.
        /// </param>
        /// <param name="endMaxDensityTime">
        /// The time (in seconds) on the time table of this RainyWeather the <paramref name="maxDensityEffect"/>
        /// is not set as the current effect value anymore.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup(
                float totalTime,
                float maxDensityEffect,
                float startMaxDensityTime,
                float endMaxDensityTime,
                IZeldaServiceProvider serviceProvider
            )
        {
            base.Setup( totalTime, maxDensityEffect, startMaxDensityTime, endMaxDensityTime, serviceProvider );

            this.effect = RainSnowParticleEffect.CreateRain(
                this.WeatherMachine.Settings.RainSettings,
                serviceProvider
            );

            this.effectOverlay = new ScrollingParticleEffectOverlay( this.effect );
        }

        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            var emitter = this.effect.Emitters[0];
            emitter.ParticleTexture = serviceProvider.TextureLoader.Load( emitter.ParticleTexture.Name );
        }

        protected override void UpdateBasedOnLinearEffect( ZeldaUpdateContext updateContext, float linearEffect )
        {
            this.effect.SetDensity( linearEffect );
            base.UpdateBasedOnLinearEffect( updateContext, linearEffect );
        }

        /// <summary>
        /// Starts this RainyWeather.
        /// </summary>
        public override void Start()
        {
            this.Scene.Add( this.effectOverlay );
            base.Start();
        }

        /// <summary>
        /// Stops this RainyWeather.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        public override void Stop( bool informWeatherMachine )
        {
            this.Scene.Remove( this.effectOverlay );
            base.Stop( informWeatherMachine );
        }

        /// <summary>
        /// The ParticleEffect that spawns the individual Particles.
        /// </summary>
        private RainSnowParticleEffect effect;

        /// <summary>
        /// The ScrollingParticleEffectOverlay that is used to visualize this RainyWeather.
        /// </summary>
        private ScrollingParticleEffectOverlay effectOverlay;
    }
}
