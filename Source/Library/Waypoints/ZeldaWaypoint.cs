// <copyright file="ZeldaWaypoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaWaypoint class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Waypoints
{
    using Atom.Waypoints;
    using Atom.Components.Transform;
    using Atom.Scene;
    using Atom.Components.Collision;

    /// <summary>
    /// Represents a Waypoint used in the Zelda game.
    /// </summary>
    public sealed class ZeldaWaypoint : Waypoint
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaWaypoint class.
        /// </summary>
        public ZeldaWaypoint()
            : base( new Transform2(), new StaticCollision2(), new QuadTreeItem2() )
        {
        }

        /// <summary>
        /// Gets the PathSegment that directly connects this Waypoint with the specified Waypoint.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to compare to.
        /// </param>
        /// <returns>
        /// true if the Waypoints are connected; otherwise false.
        /// </returns>
        public ZeldaPathSegment GetPathSegmentTo( ZeldaWaypoint waypoint )
        {
            return (ZeldaPathSegment)base.GetPathSegmentTo( waypoint );
        }
    }
}
