// <copyright file="StoryWorkspaceControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Story.StoryWorkspaceControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Story
{
    using System.Windows.Controls;
    using Zelda.Editor.Story.ViewModels;

    /// <summary>
    /// Contains the WPF user interface responsible for editing the StoryWorkspace.
    /// </summary>
    public partial class StoryWorkspaceControl : UserControl
    {
        /// <summary>
        /// Gets or sets the <see cref="StoryWorkspace"/> this StoryWorkspaceControl manipulates.
        /// </summary>
        public StoryWorkspace Workspace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the StoryboardViewModel currently beeing edited.
        /// </summary>
        public StoryboardViewModel Storyboard
        {
            get
            {
                return this.Workspace.Scene.Storyboard;
            }
        }

        /// <summary>
        /// Initializes a new instance of the StoryWorkspaceControl class.
        /// </summary>
        public StoryWorkspaceControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Called when the currently selected Timeline has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The SelectionChangedEventArgs that contain the event data.
        /// </param>
        private void OnSelectedTimelineChanged( object sender, SelectionChangedEventArgs e )
        {
            this.propertyGridTimeline.SelectedObject = this.Storyboard.SelectedTimeline;
        }

        /// <summary>
        /// Called when the currently selected Incident has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The SelectionChangedEventArgs that contain the event data.
        /// </param>
        private void OnSelectedIncidentChanged( object sender, SelectionChangedEventArgs e )
        {
            this.propertyGridIncident.SelectedObject = this.Storyboard.SelectedIncident;
        }
    }
}
