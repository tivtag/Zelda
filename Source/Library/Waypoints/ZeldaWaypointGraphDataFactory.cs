// <copyright file="ZeldaWaypointGraphDataFactory.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaWaypointGraphDataFactory class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Waypoints
{
    using Atom.Waypoints;

    /// <summary>
    /// Implements an <see cref="Atom.Math.Graph.IGraphDataFactory{Waypoint, PathSegment}"/> for the WaypointGraph that
    /// works underlying the ZeldaWaypointMap.
    /// </summary>
    internal sealed class ZeldaWaypointGraphDataFactory : IWaypointGraphDataFactory
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaWaypointGraphDataFactory class.
        /// </summary>
        /// <param name="scene">
        /// The scene that contains the ZeldaWaypointMap.
        /// </param>
        public ZeldaWaypointGraphDataFactory( ZeldaScene scene )
        {
            this.pathBuildService = new TilePathSegmentPathBuildService( ZeldaPathSegmentTileHandler.Instance, scene );
        }

        /// <summary>
        /// Builds the TVertexData stored in a <see cref="Atom.Math.Graph.Vertex{TVertexData, TEdgeData}"/>.
        /// </summary>
        /// <returns>
        /// The TVertexData that will be assigned to the Vertex{TVertexData, TEdgeData}.
        /// </returns>
        public Waypoint BuildVertexData()
        {
            return new ZeldaWaypoint();
        }

        /// <summary>
        /// Builds the TEdgeData stored in a <see cref="Atom.Math.Graph.Edge{TVertexData, TEdgeData}"/>.
        /// </summary>
        /// <returns>
        /// The TEdgeData that will be assigned to the Edge{TVertexData, TEdgeData}.
        /// </returns>
        public PathSegment BuildEdgeData()
        {
            return new ZeldaPathSegment( this.pathBuildService );
        }

        /// <summary>
        /// Provides a mechanism for building the underlying TilePath of this TilePathSegment.
        /// </summary>
        private readonly ITilePathSegmentPathBuildService pathBuildService;
    }
}
