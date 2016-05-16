// <copyright file="GiveQuestItemEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.Events.GiveQuestItemEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests.Events
{
    using System.Diagnostics;
    using Zelda.Entities;
    using Zelda.Items;

    /// <summary>
    /// Defines an <see cref="IQuestEvent"/> that gives the PlayerEntity
    /// a specific item when executed. This class can't be inherited.
    /// </summary>
    public sealed class GiveQuestItemEvent : IQuestEvent, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies the Item given by this GiveQuestItemEvent.
        /// </summary>
        /// <value>The default value is null.</value>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Setups this GiveQuestItemEvent.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.itemManager = serviceProvider.ItemManager;
        }

        /// <summary>
        /// Executes this <see cref="IQuestEvent"/>.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        public void Execute( Quest quest )
        {
            var player = quest.Player;
            Debug.Assert( player != null, "The quest has not been accepted or already completed by the player." );

            // Create Item.
            var item         = itemManager.Get( this.ItemName );
            var itemInstance = item.CreateInstance();
                        
            // Insert.
            if( !player.Inventory.Insert( itemInstance ) )
            {
                MapItem.SpawnUnder( player, itemInstance );
            }

            ItemSounds.PlayPutDown( itemInstance.Item );
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

            this.ItemName = context.ReadString();
        }

        /// <summary>
        /// Provides the mechanisms required to load <see cref="Item"/>s.
        /// </summary>
        private ItemManager itemManager;
    }
}
