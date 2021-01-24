// <copyright file="ZeldaPathSegmentTileHandler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaPathSegmentTileHandler class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Waypoints
{
    using System;
    using Atom.Scene.Tiles;

    /// <summary>
    /// Implements the ITileHandler{object} that is used to create the TilePaths that connect two Waypoints.
    /// </summary>
    internal sealed class ZeldaPathSegmentTileHandler : ITileHandler<object>
    {
        /// <summary>
        /// Represents the singleton instance of the ZeldaPathSegmentTileHandler class.
        /// </summary>
        public static readonly ZeldaPathSegmentTileHandler Instance = new ZeldaPathSegmentTileHandler();

        /// <summary>
        /// Handles the action.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the tile (in tile-space).
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the tile (in tile-space).
        /// </param>
        /// <param name="id">
        /// The id of the action.
        /// </param>
        /// <param name="caller">
        /// The object that created the event.
        /// </param>
        /// <returns>
        /// Returns <see lamg="true"/> if to stop handling actions; 
        /// otherwise <see lamg="false"/>.
        /// </returns>
        public bool Handle( int x, int y, int id, object caller )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns whether the specified tile is walkable by the specified caller.
        /// </summary>
        /// <param name="id">
        /// The id of the action.
        /// </param>
        /// <param name="caller">
        /// The object to test for.</param>
        /// <returns>
        /// Returns <see lamg="true"/> if the tile with the given <paramref name="id"/> is walkable; 
        /// otherwise <see lamg="false"/>.
        /// </returns>
        public bool IsWalkable( int id, object caller )
        {
            switch( (ActionTileId)id )
            {
                case ActionTileId.FloorUp:
                case ActionTileId.FloorDown:
                case ActionTileId.WaterWalkable:
                case ActionTileId.Normal:
                    return true;

                default:
                    return false;
            }
        }
    }
}
