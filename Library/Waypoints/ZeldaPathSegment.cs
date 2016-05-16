// <copyright file="ZeldaPathSegment.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaPathSegment class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Waypoints
{
    using Atom.Scene.Tiles;
    using Atom.Waypoints;
    using Atom.Math;

    /// <summary>
    /// Represents a <see cref="PathSegment"/> in a <see cref="ZeldaWaypointMap"/>.
    /// </summary>
    public sealed class ZeldaPathSegment : TilePathSegment
    {
        /// <summary>
        /// Gets the distance of this ZeldaPathSegment on the tile-level.
        /// </summary>
        public float TileDistance
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes a new instance of the ZeldaPathSegment class.
        /// </summary>
        /// <param name="pathBuildService">
        /// Provides a mechanism for building the underlying TilePath of this TilePathSegment.
        /// </param>
        public ZeldaPathSegment( ITilePathSegmentPathBuildService pathBuildService )
            : base( pathBuildService )
        {
        }

        /// <summary>
        /// Called when the TilePath underlying this TilePathSegment has been build.
        /// </summary>
        /// <param name="tilePath">
        /// The TilePath that has been build.
        /// </param>
        protected override void OnTilePathBuild( TilePath tilePath )
        {
            this.TileDistance = MeasureTilePathLength( tilePath );
        }

        /// <summary>
        /// Measures the length of the specified TilePath.
        /// </summary>
        /// <param name="tilePath">
        /// The input TilePath.
        /// </param>
        /// <returns>
        /// The length of specified TilePath in pixels.
        /// </returns>
        private static float MeasureTilePathLength( TilePath tilePath )
        {
            if( tilePath.Length <= 1 )
                return 0.0f;

            float length = 0.0f;

            Point2 lastTile = tilePath[0];
            Vector2 lastCenter;
            lastCenter.X = lastTile.X * 16f + 8.0f;
            lastCenter.Y = lastTile.Y * 16f + 8.0f;

            for( int i = 1; i < tilePath.Length; ++i )
            {
                Vector2 tile = tilePath[i];
                Vector2 center;
                center.X = tile.X * 16f + 8.0f;
                center.Y = tile.Y * 16f + 8.0f;

                length += Vector2.Distance( lastCenter, center );
                lastCenter = center;
            }

            return length;
        }

        /// <summary>
        /// Updates the Weight of this ZeldaPathSegment.
        /// </summary>
        protected override void UpdateWeight()
        {
            if( this.WeightModifier != null )
            {
                this.Weight = this.WeightModifier.Apply( this.TileDistance );
            }
            else
            {
                this.Weight = this.Distance;
            }
        }
    }
}
