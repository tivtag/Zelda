// <copyright file="BackgroundMusicComponent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Audio.BackgroundMusicComponent class and Zelda.Audio.BackgroundMusicMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Audio
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Fmod;
    using Atom.Fmod.Native;
    using Atom.Math;
    
    /// <summary>
    /// Manages the music that is playing in the background of the game.
    /// This class can't be inherited.
    /// </summary>
    public sealed class BackgroundMusicComponent
    {
        /// <summary>
        /// Fired when the currently playing background music has changed.
        /// </summary>
        public event Atom.RelaxedEventHandler<Atom.Fmod.Channel> Changed;

        /// <summary>
        /// Gets or sets the current mode of this BackgroundMusicComponent.
        /// </summary>
        /// <value>The default value is <see cref="BackgroundMusicMode.Random"/>.</value>
        public BackgroundMusicMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                if( value == this.Mode )
                    return;

                this.mode = value;

                switch( value )
                {
                    case BackgroundMusicMode.Loop:
                        this.next = this.current;
                        break;

                    case BackgroundMusicMode.Random:
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fading in
        /// of music is enabled.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool FadeInIsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fading out
        /// of music is enabled.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool FadeOutIsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of background music that
        /// play if the BackgroundMusicComponent's mode is set to
        /// <see cref="BackgroundMusicMode.Random"/>.
        /// </summary>
        /// <value>
        /// The list of background music.
        /// </value>
        public BackgroundMusic[] MusicList
        {
            get
            {
                return this.musicList;
            }

            set
            {
                Contract.Requires<ArgumentNullException>( value != null );

                this.musicList = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ChannelGroup"/> all background music runs under.
        /// </summary>
        public Atom.Fmod.ChannelGroup ChannelGroup
        {
            get
            {
                return this.channelGroup;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changing the background music
        /// using <see cref="ChangeTo(String)"/> is currently allowed.
        /// </summary>
        public bool ManualChangeAllowed
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundMusicComponent"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public BackgroundMusicComponent( IZeldaServiceProvider serviceProvider )
        {
            this.rand        = serviceProvider.Rand;
            this.audioSystem = serviceProvider.AudioSystem;
            this.serviceProvider = serviceProvider;

            this.ManualChangeAllowed = true;
            this.FadeInIsEnabled  = true;
            this.FadeOutIsEnabled = true;
        }

        /// <summary>
        /// Initializes this BackgroundMusicComponent.
        /// </summary>
        public void Initialize()
        {
            if( audioSystem.IsInitialized )
            {
                this.channelGroup = new Atom.Fmod.ChannelGroup( "Background Music", audioSystem );
                this.audioSystem.MasterChannelGroup.AddChildGroup( this.channelGroup );
            }

            this.ingameState = this.serviceProvider.GetService<IIngameState>();
        }

        /// <summary>
        /// Updates this BackgroundMusicComponent.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            ProcessChangeToRequest();
            if( currentChannel == null || !currentChannel.IsPlaying )
                return;

            if( this.isStoppingManually )
            {
                UpdateManualStop( updateContext );
            }
            else
            {
                UpdateFadeInOut();
            }
        }

        private void ProcessChangeToRequest()
        {
            if( requestedChangeTo != null )
            {
                ChangeToImpl( requestedChangeTo );
                requestedChangeTo = null;
            }
        }

        private void UpdateManualStop( ZeldaUpdateContext updateContext )
        {
            if( FadeOutIsEnabled )
            {
                this.fadeOutTimeLeft -= updateContext.FrameTime;

                if( this.fadeOutTimeLeft <= 0.0f )
                {
                    OnManuallyStopEnded();
                }
                else
                {
                    this.currentChannel.Volume = fadeOutTimeLeft / FadeOutTime;
                }
            }
            else
            {
                OnManuallyStopEnded();
            }
        }

        private void UpdateFadeInOut()
        {
            uint position = currentChannel.GetPosition( TIMEUNIT.MS );
            uint length = current.GetLength( TIMEUNIT.MS );

            float time = (float)position / 1000.0f;
            float timeEnd = (float)length / 1000.0f;

            // Fade In:
            if( this.FadeInIsEnabled && time <= FadeInTime )
            {
                float volumne = time / FadeInTime;

                // To fight rounding errors:
                if( volumne >= 0.99f )
                    volumne = 1.0f;

                this.currentChannel.Volume = volumne;
            }
            else if( this.FadeOutIsEnabled && (time >= timeEnd - FadeOutTime) )
            {
                // Fade Out:
                this.currentChannel.Volume = (timeEnd - time) / FadeOutTime;
            }
            else
            {
                this.currentChannel.Volume = 1.0f;
            }
        }

        private void OnManuallyStopEnded()
        {
            this.currentChannel.Stop();
            this.isStoppingManually = false;
        }
        
        /// <summary>
        /// Tells this BackgroundMusicComponent to change
        /// to the background music with the given name.
        /// </summary>
        /// <param name="musicName">
        /// The name that uniquely identifies the
        /// music resource to change to.
        /// </param>
        public void ChangeTo( string musicName )
        {
            var music = this.audioSystem.GetMusic( musicName );
            if( music == null )
                return;

            music.LoadAsMusic( false );
            this.ChangeTo( music );
        }

        /// <summary>
        /// Tells this BackgroundMusicComponent to change 
        /// to the given background <paramref name="music"/>.
        /// </summary>
        /// <param name="music">
        /// The music to change to. (Must be loaden!)
        /// </param>
        public void ChangeTo( Atom.Fmod.Sound music )
        {
            Contract.Requires<ArgumentNullException>( music != null );

            if( !this.ManualChangeAllowed )
            {
                return;
            }

            this.RequestChangeTo( music );
        }

        /// <summary>
        /// Tells this BackgroundMusicComponent to change 
        /// to the given background <paramref name="music"/>.
        /// </summary>
        /// <param name="music">
        /// The music to change to. (Must be loaden!)
        /// </param>
        private void RequestChangeTo( Atom.Fmod.Sound music )
        {
            this.requestedChangeTo = music;
        }

        private void ChangeToImpl( Atom.Fmod.Sound music )
        {
            if( this.current == null || !this.currentChannel.IsPlaying )
            {
                // Unhook old.
                Stop();
                
                // Hook new.
                this.current = music;
                this.currentChannel = music.Play( true );
                this.currentChannel.Priority = 1;
                this.currentChannel.Ended += this.OnCurrentChannel_Ended;
                this.currentChannel.ChannelGroup = this.channelGroup;

                if( this.FadeInIsEnabled )
                {
                    this.currentChannel.Volume = 0.0f;
                }
                else
                {
                    this.currentChannel.Volume = 1.0f;
                }

                this.isStoppingManually = false;
                this.currentChannel.Unpause();

                this.FindNext();
                this.OnChanged();
            }
            else
            {
                this.next = music;

                if( !isStoppingManually )
                {
                    this.fadeOutTimeLeft = FadeOutTime;
                    this.isStoppingManually = true;
                }
            }
        }

        /// <summary>
        /// Fires the <see cref="Changed"/> event.
        /// </summary>
        private void OnChanged()
        {
            if( this.Changed != null )
            {
                this.Changed( this, this.currentChannel );
            }
        }

        /// <summary>
        /// Tells this BackgroundMusicComponent to randomly change 
        /// to one of the background music in the <see cref="MusicList"/>.
        /// </summary>
        public void ChangeToRandom()
        {
            var music = this.SelectRandomMusic();
            this.Mode = BackgroundMusicMode.Random;

            if( music != null && music != this.current )
            {
                this.ChangeTo( music );
            }
        }
                
        /// <summary>
        /// Changes the volume of the background music to the given value.
        /// </summary>
        /// <param name="newVolume">
        /// The volume (a value between 0 and 1) to change to.
        /// </param>
        internal void ChangeVolumeTo( float newVolume )
        {
            this.oldVolume = this.channelGroup.Volume;
            this.channelGroup.Volume = newVolume;
        }

        /// <summary>
        /// Restores the volume of the background music to the value
        /// before the last call to <see cref="ChangeVolumeTo"/>.
        /// </summary>
        internal void RestoreVolume()
        {
            this.channelGroup.Volume = oldVolume;
        }

        /// <summary>
        /// Gets a value indicating whether the music with the given name
        /// is currently playing in the background.
        /// </summary>
        /// <param name="musicName">
        /// The name that uniquely identifies the music.
        /// </param>
        /// <returns>
        /// Returns true if the music with the given name is currently playing;
        /// otherwise false.
        /// </returns>
        internal bool IsPlaying( string musicName )
        {
            if( this.current != null && this.current.Name.Equals( musicName, StringComparison.OrdinalIgnoreCase ) )
                return true;

            if( this.isStoppingManually )
            {
                if( this.next != null && this.next.Name.Equals( musicName, StringComparison.OrdinalIgnoreCase ) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the next music to play, depending on the current BackgroundMusicMode.
        /// </summary>
        private void FindNext()
        {
            switch( this.mode )
            {
                case BackgroundMusicMode.Random:
                    this.next = this.SelectRandomMusic();
                    break;

                case BackgroundMusicMode.Loop:
                    this.next = current;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Randomly selects a music from the music list.
        /// </summary>
        /// <returns>
        /// The music that has been selected.
        /// </returns>
        private Atom.Fmod.Sound SelectRandomMusic()
        {
            BackgroundMusic music = this.ActuallySelectRandomMusic();
            if( music == null )
                return null;

            Atom.Fmod.Sound musicResource = audioSystem.GetMusic( music.FileName );

            if( musicResource != null )
            {
                musicResource.LoadAsMusic( false );
            }

            return musicResource;
        }

        /// <summary>
        /// Randomly selects a <see cref="BackgroundMusic"/> from the musicList.
        /// </summary>
        /// <param name="recursionDepth">
        /// The number of times ActuallySelectRandomMusic has been called recursively;
        /// this happens when the IRequirement of a choosen BackgroundMusic has not been fulfilled.
        /// </param>
        /// <returns>
        /// The BackgroundMusic that has been selected;
        /// or null if none.
        /// </returns>
        private BackgroundMusic ActuallySelectRandomMusic( int recursionDepth = 0 )
        {
            const int MaximumRecursionDepth = 12;
            if( this.musicList.Length == 0 )
                return null;
                        
            if( recursionDepth >= MaximumRecursionDepth )
            {
                return this.musicList[0];
            }

            BackgroundMusic music = this.musicList[rand.RandomRange( 0, musicList.Length - 1 )];
            if( music == null )
                return null;

            if( music.Requirement != null )
            {
                if( !music.Requirement.IsFulfilledBy( this.ingameState.Player ) )
                {
                    return this.ActuallySelectRandomMusic( ++recursionDepth  );
                }
            }

            return music;
        }

        /// <summary>
        /// Called when the current song has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCurrentChannel_Ended( Atom.Fmod.Channel sender )
        {
            if( this.next == null )
                this.FindNext();

            if( this.next != null )
                this.RequestChangeTo( this.next );
        }

        /// <summary>
        /// Stops playing the current background music,
        /// without starting a new one.
        /// </summary>
        public void Stop()
        {
            if( this.currentChannel != null )
            {
                this.currentChannel.Ended -= this.OnCurrentChannel_Ended;
                this.currentChannel.Stop();

                this.currentChannel = null;
            }

            this.current = null;
            this.fadeOutTimeLeft = 0.0f;
            this.isStoppingManually = false;
        }
        
        /// <summary>
        /// Identifies the currently playing background music.
        /// </summary>
        private Atom.Fmod.Sound current;

        /// <summary>
        /// Identifies the channel the current background music is playing on.
        /// </summary>
        private Atom.Fmod.Channel currentChannel;

        /// <summary>
        /// The next music to play.
        /// </summary>
        private Atom.Fmod.Sound next;

        /// <summary>
        /// The current BackgroundMusicMode.
        /// </summary>
        private BackgroundMusicMode mode;

        /// <summary>
        /// The list of random songs.
        /// </summary>
        private BackgroundMusic[] musicList = new BackgroundMusic[0];

        /// <summary>
        /// States whether the current music is currently stopping to play.
        /// </summary>
        private bool isStoppingManually;

        /// <summary>
        /// The duration the music fades in/out for.
        /// </summary>
        private const float FadeInTime = 2.5f, FadeOutTime = 3.0f;

        /// <summary>
        /// The time that has passed since the fide in/out command.
        /// </summary>
        private float fadeOutTimeLeft;

        /// <summary>
        /// Stores the volume of the background music before the last call to <see cref="ChangeVolumeTo"/>.
        /// </summary>
        private float oldVolume = 1.0f;

        /// <summary>
        /// The sound to which a change request has been noted. Change requests are progressed once per frame.
        /// </summary>
        private Atom.Fmod.Sound requestedChangeTo;

        /// <summary>
        /// Idenfities the ChannelGroup under which the background music is grouped. 
        /// </summary>
        private Atom.Fmod.ChannelGroup channelGroup;

        /// <summary>
        /// Provides access to the current ingame state.
        /// </summary>
        private IIngameState ingameState;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        /// <summary>
        /// The Atom.Fmod.AudioSystem object.
        /// </summary>
        private readonly AudioSystem audioSystem;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }

    /// <summary>
    /// Enumerates the different modes the <see cref="BackgroundMusicComponent"/> supports.
    /// </summary>
    public enum BackgroundMusicMode
    {
        /// <summary>
        /// The next song is randomly selected from a list of songs.
        /// </summary>
        Random = 0,

        /// <summary>
        /// The current song is looping until the mode is changed.
        /// </summary>
        Loop
    }
}
