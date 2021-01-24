// <copyright file="Song.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Song class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Zelda.Entities;
    
    /// <summary>
    /// Represents a song that may be played by the player on an <see cref="Instrument"/>.
    /// </summary>
    /// <remarks>
    /// When a song has been successfully played the special effect of the Song is executed;
    /// examples would be: Teleportation, Time Change, Special Area Unlocks, ..
    /// </remarks>
    public abstract class Song : IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the list of <see cref="Note"/>s that make-up this Song,
        /// in the specified order.
        /// </summary>
        public IEnumerable<Note> Notes
        {
            get { return this.notes; }
        }

        /// <summary>
        /// Gets the data object descriping properties of this Song
        /// that are visible to the player.
        /// </summary>
        public SongDescriptionData DescriptionData
        {
            get { return this.descriptionData; }
        }

        /// <summary>
        /// Gets the <see cref="SongMusic"/> associated with this Song.
        /// </summary>
        public SongMusic Music
        {
            get
            { 
                return this.music; 
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the Song class.
        /// </summary>
        /// <param name="notes">
        /// The enumeration of Notes that make-up the new Song.
        /// </param>
        /// <param name="descriptionData">
        /// The data visible to the player that is descriping this Song.
        /// </param>
        /// <param name="music">
        /// The music played of the new Song.
        /// </param>
        protected internal Song(
            IEnumerable<Note> notes,
            SongDescriptionData descriptionData,
            SongMusic music )
        {
            Debug.Assert( notes != null, "notes may not be null." );
            Debug.Assert( descriptionData != null, "descriptionData may not be null." );
            Debug.Assert( music != null, "music may not be null." );

            this.descriptionData = descriptionData;
            this.notes = notes.ToArray();
            this.music = music;

            this.music.Started += this.OnMusicStarted;
            this.music.Ended += this.OnMusicEnded;
        }   
        
        /// <summary>
        /// Setups this Song.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related data.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.music.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Plays this Song.
        /// </summary>
        /// <param name="player">
        /// The player of the Song.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If this Song is already playing.
        /// </exception>
        public void Play( Zelda.Entities.PlayerEntity player )
        {
            this.player = player;
            this.music.Play( player );
        }
        
        /// <summary>
        /// Executes the special effect of this Song.
        /// </summary>
        /// <param name="player">
        /// The player that has played the song.
        /// </param>
        protected abstract void ExecuteEffect( Zelda.Entities.PlayerEntity player );

        /// <summary>
        /// Gets called when playing of this Song has started.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        protected virtual void OnSongStarted( Zelda.Entities.PlayerEntity player )
        {
        }

        /// <summary>
        /// Gets whether the notes of this Song are equal to the given <paramref name="notes"/>.
        /// </summary>
        /// <param name="notes">
        /// The notes to compare this Song to.
        /// </param>
        /// <returns>
        /// True if the notes are equal;
        /// otherwise false.
        /// </returns>
        internal bool HasEqualNotes( IEnumerable<Note> notes )
        {
            if( notes.Count() != this.notes.Length )
                return false;

            int index = 0;

            foreach( var note in notes )
            {
                if( this.notes[index] != note )
                {
                    return false;
                }

                ++index;
            }

            return true;
        }
        
        /// <summary>
        /// Gets called when the music associated with this Song has started playing.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnMusicStarted( SongMusic sender )
        {
            this.OnSongStarted( this.player );
        }

        /// <summary>
        /// Gets called when the music associated with this Song has stopped playing.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnMusicEnded( SongMusic sender )
        {
            this.ExecuteEffect( this.player );
        }

        #endregion
        
        #region [ Fields ]

        /// <summary>
        /// The PlayerEntity that is playing this Song.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The list of notes that make-up this Song.
        /// </summary>
        private readonly Note[] notes;

        /// <summary>
        /// The music associated with this Song.
        /// </summary>
        private readonly SongMusic music;
        
        /// <summary>
        /// Stores descriping properties of this Song that are visible to the player.
        /// </summary>
        private readonly SongDescriptionData descriptionData;

        #endregion
    }
}