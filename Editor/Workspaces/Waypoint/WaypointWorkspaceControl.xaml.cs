// <copyright file="WaypointWorkspaceControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointWorkspaceControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using System.Windows.Controls;

    /// <summary>
    /// Contains the WPF user interface responsible for editing the WaypointWorkspace.
    /// </summary>
    public partial class WaypointWorkspaceControl : UserControl
    {
        /// <summary>
        /// Gets or sets the <see cref="WaypointWorkspace"/> this WaypointWorkspaceControl manipulates.
        /// </summary>
        public WaypointWorkspace Workspace
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the WaypointWorkspaceControl class.
        /// </summary>
        public WaypointWorkspaceControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets called when the user selects an item in the Waypoint list.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedWaypointChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            if( listBox.SelectedItem != null )
            {
                this.propertyGrid.SelectedObject = listBox.SelectedItem;
                this.listBoxSegments.SelectedItem = null;
                this.listBoxPaths.SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets called when the user selects an item in the PathSegments list.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedSegmentsChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            if( listBox.SelectedItem != null )
            {
                this.propertyGrid.SelectedObject = listBox.SelectedItem;
                this.listBoxWaypoints.SelectedItem  = null;
                this.listBoxPaths.SelectedItem  = null;
            }
        }

        /// <summary>
        /// Gets called when the user selects an item in the Paths list.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedPathChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            if( listBox.SelectedItem != null )
            {
                this.propertyGrid.SelectedObject = listBox.SelectedItem;
                this.listBoxSegments.SelectedItem = null;
                this.listBoxWaypoints.SelectedItem  = null;
            }
        }
    }
}
