// <copyright file="ZeldaEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ZeldaEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System.ComponentModel;

    /// <summary>
    /// Defines an abstract base class that extends the <see cref="Atom.Events.Event"/> class
    /// with Zelda-related useability features.
    /// </summary>
    /// <remarks>
    /// It is absolutely not required to extend the ZeldaEvent class
    /// over the <see cref="Atom.Events.Event"/> class.
    /// This class is only there to reduce the amount of duplicate code.
    /// </remarks>
    public abstract class ZeldaEvent : Atom.Events.Event
    {
        /// <summary>
        /// Gets the <see cref="ZeldaEventManager"/> that manages this ZeldaEvent.
        /// </summary>
        [Browsable( false )]
        public new ZeldaEventManager EventManager
        {
            get
            {
                return (ZeldaEventManager)base.EventManager; 
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> that owns this ZeldaEvent.
        /// </summary>
        [Browsable( false )]
        public ZeldaScene Scene
        {
            get 
            {
                var manager = (ZeldaEventManager)base.EventManager;
                return manager != null ? manager.Scene : null;
            }
        }
    }
}
