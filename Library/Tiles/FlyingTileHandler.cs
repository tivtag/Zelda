// <copyright file="FlyingTileHandler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.FlyingTileHandler class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using Zelda.Entities.Components;

    /// <summary>
    /// Defines the <see cref="IZeldaTileHandler"/> for ranged entities, such as Projectiles.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class FlyingTileHandler : IZeldaTileHandler
    {
        /// <summary>
        /// Receives the instance of the <see cref="FlyingTileHandler"/> class.
        /// </summary>
        public static readonly FlyingTileHandler Instance =
            new FlyingTileHandler();

        /// <summary>
        /// Prevents a default instance of the RangedTileHandler class from being created.
        /// </summary>
        private FlyingTileHandler()
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
                case ActionTileId.SolidNotAffectingFlying:
                case ActionTileId.FallUnsafe:
                    return true;

                case ActionTileId.Solid:
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
        /// Receives a value that indicates whether 
        /// a caller would be able to walk over the tile 
        /// with the specifies id by deafult.
        /// </summary>
        /// <param name="id">The id of the tile.</param>
        /// <returns>
        /// true if a caller could walk over the tile with the given id by default;
        /// otherwise false.
        /// </returns>
        public static bool IsWalkableByDefault( ActionTileId id )
        {
            switch( id )
            {
                case ActionTileId.Normal:
                case ActionTileId.WaterWalkable:
                case ActionTileId.SolidNotAffectingFlying:
                    return true;

                case ActionTileId.FallUnsafe:
                    return true;

                case ActionTileId.Solid:
                    return false;

                case ActionTileId.WaterSwimable:
                    return true;

                case ActionTileId.FloorUp:
                case ActionTileId.FloorDown:
                    return true;

                case ActionTileId.SolidForAll:
                case ActionTileId.Invalid:
                default:
                    return false;
            }
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
                case ActionTileId.SolidNotAffectingFlying:
                default:
                    return false;

                case ActionTileId.Solid:
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
                case ActionTileId.SolidForAll:
                case ActionTileId.Invalid:
                    return true;
            }
        }
    }
}
