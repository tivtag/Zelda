// <copyright file="EventManagerViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Event.ViewModels.EventManagerViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Event.ViewModels
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;
    using Atom.Events;
    using Atom.Events.Design;
    using Atom.Wpf;
    using Zelda.Events;
    using SysWin = System.Windows;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="ZeldaEventManager"/> to provide them to the View (WPF).
    /// </summary>
    public sealed class EventManagerViewModel : ViewModel<ZeldaEventManager>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the (bindable) list of Events.
        /// </summary>
        public System.ComponentModel.ICollectionView Events
        {
            get 
            {
                return this.eventListView; 
            }
        }

        /// <summary>
        /// Gets the (bindable) list of EventTriggers.
        /// </summary>
        public System.ComponentModel.ICollectionView Triggers
        {
            get
            { 
                return this.eventTriggerListView;
            }
        }

        /// <summary>
        /// Gets the currently selected <see cref="Atom.Events.Event"/>.
        /// </summary>
        public Atom.Events.Event SelectedEvent
        {
            get 
            { 
                return this.eventListView.CurrentItem as Atom.Events.Event;
            }

            set
            {
                this.eventListView.MoveCurrentTo( value );
            }
        }

        /// <summary>
        /// Gets or sts the currently selected <see cref="Atom.Events.EventTrigger"/>.
        /// </summary>
        public Atom.Events.EventTrigger SelectedTrigger
        {
            get
            {
                return this.eventTriggerListView.CurrentItem as Atom.Events.EventTrigger;
            }

            set
            {
                this.eventTriggerListView.MoveCurrentTo( value );
            }
        }

        #region > Commands <

        /// <summary>
        /// Gets the Command that when executed asks the user to create a
        /// new EventTrigger to add to the list of EventTriggers.
        /// </summary>
        public ICommand AddTrigger
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Command that when executed asks the user to create a
        /// new Event to add to the list of Events.
        /// </summary>
        public ICommand AddEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Command that when executed asks the user whether
        /// he wants to remove the currently selected Event.
        /// </summary>
        public ICommand RemoveEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Command that when executed asks the user whether
        /// he wants to remove the currently selected EventTrigger.
        /// </summary>
        public ICommand RemoveTrigger
        {
            get;
            private set;
        }        

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManagerViewModel"/> class.
        /// </summary>
        /// <param name="model">The model the new ViewModel wraps around.</param>
        public EventManagerViewModel( ZeldaEventManager model )
            : base( model )
        {
            this.eventListView        = new EventListView( this );
            this.eventTriggerListView = new EventTriggerListView( this );

            // Create Commands:
            this.AddTrigger = new AddTriggerCommand( this );
            this.AddEvent   = new AddEventCommand( this );
            this.RemoveEvent = new RemoveEventCommand( this );
            this.RemoveTrigger = new RemoveTriggerCommand( this );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The view over the Events in the EventManager.
        /// </summary>
        private readonly EventListView eventListView;

        /// <summary>
        /// The view over the EventTriggers in the EventManager.
        /// </summary>
        private readonly EventTriggerListView eventTriggerListView;

        #endregion

        #region [ Commands ]

        /// <summary>
        /// Defines the ICommand that when executed asks the user to create a
        /// new Event to add to the list of Events.
        /// </summary>
        private sealed class AddEventCommand : ViewModelCommand<EventManagerViewModel, ZeldaEventManager>, IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AddEventCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public AddEventCommand( EventManagerViewModel viewModel )
                : base( viewModel )
            {
                this.creationDialog = new EventCreationDialog( EventDataType.Event, this.Model );
            }

            /// <summary>
            /// Excutes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The paramter passed to the command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( this.creationDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                {
                    var type = this.creationDialog.SelectedType;
                    if( type == null )
                        return;

                    var e = (Atom.Events.Event)Activator.CreateInstance( type );
                    e.Name = this.creationDialog.EnteredName;

                    this.Model.Add( e );
                    this.ViewModel.SelectedEvent = e;
                }
            }

            /// <summary>
            /// Disposes this AddEventCommand.
            /// </summary>
            public void Dispose()
            {
                this.creationDialog.Dispose();
            }

            /// <summary>
            /// The dialog displayed when the user executes this AddEventCommand.
            /// </summary>
            private readonly Atom.Events.Design.EventCreationDialog creationDialog;
        }

        /// <summary>
        /// Defines the ICommand that when executed asks the user to create a
        /// new EventTrigger to add to the list of EventTriggers.
        /// </summary>
        private sealed class AddTriggerCommand : ViewModelCommand<EventManagerViewModel, ZeldaEventManager>, IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AddTriggerCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public AddTriggerCommand( EventManagerViewModel viewModel )
                : base( viewModel )
            {
                this.creationDialog = new Atom.Events.Design.EventCreationDialog( EventDataType.Trigger, this.Model );
            }
            
            /// <summary>
            /// Excutes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The paramter passed to the command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( this.creationDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                {
                    var type = this.creationDialog.SelectedType;
                    if( type == null )
                        return;

                    var trigger = (EventTrigger)Activator.CreateInstance( type );
                    trigger.Name = this.creationDialog.EnteredName;

                    this.Model.Add( trigger );
                    this.ViewModel.SelectedTrigger = trigger;
                }
            }

            /// <summary>
            /// Disposes this AddTriggerCommand.
            /// </summary>
            public void Dispose()
            {
                this.creationDialog.Dispose();
            }

            /// <summary>
            /// The dialog displayed when the user executes this AddTriggerCommand.
            /// </summary>
            private readonly Atom.Events.Design.EventCreationDialog creationDialog; 
        }

        /// <summary>
        /// Defines the ICommand that when executed asks the user 
        /// whether he wants to remove the currently selected Event.
        /// </summary>
        private sealed class RemoveEventCommand: ViewModelCommand<EventManagerViewModel, ZeldaEventManager>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveEventCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public RemoveEventCommand( EventManagerViewModel viewModel )
                : base( viewModel )
            {
                viewModel.eventListView.CurrentChanged += ( sender, e ) => {
                    this.OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this Command can be executed.
            /// </summary>
            /// <param name="parameter">
            /// The paramters passed to the Command.
            /// </param>
            /// <returns>
            /// Returns true if this Command can be executed;
            /// otherwise false.
            /// </returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedEvent != null;
            }
            
            /// <summary>
            /// Excutes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The paramter passed to the command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var e = this.ViewModel.SelectedEvent;
                
                if( AskUserShouldRemove( e ) )
                {
                    this.Model.RemoveEvent( e.Name );
                }
            }

            /// <summary>
            /// Asks the user whether he really wants to remove the given Event from
            /// the EventManager.
            /// </summary>
            /// <param name="e">The Event to remove.</param>
            /// <returns>
            /// Returns true if the Event should be removed;
            /// otherwise false.
            /// </returns>
            private static bool AskUserShouldRemove( Atom.Events.Event e )
            {
                return QuestionMessageBox.Show(
                   string.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resources.Question_ReallyRemoveSelectedEventX,
                        e.Name ?? string.Empty
                   )
                );
            }
        }

        /// <summary>
        /// Defines the ICommand that when executed asks the user 
        /// whether he wants to remove the currently selected EventTrigger.
        /// </summary>
        private sealed class RemoveTriggerCommand : ViewModelCommand<EventManagerViewModel, ZeldaEventManager>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveTriggerCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public RemoveTriggerCommand( EventManagerViewModel viewModel )
                : base( viewModel )
            {
                viewModel.eventListView.CurrentChanged += (sender, e) => {
                    this.OnCanExecuteChanged();
                };
            }

            /// <summary>
            /// Gets a value indicating whether this Command can be executed.
            /// </summary>
            /// <param name="parameter">
            /// The paramters passed to the Command.
            /// </param>
            /// <returns>
            /// Returns true if this Command can be executed;
            /// otherwise false.
            /// </returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedTrigger != null;
            }

            /// <summary>
            /// Excutes this ICommand.
            /// </summary>
            /// <param name="parameter">
            /// The paramter passed to the command.
            /// </param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                var trigger = this.ViewModel.SelectedTrigger;

                if( AskUserShouldRemove( trigger ) )
                {
                    this.Model.RemoveTrigger( trigger.Name );
                }
            }

            /// <summary>
            /// Asks the user whether he really wants to remove the given EventTrigger from
            /// the EventManager.
            /// </summary>
            /// <param name="trigger">The EventTrigger to remove.</param>
            /// <returns>
            /// Returns true if the EventTrigger should be removed;
            /// otherwise false.
            /// </returns>
            private static bool AskUserShouldRemove( Atom.Events.EventTrigger trigger )
            {
                return QuestionMessageBox.Show(
                   string.Format( 
                        CultureInfo.CurrentCulture,
                        Properties.Resources.Question_ReallyRemoveSelectedEventTriggerX,
                        trigger.Name ?? string.Empty
                   )
                );
            }
        }

        #endregion

        #region [ Classes ]

        /// <summary>
        /// Represents a view over the Events in the EventManager.
        /// </summary>
        private sealed class EventListView : CollectionView
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventListView"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The EventManagerViewModel whose events the new EventListViewModel wraps around.
            /// </param>
            public EventListView( EventManagerViewModel viewModel )
                : base( viewModel.Model.Events )
            {
                viewModel.Model.EventAdded   += OnEventAdded;
                viewModel.Model.EventRemoved += OnEventRemoved;
            }

            /// <summary>
            /// Gets called when an Event has been added to the EventManager.
            /// </summary>
            /// <param name="sender">The EventManager.</param>
            /// <param name="e">The event which has been added.</param>
            private void OnEventAdded( object sender, Atom.Events.Event e )
            {
                var args = new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, e );
                this.OnCollectionChanged( args );
            }

            /// <summary>
            /// Gets called when an Event has been added to the EventManager.
            /// </summary>
            /// <param name="sender">The EventManager.</param>
            /// <param name="e">The event which has been removed.</param>
            private void OnEventRemoved( object sender, Atom.Events.Event e )
            {
                var args = new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, e );
                OnCollectionChanged( args );
            }
        }
        
        /// <summary>
        /// Represents a view over the EventTriggers in the EventManager.
        /// </summary>
        private sealed class EventTriggerListView : CollectionView
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventTriggerListView"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The EventManagerViewModel whose EventTriggers the new EventTriggerListView wraps around.
            /// </param>
            public EventTriggerListView( EventManagerViewModel viewModel )
                : base( viewModel.Model.Triggers )
            {
                viewModel.Model.TriggerAdded   += OnTriggerAdded;
                viewModel.Model.TriggerRemoved += OnTriggerRemoved;
            }

            /// <summary>
            /// Gets called when an Event has been added to the EventManager.
            /// </summary>
            /// <param name="sender">The EventManager.</param>
            /// <param name="trigger">The EventTrigger which has been added.</param>
            private void OnTriggerAdded( object sender, Atom.Events.EventTrigger trigger )
            {
                var args = new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, trigger );
                this.OnCollectionChanged( args );
            }

            /// <summary>
            /// Gets called when an Event has been added to the EventManager.
            /// </summary>
            /// <param name="sender">The EventManager.</param>
            /// <param name="trigger">The EventTrigger which has been removed.</param>
            private void OnTriggerRemoved( object sender, Atom.Events.EventTrigger trigger )
            {
                var args = new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, trigger );
                this.OnCollectionChanged( args );
            }
        }

        #endregion
    }
}