// <copyright file="RouteOfDinTeleportationSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Teleportation.RouteOfDinTeleportationSong class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    /// <summary>
    /// Implements a <see cref="SceneTeleportationSong"/> that ports the player
    /// to Maja at the Route of Din.
    /// </summary>
    public sealed class HomeTeleportationSong : SceneTeleportationSong
    {
        /// <summary>
        /// Gets the name of the Scene this TeleportationSong ports to.
        /// </summary>
        protected override string SceneName
        {
            get 
            {
                return "BloodWoods";
            }
        }

        /// <summary>
        /// Gets the name of the SpawnPoint this TeleportationSong ports to.
        /// </summary>
        protected override string SpawnPointName
        {
            get 
            {
                return "PSP_House"; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the HomeTeleportationSong class.
        /// </summary>
        public HomeTeleportationSong()
            : base(
                new Note[] { 
                    Note.Up, Note.Down, Note.Up, Note.Up, Note.Left, Note.Right
                },
                new SongDescriptionData(
                    Resources.SongN_HomeTeleport,
                    Resources.SongD_HomeTeleport,
                    Microsoft.Xna.Framework.Color.Ivory
                ),
                new SongMusic( "Content/Music/RequiemOfSpirit.mid", SongMusicPlayMode.Background )
            )
        {
        }
    }
}

