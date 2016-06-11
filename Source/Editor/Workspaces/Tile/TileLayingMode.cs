// <copyright file="TileLayingMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.TileLayingMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile
{
    /// <summary>
    /// Enumerates the different modes that descripe how the player is laying a tile.
    /// </summary>
    internal enum TileLayingMode
    {
        /// <summary>
        /// The user lays no tile.
        /// </summary>
        None,

        /// <summary>
        /// The user lays the selected tile every frame.
        /// </summary>
        Selected,

        /// <summary>
        /// The user lays the selected tile once.
        /// </summary>
        SelectedOnce,

        /// <summary>
        /// The user lays the null tile once (0).
        /// </summary>
        EraseOnce,
    }
}
