// <copyright file="ChangeToRandomBackgroundMusicEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ChangeToRandomBackgroundMusicEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using Zelda.Audio;

    /// <summary>
    /// Represents an Event that when triggered tells
    /// the game to play random background music.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ChangeToRandomBackgroundMusicEvent : ZeldaEvent
    {
        /// <summary>
        /// Triggers this ChangeToRandomBackgroundMusicEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this ChangeToRandomBackgroundMusicEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var ingameState = this.Scene.IngameState;
            if( ingameState == null )
                return;

            var backgroundMusic = ingameState.BackgroundMusic;

            // Don't change if we are already in the right mode.
            if( backgroundMusic.Mode == BackgroundMusicMode.Random )
                return;

            backgroundMusic.Mode = BackgroundMusicMode.Random;
            backgroundMusic.ChangeToRandom();
        }
    }
}
