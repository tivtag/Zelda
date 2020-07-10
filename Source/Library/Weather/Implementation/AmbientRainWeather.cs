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
    using System;
    using Atom.Math;

    /// <summary>
    /// Represents the ambient sound effects that surrounds rainy <see cref="IWeather"/>.
    /// This class can't be inherited.
    /// </summary>
    internal class AmbientRainWeather : LinearEffectWeather, IPauseableWeather
    {
        /// <summary>
        /// The name of the rain sound sample.
        /// </summary>
        private const string RainSampleName = "Rain.ogg";

        /// <summary>
        /// The name of the thunder sound sample.
        /// </summary>
        private const string ThunderSampleName = "Thunder.ogg";

        /// <summary>
        /// Sets the time-range between two thunders.
        /// </summary>
        public FloatRange TimeBetweenThunders
        {
            //get
            //{
            //    return this.timeBetweenThunders;
            //}
            // Unused.
            set
            {
                this.timeBetweenThunders = value;
            }
        }

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientRainWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new RainyWeather.
        /// </param>
        public AmbientRainWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this RainAmbientSoundWeather.
        /// </summary>
        /// <param name="totalTime">
        /// The total this RainAmbientSoundWeather should last.
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
        public virtual void Setup(
                float totalTime,
                float maxDensityEffect,
                float startMaxDensityTime,
                float endMaxDensityTime,
                IZeldaServiceProvider serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
            base.Setup( totalTime, 0.0f, maxDensityEffect, 0.0f, startMaxDensityTime, endMaxDensityTime );

            this.LoadRainSample( serviceProvider.AudioSystem );
            this.LoadThunderSample( serviceProvider.AudioSystem );
        }

        /// <summary>
        /// Loads the RainSample that is playing in the background 
        /// while this RainyWeather is active.
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem that should be used to load the sample.
        /// </param>
        private void LoadRainSample( Atom.Fmod.AudioSystem audioSystem )
        {
            this.rainSound = audioSystem.GetSample( RainSampleName );

            if( this.rainSound != null )
            {
                try
                {
                    this.rainSound.LoadAsSample( true );
                }
                catch( Atom.Fmod.AudioException )
                {
                    // Loading .oggs crashes on platforms
                    // with faulty drivers.
                    this.rainSound = null;
                }
            }
        }

        /// <summary>
        /// Loads the ThunderSample that is occasionally played
        /// while this RainyWeather is active.
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem that should be used to load the sample.
        /// </param>
        private void LoadThunderSample( Atom.Fmod.AudioSystem audioSystem )
        {
            this.thunderSound = audioSystem.GetSample( ThunderSampleName );

            if( this.thunderSound != null )
            {
                var mode = Atom.Fmod.Native.MODE._3D_LINEARROLLOFF |
                           Atom.Fmod.Native.MODE.CREATESAMPLE |
                           Atom.Fmod.Native.MODE.LOOP_OFF;
                try
                {
                    this.thunderSound.Load( mode );
                }
                catch( Atom.Fmod.AudioException )
                {
                    // Loading .oggs crashes on platforms
                    // with faulty drivers.
                    this.thunderSound = null;
                }
            }

            this.timeUntilNextThunder = 10.0f + this.timeBetweenThunders.GetRandomValue( this.rand );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this RainyWeather.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( updateContext.IsMainUpdate )
            {
                UpdateBasedOnLinearEffect( updateContext, this.GetLinearEffect() );
            }
        }

        protected virtual void UpdateBasedOnLinearEffect( ZeldaUpdateContext updateContext, float linearEffect )
        {
            this.UpdateThunder( updateContext );
            this.UpdateRainSample( linearEffect );
        }

        /// <summary>
        /// Updates the thunder sound logic.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateThunder( ZeldaUpdateContext updateContext )
        {
            this.timeUntilNextThunder -= updateContext.FrameTime;

            if( this.timeUntilNextThunder <= 0.0f )
            {
                this.PlayThunder();
                this.RandomizeTimeUntilNextThunder();
            }
        }

        /// <summary>
        /// Starts to play a thunder at a random position.
        /// </summary>
        private void PlayThunder()
        {
            if( this.thunderSound != null )
            {
                var channel = this.thunderSound.Play( true );

                Vector2 thunderPosition = this.GetRandomThunderPosition();

                channel.Priority = 50;
                channel.Set3DAttributes( thunderPosition.X, thunderPosition.Y );
                channel.Set3DMinMaxDistance( 32.0f, 1000.0f );

                channel.Unpause();
            }
        }

        /// <summary>
        /// Gets the position of the thunder.
        /// </summary>
        /// <returns>
        /// A random center position.
        /// </returns>
        private Vector2 GetRandomThunderPosition()
        {
            Vector2 playerPosition = this.GetPlayerPosition();
            Vector2 direction = this.GetRandomThunderDirection();
            float distance = this.GetRandomThunderDistance();

            return playerPosition + (direction * distance);
        }

        /// <summary>
        /// Gets the position of the player in world-space.
        /// </summary>
        /// <returns>
        /// The center position of the player.
        /// </returns>
        private Vector2 GetPlayerPosition()
        {
            var player = this.Scene.Player;

            if( player != null )
            {
                return player.Collision.Center;
            }

            var camera = this.Scene.Camera;
            return camera.ViewArea.Center;
        }

        /// <summary>
        /// Gets the direction from the player to the thunder source.
        /// </summary>
        /// <returns>
        /// A random direction vector.
        /// </returns>
        private Vector2 GetRandomThunderDirection()
        {
            Vector2 direction = new Vector2(
                rand.RandomRange( -1.0f, 1.0f ),
                rand.RandomRange( -1.0f, 1.0f )
            );

            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// Gets the distance the thunder is away.
        /// </summary>
        /// <returns>
        /// A random distance.
        /// </returns>
        private float GetRandomThunderDistance()
        {
            return rand.RandomRange( 0.0f, 950.0f );
        }

        /// <summary>
        /// Randomizes the value of the timeUntilNextThunder field
        /// based on the timeBetweenThunders setting.
        /// </summary>
        private void RandomizeTimeUntilNextThunder()
        {
            this.timeUntilNextThunder = this.timeBetweenThunders.GetRandomValue( this.rand );
        }

        /// <summary>
        /// Updates the volume of the rain sample.
        /// </summary>
        /// <param name="linearEffect">
        /// The current strength of this RainyWeather.
        /// </param>
        private void UpdateRainSample( float linearEffect )
        {
            if( this.rainChannel != null )
            {
                this.rainChannel.Volume = Math.Min( linearEffect, 1.0f );
            }
        }

        /// <summary>
        /// Starts this RainyWeather.
        /// </summary>
        public override void Start()
        {
            this.StartPlayingRainSample();

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
            this.StopPlayingRainSample();

            base.Stop( informWeatherMachine );
        }

        /// <summary>
        /// Starts playing the rain sample.
        /// </summary>
        private void StartPlayingRainSample()
        {
            if( this.rainSound != null )
            {
                this.rainChannel = this.rainSound.Play( true );

                this.rainChannel.Priority = 25; // Quite important.
                this.rainChannel.Volume = 0.0f;
                this.rainChannel.Unpause();
            }
        }

        /// <summary>
        /// Stops playing the rain sample.
        /// </summary>
        private void StopPlayingRainSample()
        {
            if( this.rainChannel != null )
            {
                this.rainChannel.Stop();
                this.rainChannel = null;
            }
        }

        /// <summary>
        /// Pauses this RainyWeather.
        /// </summary>
        public void Pause()
        {
            this.StopPlayingRainSample();
        }

        /// <summary>
        /// Unpauses this RainyWeather.
        /// </summary>
        public void Unpause()
        {
            this.StartPlayingRainSample();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The sound sample that gets played in the background while rainy weather is active.
        /// </summary>
        private Atom.Fmod.Sound rainSound;

        /// <summary>
        /// The sound channel that is playing the rainSound.
        /// </summary>
        private Atom.Fmod.Channel rainChannel;

        /// <summary>
        /// The thunder sample that gets occasionally played.
        /// </summary>
        private Atom.Fmod.Sound thunderSound;

        /// <summary>
        /// Stores the time range between two thunders.
        /// </summary>
        private FloatRange timeBetweenThunders = new FloatRange( 2.0f, 15.0f );

        /// <summary>
        /// The time until the next thunder.
        /// </summary>
        private float timeUntilNextThunder;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        #endregion
    }
}
