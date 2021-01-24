// <copyright file="BasePlayerAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.BasePlayerAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Actions.Player
{
    using Zelda.Entities;
    using Zelda.Status;

    /// <summary>
    /// Represents an abstract base implementation of the IAction interface
    /// that provides conviniant methods for manipulating the current PlayerEntity.
    /// </summary>
    public abstract class BasePlayerAction : BaseAction
    {
        /// <summary>
        /// Gets the currently active PlayerEntity.
        /// </summary>
        protected PlayerEntity Player
        {
            get
            {
                var ingameState = IoC.Resolve<IIngameState>();
                return ingameState.Player;
            }
        }

        /// <summary>
        /// Gets the <see cref="ExtendedStatable"/> of the PlayerEntity.
        /// </summary>
        protected ExtendedStatable Statable
        {
            get
            {
                return this.Player.Statable;
            }
        }
    }
}
