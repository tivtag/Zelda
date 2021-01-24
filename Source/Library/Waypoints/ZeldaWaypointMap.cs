// <copyright file="ZeldaWaypointMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.ZeldaWaypointMap class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Waypoints
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;
    using System.Linq;
    using Atom;
    using Atom.Waypoints;

    /// <summary>
    /// Represents a WaypointMap that is part of a specific ZeldaScene.
    /// </summary>
    public sealed partial class ZeldaWaypointMap : WaypointMap
    {
        /// <summary>
        /// Raised when a new <see cref="ZeldaPath"/> has been added to this ZeldaWaypointMap.
        /// </summary>
        public event RelaxedEventHandler<ZeldaPath> PathAdded;

        /// <summary>
        /// Raised when a new <see cref="ZeldaPath"/> has been removed from this ZeldaWaypointMap.
        /// </summary>
        public event RelaxedEventHandler<ZeldaPath> PathRemoved;

        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> this ZeldaWaypointMap contains.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaPath"/>s that are part of this ZeldaWaypointMap.
        /// </summary>
        public IEnumerable<ZeldaPath> Paths
        {
            get
            {
                return this.paths;
            }
        }

        /// <summary>
        /// Gets the number of <see cref="ZeldaPath"/>s this ZeldawaypointMap contains.
        /// </summary>
        public int PathCount
        {
            get
            {
                return this.paths.Count;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ZeldaWaypointMap class.
        /// </summary>
        /// <param name="scene">
        /// The scene that owns the new ZeldaWaypointMap.
        /// </param>
        internal ZeldaWaypointMap( ZeldaScene scene )
            : base( new ZeldaWaypointGraphDataFactory( scene ) )
        {
            Contract.Requires<ArgumentNullException>( scene != null );

            this.scene = scene;
        }

        /// <summary>
        /// Initializes this ZeldaWaypointMap.
        /// </summary>
        public void Initialize()
        {
            this.Initialize( this.scene.Map.SizeInPixels, 2 );
        }

        /// <summary>
        /// Adds a new, empty, Path to this ZeldaWaypointMap.
        /// </summary>
        /// <returns>
        /// The newly created ZeldaPath.
        /// </returns>
        public ZeldaPath AddPath()
        {
            var path = new ZeldaPath( this );
           
            this.paths.Add( path );

            this.PathAdded.Raise( this, path );
            return path;
        }

        /// <summary>
        /// Attempts to remove the specified Path from this ZeldaWaypointMap.
        /// </summary>
        /// <param name="path">
        /// The Path to remove.
        /// </param>
        /// <returns>
        /// true if it has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemovePath( ZeldaPath path )
        {
            if( this.paths.Remove( path ) )
            {
                this.PathRemoved.Raise( this, path );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="ZeldaPath"/> at the specified zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the path to get.
        /// </param>
        /// <returns>
        /// The requested Path.
        /// </returns>
        public ZeldaPath GetPathAt( int index )
        {
            return this.paths[index];
        }

        /// <summary>
        /// Attempts to get the ZeldaPath with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the path to get.
        /// </param>
        /// <returns>
        /// The requested path;
        /// or null if it does not exist.
        /// </returns>
        public ZeldaPath GetPath( string name )
        {
            return this.paths.FirstOrDefault( path => path.Name.Equals( name, StringComparison.Ordinal ) );
        }

        /// <summary>
        /// The scene that owns this ZeldaWaypointMap.
        /// </summary>
        private readonly ZeldaScene scene;

        /// <summary>
        /// The <see cref="ZeldaPath"/>s that are part of this ZeldaWaypointMap.
        /// </summary>
        private readonly List<ZeldaPath> paths = new List<ZeldaPath>();
    }
}
