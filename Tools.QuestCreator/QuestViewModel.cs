// <copyright file="QuestViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuestCreator.QuestViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.QuestCreator
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using Atom.Wpf;
    using Zelda.Core.Requirements;
    using Zelda.Quests;

    /// <summary>
    /// Defines the ViewModel over a <see cref="Quest"/>.
    /// </summary>
    internal sealed class QuestViewModel : ViewModel<Quest>
    {
        #region [ Properties ]

        #region > Settings <

        /// <summary>
        /// Gets or sets the bindable <see cref="QuestDeliverType"/> of the Quest.
        /// </summary>
        public QuestDeliverType DeliverType
        {
            get 
            {
                return this.Model.DeliverType;
            }

            set
            {
                if( value == this.DeliverType )
                    return;

                this.Model.DeliverType = value;
                this.OnPropertyChanged( "DeliverType" );
            }
        }

        /// <summary>
        /// Gets or sets the bindable <see cref="QuestType"/> of the Quest.
        /// </summary>
        public QuestType QuestType
        {
            get 
            { 
                return this.Model.QuestType;
            }

            set
            {
                if( value == this.QuestType )
                    return;

                this.Model.QuestType = value;
                this.OnPropertyChanged( "QuestType" );
            }
        }

        /// <summary>
        /// Gets or sets the bindable level of the Quest.
        /// </summary>
        public int Level
        {
            get
            { 
                return this.Model.Level;
            }

            set
            {
                if( value == this.Level )
                    return;

                this.Model.Level = value;
                this.OnPropertyChanged( "Level" );
            }
        }

        /// <summary>
        /// Gets or sets a (bindable) value indicating whether
        /// the Quest is repeatable.
        /// </summary>
        public bool IsRepeatable
        {
            get 
            {
                return this.Model.IsRepeatable;
            }

            set
            {
                if( value == this.IsRepeatable )
                    return;

                this.Model.IsRepeatable = value;
                this.OnPropertyChanged( "IsRepeatable" );
            }
        }

        /// <summary>
        /// Gets or sets a (bindable) value indicating whether
        /// the current state of the Quest is hidden.
        /// </summary>
        public bool IsStateHidden
        {
            get 
            {
                return this.Model.IsStateHidden;
            }

            set
            {
                if( value == this.IsStateHidden )
                    return;

                this.Model.IsStateHidden = value;
                this.OnPropertyChanged( "IsStateHidden" );
            }
        }

        #endregion

        #region > Strings <

        /// <summary>
        /// Gets or sets the (bindable) name that
        /// uniquely idenfities the Quest.
        /// </summary>
        public string Name
        {
            get 
            { 
                return this.Model.Name;
            }

            set
            {
                if( value == this.Name )
                    return;

                this.Model.Name = value;
                OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets or sets the (bindable) resource id
        /// that is used to receive the localized desciption of the Quest.
        /// </summary>
        public string TextDescription
        {
            get
            {
                return this.Model.TextDescription;
            }

            set
            {
                if( value == this.TextDescription )
                    return;

                this.Model.TextDescription = value;
                OnPropertyChanged( "TextDescription" );
            }
        }

        /// <summary>
        /// Gets or sets the (bindable) resource id
        /// that is used to receive the localized start text of the Quest.
        /// </summary>
        public string TextStart
        {
            get
            {
                return this.Model.TextStart;
            }

            set
            {
                if( value == this.TextStart )
                    return;

                this.Model.TextStart = value;
                OnPropertyChanged( "TextStart" );
            }
        }

        /// <summary>
        /// Gets or sets the (bindable) resource id
        /// that is used to receive the localized text that is shown when 
        /// the Quest has not been completed yet.
        /// </summary>
        public string TextNotCompleted
        {
            get
            {
                return this.Model.TextNotCompleted;
            }

            set
            {
                if( value == this.TextNotCompleted )
                    return;

                this.Model.TextNotCompleted = value;
                OnPropertyChanged( "TextNotCompleted" );
            }
        }

        /// <summary>
        /// Gets or sets the (bindable) resource id
        /// that is used to receive the localized end text of the Quest.
        /// </summary>
        public string TextCompleted
        {
            get
            {
                return this.Model.TextCompleted;
            }

            set
            {
                if( value == this.TextCompleted )
                    return;

                this.Model.TextCompleted = value;
                OnPropertyChanged( "TextCompleted" );
            }
        }

        /// <summary>
        /// Gets or sets the (bindable) name of the object/location the Quest must be delivered at.
        /// </summary>
        public string DeliverLocation
        {
            get
            {
                return this.Model.DeliverLocation;
            }

            set
            {
                if( value == this.DeliverLocation )
                    return;

                this.Model.DeliverLocation = value;
                OnPropertyChanged( "DeliverLocation" );
            }
        }

        #endregion

        #region > Enumerations <

        /// <summary>
        /// Gets the bindable list of requirements that need to be fulfilled to get the Quest.
        /// </summary>
        public IEnumerable Requirements
        {
            get
            {
                return this.requirementsView; 
            }
        }

        /// <summary>
        /// Gets the bindable list of goals that need player needs to complete to complete the Quest.
        /// </summary>
        public IEnumerable Goals
        {
            get
            { 
                return this.goalsView;
            }
        }

        /// <summary>
        /// Gets the bindable list of rewards that need player receives when he has completed the Quest.
        /// </summary>
        public IEnumerable Rewards
        {
            get
            { 
                return this.rewardsView; 
            }
        }

        /// <summary>
        /// Gets the bindable list of events that are executed
        /// when the player has accepted the Quest.
        /// </summary>
        public IEnumerable StartEvents
        {
            get 
            { 
                return this.startEventsView;
            }
        }

        /// <summary>
        /// Gets the bindable list of events that are executed
        /// when the player has completed the Quest.
        /// </summary>
        public IEnumerable CompletionEvents
        {
            get
            { 
                return this.completionEventsView;
            }
        }

        #endregion

        #region > Commands <

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestRequirement"/> he wishes to add to the Quest.
        /// </summary>
        public ICommand AddRequirement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestRequirement"/> he wishes to remove from the Quest.
        /// </summary>
        public ICommand RemoveRequirement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestGoal"/> he wishes to add to the Quest.
        /// </summary>
        public ICommand AddGoal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestGoal"/> he wishes to remove from the Quest.
        /// </summary>
        public ICommand RemoveGoal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestReward"/> he wishes to add to the Quest.
        /// </summary>
        public ICommand AddReward
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestReward"/> he wishes to remove from the Quest.
        /// </summary>
        public ICommand RemoveReward
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to add to the Quest.
        /// </summary>
        public ICommand AddStartEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to remove from the Quest.
        /// </summary>
        public ICommand RemoveStartEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to add to the Quest.
        /// </summary>
        public ICommand AddCompletionEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to remove from the Quest.
        /// </summary>
        public ICommand RemoveCompletionEvent
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the QuestViewModel class.
        /// </summary>
        /// <param name="model">
        /// The Quest that is wrapped by the new QuestViewModel.
        /// </param>
        public QuestViewModel( Quest model )
            : base( model )
        {
            // Fill lists.
            foreach( var requirement in model.Requirements )
                requirements.Add( requirement );
            foreach( var goal in model.Goals )
                goals.Add( goal );
            foreach( var reward in model.Rewards )
                rewards.Add( reward );
            foreach( var e in model.StartEvents )
                startEvents.Add( e );
            foreach( var e in model.CompletionEvents )
                completionEvents.Add( e );

            // Create Views.
            this.requirementsView = new ListCollectionView( requirements );
            this.goalsView        = new ListCollectionView( goals );
            this.rewardsView      = new ListCollectionView( rewards );
            this.startEventsView  = new ListCollectionView( startEvents );
            this.completionEventsView = new ListCollectionView( completionEvents );

            // Create Commands
            this.AddRequirement = new AddRequirementCommand( this );
            this.AddGoal        = new AddGoalCommand( this );
            this.AddReward      = new AddRewardCommand( this );
            this.AddStartEvent  = new AddStartEventCommand( this );
            this.AddCompletionEvent = new AddCompletionEventCommand( this );

            this.RemoveRequirement = new RemoveRequirementCommand( this );
            this.RemoveGoal        = new RemoveGoalCommand( this );
            this.RemoveReward      = new RemoveRewardCommand( this );
            this.RemoveStartEvent  = new RemoveStartEventCommand( this );
            this.RemoveCompletionEvent = new RemoveCompletionEventCommand( this );
        }

        #endregion

        #region [ Commands ]

        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestRequirement"/> he wishes to add to the Quest.
        /// </summary>
        private sealed class AddRequirementCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the AddRequirementCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new AddRequirementCommand provides functionality for.
            /// </param>
            public AddRequirementCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Dialogs.RequirementSelectionDialog();

                if( dialog.ShowDialog() == true )
                {
                    Type type        = dialog.SelectedType;
                    var  requirement = (IRequirement)Activator.CreateInstance( type );

                    this.Model.AddRequirement( requirement );
                    this.ViewModel.requirements.Add( requirement );
                }
            }
        }

        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestRequirement"/> he wishes to remove from the Quest.
        /// </summary>
        private sealed class RemoveRequirementCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the RemoveRequirementCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new RemoveRequirementCommand provides functionality for.
            /// </param>
            public RemoveRequirementCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
                viewModel.requirementsView.CurrentChanged += (sender, e) => {
                    OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this RemoveRequirementCommand can currently be executed.
            /// </summary>
            /// <param name="parameter">
            /// The parameter send into this Command.
            /// </param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return ViewModel.requirementsView.CurrentItem != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The parameter send into this Command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var requirement = this.ViewModel.requirementsView.CurrentItem as IRequirement;

                this.ViewModel.requirements.Remove( requirement );
                this.Model.RemoveRequirement( requirement );
            }
        }
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestGoal"/> he wishes to add to the Quest.
        /// </summary>
        private sealed class AddGoalCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the AddGoalCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new AddGoalCommand provides functionality for.
            /// </param>
            public AddGoalCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Dialogs.GoalSelectionDialog();

                if( dialog.ShowDialog() == true )
                {
                    Type type = dialog.SelectedType;
                    var  goal = (IQuestGoal)Activator.CreateInstance( type );

                    this.Model.AddGoal( goal );
                    this.ViewModel.goals.Add( goal );
                }
            }
        }
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestGoal"/> he wishes to remove from the Quest.
        /// </summary>
        private sealed class RemoveGoalCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the RemoveGoalCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new RemoveGoalCommand provides functionality for.
            /// </param>
            public RemoveGoalCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
                viewModel.goalsView.CurrentChanged += ( sender, e ) =>
                {
                    OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this RemoveGoalCommand can currently be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return ViewModel.goalsView.CurrentItem != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var goal = this.ViewModel.goalsView.CurrentItem as IQuestGoal;

                this.ViewModel.goals.Remove( goal );
                this.Model.RemoveGoal( goal );
            }
        }
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestReward"/> he wishes to add to the Quest.
        /// </summary>
        private sealed class AddRewardCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the AddRewardCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new AddRewardCommand provides functionality for.
            /// </param>
            public AddRewardCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Dialogs.RewardSelectionDialog();

                if( dialog.ShowDialog() == true )
                {
                    Type type        = dialog.SelectedType;
                    var reward = (IQuestReward)Activator.CreateInstance( type );

                    this.Model.AddReward( reward );
                    this.ViewModel.rewards.Add( reward );
                }
            }
        }    
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestReward"/> he wishes to remove from the Quest.
        /// </summary>
        private sealed class RemoveRewardCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the RemoveRewardCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new RemoveRewardCommand provides functionality for.
            /// </param>
            public RemoveRewardCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
                viewModel.rewardsView.CurrentChanged += ( sender, e ) =>
                {
                    OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this RemoveRewardCommand can currently be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return ViewModel.rewardsView.CurrentItem != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var reward = this.ViewModel.rewardsView.CurrentItem as IQuestReward;

                this.ViewModel.rewards.Remove( reward );
                this.Model.RemoveReward( reward );
            }
        }
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to add to the Quest.
        /// </summary>
        private sealed class AddCompletionEventCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the AddCompletionEventCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new AddCompletionEventCommand provides functionality for.
            /// </param>
            public AddCompletionEventCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Dialogs.QuestEventSelectionDialog();

                if( dialog.ShowDialog() == true )
                {
                    Type type = dialog.SelectedType;
                    var  e    = (IQuestEvent)Activator.CreateInstance( type );

                    this.Model.AddCompletionEvent( e );
                    this.ViewModel.completionEvents.Add( e );
                }
            }
        }

        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvemt"/> he wishes to remove from the Quest.
        /// </summary>
        private sealed class RemoveCompletionEventCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the RemoveCompletionEventCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new RemoveCompletionEventCommand provides functionality for.
            /// </param>
            public RemoveCompletionEventCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
                viewModel.rewardsView.CurrentChanged += ( sender, e ) =>
                {
                    OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this RemoveCompletionEventCommand can currently be executed.
            /// </summary>
            /// <param name="parameter">
            /// The parameter send into the Command.
            /// </param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return ViewModel.completionEventsView.CurrentItem != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The parameter send into the Command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var e = this.ViewModel.completionEventsView.CurrentItem as IQuestEvent;

                this.ViewModel.completionEvents.Remove( e );
                this.Model.RemoveCompletionEvent( e );
            }
        }
        
        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvent"/> he wishes to add to the Quest.
        /// </summary>
        private sealed class AddStartEventCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the AddStartEventCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new AddCompletionEventCommand provides functionality for.
            /// </param>
            public AddStartEventCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Dialogs.QuestEventSelectionDialog();

                if( dialog.ShowDialog() == true )
                {
                    Type type = dialog.SelectedType;
                    var e    = (IQuestEvent)Activator.CreateInstance( type );

                    this.Model.AddStartEvent( e );
                    this.ViewModel.startEvents.Add( e );
                }
            }
        }

        /// <summary>
        /// Defines the ICommand that when invoked allows
        /// the user to select a <see cref="IQuestEvemt"/> he wishes to remove from the Quest.
        /// </summary>
        private sealed class RemoveStartEventCommand : ViewModelCommand<QuestViewModel, Quest>
        {
            /// <summary>
            /// Initializes a new instance of the RemoveStartEventCommand class.
            /// </summary>
            /// <param name="viewModel">
            /// The QuestViewModel the new RemoveStartEventCommand provides functionality for.
            /// </param>
            public RemoveStartEventCommand( QuestViewModel viewModel )
                : base( viewModel )
            {
                viewModel.rewardsView.CurrentChanged += ( sender, e ) => {
                    OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this RemoveStartEventCommand can currently be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return ViewModel.startEventsView.CurrentItem != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var e = this.ViewModel.startEventsView.CurrentItem as IQuestEvent;

                this.ViewModel.startEvents.Remove( e );
                this.Model.RemoveStartEvent( e );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves the Quest.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// true if the Quest has been saved;
        /// otherwise false.
        /// </returns>
        public bool Save( IZeldaServiceProvider serviceProvider )
        {
            if( string.IsNullOrEmpty( this.Name ) )
            {
                MessageBox.Show( 
                    Properties.Resources.Error_QuestNameMustBeSetForSave,
                    Atom.ErrorStrings.MissingInformation,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information 
                );

                return false;
            }

            string directory = System.IO.Path.Combine( 
                System.AppDomain.CurrentDomain.BaseDirectory,
                "Content\\Quests\\"
            );

            System.IO.Directory.CreateDirectory( directory );
            string path = directory + this.Name + Quest.Extension;

            using( var writer = new System.IO.BinaryWriter( System.IO.File.Create( path ) ) )
            {
                var context = new Zelda.Saving.SerializationContext( writer, serviceProvider );
                this.Model.Serialize( context );
            }

            return true;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The view over the requirements of the Quest.
        /// </summary>
        private readonly ObservableCollection<IRequirement> requirements 
            = new ObservableCollection<IRequirement>();

        /// <summary>
        /// The view over the goals of the Quest.
        /// </summary>
        private readonly ObservableCollection<IQuestGoal> goals 
            = new ObservableCollection<IQuestGoal>();

        /// <summary>
        /// The view over the rewards of the Quest.
        /// </summary>
        private readonly ObservableCollection<IQuestReward> rewards 
            = new ObservableCollection<IQuestReward>();

        /// <summary>
        /// The view over the start events of the Quest.
        /// </summary>
        private readonly ObservableCollection<IQuestEvent> startEvents 
            = new ObservableCollection<IQuestEvent>();

        /// <summary>
        /// The view over the completion events of the Quest.
        /// </summary>
        private readonly ObservableCollection<IQuestEvent> completionEvents 
            = new ObservableCollection<IQuestEvent>();

        #region > Views <

        /// <summary>
        /// The view over the requirements collection.
        /// </summary>
        private readonly ListCollectionView requirementsView;

        /// <summary>
        /// The view over the goals collection.
        /// </summary>
        private readonly ListCollectionView goalsView;

        /// <summary>
        /// The view over the rewards collection.
        /// </summary>
        private readonly ListCollectionView rewardsView;

        /// <summary>
        /// The view over the startEvents collection.
        /// </summary>
        private readonly ListCollectionView startEventsView;

        /// <summary>
        /// The view over the completionEvents collection.
        /// </summary>
        private readonly ListCollectionView completionEventsView;

        #endregion

        #endregion
    }
}