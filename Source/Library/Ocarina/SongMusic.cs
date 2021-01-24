// <copyright file="SongMusic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.SongMusic class and Zelda.Ocarina.SongMusicPlayMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina
{
    using System;
    using Atom;
    using Atom.Fmod;
    using Zelda.Audio;
    using Zelda.Entities;

    /// <summary>
    /// Enumerates the different ways the <see cref="SongMusic"/> of a <see cref="Song"/> is played.
    /// </summary>
    public enum SongMusicPlayMode
    {
        /// <summary>
        /// No music is played.
        /// </summary>
        None,

        /// <summary>
        /// The music is simply played.
        /// </summary>
        Normal,

        /// <summary>
        /// The current background music is changed.
        /// </summary>
        Background
    }

    public enum SongMusicAfterMode
    {
        Normal,
        PlayNothing
    }

    /// <summary>
    /// Encapsulates the music of a <see cref="Song"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SongMusic
    {
        #region [ Constants ]

        /// <summary>
        /// The full name of the music resource that gets played
        /// by default when a Song has been played.
        /// </summary>
        private const string DefaultResourceName = "Content/Samples/Bling_1.ogg";

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when playing of this SongMusic has started.
        /// </summary>
        public event SimpleEventHandler<SongMusic> Started;

        /// <summary>
        /// Fired when playing of this SongMusic has ended.
        /// </summary>
        public event SimpleEventHandler<SongMusic> Ended;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the duration of this SongMusic in seconds.
        /// </summary>
        public float Duration
        {
            get
            {
                if( this.music == null )
                    return 0.0f;

                return this.music.GetLength( Atom.Fmod.Native.TIMEUNIT.MS ) / 1000.0f;
            }
        }

        /// <summary>
        /// Gets or sets the volume at which the music is played;
        /// where 0 = silent and 1 = full volume.
        /// </summary>
        /// <value>The default value is 1.</value>
        public float Volume
        {
            get
            {
                return this.volume;
            }

            set
            {
                this.volume = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SongMusic class.
        /// </summary>
        /// <param name="resourceName">
        /// The name that uniquely identifies the music resource.
        /// </param>
        /// <param name="playMode">
        /// The SongMusicPlayMode to use.
        /// </param>
        internal SongMusic( string resourceName, SongMusicPlayMode playMode )
        {
            if( resourceName == null )
                resourceName = DefaultResourceName;

            this.resourceName = resourceName;
            this.playMode = playMode;
        }

        /// <summary>
        /// Setups this SongMusic.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider.AudioSystem != null )
            {
                this.music = serviceProvider.AudioSystem.Get( this.resourceName );

                if( this.music == null )
                {
                    serviceProvider.Log.WriteLine( "The music resource " + this.resourceName + " could not be loaded." );
                }
            }
        }

        /// <summary>
        /// Creates a new SongMusic instance that uses the default settings.
        /// </summary>
        /// <returns>
        /// The new SongMusic instance.
        /// </returns>
        internal static SongMusic CreateDefault()
        {
            return new SongMusic(
                DefaultResourceName, 
                SongMusicPlayMode.Normal
            );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Plays the Music associated with a Song.
        /// </summary>
        /// <param name="player">
        /// The player of the music.
        /// </param>
        public void Play( Zelda.Entities.PlayerEntity player )
        {
            if( this.music == null )
            {
                this.OnStarted();
                this.OnEnded();
                return;
            }

            this.Load();
            this.player = player;
            
            switch( this.playMode )
            {
                case SongMusicPlayMode.None:
                    this.OnStarted();
                    this.OnMusicEnded( null );
                    break;

                case SongMusicPlayMode.Normal:
                    this.channel = this.music.Play();
                    this.channel.Ended += OnMusicEnded;
                    this.OnStarted();
                    break;

                case SongMusicPlayMode.Background:
                    // Playing of the music is deffered.
                    var backgroundMusic = player.IngameState.BackgroundMusic;

                    backgroundMusic.Changed += OnBackgroundMusicChanged;

                    backgroundMusic.ChangeTo( this.music );
                    backgroundMusic.ManualChangeAllowed = false;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Loads this song music.
        /// </summary>
        private void Load()
        {
            if( music != null )
            {
                this.music.LoadAsMusic( false );

                if( this.volume != 1.0f )
                {
                    float frequency, volume, pan;
                    int priority;

                    this.music.GetDefaults( out frequency, out volume, out pan, out priority );
                    volume = this.volume;
                    this.music.SetDefaults( frequency, volume, pan, priority );
                }
            }
        }

        /// <summary>
        /// Called when the Background music has been changed to the Music of this Song.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="channel">The channel that has been changed to.</param>
        private void OnBackgroundMusicChanged( object sender, Channel channel )
        {
            var backgroundMusic = (BackgroundMusicComponent)sender;
            backgroundMusic.Changed -= this.OnBackgroundMusicChanged;

            this.channel = channel;

            if( this.channel != null )
            {
                this.channel.Ended += this.OnMusicEnded;
                this.OnStarted();
            }
        }

        /// <summary>
        /// Gets called when the channel has ended playing the Song's music.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        private void OnMusicEnded( Channel sender )
        {
            if( this.channel != null )
            {
                this.channel.Ended -= this.OnMusicEnded;
                this.channel = null;
            }

            if( this.player != null )
            {
                if( this.playMode == SongMusicPlayMode.Background )
                {
                    var backgroundMusic = this.player.IngameState.BackgroundMusic;
                    backgroundMusic.Changed -= this.OnBackgroundMusicChanged;
                    backgroundMusic.ManualChangeAllowed = true;
                }

                this.player = null;
            }

            OnEnded();
        }

        private void OnEnded()
        {
            this.Ended.Raise( this );
        }
        
        /// <summary>
        /// Fires the Started event.
        /// </summary>
        private void OnStarted()
        {
            if( this.Started != null )
            {
                this.Started( this );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The music resource.
        /// </summary>
        private Sound music;

        /// <summary>
        /// Identifies the Channel the music is playing on.
        /// </summary>
        private Channel channel;

        /// <summary>
        /// The PlayerEntity that is playing this Song.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The volume at which the music is played.
        /// </summary>
        private float volume = 1.0f;

        /// <summary>
        /// The name of the music resource that is played
        /// when this Song was played.
        /// </summary>
        private readonly string resourceName;

        /// <summary>
        /// States how this SongMusic should be played.
        /// </summary>
        private readonly SongMusicPlayMode playMode;

        #endregion
    }
}