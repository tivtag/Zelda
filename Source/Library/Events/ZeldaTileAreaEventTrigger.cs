// <copyright file="ZeldaTileAreaEventTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ZeldaTileAreaEventTrigger class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System.ComponentModel;
    using Atom.Events;

    /// <summary>
    ///  Represents a <see cref="TileAreaEventTrigger"/> that provides additional support for use in the Zelda game.
    /// </summary>
    public class ZeldaTileAreaEventTrigger : TileAreaEventTrigger
    {
        /// <summary>
        /// Gets the <see cref="ZeldaEventManager"/> that manages this ZeldaTileAreaEventTrigger.
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
        /// Gets the <see cref="ZeldaScene"/> that owns the <see cref="ZeldaEventManager"/>
        /// that manages this ZeldaTileAreaEventTrigger.
        /// </summary>
        [Browsable( false )]
        public ZeldaScene Scene
        {
            get
            {
                return this.EventManager.Scene;
            }
        }
    }
}
