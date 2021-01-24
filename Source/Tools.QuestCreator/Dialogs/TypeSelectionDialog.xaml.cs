// <copyright file="RewardSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.Dialogs.RewardSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuestCreator.Dialogs
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
        /// The Types that are selectable in the new TypeSelectionDialog.
        /// </param>
        protected TypeSelectionDialog( IEnumerable<Type> types )
        {
            if( types == null )
            {
                throw new ArgumentNullException( nameof( types ) );
            }

            this.InitializeComponent();

            this.listBox.Items.Add( null );
            foreach( Type type in types )
            {
                this.listBox.Items.Add( type );
            }
        }

        /// <summary>
        /// Gets or sets the Type the user has selected in this <see cref="TypeSelectionDialog"/>.
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
            if( this.SelectedType != null )
            {
                this.DialogResult = true;
            }
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
