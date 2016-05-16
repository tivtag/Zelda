// <copyright file="RubyReward.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.Rewards.RubyReward class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Quests.Rewards
{
    /// <summary>
    /// Represents an <see cref="IQuestReward"/> that rewards
    /// the player with rubies.
    /// This class can't be inherited.
    /// </summary>
    public sealed class RubyReward : IQuestReward
    {
        /// <summary>
        /// Gets or sets the number of rubies rewarded by this RubyReward.
        /// </summary>
        public int RubiesRewarded
        {
            get;
            set;
        }

        /// <summary>
        /// Rewards this RubyReward to the player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void Reward( Zelda.Entities.PlayerEntity player )
        {
            player.Statable.Rubies += this.RubiesRewarded;
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

            context.Write( this.RubiesRewarded );
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

            this.RubiesRewarded = context.ReadInt32();
        }
    }
}
