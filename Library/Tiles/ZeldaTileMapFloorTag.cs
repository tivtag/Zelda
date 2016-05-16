// <copyright file="ZeldaTileMapFloorTag.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaTileMapFloorTag class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System.Collections.Generic;
    using Atom.Scene.Tiles;

    /// <summary>
    /// Defines the tag which is applied to every TileMapFloor in a scene.
    /// </summary>
    public sealed class ZeldaTileMapFloorTag
    {
        /// <summary>
        /// Gets or sets the <see cref="Atom.AI.AStarTilePathSearcher"/> associated with the TileMapFloor.
        /// </summary>
        public Atom.AI.AStarTilePathSearcher PathSearcher 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets the list of  <see cref="IZeldaFloorDrawable"/>s that are visible on the TileMapFloor.
        /// </summary>
        public List<IZeldaFloorDrawable> VisibleDrawables
        {
            get 
            {
                return this.visibleDrawables; 
            }
        }

        /// <summary>
        /// Sorts the VisibleDrawables list of this <see cref="ZeldaTileMapFloorTag"/>.
        /// </summary>
        internal void SortVisibleDrawables()
        {
            this.visibleDrawables.Sort( CompareDrawablesOnSameFloor );
        }

        /// <summary>
        /// Cleares the VisibleDrawables list of this <see cref="ZeldaTileMapFloorTag"/>.
        /// </summary>
        internal void ClearVisibleDrawables()
        {
            this.visibleDrawables.Clear();
        }

        /// <summary>
        /// Creates a new <see cref="ZeldaTileMapFloorTag"/> for the given <see cref="TileMapFloor"/>.
        /// </summary>
        /// <param name="floor">
        /// The TileMapFloor whose ZeldaTileMapFloorTag should be created.
        /// </param>
        /// <returns>
        /// A newly created ZeldaTileMapFloorTag.
        /// </returns>
        internal static ZeldaTileMapFloorTag Create( TileMapFloor floor )
        {
            return new ZeldaTileMapFloorTag() {
                PathSearcher = CreatePathSearcher( floor )
            };
        }

        /// <summary>
        /// Creates a new Atom.AI.AStarTilePathSearcher that is responsible for finding
        /// paths on the action layer of the specified TileMapFloor.
        /// </summary>
        /// <param name="floor">
        /// The TileMapFloor whose AStarTilePathSearcher should be created.
        /// </param>
        /// <returns>
        /// The newly created AStarTilePathSearcher; 
        /// or null if none is required.
        /// </returns>
        private static Atom.AI.AStarTilePathSearcher CreatePathSearcher( TileMapFloor floor )
        {
            var actionLayer = floor.ActionLayer;

            if( actionLayer != null )
            {
                var pathSearcher = new Atom.AI.AStarTilePathSearcher();
                pathSearcher.Setup( actionLayer );

                return pathSearcher;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Compares two IFloorDrawables that are on the same TileMapFloor.
        /// </summary>
        /// <param name="x">The first IFloorDrawable.</param>
        /// <param name="y">The second IFloorDrawable.</param>
        /// <returns>
        /// Less than 0: x is below y.
        /// Equals 0: x equals y.
        /// Greater than 0: x above y.
        /// </returns>
        private static int CompareDrawablesOnSameFloor( IZeldaFloorDrawable x, IZeldaFloorDrawable y )
        {
            if( x == y )
                return 0;

            float drawOrderX = x.RelativeDrawOrder;
            float drawOrderY = y.RelativeDrawOrder;

            if( drawOrderX < drawOrderY )
            {
                return -1;
            }
            else if( drawOrderX > drawOrderY )
            {
                return 1;
            }
            else
            {
                float drawOrderX2 = x.SecondaryDrawOrder;
                float drawOrderY2 = y.SecondaryDrawOrder;

                if( drawOrderX2 < drawOrderY2 )
                {
                    return -1;
                }
                else if( drawOrderX2 > drawOrderY2 )
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// the list of <see cref="IZeldaFloorDrawable"/>s that
        /// are visible on the TileMapFloor.
        /// </summary>
        private readonly List<IZeldaFloorDrawable> visibleDrawables = new List<IZeldaFloorDrawable>();
    }
}
