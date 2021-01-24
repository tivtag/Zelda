// <copyright file="PathFollowState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.PathFollowState enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{ 
    /// <summary>
    /// Enumerates the different states a <see cref="TilePathFollower"/> can be in.
    /// </summary>
    public enum PathFollowState
    {
        /// <summary>
        /// If the object is following the path.
        /// </summary>
        Following,

        /// <summary>
        /// If the object reached the end point.
        /// </summary>
        Reached,

        /// <summary>
        /// If the object didn't manage to follow the path anymore
        /// to do unknown sources.
        /// </summary>
        Stuck,

        /// <summary>
        /// If the object can't follow the path anymore
        /// because a solid tile or object is blocking the path.
        /// </summary>
        HardStuck
    }
}
