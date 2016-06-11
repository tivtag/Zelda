// <copyright file="OcarinaBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.OcarinaBox class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Ocarina
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Encapsulates all ocarina related functionality.
    /// </summary>
    public sealed class OcarinaBox
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether the owner of this OcarinaBox
        /// is currently playing a Song.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return this.activeSong != null;
            }
        }

        /// <summary>
        /// Gets the ocarina Instrument this OcarinaBox contains.
        /// </summary>
        public Instrument Ocarina
        {
            get 
            {
                return this.ocarina;
            }
        }

        /// <summary>
        /// Gets the <see cref="Song"/>s the owner of this OcarinaBox has learned.
        /// </summary>
        public IEnumerable<Song> KnownSongs
        {
            get 
            { 
                return this.knownSongs;
            }
        }

        /// <summary>
        /// Gets the number of <see cref="Song"/>s the owner of this OcarinaBox has learned. 
        /// </summary>
        public int KnownSongCount
        {
            get 
            {
                return this.knownSongs.Count;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the OcarinaBox class.
        /// </summary>
        /// <param name="owner">
        /// The PlayerEntity that owns the new OcarinaBox.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to zelda-related services.
        /// </param>
        internal OcarinaBox( Zelda.Entities.PlayerEntity owner, IZeldaServiceProvider serviceProvider )
        {
            this.owner = owner;
            this.ocarina = Instrument.LoadOcarina( serviceProvider.AudioSystem );
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to play the song with the given Notes.
        /// </summary>
        /// <param name="notes">
        /// The notes to play.
        /// </param>
        /// <returns>
        /// true if the song has started to play;
        /// otherwise false.
        /// </returns>
        public bool PlaySong( IEnumerable<Note> notes )
        {
            if( this.activeSong != null )
                return false;

            Song song = this.FindSong( notes );

            if( song != null )
            {
                this.activeSong = song;
                this.activeSong.Music.Ended += this.OnActiveSongEnded;
                this.activeSong.Play( this.owner );
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the <see cref="Song"/> at the given zero-based <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Song to get.
        /// </param>
        /// <returns>
        /// The requested Song.
        /// </returns>
        public Song GetSong( int index )
        {
            return this.knownSongs[index];
        }

        /// <summary>
        /// Gets a value indicating whether the given index is a valid index
        /// into the list of songs the owner of this OcarinaBox knows.
        /// </summary>
        /// <param name="songIndex">
        /// The index to validate.
        /// </param>
        /// <returns>
        /// Returns whether the given index is a valid song index.
        /// </returns>
        public bool IsValidIndex( int songIndex )
        {
            return songIndex >= 0 && songIndex < this.KnownSongCount;
        }

        /// <summary>
        /// Tries to find a <see cref="Song"/> in this OcarinaBox
        /// </summary>
        /// <param name="notes">
        /// The notes of the song to find.
        /// </param>
        /// <returns>
        /// The Song; or null if there is no such Song in this OcarinaBox.
        /// </returns>
        private Song FindSong( IEnumerable<Note> notes )
        {
            foreach( var song in this.knownSongs )
            {
                if( song.HasEqualNotes( notes ) )
                {
                    return song;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this OcarinaBox contains
        /// a Song with the given Type.
        /// </summary>
        /// <param name="type">
        /// The type of the Song to check for.
        /// </param>
        /// <returns>
        /// True if this OcarinaBox contains the Song of the given Type
        /// otherwise false.
        /// </returns>
        [Pure]
        public bool HasSong( Type type )
        {
            foreach( var song in this.knownSongs )
            {
                if( song.GetType().Equals( type ) )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds the given Song to this OcarinaBox;
        /// learning the owner of the Box the Song.
        /// </summary>
        /// <param name="song">
        /// The song to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="song"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If there already exists a Song of the same type in this OcarinaBox.
        /// </exception>
        public void AddSong( Song song )
        {
            Contract.Requires<ArgumentNullException>( song != null );
            Contract.Requires<ArgumentException>( !this.HasSong( song.GetType() ), Resources.Error_TheGivenSongIsAlreadyKnown );

            this.knownSongs.Add( song );
        }

        /// <summary>
        /// Adds the given Song to this OcarinaBox;
        /// learning the owner of the Box the Song.
        /// </summary>
        /// <param name="songType">
        /// The type of the song to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="songType"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If there already exists a Song of the same type in this OcarinaBox.
        /// </exception>
        private void AddSong( Type songType )
        {
            var song = (Song)Activator.CreateInstance( songType );
            song.Setup( serviceProvider );

            this.AddSong( song );
        }

        /// <summary>
        /// Called when the currently active Song has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnActiveSongEnded( SongMusic sender )
        {
            sender.Ended -= this.OnActiveSongEnded;
            this.activeSong = null;
        }

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.knownSongs.Count );
            foreach( var song in this.knownSongs )
            {
                context.Write( Atom.ReflectionExtensions.GetTypeName( song.GetType() ) );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.knownSongs.Clear();

            int count = context.ReadInt32();
            this.knownSongs.Capacity = count;

            for( int i = 0; i < count; ++i )
            {
                string typeName = context.ReadString();
                var    type     = Type.GetType( typeName );

                this.AddSong( type );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the Song that is currently playing; if any.
        /// </summary>
        private Song activeSong;

        /// <summary>
        /// The ocarina Instrument.
        /// </summary>
        private readonly Instrument ocarina;

        /// <summary>
        /// The list of <see cref="Song"/>s known to this OcarinaBox.
        /// </summary>
        private readonly List<Song> knownSongs = new List<Song>();

        /// <summary>
        /// The player that owns this OcarinaBox.
        /// </summary>
        private readonly Zelda.Entities.PlayerEntity owner;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}