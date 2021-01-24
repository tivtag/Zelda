// <copyright file="IIngameState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IIngameState interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Zelda.Entities;
    using Zelda.UI;

    /// <summary>
    /// Provides a basic interface the 
    /// ingame game-state implements.
    /// </summary>
    public interface IIngameState
    {
        /// <summary>
        /// Gets the <see cref="Zelda.Audio.BackgroundMusicComponent"/> that manages 
        /// the music that is playing in the background of the game.
        /// </summary>
        Zelda.Audio.BackgroundMusicComponent BackgroundMusic
        {
            get;
        }
        
        /// <summary>
        /// Gets the PlayerEntity that is currently active in this IIngameState.
        /// </summary>
        PlayerEntity Player
        {
            get;
        }

        /// <summary>
        /// Gets the UserInterface shown ingame.
        /// </summary>
        ZeldaUserInterface UserInterface 
        {
            get;
        }

        /// <summary>
        /// Tells this IIngameState to change the current <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the ZeldaScene to change to.
        /// </param>
        /// <param name="cachePrevious">
        /// States whether the previous ZeldaScene should be cached using the <see cref="ZeldaScenesCache"/>.
        /// </param>
        /// <returns>
        /// The new current ZeldaScene.
        /// </returns>
        ZeldaScene RequestSceneChange( string name, bool cachePrevious );

        /// <summary>
        /// Tells this IIngameState to reload the current <see cref="ZeldaScene"/>
        /// by discarding and the loading it again.
        /// </summary>
        /// <returns>
        /// The new ZeldaScene instance.
        /// </returns>
        ZeldaScene RequestSceneReload();

        /// <summary>
        /// Attempts to change to the specified drawing pipeline.
        /// </summary>
        /// <param name="newDrawingPipeline">
        /// The DrawingPipeline to change to.
        /// </param>
        /// <returns>
        /// true if the change has been successful;
        /// otherwise false.
        /// </returns>
        bool ChangeDrawingPipeline( Zelda.Graphics.DrawingPipeline newDrawingPipeline );
    }
}
