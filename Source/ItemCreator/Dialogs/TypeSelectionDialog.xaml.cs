// <copyright file="TypeSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.Dialogs.TypeSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.ItemCreator.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Defines a dialog that allows the user to select a type.
    /// </summary>
    partial class TypeSelectionDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSelectionDialog"/> class.
        /// </summary>
        /// <param name="types">
        /// The types that should be selectable in the new TypeSelectionDialog.
        /// </param>
        public TypeSelectionDialog( IEnumerable<Type> types )
        {
            if( types == null )
                throw new ArgumentNullException( "types" );

            this.InitializeComponent();
            this.SetupSelectableTypes( types );
        }

        /// <summary>
        /// Setups this TypeSelectionDialog by adding the specified types. 
        /// </summary>
        /// <param name="types">
        /// The types that should be selectable in the SelectionDialog.
        /// </param>
        private void SetupSelectableTypes( IEnumerable<Type> types )
        {
            this.listBox.Items.Add( null );

            foreach( var type in types )
            {
                this.listBox.Items.Add( type );
            }
        }

        /// <summary>
        /// Gets or sets the Type the user has selected in this <see cref="DrawStrategySelectionDialog"/>.
        /// </summary>
        public Type SelectedType
        {
            get 
            {
                return this.listBox.SelectedItem as Type; 
            }

            set
            {
                this.listBox.SelectedItem = value; 
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
