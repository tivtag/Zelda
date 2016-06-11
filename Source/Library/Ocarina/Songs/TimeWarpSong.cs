// <copyright file="TimeWarpSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Ocarina.Songs.TimeWarpSong class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Ocarina.Songs
{
    using Zelda.Entities;

    /// <summary>
    /// Defines a <see cref="Song"/> that when played resets the zone/scene
    /// the player currently is part of.
    /// </summary>
    /// <remarks>
    /// This is useful to farm a boss over and over without having to close the game.
    /// </remarks>
    internal sealed class TimeWarpSong : Song
    {
        /// <summary>
        /// Initializes a new instance of the TimeWarpSong class.
        /// </summary>
        public TimeWarpSong()
            : base(
                new Note[] { 
                   Note.Up, Note.Down, Note.Left, Note.Right, Note.Up, Note.Down,
                },
                new SongDescriptionData(
                    Resources.SongN_TimeWarpSong,
                    Resources.SongD_TimeWarpSong,
                    Microsoft.Xna.Framework.Color.DarkGray
                ),
                new SongMusic(
                    "Content/Music/BoleroOfFire.mid",
                    SongMusicPlayMode.Background
                )
            )
        {
        }

        /// <summary>
        /// States the time this Song warps into the future.
        /// </summary>
        private const int HoursToWarp = 8;

        /// <summary>
        /// The mana penality in % of base mana of the TimeWarpSong.
        /// </summary>
        private const float ManaCost = 0.25f;

        /// <summary>
        /// Executes the effec this TimeWarpSong has.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        protected override void ExecuteEffect( PlayerEntity player )
        {
            WarpTime( player );
            ApplyManaPenality( player );
            StartBlendingOut( player );
        }

        /// <summary>
        /// Warps the current time of the given PlayerEntity.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        private static void WarpTime( PlayerEntity player )
        {
            player.WorldStatus.IngameDateTime.AddHours( HoursToWarp );
        }

        /// <summary>
        /// Applies the mana penality of playing tthis TimeWarpSong to the given PlayerEntity.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        private static void ApplyManaPenality( PlayerEntity player )
        {
            player.Statable.RemovePercentageOfBaseMana( ManaCost );
        }

        /// <summary>
        /// Gets called when playing of this Song has started.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        protected override void OnSongStarted( Zelda.Entities.PlayerEntity player )
        {
            var userInterface = player.Scene.UserInterface;

            // Blend the scene out when the song is playing.
            userInterface.BlendElement.StartBlending( this.Music.Duration, false, false );
        }

        /// <summary>
        /// Gets called when the given Player has been sucessfully respawned.
        /// </summary>
        /// <param name="player">
        /// The player that has been respawned.
        /// </param>
        private static void StartBlendingOut( Zelda.Entities.PlayerEntity player )
        {
            var userInterface = player.Scene.UserInterface;

            // Blend the scene in again.
            userInterface.BlendElement.StartBlending( 5.0f, true, true );
        }
    }
}
