// <copyright file="DefaultTileHandler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.DefaultTileHandler class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Zelda.Entities.Components;

    /// <summary>
    /// Defines the default implementations of the <see cref="IZeldaTileHandler"/> interface.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class DefaultTileHandler : IZeldaTileHandler
    {
        /// <summary>
        /// Receives the instance of the <see cref="DefaultTileHandler"/> class.
        /// </summary>
        public static readonly DefaultTileHandler Instance =
            new DefaultTileHandler();

        /// <summary>
        /// Prevents a default instance of the DefaultTileHandler class from being created.
        /// </summary>
        private DefaultTileHandler()
        {
        }

        /// <summary>
        /// Receives a value that indicates whether 
        /// the specified caller can walk over the tile 
        /// with the specifies id.
        /// </summary>
        /// <param name="id">The id of the tile.</param>
        /// <param name="caller">The caller of the IZeldaTileHandler.</param>
        /// <returns>
        /// true if the caller can walk over the tile with the given id;
        /// otherwise false.
        /// </returns>
        public bool IsWalkable( int id, Moveable caller )
        {
            switch( (ActionTileId)id )
            {
                case ActionTileId.Normal:
                case ActionTileId.WaterWalkable:
                case ActionTileId.FallUnsafe:
                    return true;

                case ActionTileId.Solid:
                case ActionTileId.SolidNotAffectingFlying:
                    return !caller.CollidesWithMap;

                case ActionTileId.WaterSwimable:
                    return caller.CanCurrentlySwim;

                case ActionTileId.FloorUp:
                case ActionTileId.FloorDown:
                    return caller.CanChangeFloor;

                case ActionTileId.SolidForAll:
                case ActionTileId.Invalid:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether -by default-
        /// the specified id would be considered
        /// a walkable tile id.
        /// </summary>
        /// <param name="id">
        /// The id of the tile to check.
        /// </param>
        /// <returns>
        /// true if it is walkable;
        /// -or- otherwise false.
        /// </returns>
        public static bool IsWalkable( int id )
        {
            var tileId = (ActionTileId)id;

            return
                tileId == ActionTileId.Normal || 
                tileId == ActionTileId.FloorDown ||
                tileId == ActionTileId.FloorUp;
        }

        /// <summary>
        /// Handles the tile at the given position and with the given id for the given caller.
        /// </summary>
        /// <param name="x">The position on the x-axis of the tile to handle (in tilespace).</param>
        /// <param name="y">The position on the y-axis of the tile to handle (in tilespace).</param>
        /// <param name="id">The id of the tile.</param>
        /// <param name="caller">The caller that wants a tile to be handled.</param>
        /// <returns>
        /// true if to stop handling tiles;
        /// otherwise false.
        /// </returns>
        public bool Handle( int x, int y, int id, Moveable caller )
        {
            switch( (ActionTileId)id )
            {
                case ActionTileId.Normal:
                case ActionTileId.WaterWalkable:
                default:
                    return false;

                case ActionTileId.Solid:
                case ActionTileId.SolidNotAffectingFlying:
                    return caller.CollidesWithMap;

                case ActionTileId.WaterSwimable:
                    return !caller.CanCurrentlySwim;

                case ActionTileId.FloorUp:
                    if( !caller.CanChangeFloor )
                        return true;

                    caller.MoveFloorUp();
                    return false;

                case ActionTileId.FloorDown:
                    if( !caller.CanChangeFloor )
                        return true;

                    caller.MoveFloorDown();
                    return false;

                case ActionTileId.FallUnsafe:
                    // caller.StartFall( FallingMode.Unsafe );
                    return true;

                case ActionTileId.SolidForAll:
                case ActionTileId.Invalid:
                    return true;
            }
        }
    }
}
