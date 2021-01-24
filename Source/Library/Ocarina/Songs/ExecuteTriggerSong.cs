// <copyright file="TimeWarpSong.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Ocarina.Songs.TimeWarpSong class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Ocarina.Songs
{
    using System.Linq;
    using Atom.Events;
    using Zelda.Entities;
    using Zelda.Events;

    /// <summary>
    /// Defines a <see cref="Song"/> that when played executes all SongTileAreaTriggers
    /// that are under the player.
    /// </summary>
    internal sealed class ExecuteTriggerSong : Song
    {
        /// <summary>
        /// Initializes a new instance of the ExecuteTriggerSong class.
        /// </summary>
        public ExecuteTriggerSong()
            : base(
                new Note[] { 
                   Note.Left, Note.Left, Note.Right, Note.Down,
                },
                new SongDescriptionData(
                    Resources.SongN_ExecuteTriggerSong,
                    Resources.SongD_ExecuteTriggerSong,
                    Microsoft.Xna.Framework.Color.Orange
                ),
                new SongMusic(
                    null,
                    SongMusicPlayMode.None
                )
            )
        {
        }

        /// <summary>
        /// The mana penality in % of base mana of the TimeWarpSong.
        /// </summary>
        private const float ManaCost = 0.15f;

        /// <summary>
        /// Executes the effec this TimeWarpSong has.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has caused the effect to be executed.
        /// </param>
        protected override void ExecuteEffect( PlayerEntity player )
        {
            ApplyManaPenality( player );

            SongTileAreaEventTrigger[] triggers = player.Scene.EventManager
                .GetTriggers<SongTileAreaEventTrigger>( player, this )
                .ToArray();

            if( triggers.Length == 0 )
            {
                OnNoTriggersFound();
            }
            else
            {
                OnTriggersFound( triggers, player );
            }
        }

        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.audioSystem = serviceProvider.AudioSystem;
            base.Setup( serviceProvider );
        }

        private void OnNoTriggersFound()
        {
            audioSystem
                .GetSample( "TextNext.ogg" )
                .LoadAsSample()
                .Play();
        }

        private void OnTriggersFound( SongTileAreaEventTrigger[] triggers, PlayerEntity player )
        {
            foreach( var trigger in triggers )
            {
                trigger.Trigger( player );
            }

            audioSystem
                .GetSample( "Bell.ogg" )
                .LoadAsSample()
                .Play( volume: 0.5f );
        }

        /// <summary>
        /// Applies the mana penality of playing this ExecuteTriggerSong to the given PlayerEntity.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that started to play this Song.
        /// </param>
        private static void ApplyManaPenality( PlayerEntity player )
        {
            player.Statable.RemovePercentageOfBaseMana( ManaCost );
        }

        private Atom.Fmod.AudioSystem audioSystem;
    }
}
