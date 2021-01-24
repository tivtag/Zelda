// <copyright file="QuestViewModelControl.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.QuestViewModelControl class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuestCreator
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the view over a <see cref="QuestViewModel"/>.
    /// </summary>
    sealed partial class QuestViewModelControl : UserControl
    {
        /// <summary>
        /// Gets the QuestViewModel this QuestViewModelControl is bound to.
        /// </summary>
        internal QuestViewModel BoundQuestViewModel
        {
            get 
            { 
                return (QuestViewModel)this.DataContext; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the QuestViewModelControl class.
        /// </summary>
        /// <param name="questViewModel">
        /// The QuestViewModel the new QuestViewModelControl provides a view over.
        /// </param>
        internal QuestViewModelControl( QuestViewModel questViewModel )
        {
            if( questViewModel == null )
                throw new ArgumentNullException( "questViewModel" );

            InitializeComponent();

            this.DataContext = questViewModel;
        }

        /// <summary>
        /// Called when the currently selected item in the requirements list box has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs that contain the event data.</param>
        private void ListBox_Requirements_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            propertyGrid_Requirement.SelectedObject = listBox.SelectedItem;
        }
        
        /// <summary>
        /// Called when the currently selected item in the goals list box has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs that contain the event data.</param>
        private void ListBox_Goals_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            propertyGrid_Goal.SelectedObject = listBox.SelectedItem;
        }

        /// <summary>
        /// Called when the currently selected item in the rewards list box has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs that contain the event data.</param>
        private void ListBox_Rewards_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            propertyGrid_Reward.SelectedObject = listBox.SelectedItem;
        }

        /// <summary>
        /// Called when the currently selected item in the start events list box has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs that contain the event data.</param>
        private void ListBox_StartEvents_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            propertyGrid_StartEvent.SelectedObject = listBox.SelectedItem;
        }

        /// <summary>
        /// Called when the currently selected item in the completion events list box has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs that contain the event data.</param>
        private void ListBox_CompletionEvents_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var listBox = (ListBox)sender;

            propertyGrid_CompletionEvent.SelectedObject = listBox.SelectedItem;
        }
    }
}
