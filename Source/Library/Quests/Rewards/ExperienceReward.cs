// <copyright file="ExperienceReward.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.Rewards.ExperienceReward class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests.Rewards
{
    /// <summary>
    /// Represents an <see cref="IQuestReward"/> that rewards
    /// the player with experience.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ExperienceReward : IQuestReward
    {
        /// <summary>
        /// Gets or sets the amount of experience rewarded by this ExperienceReward.
        /// </summary>
        public int ExperienceRewarded
        {
            get;
            set;
        }

        /// <summary>
        /// Rewards this ExperienceReward to the player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void Reward( Zelda.Entities.PlayerEntity player )
        {
            player.Statable.AddExperience( this.ExperienceRewarded );
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

            context.Write( this.ExperienceRewarded );
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

            this.ExperienceRewarded = context.ReadInt32();
        }
    }
}
