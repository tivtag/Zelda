// <copyright file="DeletionConfirmationDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.DeletionConfirmationDialog class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Waypoint
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using Atom;
    using Atom.Wpf;

    /// <summary>
    /// Implements a dialog that allows the user to decide whether he really wants to delete
    /// an object.
    /// </summary>
    public sealed partial class DeletionConfirmationDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the DeletionConfirmationDialog class.
        /// </summary>
        private DeletionConfirmationDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Allows the users to decide whether he truely wants to delete the specified Waypoint.
        /// </summary>
        /// <param name="waypointToDelete">
        /// The Waypoint that is about to be deleted.
        /// </param>
        /// <param name="affectedObjects">
        /// The objects that will be affected by the deletion operation.
        /// </param>
        /// <returns>
        /// True if the user has decided to delete / refactor them;
        /// otherwise false.
        /// </returns>
        internal static bool ShowDialog( WaypointViewModel waypointToDelete, IEnumerable<object> affectedObjects )
        {
            return ShowDialog(
                waypointToDelete,
                affectedObjects,
                "Do you really want to delete the Waypoint '{0}'?",
                "Confirm Deletion of Waypoint {0}"
            );
        }

        /// <summary>
        /// Allows the users to decide whether he truely wants to delete the specified Path Segment.
        /// </summary>
        /// <param name="pathSegmentToDelete">
        /// The PathSegment that is about to be deleted.
        /// </param>
        /// <param name="affectedObjects">
        /// The objects that will be affected by the deletion operation.
        /// </param>
        /// <returns>
        /// True if the user has decided to delete / refactor them;
        /// otherwise false.
        /// </returns>
        internal static bool ShowDialog( PathSegmentViewModel pathSegmentToDelete, IEnumerable<object> affectedObjects )
        {
            return ShowDialog(
                pathSegmentToDelete,
                affectedObjects,
                "Do you really want to delete the Path Segment '{0}'?",
                "Confirm Deletion of Segment {0}"
            );
        }


        /// <summary>
        /// Allows the users to decide whether he truely wants to delete the specified object.
        /// </summary>
        /// <param name="objectToDelete">
        /// The object that is about to be deleted.
        /// </param>
        /// <param name="affectedObjects">
        /// The objects that will be affected by the deletion operation.
        /// </param>
        /// <param name="formatQuickDelete">
        /// The format string that is displayed to the user in-case no objects are
        /// affected by the deletion operation.
        /// </param>
        /// <param name="formatTitle">
        /// The format string that is displayed to the user in-case objects are
        /// affected by the deletion operation.
        /// </param>
        /// <returns>
        /// True if the user has decided to delete / refactor them;
        /// otherwise false.
        /// </returns>
        private static bool ShowDialog( 
            IReadOnlyNameable objectToDelete,
            IEnumerable<object> affectedObjects, 
            string formatQuickDelete,
            string formatTitle )
        {
            if( !affectedObjects.Any() )
            {
                string question = string.Format(
                    CultureInfo.CurrentCulture,
                    formatQuickDelete,
                    objectToDelete.Name
                );

                return QuestionMessageBox.Show( question );
            }

            var dialog = new DeletionConfirmationDialog() {
                Title = string.Format(
                    CultureInfo.CurrentCulture,
                    formatTitle,
                    objectToDelete.Name
                )
            };

            foreach( var affectedObject in affectedObjects )
            {
                dialog.listBox.Items.Add( affectedObject );
            }

            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Gets called when the user presses on the 'Ok'-buton.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnOkButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Gets called when the user presses on the 'Cancel' button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnCancelButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Gets called when the user presses any key while this Window has focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The KeyEventArgs that contain the event data.</param>
        private void OnWindowKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if( e.Key == System.Windows.Input.Key.Escape )
            {
                this.DialogResult = false;
            }
            else if( e.Key == System.Windows.Input.Key.Enter )
            {
                this.DialogResult = true;
            }
        }
    }
}
