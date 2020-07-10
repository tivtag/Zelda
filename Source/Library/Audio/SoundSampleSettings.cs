// <copyright file="SoundSampleSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Audio.SoundSampleSettings class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Audio
{
    using System.ComponentModel;
    using Atom.Fmod;
    using Atom.Math;
    using Zelda.Saving;

    /// <summary>
    /// Encapsulates the settings of a simple Sound Sample.
    /// This is a sealed class.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class SoundSampleSettings : IZeldaSetupable, ISaveable
    {
        /// <summary>
        /// Gets or sets the name of the sound sample that is played
        /// when a Projectile hits.
        /// </summary>
        public string SampleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the volumne the sound sample should
        /// be played at.
        /// </summary>
        /// <value>The default value is 1.</value>
        public float Volumne
        {
            get { return this.volumne; }
            set { this.volumne = value; }
        }

        /// <summary>
        /// Gets or sets the distance at which the sound is still audible.
        /// </summary>
        public FloatRange Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sample is looping.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool IsLooping
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the SoundSampleSettings class.
        /// </summary>
        public SoundSampleSettings()
        {
        }

        /// <summary>
        /// Setups this ProjectileHitSoundSettings.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.audioSystem = serviceProvider.AudioSystem;
        }

        /// <summary>
        /// Plays the sound sample at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">
        /// The position of the projectile.
        /// </param>
        /// <returns>
        /// The Channel in which the sample is playing.
        /// Might be null.
        /// </returns>
        public Channel PlayAt( Vector2 position )
        {
            if( this.sound == null )
            {
                this.LoadSound();

                if( this.sound == null )
                {
                    return null;
                }
            }

            Channel channel = this.sound.Play( true );

            channel.Volume = this.Volumne;
            channel.Set3DAttributes( position.X, position.Y );
            channel.Set3DMinMaxDistance( distance.Minimum, distance.Maximum );
            
            channel.IsPaused = false;
            return channel;
        }

        /// <summary>
        /// Loads the Sound sample.
        /// </summary>
        private void LoadSound()
        {
            if( this.audioSystem == null || string.IsNullOrEmpty( this.SampleName ) )
                return;

            this.sound = audioSystem.GetSample( this.SampleName );
            
            if( this.sound != null )
            {
                var mode =
                    Atom.Fmod.Native.MODE._3D |
                    Atom.Fmod.Native.MODE._3D_LINEARROLLOFF;

                if( this.IsLooping )
                {
                    mode |= Atom.Fmod.Native.MODE.LOOP_NORMAL;
                }
                
                this.sound.Load( mode );
            }
        }
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( this.SampleName ?? string.Empty );
            context.Write( this.Volumne );

            context.Write( this.distance );
            context.Write( this.IsLooping );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            this.SampleName = context.ReadString();
            this.Volumne = context.ReadSingle();

            if( version >= 2 )
            {
                this.distance = context.ReadFloatRange();
                this.IsLooping = context.ReadBoolean();
            }
        }

        /// <summary>
        /// The actual -loaded- sound.
        /// </summary>
        private Sound sound;

        /// <summary>
        /// Represents the backend storage field of the <see cref="Volumne"/> property.
        /// </summary>
        private float volumne = 1.0f;

        /// <summary>
        /// Represents the backend storage field of the <see cref="Distance"/> property.
        /// </summary>
        private FloatRange distance = new FloatRange( 16.0f * 1.5f, 16.0f * 8 );

        /// <summary>
        /// The AudioSystem responsible for loading the Sound.
        /// </summary>
        private AudioSystem audioSystem;
    }
}
