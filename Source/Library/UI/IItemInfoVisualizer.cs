// <copyright file="IItemInfoVisualizer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.IItemInfoVisualizer interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Zelda.Items;
    using Zelda.Status;

    /// <summary>
    /// Provides a mechanism for visualizing the information the player needs to know about an Item.
    /// </summary>
    public interface IItemInfoVisualizer : IZeldaSetupable
    {
        /// <summary>
        /// Draws the specified ItemInstance at the given position and depth.
        /// </summary>
        /// <param name="context">
        /// Encapsulates all draw and configuration data required for this operation.
        /// </param>
        void Draw( ItemInfoVisualizationDrawContext context );

        /// <summary>
        /// Resets the internaly cached visualizations of items.
        /// </summary>
        void ResetCache();
    }
}
