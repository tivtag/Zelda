// <copyright file="StoryboardViewModel.Commands.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the commands of the Zelda.Editor.Story.ViewModels.StoryboardViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Story.ViewModels
{
    using Atom.Wpf;
    using Zelda.Story;

    public partial class StoryboardViewModel
    {
        /// <summary>
        /// Represents a command that adds a new incident to the selected Timeline of a StoryboardViewModel.
        /// </summary>
        private sealed class AddIncidentCommand : ViewModelCommand<StoryboardViewModel, ZeldaStoryboard>
        {
            /// <summary>
            /// Initializes a new instance of the AddIncidentCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The view-model that is modified by the new AddIncidentCommand.
            /// </param>
            public AddIncidentCommand( StoryboardViewModel viewModel )
                : base( viewModel )
            {
                viewModel.SelectedTimelineChanged += (sender, e) => this.OnCanExecuteChanged();
            }

            /// <summary>
            /// Gets a value indicating whether this ICommand
            /// can currently be executed.
            /// </summary>
            /// <param name="parameter">
            /// Unused.
            /// </param>
            /// <returns>
            /// true if it can be exeuted; otherwise false.
            /// </returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedTimeline != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// Unused.
            /// </param>
            public override void Execute( object parameter )
            {
                if( this.CanExecute( parameter ) )
                {
                    var timeline = this.ViewModel.SelectedTimeline;
                    timeline.AddNewIncident();
                }
            }
        }
    }
}
