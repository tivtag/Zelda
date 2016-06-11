// <copyright file="QuestCompletedRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.Requirements.QuestCompletedRequirement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests.Requirements
{
    using Zelda.Core.Requirements;

    /// <summary>
    /// Defines an <see cref="IRequirement"/> that requires
    /// the player to have completed a specific quest to fulfill it.
    /// This class can't be inherited.
    /// </summary>
    public sealed class QuestCompletedRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies the quest
        /// the player must have completed to fulfill this IQuestRequirement.
        /// </summary>
        public string QuestName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the player fulfills this <see cref="QuestCompletedRequirement"/>.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the given PlayerEntity fulfills this QuestCompletedRequirement; 
        /// otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity player )
        {
            if( string.IsNullOrEmpty( this.QuestName ) )
                return true;

            return player.QuestLog.HasCompletedQuest( this.QuestName );
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

            context.Write( this.QuestName ?? string.Empty );
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

            this.QuestName = context.ReadString();
        }
    }
}
