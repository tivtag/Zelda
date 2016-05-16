// <copyright file="ActionLayerVisabilityMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ActionLayerVisabilityMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile
{
    /// <summary>
    /// Describes which/how/when an ActionLayer is visible.
    /// </summary>
    public enum ActionLayerVisabilityMode
    {
        /// <summary>
        /// Only the ActionLayer of the currently selected TileMapFloor is visible.
        /// </summary>
        OnlyCurrent = 0,

        /// <summary>
        /// All ActionLayers of the TileMap are visible.
        /// </summary>
        All,

        /// <summary>
        /// All ActionLayers are hidden.
        /// </summary>
        None
    }
}
