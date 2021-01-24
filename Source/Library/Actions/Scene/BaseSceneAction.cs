// <copyright file="BaseSceneAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.BaseSceneAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Actions.Scene
{
    /// <summary>
    /// Represents an abstract base implementation of the IAction interface
    /// that provides conviniant methods for manipulating the current ZeldaScene.
    /// </summary>
    public abstract class BaseSceneAction : BaseAction
    {
        /// <summary>
        /// Gets the currently active ZeldaScene.
        /// </summary>
        protected ZeldaScene Scene
        {
            get
            {
                var ingameState = IoC.Resolve<IIngameState>();
                return ingameState.Player.Scene;
            }
        }
    }
}
