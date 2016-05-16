// <copyright file="WorkspaceType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.WorkspaceType enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    /// <summary>
    /// Enumerates the different <see cref="IWorkspace"/>s of the Editor.
    /// </summary>
    public enum WorkspaceType
    {
        /// <summary>
        /// Represents no specific IWorkspace.
        /// </summary>
        None,

        /// <summary>
        /// The TileWorkspace interacts with the TileMap of the Scene.
        /// </summary>
        Tile,
        
        /// <summary>
        /// The ObjectWorkspace interacts with Entities of the Scene.
        /// </summary>
        Object,

        /// <summary>
        /// The EventWorkspace interacts with Events and EventTriggers of the Scene.
        /// </summary>
        Event,

        /// <summary>
        /// The WaypointWorkspace interacts with Waypoints of the Scene.
        /// </summary>
        Waypoint,

        /// <summary>
        /// The StoryWorkspace interacts with the Storyboard and the invidual Timelines of the Scene.
        /// </summary>
        Story
    }
}
