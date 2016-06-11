// <copyright file="ObjectWorkspaceControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Object.ObjectWorkspaceControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Contains the WPF user interface responsible for editing the ObjectWorkspace.
    /// </summary>
    public partial class ObjectWorkspaceControl : UserControl
    {
        /// <summary>
        /// Gets or sets the <see cref="ObjectWorkspace"/> this ObjectWorkspaceControl manipulates.
        /// </summary>
        public ObjectWorkspace Workspace
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the ObjectWorkspaceControl class.
        /// </summary>
        public ObjectWorkspaceControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets called when the currently selected object has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedObjectChanged( object sender, SelectionChangedEventArgs e )
        {
            var item = this.listBox.SelectedItem;

            this.propertyGrid.SelectedObject = item;
            this.listBox.ScrollIntoView( item );

            e.Handled = true;
        }

        /// <summary>
        /// Gets called when the user presses any key while he has focused the Object list box.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The KeyEventArgs that contains the event data.
        /// </param>
        private void OnKeyDownOnObjectList( object sender, KeyEventArgs e )
        {
            var scene = this.Workspace.Scene;
            if( scene == null )
                return;

            if( e.Key == Key.Delete || e.Key == Key.Back )
            {
                scene.RemoveObject.Execute( null );
            }
        }
    }
}
