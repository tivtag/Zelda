// <copyright file="InactiveSceneUpdater.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.InactiveSceneUpdater class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    
    /// <summary>
    /// Implements a mechanism that silently updates ZeldaScenes that the player
    /// has left.
    /// </summary>
    /// <remarks>
    /// This is used to simulate that time still continues in the previous scene.
    /// </remarks>
    public sealed class InactiveSceneUpdater : IZeldaUpdateable
    {
        /// <summary>
        /// Updates this InactiveSceneUpdater.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.scene == null )
                return;            
            
            // Only enable this feature if we are on a 'fast' machine;
            // this will also return true if the user has changed focus
            // away from the game window.
            if( updateContext.IsRunningSlowly )
                return;

            this.updateTime += updateContext.FrameTime;
            if( this.updateTime >= MaximumUpdateTime )
            {
                this.scene = null;
                return;
            }

            this.SetUpdateContext( updateContext );
            this.scene.Update( this.inactiveUpdateContext );
        }

        /// <summary>
        /// Setups the inactiveUpdateContext this InactiveSceneUpdater uses
        /// to update the current scene.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void SetUpdateContext( ZeldaUpdateContext updateContext )
        {
            var realGameTime = updateContext.GameTime;
            inactiveUpdateContext.GameTime = new Microsoft.Xna.Framework.GameTime(
                realGameTime.TotalGameTime,
                TimeSpan.FromMilliseconds( realGameTime.ElapsedGameTime.TotalMilliseconds * 2.0 )
            );
        }

        /// <summary>
        /// Notifies this InactiveSceneUpdater that the specified ZeldaScene
        /// has turned inactive and ahouls be updated silently in the background.
        /// </summary>
        /// <param name="scene">
        /// The scene that has turned inactive.
        /// </param>
        public void NotifyInacitify( ZeldaScene scene )
        {
            this.scene = scene;
            this.updateTime = 0.0f;
        }

        /// <summary>
        /// Clears this InactiveSceneUpdater; removing all inacticely updated scenes.
        /// </summary>
        public void Clear()
        {
            this.scene = null;
        }

        /// <summary>
        /// The scene that last turned inactive and should be updated in the background.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The time in seconds the scene has been updated silently.
        /// </summary>
        private float updateTime;

        /// <summary>
        /// The maximum time in seconds updates to the scene are done in the background
        /// until it is discarded.
        /// </summary>
        private const float MaximumUpdateTime = 20.0f;

        /// <summary>
        /// The ZeldaUpdateContext that is used 
        /// </summary>
        private readonly ZeldaUpdateContext inactiveUpdateContext = new ZeldaUpdateContext( isMainUpdate: false );
    }
}
