// <copyright file="SelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.Dialogs.SelectionDialog class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator.Dialogs
{
    using System;
    using System.Windows;

    /// <summary>
    /// Defines a dialog that allows the user to select a type.
    /// </summary>
    partial class SelectionDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDialog"/> class.
        /// </summary>
        /// <param name="items">
        /// The items that should be selectable in the new SelectionDialog.
        /// </param>
        public SelectionDialog( System.Collections.IEnumerable items )
        {
            if( items == null )
                throw new ArgumentNullException( "items" );

            this.InitializeComponent();
            this.SetupSelectableItems( items );
        }

        /// <summary>
        /// Setups this SelectionDialog by adding the specified items. 
        /// </summary>
        /// <param name="items">
        /// The items that should be selectable in the SelectionDialog.
        /// </param>
        private void SetupSelectableItems( System.Collections.IEnumerable items )
        {
            this.listBox.Items.Add( null );
            
            foreach( var item in items )
            {
                this.listBox.Items.Add( item );
            }
        }

        /// <summary>
        /// Gets the object the user has selected in this <see cref="SelectionDialog"/>.
        /// </summary>
        public object SelectedObject
        {
            get
            { 
                return this.listBox.SelectedItem;
            }
        }

        /// <summary>
        /// Gets the Type the user has selected in this <see cref="SelectionDialog"/>.
        /// </summary>
        /// <remarks>
        /// Only valid if the input items are of type <see cref="Type"/>.
        /// </remarks>
        public Type SelectedType
        {
            get 
            {
                return this.listBox.SelectedItem as Type;
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "OK" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnOkButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Gets called when the user clicks on the "Cancel" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnCancelButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
        }
    }
}
