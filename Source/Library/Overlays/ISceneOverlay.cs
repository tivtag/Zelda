// <copyright file="ISceneOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Overlays.ISceneOverlay interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Overlays
{
    /// <summary>
    /// An <see cref="ISceneOverlay"/> is an overlay that is 
    /// rendered ontop of the scene, just before the user interface.
    /// </summary>
    public interface ISceneOverlay
    {
        /// <summary>
        /// Updates this ISceneOverlay.
        /// </summary>
        /// <param name="updateContext">
        /// The current <see cref="ZeldaUpdateContext"/>.
        /// </param>
        void Update( ZeldaUpdateContext updateContext );

        /// <summary>
        /// Draws this ISceneOverlay.
        /// </summary>
        /// <param name="drawContext">
        /// The current <see cref="ZeldaDrawContext"/>.
        /// </param>
        void Draw( ZeldaDrawContext drawContext );

        /// <summary>
        /// Gets called when this ISceneOverlay has been added to the given <see cref="ZeldaScene"/>.
        /// </summary>
        /// <remarks>
        /// This method should not directly be called from user-code.
        /// </remarks>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        void AddedToScene( ZeldaScene scene );

        /// <summary>
        /// Gets called when this ISceneOverlay has been removed from the given <see cref="ZeldaScene"/>.
        /// </summary>
        /// <remarks>
        /// This method should not directly be called from user-code.
        /// </remarks>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        void RemovedFromScene( ZeldaScene scene );
    }
}
