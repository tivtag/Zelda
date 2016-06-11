// <copyright file="TileMapFloorEditMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.TileMapFloorEditMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile
{
    /// <summary>
    /// Enumerates the editing modes of a TileMapFloor.
    /// The user can change the 'Space' key (by default).
    /// </summary>
    public enum TileMapFloorEditMode
    {
        /// <summary>
        /// Indicates that the user is editing the
        /// currently selected TileMapSpriteDataLayer.
        /// </summary>
        EditSelectedLayer,

        /// <summary>
        /// Inidicates that the user is editing the
        /// action layer of the currently selected TileMapFloor.
        /// </summary>
        EditActionLayer
    }
}
