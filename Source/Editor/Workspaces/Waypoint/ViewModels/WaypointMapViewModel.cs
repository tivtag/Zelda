// <copyright file="WaypointWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Data;
    using Atom.Math;
    using Atom.Waypoints;
    using Atom.Wpf;
    using Zelda.Waypoints;

    /// <summary>
    /// Represents the ViewModel that wraps around a <see cref="ZeldaWaypointMap"/> model.
    /// </summary>
    public sealed class WaypointMapViewModel : ViewModel<ZeldaWaypointMap>
    {
        /// <summary>
        /// Gets the <see cref="CollectionView"/> over the <see cref="WaypointViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public CollectionView WaypointsView
        {
            get
            {
                return this.waypointsView;
            }
        }

        /// <summary>
        /// Gets the <see cref="WaypointViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public IEnumerable<WaypointViewModel> Waypoints
        {
            get
            {
                return this.waypoints;
            }
        }

        /// <summary>
        /// Gets the <see cref="CollectionView"/> over the <see cref="PathSegmentViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public CollectionView PathSegmentsView
        {
            get
            {
                return this.pathSegmentsView;
            }
        }

        /// <summary>
        /// Gets the <see cref="PathSegmentViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public IEnumerable<PathSegmentViewModel> PathSegments
        {
            get
            {
                return this.pathSegments;
            }
        }

        /// <summary>
        /// Gets the <see cref="CollectionView"/> over the <see cref="PathViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public CollectionView PathsView
        {
            get
            {
                return this.pathsView;
            }
        }

        /// <summary>
        /// Gets the <see cref="PathViewModel"/>s this <see cref="WaypointMapViewModel"/> contains.
        /// </summary>
        public IEnumerable<PathViewModel> Paths
        {
            get
            {
                return this.paths;
            }
        }

        /// <summary>
        /// Gets or sets the WaypointViewModel that the user currently has selected.
        /// </summary>
        public WaypointViewModel SelectedWaypoint
        {
            get
            {
                return this.waypointsView.CurrentItem as WaypointViewModel;
            }

            set
            {
                this.waypointsView.MoveCurrentTo( value );
            }
        }

        /// <summary>
        /// Gets or sets the PathSegmentViewModel that the user currently has selected.
        /// </summary>
        public PathSegmentViewModel SelectedPathSegment
        {
            get
            {
                return this.pathSegmentsView.CurrentItem as PathSegmentViewModel;
            }

            set
            {
                this.pathSegmentsView.MoveCurrentTo( value );
            }
        }

        /// <summary>
        /// Gets or sets the PathViewModel that the user currently has selected.
        /// </summary>
        public PathViewModel SelectedPath
        {
            get
            {
                return this.pathsView.CurrentItem as PathViewModel;
            }

            set
            {
                this.pathsView.MoveCurrentTo( value );
            }
        }

        /// <summary>
        /// Initializes a new instance of the WaypointMapViewModel class.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ZeldaWaypointMap"/> the new WaypointMapViewModel wraps around.
        /// </param>
        public WaypointMapViewModel( ZeldaWaypointMap model )
            : base( model )
        {
            // Collections.
            foreach( ZeldaWaypoint waypoint in model.Waypoints )
            {
                this.waypoints.Add( new WaypointViewModel( this, waypoint ) );
            }

            foreach( var pathSegment in model.PathSegments )
            {
                this.pathSegments.Add( this.CreatePathSegmentViewModel( pathSegment ) );
            }

            foreach( var path in model.Paths )
            {
                this.paths.Add( new PathViewModel( this, path ) );
            }

            // Collection Views.
            this.waypointsView = new ListCollectionView( this.waypoints );
            this.pathSegmentsView = new ListCollectionView( this.pathSegments );
            this.pathsView = new ListCollectionView( this.paths );

            // Events.
            this.Model.WaypointAdded += this.OnModelWaypointAdded;
            this.Model.WaypointRemoved += this.OnModelWaypointRemoved;
            this.Model.PathSegmentAdded += this.OnModelPathSegmentAdded;
            this.Model.PathSegmentRemoved += this.OnModelPathSegmentRemoved;
            this.Model.PathAdded += this.OnModelPathAdded;
            this.Model.PathRemoved += this.OnModelPathRemoved;

            this.pathSegmentsView.CurrentChanged += this.OnCurrentPathSegmentChanged;
        }

        /// <summary>
        /// Creates a new PathSegmentViewModel for the specified PathSegment.
        /// </summary>
        /// <param name="pathSegment">
        /// The PathSegment that should be wrapped.
        /// </param>
        /// <returns>
        /// The newly created PathSegmentViewModel.
        /// </returns>
        private PathSegmentViewModel CreatePathSegmentViewModel( PathSegment pathSegment )
        {
            var from = this.waypoints.First( viewModel => viewModel.Model == pathSegment.From );
            var to = this.waypoints.First( viewModel => viewModel.Model == pathSegment.To );

            return new PathSegmentViewModel( from, to, (ZeldaPathSegment)pathSegment );
        }

        /// <summary>
        /// Adds a new <see cref="WaypointViewModel"/> to this WaypointMapViewModel.
        /// </summary>
        /// <param name="position">
        /// The location at which the Waypoint should be placed.
        /// </param>
        /// <returns>
        /// The newly created WaypointViewModel.
        /// </returns>
        public WaypointViewModel AddWaypoint( Vector2 position )
        {
            var waypoint = (ZeldaWaypoint)this.Model.AddWaypoint( position );
            waypoint.Name = UniqueNameHelper.Get( "WP_", this.Model.Waypoints );

            return this.waypoints.FirstOrDefault( viewModel => viewModel.Model == waypoint );
        }

        /// <summary>
        /// Connects the specified Waypoints.
        /// </summary>
        /// <param name="from">
        /// The first WaypointViewModel.
        /// </param>
        /// <param name="to">
        /// The second WaypointViewModel.
        /// </param>
        /// <returns>
        /// The PathSegmentViewModel that now connects the specified WaypointViewModel.
        /// </returns>
        public PathSegmentViewModel AddPathSegment( WaypointViewModel from, WaypointViewModel to )
        {
            Contract.Requires<ArgumentNullException>( from != null );
            Contract.Requires<ArgumentNullException>( to != null );

            PathSegment pathSegment;

            if( !from.HasPathSegmentTo( to ) )
            {
                pathSegment = this.Model.AddPathSegment( from.Model, to.Model );
            }
            else
            {
                pathSegment = this.Model.GetPathSegment( from.Model, to.Model );
            }

            return this.pathSegments.FirstOrDefault( viewModel => viewModel.Model == pathSegment );
        }

        /// <summary>
        /// Adds a new <see cref="PathViewModel"/> to this WaypointMapViewModel.
        /// </summary>
        /// <param name="startWaypoint">
        /// The WaypointViewModel the new path starts at.
        /// </param>
        /// <returns>
        /// The newly created PathViewModel.
        /// </returns>
        public PathViewModel AddPath( WaypointViewModel startWaypoint )
        {
            var path = this.Model.AddPath();
            path.Name = UniqueNameHelper.Get( "P_", this.Model.Paths );

            var pathViewModel = this.paths.FirstOrDefault( viewModel => viewModel.Model == path );
            
            path.Add( startWaypoint.Model );
            return pathViewModel;
        }

        /// <summary>
        /// Called when a new PathSegment has been added to the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="segment">
        /// The segment that has been added.
        /// </param>
        private void OnModelPathSegmentAdded( WaypointMap sender, PathSegment segment )
        {
            this.pathSegments.Add( CreatePathSegmentViewModel( segment ) );
        }

        /// <summary>
        /// Called when a PathSegment has been removed from the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="segment">
        /// The segment that has been added.
        /// </param>
        private void OnModelPathSegmentRemoved( WaypointMap sender, PathSegment segment )
        {
            this.pathSegments.Remove( this.pathSegments.FirstOrDefault( viewModel => viewModel.Model == segment ) );
        }
        
        /// <summary>
        /// Called when a Waypoint has been added to the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="waypoint">
        /// The waypoint that has been added.
        /// </param>
        private void OnModelWaypointAdded( WaypointMap sender, Waypoint waypoint )
        {
            this.waypoints.Add( new WaypointViewModel( this, (ZeldaWaypoint)waypoint ) );
        }

        /// <summary>
        /// Called when a Waypoint has been removed from the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="waypoint">
        /// The waypoint that has been removed.
        /// </param>
        private void OnModelWaypointRemoved( WaypointMap sender, Waypoint waypoint )
        {
            this.waypoints.Remove( this.GetViewModel( waypoint ) );
        }

        /// <summary>
        /// Called when a new ZeldaPath has been added to the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="path">
        /// The path that has been added.
        /// </param>
        private void OnModelPathAdded( object sender, ZeldaPath path )
        {
            this.paths.Add( new PathViewModel( this, path ) );
        }

        /// <summary>
        /// Called when a Path has been removed from the WaypointMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="path">
        /// The Path that has been added.
        /// </param>
        private void OnModelPathRemoved( object sender, ZeldaPath path )
        {
            this.paths.Remove( this.paths.FirstOrDefault( viewModel => viewModel.Model == path ) );
        }

        /// <summary>
        /// Gets the <see cref="WaypointViewModel"/> that is associated with the specified <see cref="Waypoint"/>.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to locate.
        /// </param>
        /// <returns>
        /// The associated WaypointViewModel; or null if the specified Waypoint is not part of the WaypointMap.
        /// </returns>
        public WaypointViewModel GetViewModel( Waypoint waypoint )
        {
            return this.waypoints.FirstOrDefault( viewModel => viewModel.Model == waypoint );
        }

        /// <summary>
        /// Called when the currently selected PathSegment has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnCurrentPathSegmentChanged( object sender, EventArgs e )
        {
            var segment = this.SelectedPathSegment;

            if( segment != null )
            {
                segment.NotifySelected();
            }
        }

        /// <summary>
        /// Removes the specified WaypointViewModel and Waypoint from the WaypointMap.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to remove.
        /// </param>
        public void DeleteWaypoint( WaypointViewModel waypoint )
        {
            if( this.Model.RemoveWaypoint( waypoint.Model, true ) )
            {
                foreach( var path in this.paths )
                {
                    path.HandleRemovedWaypoint( waypoint.Model );
                }
            }
        }

        /// <summary>
        /// Removes the specified PathSegmentViewModel and PathSegment from the WaypointMap.
        /// </summary>
        /// <param name="segment">
        /// The PathSegment to remove.
        /// </param>
        public void DeletePathSegment( PathSegmentViewModel segment )
        {
            if( this.Model.RemovePathSegment( segment.Model ) )
            {
                foreach( var path in this.paths )
                {
                    path.HandleRemovedSegment( segment.Model );
                }
            }
        }

        /// <summary>
        /// Removes the specified PathViewModel from the WaypointMap.
        /// </summary>
        /// <param name="segment">
        /// The Path to remove.
        /// </param>
        public void DeletePath( PathViewModel path )
        {
            this.Model.RemovePath( path.Model );
        }

        /// <summary>
        /// Returns the objects that would be affected if the specified WaypointViewModel
        /// would be modified or deleted.
        /// </summary>
        /// <param name="waypointViewModel">
        /// The WaypointViewModel to locate.
        /// </param>
        /// <returns>
        /// The affected objects.
        /// </returns>
        public IEnumerable<object> FindRelated( WaypointViewModel waypointViewModel )
        {
            Contract.Requires<ArgumentNullException>( waypointViewModel != null );

            foreach( var path in this.paths )
            {
                if( path.Model.Contains( waypointViewModel.Model ) )
                {
                    yield return path;
                }
            }
        }

        /// <summary>
        /// Returns the objects that would be affected if the specified PathSegmentViewModel
        /// would be modified or deleted.
        /// </summary>
        /// <param name="pathSegmentViewModel">
        /// The PathSegmentViewModel to locate.
        /// </param>
        /// <returns>
        /// The affected objects.
        /// </returns>
        public IEnumerable<object> FindRelated( PathSegmentViewModel pathSegmentViewModel )
        {
            Contract.Requires<ArgumentNullException>( pathSegmentViewModel != null );

            foreach( var path in this.paths )
            {
                if( path.Model.Contains( pathSegmentViewModel.Model ) )
                {
                    yield return path;
                }
            }
        }

        /// <summary>
        /// Represents the view over the waypoints collections.
        /// </summary>
        private readonly ListCollectionView waypointsView;

        /// <summary>
        /// The WaypointViewModels this WaypointMapViewModel contains.
        /// </summary>
        private readonly ObservableCollection<WaypointViewModel> waypoints = new ObservableCollection<WaypointViewModel>();

        /// <summary>
        /// Represents the view over the pathSegments collections.
        /// </summary>
        private readonly ListCollectionView pathSegmentsView;

        /// <summary>
        /// The PathSegmentViewModels this WaypointMapViewModel contains.
        /// </summary>
        private readonly ObservableCollection<PathSegmentViewModel> pathSegments = new ObservableCollection<PathSegmentViewModel>();

        /// <summary>
        /// Represents the view over the paths collections.
        /// </summary>
        private readonly ListCollectionView pathsView;

        /// <summary>
        /// The PathViewModels this WaypointMapViewModel contains.
        /// </summary>
        private readonly ObservableCollection<PathViewModel> paths = new ObservableCollection<PathViewModel>();
    }
}
