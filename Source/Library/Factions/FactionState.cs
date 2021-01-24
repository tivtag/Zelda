// <copyright file="FactionState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Factions.FactionState class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Factions
{
    using System;

    /// <summary>
    /// Descripes the state of the player towards a specific <see cref="Faction"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class FactionState : Saving.IStateSaveable
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="ReputationLevel"/> encapsulated by
        /// this FactionState changes.
        /// </summary>
        /// <remarks>
        /// Only fired when it has changed while adding reputation using <see cref="AddReputation"/>.
        /// </remarks>
        public event EventHandler<ReputationLevelChangedEventArgs> ReputationLevelChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Faction"/> this FactionState is related to.
        /// </summary>
        public Faction Faction
        {
            get { return this.faction; }
        }

        /// <summary>
        /// Gets the reputation towards the <see cref="Faction"/>.
        /// </summary>
        public int Reputation
        {
            get { return this.reputation; }
        }

        /// <summary>
        /// Gets the current <see cref="ReputationLevel"/> towards the <see cref="Faction"/>.
        /// </summary>
        public ReputationLevel ReputationLevel
        {
            get
            {
                return Faction.GetReputationLevel( this.Reputation );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the FactionState class.
        /// </summary>
        /// <param name="faction">
        /// The Faction the new FactionState is related to.
        /// </param>
        public FactionState( Faction faction )
        {
            if( faction == null )
                throw new ArgumentNullException( "faction" );

            this.faction    = faction;
            this.reputation = faction.InitialReputation;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Adds the given value to the reputation value
        /// encapsulated by this FactionState.
        /// </summary>
        /// <param name="reputationValue">
        /// The reputation value to add.
        /// </param>
        public void AddReputation( int reputationValue )
        {
            var oldLevel     = this.ReputationLevel;
            this.reputation += reputationValue;
            var newLevel     = this.ReputationLevel;

            if( oldLevel != newLevel )
            {
                if( this.ReputationLevelChanged != null )
                {
                    this.ReputationLevelChanged( this, new ReputationLevelChangedEventArgs( this.Faction, oldLevel, newLevel ) );
                }
            }
        }

        /// <summary>
        /// Serializes the current state of this FactionState.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.reputation );
        }

        /// <summary>
        /// Deserializes the current state of this FactionState.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.reputation = context.ReadInt32();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The related Faction.
        /// </summary>
        private readonly Faction faction;

        /// <summary>
        /// The reputation towards the <see cref="Faction"/>.
        /// </summary>
        private int reputation;

        #endregion
    }
}
