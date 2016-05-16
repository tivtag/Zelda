// <copyright file="PathViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.PathViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using System;
    using System.Linq;
    using System.Text;
    using Atom.Waypoints;
    using Atom.Wpf;
    using Atom.Collections;
    using Zelda.Waypoints;

    /// <summary>
    /// Represents the ViewModel that wraps around the ZeldaPath class.
    /// </summary>
    public sealed class PathViewModel : ViewModel<ZeldaPath>
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies the Path.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.Name;
            }

            set
            {
                if( string.IsNullOrWhiteSpace( value ) )
                    throw new ArgumentNullException( "value" );

                if( value == this.Name )
                    return;

                if( this.owner.Paths.Any( path => value.Equals( path.Name, StringComparison.OrdinalIgnoreCase ) ) )
                    throw new ArgumentException( Properties.Resources.Error_NameMustBeUnique );

                this.Model.Name = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets a string descriping the Waypoints that the Path contains.
        /// </summary>
        [LocalizedDisplayName( "PropDisp_WaypointDescription" )]
        public string WaypointDescription
        {
            get
            {
                var sb = new StringBuilder();

                for( int index = 0; index < this.Model.Length; ++index )
                {
                    Waypoint waypoint = this.Model[index];
                    sb.Append( waypoint.Name ?? string.Empty );

                    if( index < this.Model.Length - 1 )
                    {
                        sb.Append( " --> " );
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the WaypointViewModel that has been selected within this PathViewModel.
        /// </summary>
        public WaypointViewModel SelectedWaypoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the PathViewModel class.
        /// </summary>
        /// <param name="owner">
        /// The WaypointMapViewModel that owns the new PathViewModel.
        /// </param>
        /// <param name="model">
        /// The ZeldaPath the new PathViewModel wraps around.
        /// </param>
        public PathViewModel( WaypointMapViewModel owner, ZeldaPath model )
            : base( model )
        {
            this.owner = owner;
        }

        /// <summary>
        /// Attempts to select the specified WaypointViewModel in this PathViewModel.
        /// </summary>
        /// <param name="waypoint">
        /// The WaypointViewModel to select.
        /// </param>
        /// <returns>
        /// True if the specified WaypointViewModel has been selected;
        /// otherwise false.
        /// </returns>
        public bool SelectWaypoint( WaypointViewModel waypoint )
        {
            if( waypoint == null )
                return false;

            int index = this.Model.IndexOf( waypoint.Model );
            if( index == -1 )
                return false;

            this.SelectedWaypoint = waypoint;
            return true;
        }

        /// <summary>
        /// Attempts to directly connect the end of the Path
        /// with the specified Waypoint.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to connect the end with.
        /// </param>
        public void ConnectEndWith( Waypoint waypoint )
        {
            var end = this.Model.End;

            if( end == null || end.HasPathSegmentTo( waypoint ) )
            {
                this.Model.Add( waypoint );
            }
        }

        /// <summary>
        /// Attempts to connect the currently SelectedWaypoint with the specified Waypoint.
        /// </summary>
        /// <param name="waypoint">
        /// The waypoint to connect with.
        /// </param>
        public void ConnectSelectedWith( Waypoint waypoint )
        {
            if( this.SelectedWaypoint == null )
                return;

            int index = this.Model.IndexOf( this.SelectedWaypoint.Model );
            SafeExecute.WithMsgBox(
                () => {
                    this.Model.Insert( index, waypoint );
                }
            );
        }

        /// <summary>
        /// Overriden to return the string returned by ToString of the Model this PathViewModel wraps around.
        /// </summary>
        /// <returns>
        /// The string returned by the model.
        /// </returns>
        public override string ToString()
        {
            return "Path " + this.Name ?? string.Empty;
        }

        /// <summary>
        /// Called when the specified Waypoint has been removed/deleted.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint that has been removed; is part of the Path.
        /// </param>
        public void HandleRemovedWaypoint( Waypoint waypoint )
        {
            RefactorResult refactorResult;
            while( (refactorResult = this.RefactorRemovedWaypoint( waypoint )) == RefactorResult.Success );

            if( refactorResult == RefactorResult.Error )
            {
                this.Model.RemoveAllAfter( this.Model.IndexOf( waypoint ) );
            }
        }

        /// <summary>
        /// Attempts to refactor the Path in response for the given Waypoint turning invalid.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint that was removed.
        /// </param>
        /// <returns>
        /// The result of the refactoring operation.
        /// </returns>
        private RefactorResult RefactorRemovedWaypoint( Waypoint waypoint )
        {
            int index = this.Model.IndexOf( waypoint );
            if( index == -1 )
                return RefactorResult.Nothing;

            return this.Model.RemoveAt( index ) ? RefactorResult.Success : RefactorResult.Error;
        }

        /// <summary>
        /// Called when the specified ZeldaPathSegment has been removed/deleted.
        /// </summary>
        /// <param name="segment">
        /// The ZeldaPathSegment that has been removed; is part of the Path.
        /// </param>
        public void HandleRemovedSegment( ZeldaPathSegment segment )
        {
            RefactorResult refactorResult;
            while( (refactorResult = this.RefactorRemovedSegment( segment )) == RefactorResult.Success ) ;

            if( refactorResult == RefactorResult.Error )
            {
                Waypoint before = null;

                for( int index = 0; index < this.Model.Length; ++index )
                {
                    Waypoint current = this.Model[index];

                    if( before != null )
                    {
                        if( !before.HasPathSegmentTo( current ) )
                        {
                            this.Model.RemoveAllAfter( index );
                        }
                    }

                    before = current;
                }
            }
        }

        /// <summary>
        /// Attempts to refactor the Path in response for the given PathSegment turning invalid.
        /// </summary>
        /// <param name="segment">
        /// The ZeldaPathSegment that was removed.
        /// </param>
        /// <returns>
        /// The result of the refactoring operation.
        /// </returns>
        private RefactorResult RefactorRemovedSegment( ZeldaPathSegment segment )
        {
            return RefactorResult.Error;
        }

        /// <summary>
        /// The WaypointMapViewModel that owns this PathViewModel.
        /// </summary>
        private readonly WaypointMapViewModel owner;
    }
}
