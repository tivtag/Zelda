// <copyright file="SceneTeleportationSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Teleportation.SceneTeleportationSong class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a <see cref="Song"/> that when played teleports
    /// the player to a specific location.
    /// </summary>
    public abstract class SceneTeleportationSong : Song
    {
        /// <summary>
        /// Gets the name of the Scene this SceneTeleportationSong ports to.
        /// </summary>
        protected abstract string SceneName
        { 
            get;
        }

        /// <summary>
        /// Gets the name of the SpawnPoint this SceneTeleportationSong ports to.
        /// </summary>
        protected abstract string SpawnPointName
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the SceneTeleportationSong class.
        /// </summary>
        /// <param name="notes">
        /// The enumeration of Notes that make-up the new SceneTeleportationSong.
        /// </param>
        /// <param name="descriptionData">
        /// The data visible to the player that is descriping this SceneTeleportationSong.
        /// </param>
        /// <param name="music">
        /// The music played of the new SceneTeleportationSong.
        /// </param>
        protected internal SceneTeleportationSong(
            IEnumerable<Note> notes,
            SongDescriptionData descriptionData,
            SongMusic music )
            : base( notes, descriptionData, music )
        {
        }

        /// <summary>
        /// Plays this SceneTeleportationSong, porting the player.
        /// </summary>
        /// <param name="player">
        /// The player that has played the song.
        /// </param>
        protected override void ExecuteEffect( Zelda.Entities.PlayerEntity player )
        {
            if( player == null )
                return;
            
            var scene = player.IngameState.RequestSceneChange( this.SceneName, true );

            if( scene != null )
            {
                var spawnPoint = scene.GetSpawnPoint( this.SpawnPointName );

                if( spawnPoint != null )
                {
                    spawnPoint.Spawn( player );
                    this.OnTeleported( player );
                }
            }
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

            var scene = player.Scene;
            if( scene == null )
                return;

            // Blend the scene out when the song is playing.
            var userInterface = scene.UserInterface;
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
            var userInterface = player.Scene.UserInterface;
            userInterface.BlendElement.StartBlending( 5.0f, true, true );
        }
    }
}
