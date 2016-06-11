// <copyright file="FactionStates.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Factions.FactionStates class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Factions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Enumerates the <see cref="FactionState"/>s of the PlayerEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class FactionStates : Saving.IStateSaveable
    {
        /// <summary>
        /// Invoked when the <see cref="ReputationLevel"/> of one of the <see cref="FactionState"/>s has changed.
        /// </summary>
        public event EventHandler<ReputationLevelChangedEventArgs> ReputationLevelChanged;

        /// <summary>
        /// Gets the states of the factions that are known by the player.
        /// </summary>
        public ICollection<FactionState> KnownStates
        {
            get
            {
                return this.states.Values;
            }
        }

        /// <summary>
        /// Initializes a new instance of the FactionStates class.
        /// </summary>
        internal FactionStates()
        {
            this.onReputationLevelChanged = new EventHandler<ReputationLevelChangedEventArgs>( OnReputationLevelChanged );
        }

        /// <summary>
        /// Gets the <see cref="FactionState"/> of the <see cref="Faction"/> that has the given <paramref name="factionName"/>.
        /// </summary>
        /// <param name="factionName">
        /// The name that uniquely identifies the Faction whose FactionState to get.
        /// </param>
        /// <returns>
        /// The requested FactionState;
        /// or null if there exists no such <see cref="Faction"/>.
        /// </returns>
        public FactionState GetState( string factionName )
        {
            FactionState state;

            if( !this.states.TryGetValue( factionName, out state ) )
            {
                Faction faction = FactionList.Get( factionName );
                if( faction == null )
                    return null;

                state = new FactionState( faction );
                this.AddState( state );
            }

            return state;
        }

        /// <summary>
        /// Gets the <see cref="FactionState"/> of the given <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">
        /// The Faction whose FactionState to get.
        /// </param>
        /// <returns>
        /// The requested FactionState;
        /// or null if there exists no such <see cref="Faction"/>.
        /// </returns>
        public FactionState GetState( Faction faction )
        {
            return this.GetState( faction.Name );
        }

        /// <summary>
        /// Gets the ReputationLevel towards the given Faction.
        /// </summary>
        /// <param name="faction">
        /// The Faction whose ReputationLevel to get.
        /// </param>
        /// <returns>
        /// The ReputationLevel towards the given Faction.
        /// </returns>
        internal ReputationLevel GetReputationLevelTowards( Faction faction )
        {
            var state = this.GetState( faction );
            return state.ReputationLevel;
        }

        /// <summary>
        /// Serializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.states.Count );

            foreach( var state in this.states )
            {
                context.Write( state.Key );
                state.Value.SerializeState( context );
            }
        }

        /// <summary>
        /// Deserializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.states.Clear();
            int stateCount = context.ReadInt32();

            for( int i  = 0; i < stateCount; ++i )
            {
                // Read Faction
                string  factionName = context.ReadString();
                Faction faction     = FactionList.Get( factionName );
                Debug.Assert( faction != null );

                // Read FactionState
                FactionState state = new FactionState( faction );
                state.DeserializeState( context );

                this.AddState( state );
            }
        }

        /// <summary>
        /// Helper method that adds the given <see cref="FactionState"/>
        /// to this FactionStates instance.
        /// </summary>
        /// <param name="state">
        /// The FactionState to add.
        /// </param>
        private void AddState( FactionState state )
        {
            this.states.Add( state.Faction.Name, state );

            // Register events
            state.ReputationLevelChanged += onReputationLevelChanged;
        }

        /// <summary>
        /// Gets invoked when the <see cref="ReputationLevel"/> of one of the <see cref="FactionState"/>s has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="ReputationLevelChangedEventArgs"/> that contains the event data.</param>
        private void OnReputationLevelChanged( object sender, ReputationLevelChangedEventArgs e )
        {
            // Redirect event
            if( this.ReputationLevelChanged != null )
                this.ReputationLevelChanged( this, e );
        }

        /// <summary>
        /// The EventHandler that gets invoked when the <see cref="ReputationLevel"/> of one of the <see cref="FactionState"/>s has changed.
        /// </summary>
        private readonly EventHandler<ReputationLevelChangedEventArgs> onReputationLevelChanged;

        /// <summary>
        /// Dictionary that maps the name of a <see cref="Faction"/> onto its <see cref="FactionState"/>.
        /// </summary>
        private readonly Dictionary<string, FactionState> states = new Dictionary<string, FactionState>();
    }
}
