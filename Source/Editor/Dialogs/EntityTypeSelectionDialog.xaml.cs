// <copyright file="EntityTypeSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Editor.Dialogs.EntityTypeSelectionDialog class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    
    /// <summary>
    /// Interaction logic for EntityTypeSelectionDialog.xaml
    /// </summary>
    sealed partial class EntityTypeSelectionDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTypeSelectionDialog"/> class.
        /// </summary>
        /// <param name="types">
        /// The entity types selectable by the user.
        /// </param>
        public EntityTypeSelectionDialog( IEnumerable<Type> types )
        {
            if( types == null )
                throw new ArgumentNullException( "types" );

            this.InitializeComponent();
            this.SetupSelectableTypes( types );
        }
        
        /// <summary>
        /// Setups this EntityTypeSelectionDialog to allow the user to
        /// select the specified entity types.
        /// </summary>
        /// <param name="types">
        /// The types that should be selectable.
        /// </param>
        private void SetupSelectableTypes( IEnumerable<Type> types )
        {
            foreach( var type in types )
            {
                this.listBox_Types.Items.Add( type );
            }
        }

        /// <summary>
        /// Receives the <see cref="Type"/> which has been selected by the user in this <see cref="EntityTypeSelectionDialog"/>.
        /// </summary>
        public Type SelectedEntityType
        {
            get
            { 
                return listBox_Types.SelectedItem as Type;
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the OK button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnOkButtonClicked( object sender, RoutedEventArgs e )
        {
            if( this.SelectedEntityType == null )
                return;

            this.DialogResult = true;
        }

        /// <summary>
        /// Gets called when the user clicks on the Cancel button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnCancelButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
        }
    }
}
