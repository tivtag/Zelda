// <copyright file="TimelineViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Story.ViewModels.TimelineViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Story.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Data;
    using Atom.Wpf;
    using Zelda.Story;
    using Atom;
    using System.Collections;

    /// <summary>
    /// Represents the view-model that exposes the ZeldaTimeline to the view.
    /// </summary>
    public sealed class TimelineViewModel : ViewModel<ZeldaTimeline>
    {
        /// <summary>
        /// Raised when the currently <see cref="SelectedIncident"/> has changed.
        /// </summary>
        public event RelaxedEventHandler<IncidentViewModel> SelectedIncidentChanged;

        /// <summary>
        /// Gets a view over the IncidentViewModels that this StoryboardViewModel contains.
        /// </summary>
        public IEnumerable IncidentsView
        {
            get
            {
                return this.incidentsView;
            }
        }

        /// <summary>
        /// Gets the IncidentViewModel that the user has currently selected.
        /// </summary>
        public IncidentViewModel SelectedIncident
        {
            get
            {
                return this.incidentsView.CurrentItem as IncidentViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the name that uniquely identifies the Timeline.
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
                {
                    return;
                }

                if( this.storyboard.HasTimelineNamed( value ) )
                {
                    throw new ArgumentException( "The name of each Timeline must be unique within a Storyboard.", "value" );
                }

                this.Model.Name = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the TimelineViewModel class.
        /// </summary>
        /// <param name="storyboard">
        /// The StoryboardViewModel that owns the new TimelineViewModel.
        /// </param>
        /// <param name="model">
        /// The ZeldaTimeline that the new TimelineViewModel wraps around.
        /// </param>
        public TimelineViewModel( StoryboardViewModel storyboard, ZeldaTimeline model )
            : base( model )
        {
            this.incidentsView = new ListCollectionView( this.incidents );
            this.incidentsView.CurrentChanged += this.OnCurrentIncidentChanged;

            foreach( var incident in model.Incidents )
            {
                this.incidents.Add( new IncidentViewModel( incident ) );
            }

            this.storyboard = storyboard;
        }

        /// <summary>
        /// Adds a new Incident to this TimelineViewModel.
        /// </summary>
        public void AddNewIncident()
        {
            var incident = new ZeldaIncident() {             
                RelativeTick = this.storyboard.CurrentTick - this.Model.StartOffset
            };

            this.Model.Insert( incident );
            this.Model.Rebuild();

            var viewModel = new IncidentViewModel( incident );
            this.incidents.Add( viewModel );
        }

        /// <summary>
        /// Called when the currently selected incident has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnCurrentIncidentChanged( object sender, EventArgs e )
        {
            this.OnPropertyChanged( "SelectedIncident" );
            this.SelectedIncidentChanged.Raise( this, this.SelectedIncident );
        }
        
        /// <summary>
        /// Contains the IncidentViewModels that wrap around the ZeldaIncidents of the Storyboard.
        /// </summary>
        private readonly ObservableCollection<IncidentViewModel> incidents = new ObservableCollection<IncidentViewModel>();

        /// <summary>
        /// Represents the view over the timelines collection.
        /// </summary>
        private readonly ListCollectionView incidentsView;

        /// <summary>
        /// The StoryboardViewModel that owns this TimelineViewModel.
        /// </summary>
        private readonly StoryboardViewModel storyboard;
    }
}