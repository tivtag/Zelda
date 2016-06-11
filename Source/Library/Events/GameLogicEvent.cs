// <copyright file="GameLogicEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.GameLogicEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    /// <summary>
    /// Represents the base class of all pluginable, turn on/off-able
    /// and long term active game logic that gets updated every frame.
    /// </summary>
    /// <remarks>
    /// Not all game logic-like events must inherit from this class.
    /// Only those that wish to get updated each frame.
    /// </remarks>
    public abstract class GameLogicEvent : Atom.Events.PermanentEvent
    {
        /// <summary>
        /// Initializes a new instance of the GameLogicEvent class.
        /// </summary>
        protected GameLogicEvent()
            : base()
        {
        }

        /// <summary>
        /// Updates this GameLogicEvent.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            // Overriden to do nothing by default,
            // so that sub classes don't -have to-
            // implement it.
        }
    }
}
