// <copyright file="TeleportationSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Teleportation.TeleportationSong class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a <see cref="Song"/> that when played teleports the player.
    /// </summary>
    public abstract class TeleportationSong : Song
    {
        /// <summary>
        /// Initializes a new instance of the TeleportationSong class.
        /// </summary>
        /// <param name="notes">
        /// The enumeration of Notes that make-up the new TeleportationSong.
        /// </param>
        /// <param name="descriptionData">
        /// The data visible to the player that is descriping this TeleportationSong.
        /// </param>
        /// <param name="music">
        /// The music played of the new TeleportationSong.
        /// </param>
        protected internal TeleportationSong(
            IEnumerable<Note> notes,
            SongDescriptionData descriptionData,
            SongMusic music )
            : base( notes, descriptionData, music )
        {
        }

        /// <summary>
        /// Gets called when playing of this Song has started.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        protected override void OnSongStarted( Zelda.Entities.PlayerEntity player )
        {
            if( player == null )
                return;

            // Blend the scene out when the song is playing.
            var userInterface = player.IngameState.UserInterface;
            userInterface.BlendElement.StartBlending( this.Music == null ? 5.0f : this.Music.Duration, false, false );
        }

        /// <summary>
        /// Gets called when the given Player has beens ucessfully teleported.
        /// </summary>
        /// <param name="player">
        /// The player that has been teleported.
        /// </param>
        protected virtual void OnTeleported( Zelda.Entities.PlayerEntity player )
        {
            // Blend the scene in again.
            var userInterface = player.IngameState.UserInterface;
            userInterface.BlendElement.StartBlending( 5.0f, true, true );
        }
    }
}
