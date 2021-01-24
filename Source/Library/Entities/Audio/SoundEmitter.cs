// <copyright file="SoundEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.SoundEmitter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom.Fmod;

    /// <summary>
    /// Represents a looping <see cref="Sound"/> effect that can be added to a <see cref="ZeldaScene"/>.
    /// </summary>
    public class SoundEmitter : ZeldaEntity, ISceneChangeListener
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

                // Notify.
                OnSoundChangedInternal( sound );
            }
        }

        /// <summary>
        /// Gets the current <see cref="Channel"/> object.
        /// </summary>
        public Channel Channel
        {
            get
            {
                return this.channel;
            }
        }

        /// <summary>
        /// Gets or sets the volumne the sound is playing at; 
        /// where 0 = silence and 1 = full effect.
        /// </summary>
        public float Volume
        {
            get
            {
                return this.volumne;
            }

            set 
            {
                this.volumne = value;
                OnVolumeChanged( value );
            }
        }
        
        /// <summary>
        /// Gets the combined volume of the channel after 3d sound, volume 
        /// and channel group volume  calculations have been performed on it.  
        /// </summary>
        public float Audibility
        {
            get
            {
                return this.channel == null ? 0.0f : this.channel.Audibility;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the sound is looping.
        /// </summary>
        public bool IsLooping
        {
            get 
            { 
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SoundEmitter has been muted.
        /// </summary>
        public bool IsMuted
        {
            get
            {
                return this.channel == null ? true : this.channel.IsMuted;
            }

            set
            {
                this.isMuted = value;
                if( this.channel != null )
                    this.channel.IsMuted = value;
            }
        }
        
        /// <summary>
        /// Called when the current Sound has changed.
        /// </summary>
        /// <param name="sound">
        /// The new Sound value.
        /// </param>
        private void OnSoundChangedInternal( Sound sound )
        {
            this.OnSoundChanged( sound );

            if( sound != null )
            {
                if( this.channel != null )
                {
                    if( this.channel.IsPlaying )
                        this.channel.Stop();

                    this.channel = null;
                }

                this.LoadSound( sound );
                this.channel = CreateChannel( sound );
                this.SetupChannel( this.channel );
            }
            else
            {
                this.channel = null;
            }
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
        /// Called when the current Sound has changed.
        /// </summary>
        /// <param name="sound">
        /// The new Sound value.
        /// </param>
        protected virtual void OnSoundChanged( Sound sound )
        {
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
            return sound.Play();
        }

        /// <summary>
        /// Setups the given channel for playback.
        /// </summary>
        /// <param name="channel">
        /// The channel object. Is never null.
        /// </param>
        protected virtual void SetupChannel( Channel channel )
        {
            channel.IsMuted = this.isMuted;
            channel.Volume  = this.volumne;
        }
        
        /// <summary>
        /// Notifies this SoundEntity that a scene change has occured.
        /// </summary>
        /// <param name="changeType">
        /// States whether the current scene has changed away or to its current scene.
        /// </param>
        public void NotifySceneChange( ChangeType changeType )
        {
            if( changeType == ChangeType.To )
            {
                if( this.sound != null && this.channel == null )
                {
                    this.channel = this.CreateChannel( this.sound );
                    this.SetupChannel( channel );
                }
            }
            else // if( changeType == ChangeType.Away )
            {
                // We release the channel for reuse.
                if( this.channel != null )
                {
                    this.channel.Stop();
                    this.channel = null;
                }
            }
        }

        /// <summary>
        /// Called when this SoundEmitter gets added to the given Scene.
        /// </summary>
        /// <param name="scene">
        /// The related scene.
        /// </param>
        public override void AddToScene( ZeldaScene scene )
        {
            base.AddToScene( scene );

            if( this.channel != null )
                this.channel.IsPaused = false;
        }

        /// <summary>
        /// Called when this SoundEmitter gets removed from its current Scene.
        /// </summary>
        public override void RemoveFromScene()
        {
            base.RemoveFromScene();

            if( this.channel != null )
                this.channel.IsPaused = true;
        }

        /// <summary>
        /// Called when the value of the Volume property has changed.
        /// </summary>
        protected virtual void OnVolumeChanged( float value )
        {
            if( this.channel != null )
                this.channel.Volume = value;
        }

        /// <summary>
        /// The volumne the sound is playing at; 
        /// where 0 = silence and 1 = full effect.
        /// </summary>
        private float volumne = 1.0f;

        /// <summary>
        /// States whether the channel has been muted.
        /// </summary>
        private bool isMuted;

        /// <summary>
        /// The <see cref="Sound"/> this SoundEntity is playing.
        /// </summary>
        private Sound sound;

        /// <summary>
        /// The channel the sound is playing on.
        /// </summary>
        private Channel channel;
    }
}