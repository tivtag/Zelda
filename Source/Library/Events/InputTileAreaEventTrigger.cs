// <copyright file="InputTileAreaEventTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.InputTileAreaEventTrigger class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents an <see cref="ZeldaTileAreaEventTrigger"/> that also requires keyboard input to trigger.
    /// </summary>
    public abstract class InputTileAreaEventTrigger : ZeldaTileAreaEventTrigger
    {
        /// <summary>
        /// Gets the <see cref="UserInterface"/> of the <see cref="ZeldaScene"/> that owns this InputTileAreaEventTrigger.
        /// </summary>
        protected UserInterface UserInterface
        {
            get
            {
                return this.Scene.UserInterface;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the specified Key is currently being pressed.
        /// </summary>
        /// <param name="key">
        /// The key to query.
        /// </param>
        /// <returns>
        /// true if they key is currently being pressed;
        /// otherwise false.
        /// </returns>
        protected bool IsKeyDown( Keys key )
        {
            return this.UserInterface.IsKeyDown( key );
        }

        /// <summary>
        /// Gets a value indicating whether the specified Key is currently not being pressed.
        /// </summary>
        /// <param name="key">
        /// The key to query.
        /// </param>
        /// <returns>
        /// true if they key is currently not being pressed;
        /// otherwise false.
        /// </returns>
        protected bool IsKeyUp( Keys key )
        {
            return this.UserInterface.IsKeyUp( key );
        }
    }
}
