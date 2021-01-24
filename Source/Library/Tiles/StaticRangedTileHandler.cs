// <copyright file="StaticRangedTileHandler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.StaticRangedTileHandler class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Atom.Scene.Tiles;
    using Zelda.Entities;

    /// <summary>
    /// Defines the <see cref="IZeldaTileHandler"/> for 'unmoveable' ranged entities.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class StaticRangedTileHandler : ITileHandler<ZeldaEntity>
    {
        /// <summary>
        /// Receives the instance of the <see cref="StaticRangedTileHandler"/> class.
        /// </summary>
        public static readonly StaticRangedTileHandler Instance =
            new StaticRangedTileHandler();

        /// <summary>
        /// Prevents a default instance of the StaticRangedTileHandler class from being created.
        /// </summary>
        private StaticRangedTileHandler()
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
        public bool IsWalkable( int id, ZeldaEntity caller )
        {
            return FlyingTileHandler.IsWalkableByDefault( (ActionTileId)id );
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
        public bool Handle( int x, int y, int id, ZeldaEntity caller )
        {
            switch( (ActionTileId)id )
            {
                case ActionTileId.Normal:
                case ActionTileId.WaterWalkable:
                case ActionTileId.SolidNotAffectingFlying:
                default:
                    return false;

                case ActionTileId.Solid:
                case ActionTileId.WaterSwimable:
                case ActionTileId.FloorUp:
                case ActionTileId.FloorDown:       
                case ActionTileId.FallUnsafe:
                case ActionTileId.SolidForAll:
                case ActionTileId.Invalid:
                    return true;
            }
        }
    }
}
