// <copyright file="QuestNotActiveNorCompletedRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.DropRequirements.QuestNotActiveNorCompletedRequirement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.DropRequirements
{
    using Zelda.Core.Requirements;
    using Zelda.Quests;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="IRequirement"/> that requires the player
    /// to not have completed a specified <see cref="Quest"/> nor currently have it active.
    /// </summary>
    public sealed class QuestNotActiveNorCompletedRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the name that uniquely idenfifies the <see cref="Quest"/>
        /// that is not allowed to be active nor completed.
        /// </summary>
        public string QuestName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the given PlayerEntity
        /// fulfills the requirements as specified by this IItemDropRequirement.
        /// </summary>
        /// <param name="player">
        /// The realted PlayerEntity.
        /// </param>
        /// <returns>
        /// Returns true if the given PlayerEntity fulfills the specified requirement;
        /// or otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity player )
        {
            var questLog = player.QuestLog;

            if( questLog.HasCompletedQuest( this.QuestName ) )
                return false;

            if( questLog.HasActiveQuest( this.QuestName ) )
                return false;
            return true;
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
            context.WriteDefaultHeader();
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
            context.ReadDefaultHeader( this.GetType() );
            this.QuestName = context.ReadString();
        }
    }
}
