// <copyright file="ZoneResetSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.ZoneResetSong class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina.Songs
{
    using Zelda.Core.Properties.Scene;
    using Zelda.Entities;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines "The Song of the Past" that when played resets the zone/scene
    /// the player currently is part of.
    /// </summary>
    /// <remarks>
    /// This is useful to farm a boss over and over without having to close the game.
    /// </remarks>
    internal sealed class ZoneResetSong : Song
    {
        /// <summary>
        /// Initializes a new instance of the ZoneResetSong class.
        /// </summary>
        public ZoneResetSong()
            : base(
                new Note[] { 
                    Note.Up, Note.Down, Note.Down, Note.Up 
                },
                new SongDescriptionData(
                    Resources.SongN_ZoneReset,
                    Resources.SongD_ZoneReset,
                    Microsoft.Xna.Framework.Color.LightGray
                ),
                new SongMusic(
                    "Content/Music/NocturneOfShadows.mid",
                    SongMusicPlayMode.Background
                )
            )
        {
        }

        /// <summary>
        /// Executes the effec this ZoneResetSong has.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        protected override void ExecuteEffect( PlayerEntity player )
        {
            if( player == null )
                return;

            var scene = player.Scene;
            if( scene == null )
                return;

            if( AnalyzeSceneProperties( player ) )
                return;

            Reset( player );
            ReloadAndSpawn( player );
        }

        /// <summary>
        /// Analayzes the IProperties applied to the current scene.
        /// </summary>
        /// <param name="player">
        /// The player entity.
        /// </param>
        /// <returns>
        /// true if the ExecuteEffect should return after analyzing the scene properties;
        /// other false if ExecuteEffect should continue.
        /// </returns>
        private bool AnalyzeSceneProperties( PlayerEntity player )
        {
            var currentScene = player.Scene;
            var sceneProperties = currentScene.Settings.Properties;

            // Analyze
            var redirectZoneReset = sceneProperties.TryGet<RedirectZoneResetProperty>();

            if( redirectZoneReset != null )
            {
                HandleRedirectProperty( redirectZoneReset, player );
                return true;
            }

            var alsoResetOtherZone = sceneProperties.TryGet<AlsoResetAnotherZoneOnZoneResetProperty>();

            if( alsoResetOtherZone != null )
            {
                HandleAlsoResetOtherZoneProperty( alsoResetOtherZone, player );
            }

            return false;
        }

        /// <summary>
        /// Handles the specified RedirectZoneResetProperty.
        /// </summary>
        /// <param name="redirectZoneReset">
        /// The property that has been detected on the scene.
        /// </param>
        /// <param name="player">
        /// The player that caused the effect to execute.
        /// </param>
        private void HandleRedirectProperty( RedirectZoneResetProperty redirectZoneReset, PlayerEntity player )
        {
            IIngameState ingameState = player.IngameState;
            ZeldaScene currentScene = player.Scene;
                        
            // Remove current scene from cache 
            // if we reset the scene.
            if( redirectZoneReset.ResetCurrentBeforeRedirecting )
            {
                player.WorldStatus.ScenesCache.Remove( currentScene );
            }

            // And do the actual work .. :)
            bool shouldCache = !redirectZoneReset.ResetCurrentBeforeRedirecting;
            ZeldaScene newScene = ingameState.RequestSceneChange( redirectZoneReset.ToScene, shouldCache );
            player.AddToScene( newScene );

            // Re-execute effect with the scene
            // we just changed to.
            this.ExecuteEffect( player );
        }

        /// <summary>
        /// Handles the <see cref="AlsoResetAnotherZoneOnZoneResetProperty"/>.
        /// </summary>
        /// <param name="alsoResetOtherZone">
        /// The property that has been detected on the scene.
        /// </param>
        /// <param name="player">
        /// The player that caused the effect to execute.
        /// </param>
        private static void HandleAlsoResetOtherZoneProperty( AlsoResetAnotherZoneOnZoneResetProperty alsoResetOtherZone, PlayerEntity player )
        {
            var scenesCache = player.WorldStatus.ScenesCache;
            scenesCache.Remove( alsoResetOtherZone.Scene );
        }

        /// <summary>
        /// Resets the ZeldaScene the specified PlayerEntity is currently part of.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        private static void Reset( PlayerEntity player )
        {
            player.WorldStatus.ScenesCache.Remove( player.Scene.Name );
            player.RemoveFromScene();
        }        

        /// <summary>
        /// Reloads the old ZeldaScene from the Hard-Disc.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        private static void ReloadAndSpawn( PlayerEntity player )
        {
            ZeldaScene newScene = player.IngameState.RequestSceneReload();
            Respawn( player, newScene );
        }

        /// <summary>
        /// Spawns the player at the last used spawn point.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        /// <param name="newScene">
        /// The newly loaded Scene.
        /// </param>
        private static void Respawn( PlayerEntity player, ZeldaScene newScene )
        {
            ISpawnPoint spawnPoint = GetSpawnPoint( player, newScene );

            if( spawnPoint != null )
            {
                spawnPoint.Spawn( player );
            }

            OnRespawned( player );
        }

        /// <summary>
        /// Gets the ISpawnPoint the player should spawn at.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        /// <param name="newScene">
        /// The newly loaded Scene.
        /// </param>
        /// <returns>
        /// The ISpawnPoint the player should be spawned at.
        /// </returns>
        private static ISpawnPoint GetSpawnPoint( PlayerEntity player, ZeldaScene newScene )
        {
            var lastSavePoint = player.Profile.LastSavePoint;
            ISpawnPoint spawnPoint = null;

            if( newScene.Name.Equals( lastSavePoint.Scene ) )
            {
                spawnPoint = newScene.GetSpawnPoint( lastSavePoint.SpawnPoint );
            }

            if( spawnPoint == null )
            {
                spawnPoint = newScene.GetEntity<PlayerSpawnPoint>();

                if( spawnPoint == null )
                {
                    spawnPoint = newScene.GetEntity<ISpawnPoint>();
                }
            }

            return spawnPoint;
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
            userInterface.BlendElement.StartBlending( this.Music.Duration, false, false );
        }

        /// <summary>
        /// Gets called when the given Player has been sucessfully respawned.
        /// </summary>
        /// <param name="player">
        /// The player that has been respawned.
        /// </param>
        private static void OnRespawned( Zelda.Entities.PlayerEntity player )
        {
            var userInterface = player.Scene.UserInterface;

            // Blend the scene in again.
            userInterface.BlendElement.StartBlending( 5.0f, true, true );
        }
    }
}
