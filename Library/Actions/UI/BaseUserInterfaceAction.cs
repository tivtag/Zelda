// <copyright file="BaseUserInterfaceAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.BaseUserInterfaceAction class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Actions.UI
{
    using Zelda.UI;

    /// <summary>
    /// Represents an abstract base implementation of the IAction interface
    /// that provides conviniant methods for manipulating the current ZeldaUserInterface.
    /// </summary>
    public abstract class BaseUserInterfaceAction : BaseAction
    {      
        /// <summary>
        /// Gets the currently active ZeldaUserInterface.
        /// </summary>
        protected ZeldaUserInterface UserInterface
        {
            get
            {
                var ingameState = IoC.Resolve<IIngameState>();
                return ingameState.UserInterface;
            }
        }
    }
}
