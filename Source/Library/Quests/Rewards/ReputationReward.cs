// <copyright file="ReputationReward.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.Rewards.ReputationReward class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Quests.Rewards
{
    using Zelda.Factions;

    /// <summary>
    /// Defines an <see cref="IQuestReward"/> that awards
    /// reputation towards a <see cref="Faction"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ReputationReward : IQuestReward
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies the <see cref="Faction"/>
        /// this ReputationReward rewards point towards.
        /// </summary>
        public string FactionName
        {
            get 
            {
                if( this.Faction == null )
                    return null;

                return this.Faction.Name;
            }

            set
            {
                this.Faction = Factions.FactionList.Get( value );
            }
        }

        /// <summary>
        /// Gets the <see cref="Factions.Faction"/> this ReputationReward rewards point towards.
        /// </summary>
        public Faction Faction
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the reputation this ReputationReward rewards towards the <see cref="Faction"/>.
        /// </summary>
        public int ReputationAwarded
        {
            get;
            set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Rewards this ReputationReward to the player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void Reward( Zelda.Entities.PlayerEntity player )
        {
           var factionState = player.FactionStates.GetState( this.FactionName );
           if( factionState == null )
               return;

           factionState.AddReputation( this.ReputationAwarded );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.FactionName ?? string.Empty );
            context.Write( this.ReputationAwarded );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.FactionName = context.ReadString();
            this.ReputationAwarded = context.ReadInt32();
        }

        #endregion
    }
}
