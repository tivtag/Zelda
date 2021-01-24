// <copyright file="ReputationLevelChangedEventArgs.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Factions.ReputationLevelChangedEventArgs class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Factions
{
    /// <summary>
    /// Defines the EventArgs used by the <see cref="FactionState.ReputationLevelChanged"/> event.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ReputationLevelChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the <see cref="Faction"/> related to this ReputationLevelChangedEventArgs.
        /// </summary>
        public Faction Faction
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the old <see cref="ReputationLevel"/> -before- the event has occured.
        /// </summary>
        public ReputationLevel NewLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the new <see cref="ReputationLevel"/> -after- the event has occured.
        /// </summary>
        public ReputationLevel OldLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the ReputationLevelChangedEventArgs class.
        /// </summary>
        /// <param name="faction">
        /// The <see cref="Faction"/> related to the new ReputationLevelChangedEventArgs.
        /// </param>
        /// <param name="oldLevel">
        /// The old <see cref="ReputationLevel"/> -before- the event has occured.
        /// </param>
        /// <param name="newLevel">
        /// The new <see cref="ReputationLevel"/> -after- the event has occured.
        /// </param>
        internal ReputationLevelChangedEventArgs( Faction faction, ReputationLevel oldLevel, ReputationLevel newLevel )
        {
            System.Diagnostics.Debug.Assert( faction != null, "The given Faction is null." );

            this.Faction  = faction;
            this.OldLevel = oldLevel;
            this.NewLevel = newLevel;
        }
    }
}
