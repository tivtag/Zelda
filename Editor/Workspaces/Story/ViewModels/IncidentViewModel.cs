// <copyright file="IncidentViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Story.ViewModels.IncidentViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Story.ViewModels
{
    using System.ComponentModel;
    using Atom;
    using Atom.Story;
    using Atom.Wpf;
    using Zelda.Story;

    /// <summary>
    /// Represents the view-model that exposes the ZeldaIncident to the view.
    /// </summary>
    public sealed class IncidentViewModel : ViewModel<ZeldaIncident>
    {
        /// <summary>
        /// Gets the 'name' of this IncidentViewModel.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                return this.Action != null ? this.Action.ToString() : "New Incident";
            }
        }

        /// <summary>
        /// Gets or sets the action that is executed when this Incident occurs.
        /// </summary>
        public IAction Action
        {
            get 
            {
                return this.Model.Action;
            }

            set
            {
                this.Model.Action = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets or sets the Atom.Story.TimeTick this Incident occurs on.
        /// </summary>
        public TimeTick RelativeTick 
        {
            get
            { 
                return this.Model.RelativeTick;
            }
 
            set
            {
                this.Model.RelativeTick = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the IncidentViewModel class.
        /// </summary>
        /// <param name="model">
        /// The ZeldaIncident that the new IncidentViewModel wraps around.
        /// </param>
        public IncidentViewModel( ZeldaIncident model )
            : base( model )
        {
        }
    }
}
