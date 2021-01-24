// <copyright file="PlayAudioSampleEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.PlayAudioSampleEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using Atom.Fmod;

    /// <summary>
    /// Represents an <see cref="Atom.Events.Event"/> that when
    /// triggered plays a <see cref="Sound"/> sample.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PlayAudioSampleEvent : Atom.Events.Event, IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the <see cref="Sound"/> sample to play.
        /// </summary>
        /// <remarks>Samples must be in the Content/Samples/ folder.</remarks>
        /// <value>The default value is null.</value>
        public string SampleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the volumne the sample should play at,
        /// 0 = silence and 1 = full volumne.
        /// </summary>
        /// <value>The default value is 1.</value>
        public float Volumne
        {
            get
            { 
                return this.volumne;
            }

            set
            {
                this.volumne = Atom.Math.MathUtilities.Clamp( value, 0.0f, 1.0f );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Triggers this PlayAudioSampleEvent, playing the <see cref="Sound"/> sample.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this Event.
        /// </param>
        public override void Trigger( object obj )
        {
            Sound sound = audioSystem.GetSample( this.SampleName );

            if( sound != null )
            {
                sound.LoadAsSample( false );

                // Start paused to apply the volumne.
                var channel = sound.Play( true );
                channel.Volume = this.Volumne;

                // And play the sample (:
                channel.IsPaused = false;
            }
        }

        /// <summary>
        /// Setups this PlayAudioSampleEvent.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.audioSystem = serviceProvider.AudioSystem;
        }

        #region > Storage <

        /// <summary>
        /// Serializes this PlayAudioSampleEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Volumne );
            context.Write( this.SampleName ?? string.Empty );
        }

        /// <summary>
        /// Deserializes this PlayAudioSampleEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Volumne = context.ReadSingle();
            this.SampleName = context.ReadString();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the <see cref="Volumne"/> property.
        /// </summary>
        private float volumne = 1.0f;

        /// <summary>
        /// The <see cref="AudioSystem"/> object.
        /// </summary>
        private AudioSystem audioSystem;

        #endregion
    }
}
