// <copyright file="StoryboardViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Story.ViewModels.StoryboardViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Story.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;
    using Atom;
    using Atom.Story;
    using Atom.Wpf;
    using Zelda.Story;

    /// <summary>
    /// Represents the view-model that exposes the ZeldaStoryboard to the view.
    /// </summary>
    public sealed partial class StoryboardViewModel : ViewModel<ZeldaStoryboard>
    {
        /// <summary>
        /// Raised when the currently <see cref="SelectedTimeline"/> has changed.
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<TimelineViewModel>> SelectedTimelineChanged;

        /// <summary>
        /// Gets a view over the TimelineViewModels that this StoryboardViewModel contains.
        /// </summary>
        public IEnumerable TimelinesView
        {
            get
            {
                return this.timelinesView;
            }
        }

        /// <summary>
        /// Gets the TimelineViewModel that the user has currently selected.
        /// </summary>
        public TimelineViewModel SelectedTimeline
        {
            get
            {
                return this.timelinesView.CurrentItem as TimelineViewModel;
            }
        }

        /// <summary>
        /// Gets the TimelineViewModel that the user has currently selected.
        /// </summary>
        public IncidentViewModel SelectedIncident
        {
            get
            {
                var timeline = this.SelectedTimeline;

                if( timeline == null )
                {
                    return null;
                }

                return timeline.SelectedIncident;
            }
        }

        /// <summary>
        /// Gets or sets the current TimeTick of the Storyboard.
        /// </summary>
        public TimeTick CurrentTick 
        {
            get
            {
                return this.Model.CurrentTick;
            }

            set
            {
                this.Model.CurrentTick = value;
            }
        }
        
        /// <summary>
        /// Gets the command that when executed adds a new Timeline to this StoryboardViewModel.
        /// </summary>
        public ICommand AddTimeline
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command that when executed adds a new Incident to the currently SelctedTimeline of this StoryboardViewModel.
        /// </summary>
        public ICommand AddIncident
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the StoryboardViewModel class.
        /// </summary>
        /// <param name="model">
        /// The ZeldaStoryboard that the new StoryboardViewModel wraps around.
        /// </param>
        public StoryboardViewModel( ZeldaStoryboard model )
            : base( model )
        {
            this.timelinesView = new ListCollectionView( this.timelines );
            this.timelinesView.CurrentChanging += this.OnCurrentTimelineChanging;
            this.timelinesView.CurrentChanged += this.OnCurrentTimelineChanged;

            foreach( var timeline in model.Timelines )
            {
                this.timelines.Add( new TimelineViewModel( this, timeline ) );
            }

            this.InitializeCommands();
        }

        /// <summary>
        /// Initializes the commands this view-model exposes.
        /// </summary>
        private void InitializeCommands()
        {
            this.AddTimeline = new LambdaCommand( this.AddNewTimeline );
            this.AddIncident = new AddIncidentCommand( this );
        }

        /// <summary>
        /// Adds a new Timeline to the Storyboard.
        /// </summary>
        /// <param name="argument">
        /// Not used.
        /// </param>
        private void AddNewTimeline( object argument )
        {
            var timeline = new ZeldaTimeline() {
                Name = this.GetNameForNewTimeline()
            };

            this.Model.AddTimeline( timeline );

            var viewModel = new TimelineViewModel( this, timeline );       
            this.timelines.Add( viewModel );
            this.timelinesView.MoveCurrentTo(viewModel);
        }

        /// <summary>
        /// Gets an unused name that would uniquely identify
        /// a new Timeline within this Storyboard.
        /// </summary>
        /// <returns>
        /// The name of the new Timeline.
        /// </returns>
        private string GetNameForNewTimeline()
        {
            string name = "Timeline_0";
            int counter = 0; 
            CultureInfo culture = CultureInfo.CurrentCulture;

            while( this.HasTimelineNamed(name) )
            {
                name = "Timeline_" + (++counter).ToString( culture );
            }

            return name;
        }

        /// <summary>
        /// Called when the currently selected Timeline has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnCurrentTimelineChanged( object sender, EventArgs e )
        {
            this.OnPropertyChanged( "SelectedTimeline" );
            
            var changedValue = new ChangedValue<TimelineViewModel>( this.timelineBeforeChange, this.SelectedTimeline );
            this.SelectedTimelineChanged.Raise( this, changedValue );
            this.timelineBeforeChange = null;

            this.UpdateSelectedTimelineHook( changedValue );
        }

        /// <summary>
        /// Adds/Removes that SelectedIncidentChanged event handler.
        /// </summary>
        /// <param name="changedValue">
        /// The old and new selected timeline.
        /// </param>
        private void UpdateSelectedTimelineHook( ChangedValue<TimelineViewModel> changedValue )
        {
            if( changedValue.OldValue != null )
            {
                changedValue.OldValue.SelectedIncidentChanged -= this.OnSelectedIncidentChanged;
            }

            if( changedValue.NewValue != null )
            {
                changedValue.NewValue.SelectedIncidentChanged += this.OnSelectedIncidentChanged;
            }

            this.OnSelectedIncidentChanged();
        }

        /// <summary>
        /// Called when the currently selected incident of the currently selected Timeline
        /// has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnSelectedIncidentChanged( object sender, IncidentViewModel e )
        {
            this.OnSelectedIncidentChanged();
        }

        /// <summary>
        /// Called when the currently selected incident of the currently selected Timeline
        /// has changed.
        /// </summary>
        private void OnSelectedIncidentChanged()
        {
            this.OnPropertyChanged( "SelectedIncident" );
        }

        /// <summary>
        /// Called when the currently selected Timeline is about to change.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The CurrentChangingEventArgs that contain the event data.
        /// </param>
        private void OnCurrentTimelineChanging( object sender, System.ComponentModel.CurrentChangingEventArgs e )
        {
            this.timelineBeforeChange = this.SelectedTimeline;
        }

        /// <summary>
        /// Gets a value indicating whether the Storyboard contains a timeline
        /// with the given name.
        /// </summary>
        /// <param name="name">
        /// The name to look for.
        /// </param>
        /// <returns>
        /// true if it contains one; or otherwise false.
        /// </returns>
        public bool HasTimelineNamed( string name )
        {
            return this.Model.Timelines.Any( timeline => timeline.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary>
        /// Captures the currently selected TimelineViewModel just before the selected Timeline has changed.
        /// </summary>
        private TimelineViewModel timelineBeforeChange;

        /// <summary>
        /// Contains the TimelineViewModels that wrap around the ZeldaTimelines of the Storyboard.
        /// </summary>
        private readonly ObservableCollection<TimelineViewModel> timelines = new ObservableCollection<TimelineViewModel>();

        /// <summary>
        /// Represents the view over the timelines collection.
        /// </summary>
        private readonly ListCollectionView timelinesView;
    }
}
