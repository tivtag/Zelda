// <copyright file="TilePathDrawer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.TilePathDrawer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics
{
    using Atom.Math;
    using Atom.Scene.Tiles;

    /// <summary>
    /// Implements a mechanism that visualizes <see cref="TilePath"/>s.
    /// </summary>
    public sealed class TilePathDrawer
    {
        /// <summary>
        /// Draws the specified TilePath in the specified Color.
        /// </summary>
        /// <param name="path">
        /// The TilePath to visualize.
        /// </param>
        /// <param name="color">
        /// The color the path should be tinted in.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( TilePath path, Microsoft.Xna.Framework.Color color, ZeldaDrawContext drawContext )
        {
            if( path == null )
                return;

            const int TileSize = 16;
            var batch = drawContext.Batch;

            for( int i = 0; i < path.Length; ++i )
            {
                Point2 tile = path[i];

                Point2 sceneSpace;
                sceneSpace.X = tile.X * TileSize;
                sceneSpace.Y = tile.Y * TileSize;

                batch.DrawRect( new Rectangle( sceneSpace.X, sceneSpace.Y, TileSize, TileSize ), color );
            }
        }
    }
}
