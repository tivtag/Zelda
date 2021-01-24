// <copyright file="EventWorkspaceControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.EventWorkspaceControl class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Event
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Contains the WPF user interface responsible for editing the EventWorkspace.
    /// </summary>
    public partial class EventWorkspaceControl : UserControl
    {
        /// <summary>
        /// Gets or sets the <see cref="EventWorkspace"/> this EventWorkspaceControl manipulates.
        /// </summary>
        public EventWorkspace Workspace
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the EventWorkspaceControl class.
        /// </summary>
        public EventWorkspaceControl()
        {
            this.InitializeComponent();
        }
        
        /// <summary>
        /// Gets called when the user presses any key while he has focused the Events list box.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The KeyEventArgs that contains the event data.
        /// </param>
        private void OnKeyDownEventList( object sender, KeyEventArgs e )
        {
            var scene = this.Workspace.Scene;
            if( scene == null )
                return;

            if( e.Key == Key.Delete || e.Key == Key.Back )
            {
                scene.EventManager.RemoveEvent.Execute( null );
            }
        }

        /// <summary>
        /// Gets called when the user presses any key while he has focused the EventTriggers list box.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The KeyEventArgs that contains the event data.
        /// </param>
        private void OnKeyDownTriggerList( object sender, KeyEventArgs e )
        {
            var scene = this.Workspace.Scene;
            if( scene == null )
                return;

            if( e.Key == Key.Delete || e.Key == Key.Back )
            {
                scene.EventManager.RemoveTrigger.Execute( null );
            }
        }
        
        /// <summary>
        /// Gets called when the user selects an item in the Event-list.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedEventChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            if( listBox.SelectedItem != null )
            {
                this.propertyGrid.SelectedObject = listBox.SelectedItem;
                this.listBoxTriggers.SelectedItem   = null;
            }
        }

        /// <summary>
        /// Gets called when the user selects an item in the EventTrigger-list.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedTriggerChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            if( listBox.SelectedItem != null )
            {
                this.propertyGrid.SelectedObject = listBox.SelectedItem;
                this.listBoxEvents.SelectedItem  = null;
            }
        }
    }
}
