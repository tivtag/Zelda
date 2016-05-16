// <copyright file="PathSegmentViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.PathSegmentViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using Atom;
    using Atom.Waypoints;
    using Atom.Wpf;
    using Zelda.Waypoints;

    /// <summary>
    /// Represents the ViewModel that wraps around a <see cref="PathSegement"/> model.
    /// </summary>
    public sealed class PathSegmentViewModel : ViewModel<ZeldaPathSegment>, IReadOnlyNameable
    {
        /// <summary>
        /// Gets the first <see cref="WaypointViewModel"/> this PathSegment is connected to.
        /// </summary>
        [Browsable( false )]
        public WaypointViewModel From
        {
            get
            {
                return this.from; 
            }
        }

        /// <summary>
        /// Gets the second <see cref="WaypointViewModel"/> this PathSegment is connected to.
        /// </summary>
        [Browsable( false )]
        public WaypointViewModel To
        {
            get
            {
                return this.to;
            }
        }

        /// <summary>
        /// Gets the name of this PathSegment.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.ToString();
            }
        }

        /// <summary>
        /// Gets the distance between the From Waypoint to the To Waypoint.
        /// </summary>
        public float Distance
        {
            get
            {
                return this.Model.Distance;
            }
        }

        /// <summary>
        /// Gets the distance between the From Waypoint to the To Waypoint
        /// on the tile level.
        /// </summary>
        [LocalizedDisplayName( "PropDisp_TileDistance" )]
        public float TileDistance
        {
            get
            {
                return this.Model.TileDistance;
            }
        }

        /// <summary>
        /// Gets the <see cref="WaypointMapViewModel"/> that owns this PathSegmentViewModel.
        /// </summary>
        [Browsable( false )]
        public WaypointMapViewModel Owner
        {
            get
            {
                return this.from.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PathSegmentViewModel class.
        /// </summary>
        /// <param name="model">
        /// The PathSegement the new PathSegmentViewModel wraps around.
        /// </param>
        public PathSegmentViewModel( WaypointViewModel fromWaypoint, WaypointViewModel toWaypoint, ZeldaPathSegment model )
            : base( model )
        {
            Contract.Requires<ArgumentNullException>( fromWaypoint != null );
            Contract.Requires<ArgumentNullException>( toWaypoint != null );

            this.from = fromWaypoint;
            this.to = toWaypoint;

            // Events.
            this.from.PropertyChanged += this.OnWaypointPropertyChanged;
            this.to.PropertyChanged += this.OnWaypointPropertyChanged;
        }

        /// <summary>
        /// Called when any property of the From or To Waypoints has changed. 
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The PropertyChangedEventArgs that contains the event data.
        /// </param>
        private void OnWaypointPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == "Name" )
            {
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Notifies this PathSegmentViewModel that it has been selected by the user.
        /// </summary>
        public void NotifySelected()
        {
            this.InvalidateCachedTilePath();
        }

        /// <summary>
        /// Invalidates the TilePath that has been cached for the PathSegment.
        /// </summary>
        private void InvalidateCachedTilePath()
        {
            var tileSegment = this.Model as TilePathSegment;

            if( tileSegment != null )
            {
                tileSegment.InvalidateCachedTilePath();
            }
        }

        /// <summary>
        /// The first <see cref="WaypointViewModel"/> this PathSegment is connected to.
        /// </summary>
        private readonly WaypointViewModel from;

        /// <summary>
        /// The second <see cref="WaypointViewModel"/> this PathSegment is connected to.
        /// </summary>
        private readonly WaypointViewModel to;
    }
}
