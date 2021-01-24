// <copyright file="MusicCrossSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Teleportation.MusicCrossSong class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Ocarina.Songs.Teleportation
{
    /// <summary>
    /// Teleports the player to the location that has been
    /// stored in the Cross of Teleportation.
    /// </summary>
    public sealed class MusicCrossSong : TeleportationSong
    {
        /// <summary>
        /// Initializes a new instance of the MusicCrossSong class.
        /// </summary>
        public MusicCrossSong()
            : base(
                new Note[] { 
                    Note.Up, Note.Left, Note.Up, Note.Down, Note.Right, Note.Down
                },
            new SongDescriptionData(
                Resources.SongN_MusicCross,
                Resources.SongD_MusicCross,
                new Microsoft.Xna.Framework.Color( 1.0f, 0.843f, 0.0f )
            ),
            new SongMusic(
                "Content/Music/MusicBox-UVII.mp3",
                SongMusicPlayMode.Background
            ) {
                Volume = 0.8f
            } )
        {
        }
        
        /// <summary>
        /// Executes the special effect of this Song.
        /// </summary>
        /// <param name="player">
        /// The player that has played the song.
        /// </param>
        protected override void ExecuteEffect( Entities.PlayerEntity player )
        {
            var storage = player.WorldStatus.DataStore
                .TryGet<CrossTeleportLocationStorage>( CrossTeleportLocationStorage.Identifier );

            if( storage != null )
            {
                var scene = player.IngameState.RequestSceneChange( storage.SceneName, true );

                if( scene != null )
                {
                    if( player.Scene != null )
                    {
                        player.RemoveFromScene();
                    }

                    player.Transform.Position = storage.Position;
                    player.FloorNumber = storage.FloorNumber;
                    
                    player.AddToScene( scene );
                }
            }

            this.OnTeleported( player );
        }
    }
}
