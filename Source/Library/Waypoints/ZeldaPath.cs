// <copyright file="ZeldaPath.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaPath class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Waypoints
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Waypoints;

    /// <summary>
    /// Represents a <see cref="Path"/> used by the Zelda game.
    /// </summary>
    public sealed class ZeldaPath : Path
    {
        /// <summary>
        /// Gets the <see cref="ZeldaWaypointMap"/> that owns this ZeldaPath.
        /// </summary>
        public ZeldaWaypointMap Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaWaypoint"/> at the specified zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the ZeldaWaypoint to get.
        /// </param>
        /// <returns>
        /// The requested ZeldaWaypoint.
        /// </returns>
        public new ZeldaWaypoint this[int index]
        {
            get
            {
                return (ZeldaWaypoint)base[index];
            }
        }

        /// <summary>
        /// Initializes a new instance of the ZeldaPath class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ZeldaWaypointMap"/> that owns the new ZeldaPath.
        /// </param>
        internal ZeldaPath( ZeldaWaypointMap owner )
        {
            Contract.Requires<ArgumentNullException>( owner != null );

            this.owner = owner;
        }

        /// <summary>
        /// The <see cref="ZeldaWaypointMap"/> that owns this ZeldaPath.
        /// </summary>
        private readonly ZeldaWaypointMap owner;
    }
}
