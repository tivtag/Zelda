// <copyright file="ActionTileId.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ActionTileId enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    /// <summary>
    /// Enumerates the different action tile ids used in the game.
    /// </summary>
    public enum ActionTileId
    {
        /// <summary>
        /// An invalid ActionTileId.
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// An normal ActionTileId, nothing special happens here.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// A solid ActionTileId, solid objects get blocked by these.
        /// </summary>
        Solid = 1,

        /// <summary>
        /// A solid tile, which doesn't affect flying entities (such as arrows, magical bolts or the fairy).
        /// </summary>
        SolidNotAffectingFlying = 2,

        /// <summary>
        /// A solid tile, which is solid FOR ALL objects, even 'ghost like' ones that have IsSolid set to false.
        /// </summary>
        SolidForAll = 3,

        /// <summary>
        /// A water tile, where if possible, the object can swim in.
        /// </summary>
        WaterSwimable = 4,

        /// <summary>
        /// A water tile, any object can walk in it.
        /// </summary>
        WaterWalkable = 5,

        /// <summary>
        /// When triggered the object moves one TileMapFloor up.
        /// </summary>
        FloorUp = 6,

        /// <summary>
            /// When triggered the object moves one TileMapFloor down.
        /// </summary>
        FloorDown = 7,

        /// <summary>
        /// An unsafe tile, the object 'falls' down.
        /// </summary>
        FallUnsafe = 8
    }
}
