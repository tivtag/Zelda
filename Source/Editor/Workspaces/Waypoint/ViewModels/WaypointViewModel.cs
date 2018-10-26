// <copyright file="WaypointViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using System;
    using System.ComponentModel;
    using Atom.Diagnostics.Contracts;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Atom.Wpf;
    using Zelda.Waypoints;

    /// <summary>
    /// Represents the ViewModel that wraps around a <see cref="Waypoint"/>.
    /// </summary>
    public sealed class WaypointViewModel : ViewModel<ZeldaWaypoint>, INameable
    {
        /// <summary>
        /// Gets or sets the position of the Waypoint.
        /// </summary>
        public Vector2 Position
        {
            get 
            {
                return this.Model.Position;
            }

            set
            {
                this.Model.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the name that uniquely identifies the Waypoint.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.Name;
            }

            set
            {
                if( string.IsNullOrWhiteSpace( value ) )
                    throw new ArgumentNullException( "value" );

                if( value == this.Name )
                    return;

                if( this.owner.Waypoints.Any( waypoint => value.Equals( waypoint.Name, StringComparison.OrdinalIgnoreCase ) ) )
                    throw new ArgumentException( Properties.Resources.Error_NameMustBeUnique );

                this.Model.Name = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets or sets the number that uniquely identifies the floor the Waypoint is placed on.
        /// </summary>
        public int FloorNumber
        {
            get
            {
                return this.Model.FloorNumber;
            }

            set
            {
                if( value == this.FloorNumber )
                    return;

                this.Model.FloorNumber = value;
                this.OnPropertyChanged( "FloorNumber" );
            }
        }

        /// <summary>
        /// Gets the <see cref="WaypointMapViewModel"/> that owns this WaypointViewModel.
        /// </summary>
        [Browsable(false)]
        public WaypointMapViewModel Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the new WaypointViewModel class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="WaypointMapViewModel"/> that owns the new WaypointViewModel. 
        /// </param>
        /// <param name="model">
        /// The Waypoint the new WaypointViewModel wraps around. 
        /// </param>
        public WaypointViewModel( WaypointMapViewModel owner, ZeldaWaypoint model )
            : base( model )
        {
            Contract.Requires<ArgumentNullException>( owner != null );

            this.owner = owner;
        }

        /// <summary>
        /// Gets a value indicating whether this WaypointViewModel is connected
        /// with the specified WaypointViewModel.
        /// </summary>
        /// <param name="waypoint">
        /// The WaypointViewModel to check for.
        /// </param>
        /// <returns>
        /// true if they are connected;
        /// otherwise false.
        /// </returns>
        public bool HasPathSegmentTo( WaypointViewModel waypoint )
        {
            if( waypoint == null )
                return false;

            return this.Model.HasPathSegmentTo( waypoint.Model );
        }

        /// <summary>
        /// Overriden to return the string returned by ToString of the Model this WaypointViewModel wraps around.
        /// </summary>
        /// <returns>
        /// The string returned by the model.
        /// </returns>
        public override string ToString()
        {
            return "Waypoint " + this.Name ?? string.Empty;
        }

        /// <summary>
        /// The <see cref="WaypointMapViewModel"/> that owns this WaypointViewModel.
        /// </summary>
        private readonly WaypointMapViewModel owner;
    }
}
