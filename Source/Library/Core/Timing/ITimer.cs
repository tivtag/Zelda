// <copyright file="ITimer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Timing.ITimer interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Timing
{
    using System;
    using Atom;
    
    /// <summary>
    /// Represents a timer that raises an event after a specified time.
    /// </summary>
    public interface ITimer : IZeldaUpdateable, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Raised when this ITimer has ended.
        /// </summary>
        event SimpleEventHandler<ITimer> Ended;
    }
}
