// <copyright file="SetPlayerDirectionEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.SetPlayerDirectionEvent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Events
{
    using Atom.Math;

    /// <summary>
    /// Defines an Event that changes the direction the player is turned.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SetPlayerDirectionEvent : ZeldaEvent
    {
        /// <summary>
        /// Gets or sets the direction to change the player to.
        /// </summary>
        public Direction4 Direction
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this SetPlayerDirectionEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this SetPlayerDirectionEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var player = this.Scene.Player;
            var transform = player.Transform;

            transform.Direction = this.Direction;
        }
    }
}
