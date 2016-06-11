// <copyright file="ShotSoundEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ShotSoundEmitter class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom.Fmod;
    using Atom.Math;
    using Zelda.Timing;

    /// <summary>
    /// Represents a one-shot <see cref="Sound"/> effect that can be added to a <see cref="ZeldaScene"/>.
    /// It doesn't keep track of it's active sound channel(s).
    /// </summary>
    public class ShotSoundEmitter : ZeldaEntity, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the <see cref="Sound"/> this SoundEntity is playing.
        /// </summary>
        public Sound Sound
        {
            get
            {
                return this.sound;
            }

            set
            {
                if( value == this.Sound )
                    return;

                this.sound = value;
                this.LoadSound( sound );
            }
        }

        /// <summary>
        /// Gets or sets the range of volumnes the sound is playing at; 
        /// where 0 = silence and 1 = full effect.
        /// </summary>
        public FloatRange Volume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the sound is looping.
        /// </summary>
        public bool IsLooping
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the range of time in seconds between two triggers of this ShotSoundEmitter.
        /// </summary>
        public FloatRange TriggerPeriod
        {
            get
            {
                return this.timer.TimeRange;
            }

            set
            {
                this.timer.TimeRange = value;
                this.timer.Reset();
            }
        }

        /// <summary>
        /// Initializes a new instance of the ShotSoundEmitter class.
        /// </summary>
        public ShotSoundEmitter()
        {
            this.Volume = new FloatRange( 1.0f, 1.0f );
            this.timer.TimeRange = new FloatRange( 1.0f, 10.0f );
        }

        /// <summary>
        /// Setups this ShotSoundEmitter.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
            this.timer.Setup( serviceProvider );
        }

        /// <summary>
        /// Updates this ShotSoundEmitter.
        /// </summary>
        /// <param name="updateContext">
        /// The current update context.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            timer.Update( updateContext );

            if( timer.HasEnded )
            {
                Play();
                timer.Reset();
            }

            base.Update( updateContext );
        }
        
        /// <summary>
        /// Triggers this ShotSoundEmitter.
        /// </summary>
        public void Play()
        {
            if( this.sound == null )
                return;

            Channel channel = CreateChannel( this.sound );
            SetupChannel( channel );

            // Start playing now.
            channel.Unpause();
        }

        /// <summary>
        /// Loads the given Sound object, by default as a simple music file.
        /// </summary>
        /// <param name="sound">
        /// The sound object. Is never null.
        /// </param>
        protected virtual void LoadSound( Sound sound )
        {
            sound.LoadAsSample( this.IsLooping );
        }

        /// <summary>
        /// Creates a new Channel object of the given Sound object.
        /// </summary>
        /// <param name="sound">
        /// The sound object. Is never null.
        /// </param>
        /// <returns>
        /// The new channel.
        /// </returns>
        protected virtual Channel CreateChannel( Sound sound )
        {
            // Start playing the sound paused so
            // we have enough time to initialize the channel.
            return sound.Play( true );
        }

        /// <summary>
        /// Setups the given channel for playback.
        /// </summary>
        /// <param name="channel">
        /// The channel object. Is never null.
        /// </param>
        protected virtual void SetupChannel( Channel channel )
        {
            channel.Volume = this.Volume.GetRandomValue( this.rand );
        }

        private readonly ResetableRangeTimer timer = new ResetableRangeTimer();

        /// <summary>
        /// The <see cref="Sound"/> this SoundEntity is playing.
        /// </summary>
        private Sound sound;

        /// <summary>
        /// A random number generator. Used for the random elements of the ShotSoundEmitter.
        /// </summary>
        private RandMT rand;
    }
}