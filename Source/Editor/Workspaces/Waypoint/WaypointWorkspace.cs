// <copyright file="WaypointWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Waypoint
{
    using System;
    using System.ComponentModel;
    using Atom.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;
    using Atom.Math;
    using Atom.Wpf;
    using Zelda.Waypoints;

    /// <summary>
    /// Implements an IWorkspace that allows the user to create and manage Waypoints, Path Segments and Paths.
    /// </summary>
    public sealed class WaypointWorkspace : IWorkspace, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurrs when a property of this WaypointWorkspace has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="WorkspaceType"/> of this <see cref="IWorkspace"/>.
        /// </summary>
        public WorkspaceType Type 
        {
            get
            {
                return WorkspaceType.Waypoint;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what kind of operations are executed as the user interacts with the WaypointMapViewModel.
        /// </summary>
        public WaypointWorkspaceEditMode EditMode
        {
            get
            {
                return this._editMode;
            }

            set
            {
                this._editMode = value;
                this.OnPropertyChanged( "EditMode" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaypointWorkspace"/> class.
        /// </summary>
        /// <param name="application">
        /// Provides access to XNA related objects of the Editor application.
        /// </param>
        public WaypointWorkspace( XnaEditorApp application )
        {
            Contract.Requires<ArgumentNullException>( application != null );

            this.application = application;
            this.application.SceneChanged += this.OnSceneChanged;
            this.sceneScroller = new SceneScroller( application );
            this.waypointMapDrawer = new WaypointMapDrawer( application );
        }

        /// <summary>
        /// Loads the content this WaypointWorkspace requires.
        /// </summary>
        public void LoadContent()
        {
            this.waypointMapDrawer.LoadContent();
        }

        /// <summary>
        /// Called when the currently active ZeldaScene has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ChangedValue{SceneViewModel} that contains the event data.
        /// </param>
        private void OnSceneChanged( object sender, Atom.ChangedValue<SceneViewModel> e )
        {
            if( e.NewValue != null )
            {
                this.waypointMap = e.NewValue.WaypointMap;
            }
            else
            {
                this.waypointMap = null;
            }

            this.scene = e.NewValue;
            this.waypointMapDrawer.WaypointMap = this.waypointMap;
        }

        /// <summary>
        /// Updates this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="updateContext">
        /// Tje current IUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.scene == null )
                return;

            this.scene.Model.Update( updateContext );
            this.sceneScroller.Update( updateContext );
            this.waypointMapDrawer.Update( updateContext );
        }

        /// <summary>
        /// Draws this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.scene == null )
                return;

            // Draw Scene.
            this.application.SceneDrawer.Draw( scene.Model, drawContext );
            this.application.DrawQuadTree();
            this.application.DrawTileMapBorder( drawContext );
            this.waypointMapDrawer.Draw( drawContext );
        }

        /// <summary>
        /// Gets called when the application has entered this <see cref="IWorkspace"/>.
        /// </summary>
        public void Enter()
        {
        }

        /// <summary>
        /// Gets called when the application has left this <see cref="IWorkspace"/>.
        /// </summary>
        public void Leave()
        {
            this.grapOffset = Vector2.Zero;
            this.grappedWaypoint = null;
            this.sceneScroller.Reset();
        }

        /// <summary>
        /// Gets called when the user clicks on the Xna-PictureBox.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseClick( System.Windows.Forms.MouseEventArgs e )
        {
        }

        /// <summary>
        /// Attempts to select the Waypoint at the specified location (in scene space).
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <returns>
        /// The WaypointViewModel that has been selected;
        /// or null if there was no Waypoint to select.
        /// </returns>
        private WaypointViewModel SelectWaypoint( int x, int y )
        {
            var waypoint = this.FindWaypoint( x, y );
            
            if( waypoint != null )
            {
                this.waypointMap.WaypointsView.MoveCurrentTo( waypoint );
            }

            return waypoint;
        }

        /// <summary>
        /// Attempts to find the Waypoint at the specified position,
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <returns>
        /// The WaypointViewModel that has been found;
        /// or null if there was no Waypoint at the specified location.
        /// </returns>
        private WaypointViewModel FindWaypoint( int x, int y )
        {
            const int SearchSize = 16;
            const int HalfSearchSize = SearchSize / 2;
            var searchArea = new Rectangle( x - HalfSearchSize, y - HalfSearchSize, SearchSize, SearchSize );

            var waypoints = 
                from wp in this.waypointMap.Model.GetWaypointsIn( searchArea )
                where (new Rectangle(
                         (Point2)wp.Position - new Point2( HalfSearchSize, HalfSearchSize ), new Point2( SearchSize, SearchSize )
                      ).Contains( x, y ))
                select wp;

            var waypoint = waypoints.FirstOrDefault();
            if( waypoints == null )
                return null;

            return this.waypointMap.Waypoints.FirstOrDefault(
                viewModel => viewModel.Model == waypoint
            );
        }

        /// <summary>
        /// Adds a new Waypoint at the specified position to the WaypointMap.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <param name="centerAtTile">
        /// States whether the waypoint should be centered at the current tile.
        /// </param>
        private void AddWaypoint( int x, int y, bool centerAtTile )
        {
            var position = new Vector2( x, y );

            if( centerAtTile )
            {
                // center within the current tile:
                position = new Vector2( x / 16, y / 16 );
                position *= 16.0f;
                position += 8.0f;
            }

            var waypoint = this.waypointMap.AddWaypoint( position );
            this.waypointMap.SelectedWaypoint = waypoint;
        }

        /// <summary>
        /// Gets called when the user presses any key while the window has focus but no other control.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyDown( System.Windows.Input.KeyEventArgs e )
        {
            switch( e.Key )
            {
                case Key.F1:
                    this.waypointMapDrawer.ShowAllTilePaths = !this.waypointMapDrawer.ShowAllTilePaths;
                    e.Handled = true;
                    return;

                case Key.F2:
                    this.waypointMapDrawer.ShowPathTraveller = !this.waypointMapDrawer.ShowPathTraveller;
                    e.Handled = true;
                    return;

                case Key.Space:
                    if( this.EditMode == WaypointWorkspaceEditMode.Paths )
                        this.EditMode = WaypointWorkspaceEditMode.WaypointsAndSegments;
                    else
                        this.EditMode = WaypointWorkspaceEditMode.Paths;
                    e.Handled = true;
                    return;

                case Key.Delete:
                case Key.Back:
                    this.DeleteSelected();
                    break;

                case Key.R:
                    this.ResetSelectedPath( e.KeyboardDevice.IsKeyDown( Key.LeftShift ) );
                    break;

                default:
                    break;
            }

            this.sceneScroller.HandleKeyDown( e );
        }

        /// <summary>
        /// Resets the currently selected path.
        /// </summary>
        /// <param name="preserveStartWaypoint">
        /// States whether the start Waypoint of the selected Path
        /// should be preserved and not removed from the Path.
        /// </param>
        private void ResetSelectedPath( bool preserveStartWaypoint )
        {
            var path = this.waypointMap.SelectedPath;
            if( path == null )
                return;

            string question = string.Format(
                CultureInfo.CurrentCulture,
                Properties.Resources.Question_ReallyResetSelectedPathX,
                path.Name ?? string.Empty
            );

            if( !QuestionMessageBox.Show( question ) )
                return;

            var startWaypoint = path.Model.Start;

            path.Model.Clear();

            if( preserveStartWaypoint )
            {
                path.Model.Add( startWaypoint );
                path.SelectWaypoint( this.waypointMap.GetViewModel( startWaypoint ) );
            }
            else
            {
                path.SelectWaypoint( null );
            }
        }

        /// <summary>
        /// Deletes the currently selected Waypoint, Path Segment or Path.
        /// </summary>
        private void DeleteSelected()
        {
            if( this.waypointMap.SelectedPath != null )
            {
                string question = string.Format(
                    CultureInfo.CurrentCulture,
                    "Do you really want to delete the Path '{0}'?",
                    this.waypointMap.SelectedPath.Name
                );

                if( QuestionMessageBox.Show( question ) )
                {
                    this.waypointMap.DeletePath( this.waypointMap.SelectedPath );
                }

                return;
            }

            if( this.waypointMap.SelectedWaypoint != null )
            {
                if( DeletionConfirmationDialog.ShowDialog( 
                        this.waypointMap.SelectedWaypoint,
                        this.waypointMap.FindRelated( this.waypointMap.SelectedWaypoint ) 
                    ) )
                {
                    this.waypointMap.DeleteWaypoint( this.waypointMap.SelectedWaypoint );
                }

                return;
            }

            if( this.waypointMap.SelectedPathSegment != null )
            {
                if( DeletionConfirmationDialog.ShowDialog(
                        this.waypointMap.SelectedPathSegment,
                        this.waypointMap.FindRelated( this.waypointMap.SelectedPathSegment )
                    ) )
                {
                    this.waypointMap.DeletePathSegment( this.waypointMap.SelectedPathSegment );
                }

                return;
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that has changed.
        /// </param>
        private void OnPropertyChanged( string propertyName )
        {
            if( this.PropertyChanged != null )
            {
                this.PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }

        /// <summary>
        /// Gets called when the user releases any key while the window has focus but no other control.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyUp( System.Windows.Input.KeyEventArgs e )
        {
            this.sceneScroller.HandleKeyUp( e );
        }

        /// <summary>
        /// Gets called when the user presses the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseDown( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.waypointMap == null )
                return;

            var scroll = this.scene.Camera.Scroll;
            int x = e.X + (int)scroll.X;
            int y = e.Y + (int)scroll.Y;

            switch( e.Button )
            {
                case System.Windows.Forms.MouseButtons.Middle:
                    this.HandleMiddleMouseDown( x, y );
                    break;

                case System.Windows.Forms.MouseButtons.Left:
                    this.HandleLeftMouseDown( x, y );
                    break;

                case System.Windows.Forms.MouseButtons.Right:
                    this.HandleRightMouseDown( x, y );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the case of the user pressing the left mouse anywhere
        /// on the workspace.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the x-axis in scene space.
        /// </param>
        private void HandleLeftMouseDown( int x, int y )
        {
            if( this.EditMode == WaypointWorkspaceEditMode.WaypointsAndSegments )
            {
                if( Keyboard.IsKeyDown( Key.LeftCtrl ) )
                {
                    this.ConnectSelectedWaypointWith( x, y );
                }
                else
                {
                    this.GrapWaypointOrSelectSegment( x, y );
                }
            }
            else
            {
                if( this.waypointMap.SelectedPath == null )
                    return;

                var waypoint = this.FindWaypoint( x, y );
                if( waypoint == null )
                    return;

                this.waypointMap.SelectedPath.SelectWaypoint( waypoint );
            }
        }

        /// <summary>
        /// Handles the case of the user pressing the middle mouse anywhere
        /// on the workspace.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the x-axis in scene space.
        /// </param>
        private void HandleMiddleMouseDown( int x, int y )
        {
            if( this.EditMode == WaypointWorkspaceEditMode.WaypointsAndSegments )
            {
                if( this.SelectWaypoint( x, y ) == null )
                {
                    this.AddWaypoint( x, y, centerAtTile: Keyboard.IsKeyDown( Key.LeftShift ) );
                }
            }
            else
            {
                var startWaypoint = this.FindWaypoint( x, y );

                if( startWaypoint == null )
                    return;

                bool result = QuestionMessageBox.Show(
                    string.Format(
                        "Do you wish to create a new path starting at {0}?",
                        startWaypoint.Name
                    )
                );

                if( !result )
                    return;

                var path = this.waypointMap.AddPath( startWaypoint );
                this.waypointMap.SelectedPath = path;
            }
        }

        /// <summary>
        /// Handles the case of the user pressing the right mouse anywhere
        /// on the workspace.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the x-axis in scene space.
        /// </param>
        private void HandleRightMouseDown( int x, int y )
        {
            if( this.EditMode == WaypointWorkspaceEditMode.WaypointsAndSegments )
            {
                this.ConnectSelectedWaypointWith( x, y );
            }
            else
            {
                var path = this.waypointMap.SelectedPath;
                if( path == null )
                    return;

                var waypoint = this.FindWaypoint( x, y );
                if( waypoint == null )
                    return;

                if( path.SelectedWaypoint == null || path.SelectedWaypoint.Model == path.Model.End || path.Model.Length <= 0 )
                {
                    path.ConnectEndWith( waypoint.Model );
                }
                else
                {
                    path.ConnectSelectedWith( waypoint.Model );
                }

                path.SelectWaypoint( waypoint );
            }
        }

        /// <summary>
        /// Attempts to grap the Waypoint or select the PathSegement at the specified location.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        private void GrapWaypointOrSelectSegment( int x, int y )
        {
            if( this.GrapWaypoint( x, y ) == null )
            {
                this.SelectSegment( x, y );
            }
        }

        /// <summary>
        /// Attempts to select a PathSegmentViewModel near the specified location.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <returns>
        /// The PathSegmentViewModel that has been selected; -or- null.
        /// </returns>
        private PathSegmentViewModel SelectSegment( int x, int y )
        {
            var segment = FindSegment( x, y );
            
            if( segment != null )
            {
                this.waypointMap.SelectedPathSegment = segment;
            }

            return segment;
        }

        /// <summary>
        /// Attempts to finds a PathSegmentViewModel near the specified location.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <returns>
        /// The PathSegmentViewModel that has been found; -or- null.
        /// </returns>
        private PathSegmentViewModel FindSegment( int x, int y )
        {
            var query = 
                from s in this.waypointMap.PathSegments.AsParallel()
                let line = s.Model.Line
                let distance = line.DistanceTo( new Vector2( x, y ) )
                where distance <= 5.0f
                select s;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Connects the currently selected Waypoint with the Waypoint at the given location.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        private void ConnectSelectedWaypointWith( int x, int y )
        {
            var from = this.waypointMap.SelectedWaypoint;

            if( from != null )
            {
                var to = this.FindWaypoint( x, y );

                if( to != null && from != to )
                {
                    this.waypointMap.AddPathSegment( from, to );
                    this.waypointMap.SelectedWaypoint = to;
                }
            }
        }

        /// <summary>
        /// Attempts to graph the Waypoint at the specified location.
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis in scene space.
        /// </param>
        /// <param name="y">
        /// The position on the y-axis in scene space.
        /// </param>
        /// <returns>
        /// The WaypointViewModel that has been grapped.
        /// </returns>
        private WaypointViewModel GrapWaypoint( int x, int y )
        {
            if( this.grappedWaypoint == null )
            {
                this.grappedWaypoint = this.SelectWaypoint( x, y );
            }

            return this.grappedWaypoint;
        }

        /// <summary>
        /// Gets called when the user releases the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseUp( System.Windows.Forms.MouseEventArgs e )
        {
            if( e.Button == System.Windows.Forms.MouseButtons.Left )
            {
                if( this.grappedWaypoint != null )
                {
                    this.grappedWaypoint = null;
                }
            }
        }

        /// <summary>
        /// Gets called when the user moves the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.scene == null )
                return;

            if( this.grappedWaypoint != null )
            {
                var scroll = scene.Camera.Scroll;
                var newPosition = new Vector2( e.X + grapOffset.X + scroll.X, e.Y + grapOffset.Y + scroll.Y );

                // Clamp to tile if the user has Shift pressed.
                if( System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift )
                {
                    newPosition /= 16.0f;
                    newPosition = Vector2.Round( newPosition );
                    newPosition *= 16.0f;
                    newPosition += 8.0f;
                }

                newPosition = Vector2.Clamp( newPosition, Vector2.Zero, scene.SizeInPixels );
                this.grappedWaypoint.Position = newPosition;
            }
        }

        /// <summary>
        /// The Waypoint the user is currently dragging.
        /// </summary>
        private WaypointViewModel grappedWaypoint;

        /// <summary>
        /// The offset from where the use clicked on the grappedWaypoint to its position.
        /// </summary>
        private Vector2 grapOffset;
        
        /// <summary>
        /// Represents a reference of the WaypointMapViewModel of the currently active scene.
        /// </summary>
        private WaypointMapViewModel waypointMap;

        /// <summary>
        /// Represents a reference of the currently active SceneViewModel.
        /// </summary>
        private SceneViewModel scene;

        /// <summary>
        /// Represents the storage field of the EditMode property.
        /// </summary>
        private WaypointWorkspaceEditMode _editMode;

        /// <summary>
        /// Provides access to XNA related objects of the Editor application.
        /// </summary>
        private readonly XnaEditorApp application;

        /// <summary>
        /// Implements a mechanism that reacts to user input to scroll the scene.
        /// </summary>
        private readonly SceneScroller sceneScroller;

        /// <summary>
        /// Implements a mechanism that visualizes a <see cref="WaypointMapViewModel"/>.
        /// </summary>
        private readonly WaypointMapDrawer waypointMapDrawer;
    }
}
