using System;
using System.Globalization;
using Atom;
using Zelda.Items;

namespace Zelda.Quests.Goals
{
    /// <summary>
    /// Defines an <see cref="IQuestGoal"/> that requires
    /// the player to get N-amount items of type X.
    /// This class can't be inherited.
    /// </summary>
    /// <todo>
    /// Tracking of the goal's state using the StateChanged
    /// event isn't implemented yet.
    /// </todo>
    public sealed class GetItemsGoal : IQuestGoal, IZeldaSetupable
    {
        #region [ Events ]

        /// <summary>
        /// Invoked when the state of this IQuestGoal has changed.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the item required to be picked-up for this GetItemsGoal.
        /// </summary>
        public string ItemName
        {
            get { return this.itemName; }
            set
            {
                this.itemName = value;

                if( itemManager != null )
                {
                    item = itemManager.Load( value );
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of items (of the same type) required to be picked-up for this GetItemsGoal.
        /// </summary>
        public int ItemAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the state of this GetItemsGoal in percentage.
        /// </summary>
        public float State
        {
            get 
            {
                if( player == null )
                    return 0.0f;

                // Query the number of items :)
                int amountFound;
                player.Inventory.Contains( this.ItemName, this.ItemAmount, out amountFound );

                if( amountFound >= this.ItemAmount )
                    return 1.0f;

                return amountFound / (float)this.ItemAmount;
            }
        }

        /// <summary>
        /// Gets a short (localized) string that descripes
        /// the current of this GetItemsGoal.
        /// </summary>
        public string StateDescription
        {
            get
            {
                if( this.player == null )
                    return null;

                // Query the number of items :)
                int amountFound;
                this.player.Inventory.Contains( this.ItemName, this.ItemAmount, out amountFound );

                if( amountFound > this.ItemAmount )
                    amountFound = this.ItemAmount;

                string name = this.item != null ? this.item.LocalizedName : this.ItemName;

                if( this.ItemAmount != 1 )
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        QuestResources.QuestGoalDesc_GetItems,
                        name,
                        amountFound.ToString( CultureInfo.CurrentCulture ),
                        this.ItemAmount.ToString( CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        QuestResources.QuestGoalDesc_GetItem,
                        name,
                        amountFound.ToString( CultureInfo.CurrentCulture )
                    );
                }
            }
        }    

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Setups this <see cref="GetItemsGoal"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides a fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.itemManager = serviceProvider.ItemManager;
        }

        /// <summary>
        /// Gets a value indicating whether the player accomplished this GetItemsGoal.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the given PlayerEntity has accomplished this IQuestGoal; 
        /// otherwise false.
        /// </returns>
        public bool IsAccomplished( Zelda.Entities.PlayerEntity player )
        {
            return player.Inventory.Contains( this.ItemName, this.ItemAmount );
        }
        
        /// <summary>
        /// Fired when the player has accepeted the quest this GetItemsGoal is related to.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void OnAccepted( Zelda.Entities.PlayerEntity player )
        {
            this.player = player;
            this.player.ItemCollected += this.OnPlayerCollectedOrDroppedItem;
            this.player.PickedupItemContainer.Dropped += this.OnPlayerCollectedOrDroppedItem;
        }

        /// <summary>
        /// Fired when the player has accomplished all goals (including this!) of a <see cref="Quest"/>.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void OnTurnIn( Zelda.Entities.PlayerEntity player )
        {
            this.player.Inventory.Remove( this.ItemName, this.ItemAmount );

            this.player.ItemCollected -= this.OnPlayerCollectedOrDroppedItem;
            this.player.PickedupItemContainer.Dropped -= this.OnPlayerCollectedOrDroppedItem;
            this.player = null;
        }

        /// <summary>
        /// Called when the player has collected an Item.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="item">
        /// The Item that the player has collected.
        /// </param>
        private void OnPlayerCollectedOrDroppedItem( object sender, Item item )
        {
            if( item.Name.Equals( this.ItemName, StringComparison.Ordinal ) )
            {
                this.StateChanged.Raise( this );
            }
        }
        
        #region > Storage <

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
            context.Write( this.ItemAmount );
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

            this.ItemName   = context.ReadString();
            this.ItemAmount = context.ReadInt32();
        }

        /// <summary>
        /// Serializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Saving.IStateSaveable.SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
        }

        /// <summary>
        /// Deserializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Saving.IStateSaveable.DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Tue storage field of the <see cref="ItemName"/> property.
        /// </summary>
        private string itemName;

        /// <summary>
        /// Identifies the Item that is required to be found for this Quest.
        /// </summary>
        private Item item;
        
        /// <summary>
        /// Identifies the PlayerEntity that has accepted this GetItemsGoal.
        /// </summary>
        private Zelda.Entities.PlayerEntity player;

        /// <summary>
        /// Identifies the ItemManager that is used to load the Item.
        /// </summary>
        private ItemManager itemManager;

        #endregion        
    }
}
