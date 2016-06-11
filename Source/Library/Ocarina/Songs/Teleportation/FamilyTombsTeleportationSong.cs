// <copyright file="FamilyTombsTeleportationSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Ocarina.Songs.Teleportation.FamilyTombsTeleportationSong class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    /// <summary>
    /// Implements a <see cref="SceneTeleportationSong"/> that ports the player
    /// to the Family Stone Tombs in the Tia Woods.
    /// </summary>
    public sealed class FamilyTombsTeleportationSong : SceneTeleportationSong
    {
        /// <summary>
        /// Gets the name of the Scene this TeleportationSong ports to.
        /// </summary>
        protected override string SceneName
        {
            get 
            {
                return "TiaWoodsNorth"; 
            }
        }

        /// <summary>
        /// Gets the name of the SpawnPoint this TeleportationSong ports to.
        /// </summary>
        protected override string SpawnPointName
        {
            get
            { 
                return "SP_Teleport_FamilyTombs"; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the FamilyTombsTeleportationSong class.
        /// </summary>
        public FamilyTombsTeleportationSong()
            : base(                
                new Note[] { 
                    Note.Left, Note.Right, Note.Left, Note.Right
                },
                new SongDescriptionData(
                    Resources.SongN_FamilyTombsTeleport, 
                    Resources.SongD_FamilyTombsTeleport,
                    Microsoft.Xna.Framework.Color.Red
                ),
                SongMusic.CreateDefault()
            )
        {
        }
    }
}
