// <copyright file="ChangeBackgroundMusicEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ChangeBackgroundMusicEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System;
    using Zelda.Audio;
    using Atom;

    /// <summary>
    /// Represents an Event that when triggered changes
    /// the currently playing background music.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ChangeBackgroundMusicEvent : ZeldaEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the background music
        /// to change to should continue looping.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool IsLooping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name that uniquely identifies the music resource
        /// this ChangeBackgroundMusicEvent should change to.
        /// </summary>
        /// <value>The default value is null.</value>
        public string MusicName
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this ChangeBackgroundMusicEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this ChangeBackgroundMusicEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var ingameState = this.Scene.IngameState;
            if( ingameState == null )
                return;

            var music = ingameState.BackgroundMusic;

            // Don't change if we are already playing the right song.
            if( music.IsPlaying( this.MusicName ) )
                return;

            if( this.IsLooping )
                music.Mode = BackgroundMusicMode.Loop;

            music.ChangeTo( this.MusicName );
        }

        /// <summary>
        /// Serializes this ChangeBackgroundMusicEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Data
            context.Write( this.IsLooping );
            context.Write( this.MusicName ?? string.Empty );
        }

        /// <summary>
        /// Deserializes this ChangeBackgroundMusicEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Data
            this.IsLooping = context.ReadBoolean();
            this.MusicName = context.ReadString();
        }
    }
}
