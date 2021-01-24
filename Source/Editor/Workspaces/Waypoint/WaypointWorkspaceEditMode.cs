// <copyright file="WaypointWorkspaceEditMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointWorkspaceEditMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Waypoint
{
    /// <summary>
    /// Enumerates the different edit modes the WaypointWorkspace can be in.
    /// </summary>
    public enum WaypointWorkspaceEditMode
    {
        /// <summary>
        /// Allows the creation and modification of named Waypoints and Path Segments.
        /// </summary>
        WaypointsAndSegments,

        /// <summary>
        /// Allows the creation and modification of named Paths.
        /// </summary>
        Paths
    }
}
