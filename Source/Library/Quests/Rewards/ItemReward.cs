// <copyright file="ItemReward.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.Rewards.ItemReward class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests.Rewards
{
    using Zelda.Entities;
    using Zelda.Items;
    
    /// <summary>
    /// Represents an <see cref="IQuestReward"/> that rewards
    /// the player with item(s).
    /// This class can't be inherited.
    /// </summary>
    public sealed class ItemReward : IQuestReward, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the name of the item rewarded by this ItemReward.
        /// </summary>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of items (of the same type) rewarded by this ItemReward.
        /// </summary>
        /// <remarks>
        /// Usually used to reward multiple instances of a stackable item;
        /// such as a potion.
        /// </remarks>
        public int ItemCount
        {
            get;
            set;
        }

        /// <summary>
        /// Setups this ItemReward instance.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.itemManager = serviceProvider.ItemManager;
        }

        /// <summary>
        /// Rewards this ItemReward to the player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void Reward( PlayerEntity player )
        {
            var item = this.itemManager.Get( this.ItemName );

            var itemInstance = item.CreateInstance();
            itemInstance.Count = this.ItemCount;

            // Spawn.
            ItemSounds.PlayPickUp( item );
            player.Inventory.FailSafeInsert( itemInstance );
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

            context.Write( this.ItemName ?? string.Empty );
            context.Write( this.ItemCount );
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

            this.ItemName  = context.ReadString();
            this.ItemCount = context.ReadInt32();
        }

        /// <summary>
        /// The ItemManager that is used to load the Items.
        /// </summary>
        private ItemManager itemManager;
    }
}
