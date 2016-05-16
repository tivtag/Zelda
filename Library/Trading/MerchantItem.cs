// <copyright file="MerchantItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.MerchantItem class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading
{
    using System;
    using System.ComponentModel;
    using Atom;
    using Zelda.Factions;
    using Zelda.Items;
    using Zelda.Saving;
    using Zelda.Saving.Storage;
    using Zelda.Trading.Restocking;

    /// <summary>
    /// Represents an item that an <see cref="IMerchant"/> is selling
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class MerchantItem : Zelda.Saving.ISaveable
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this MerchantItem has been sold.
        /// </summary>
        public event EventHandler Sold;

        #endregion

        #region [ Enums ]

        /// <summary>
        /// Enumerates the different ways an ItemInstance for this MerchantItem is created.
        /// </summary>
        public enum ItemInstanceCreationMode
        {
            /// <summary>
            /// By default ItemInstances don't have any affixes.
            /// </summary>
            Default,

            /// <summary>
            /// Allows the ItemInstance to have affixes.
            /// </summary>
            AllowAffixes
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Item that is encapulsated by this MerchantItem.
        /// </summary>
        [Editor( typeof( Zelda.Items.Design.ItemEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Item Item
        {
            get;
            set;
        }

        /// <summary>
        /// Gets how much the <see cref="Item"/> is worth.
        /// </summary>
        public int RubiesWorth
        {
            get
            {
                if( this.Item == null )
                {
                    return 0;
                }
                else
                {
                    return this.Item.RubiesWorth;
                }
            }
        }

        /// <summary>
        /// Gets the ItemInstance that has been created for this MerchantItem.
        /// </summary>
        [Browsable( false )]
        public ItemInstance ItemInstance
        {
            get
            {
                return this.dataStorage.ItemInstance;
            }

            private set
            {
                this.dataStorage.ItemInstance = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of rubees the <see cref="Item"/>
        /// costs.
        /// </summary>
        public int Price
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum required reputation level for this MerchantItem
        /// to be buyable.
        /// </summary>
        public ReputationLevel MinimumReputation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial number of items a merchant has in stock
        /// when he initially opens his store.
        /// </summary>
        public int InitialStockCount
        {
            get
            {
                return this._initialStockCount;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value", 0, Atom.ErrorStrings.SpecifiedValueIsNegative );

                this._initialStockCount = value;
            }
        }

        /// <summary>
        /// Gets or sets how the ItemInstance of this MerchantItem is created.
        /// </summary>
        public ItemInstanceCreationMode CreationMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of items the merchant currently has in stock.
        /// </summary>
        [Browsable(false)]
        public int StockCount
        {
            get
            {
                if( this.dataStorage == null )
                    return 0;
                return this.dataStorage.StockCount;
            }

            internal set
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value", 0, Atom.ErrorStrings.SpecifiedValueIsNegative );

                if( this.dataStorage == null )
                    return;
                this.dataStorage.StockCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRestockMethod"/> used by the IMerchant
        /// that owns this MerchantItem to restock it.
        /// </summary>
        [Editor( typeof( Zelda.Trading.Restocking.Design.RestockMethodEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IRestockMethod RestockMethod
        {
            get
            {
                return this._restockMethod;
            }

            set
            {
                if( this._restockMethod != null )
                    this._restockMethod.Unhook();

                this._restockMethod = value;

                if( this._restockMethod != null )
                    this._restockMethod.Hook( this );
            }
        }

        /// <summary>
        /// Gets the <see cref="IMerchant"/> that owns this MerchantItem.
        /// </summary>
        [Browsable(false)]
        public IMerchant Merchant
        {
            get
            {
                return this.merchant;
            }
        }

        /// <summary>
        /// Gets the string that uniquely identifies this MerchantItem.
        /// </summary>
        public string Identifier
        {
            get
            {
                if( this.Item == null )
                {
                    return string.Empty;
                }

                return this.GetDataStorageIdentifier();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IStorage"/> that holds data saved about
        /// the current state of the restocking process to the save file.
        /// </summary>
        [Browsable(false)]
        public IStorage RestockDataStorage
        {
            get
            {
                if( this.dataStorage == null )
                    return null;

                return this.dataStorage.RestockDataStorage;
            }

            set
            {
                if( this.dataStorage == null )
                    return;

                this.dataStorage.RestockDataStorage = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MerchantItem class.
        /// </summary>
        /// <param name="merchant">
        /// The IMerchant that owns the new MerchantItem.
        /// </param>
        public MerchantItem( IMerchant merchant )
        {
            this.merchant = merchant;
            this.merchant.Added += this.OnMerchantAddedToScene;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the final price the given buyer has to pay for this MerchantItem.
        /// </summary>
        /// <param name="buyer">
        /// The entity that wants to buy this MerchantItem.
        /// </param>
        /// <returns>
        /// The number of rubies the buyer has to buy; after substracting discount.
        /// </returns>
        public int GetFinalPrice( Zelda.Entities.PlayerEntity buyer )
        {
            return (int)(this.Price * this.merchant.GetDiscountModifier( buyer ) );
        }

        /// <summary>
        /// Sells this MerchantItem to the given buyer.
        /// </summary>
        /// <param name="buyer">
        /// The entity that wants to buy this MerchantItem.
        /// </param>
        /// <returns>
        /// The ItemInstance that has been sold;
        /// or null if no item has been sold.
        /// </returns>
        public ItemInstance SellTo( Zelda.Entities.PlayerEntity buyer )
        {
            if( !this.CanSellTo( buyer ) )
                return null;

            this.CreateItemInstanceIfRequired();

            var itemInstance = this.ItemInstance;
            this.StockCount -= 1;
            buyer.Statable.Rubies -= this.GetFinalPrice( buyer );
            
            this.dataStorage.ItemInstance = null;
            this.Sold.Raise( this );

            this.CreateItemInstanceIfRequired();
            return itemInstance;
        }

        /// <summary>
        /// Gets a value indicating whether this MerchantItem
        /// could potentially be sold to the given buyer.
        /// </summary>
        /// <param name="buyer">
        /// The entity that wants to buy this MerchantItem.
        /// </param>
        /// <returns>
        /// true if it could be sold;
        /// otherwise false.
        /// </returns>
        public bool CanSellTo( Zelda.Entities.PlayerEntity buyer )
        {
            if( this.StockCount == 0 || this.Item == null )
                return false;

            return this.HasRequiredRubies( buyer ) && this.HasRequiredReputation( buyer );
        }

        /// <summary>
        /// Gets a value indicating whether the given potential buyer
        /// has the required reputation to buy this MerchantItem.
        /// </summary>
        /// <param name="buyer">
        /// The entity that wants to buy this MerchantItem.
        /// </param>
        /// <returns>
        /// true if the required reputation level is fulfilled;
        /// otherwise false.
        /// </returns>
        internal bool HasRequiredReputation( Zelda.Entities.PlayerEntity buyer )
        {
            var reputationLevel = buyer.FactionStates.GetReputationLevelTowards( this.Merchant.Faction );
            return this.HasRequiredReputation( reputationLevel );
        }

        /// <summary>
        /// Gets a value indicating whether the given <see cref="ReputationLevel"/>
        /// is enough to be able to buy this MerchantItem.
        /// </summary>
        /// <param name="reputationLevel">
        /// The ReputationLevel of the buyer towards the IMerchant.
        /// </param>
        /// <returns>
        /// true if the required reputation level is fulfilled;
        /// otherwise false.
        /// </returns>
        internal bool HasRequiredReputation( ReputationLevel reputationLevel )
        {
            return (int)reputationLevel >= (int)this.MinimumReputation;
        }

        /// <summary>
        /// Gets a value indicating whether the given potential buyer
        /// has enough rubies to buy this MerchantItem.
        /// </summary>
        /// <param name="buyer">
        /// The ReputationLevel of the buyer towards the IMerchant.
        /// </param>
        /// <returns>
        /// true if the required reputation level is fulfilled;
        /// otherwise false.
        /// </returns>
        public bool HasRequiredRubies( Zelda.Entities.PlayerEntity buyer )
        {
            return buyer.Statable.Rubies >= this.GetFinalPrice( buyer );
        }

        /// <summary>
        /// Creates the <see cref="ItemInstance"/>; if required.
        /// </summary>
        public void CreateItemInstanceIfRequired()
        {
            if( this.Item == null )
                return;

            if( this.StockCount >= 1 && this.ItemInstance == null )
            {
                this.CreateItemInstance();
            }
        }

        /// <summary>
        /// Creates the <see cref="ItemInstance"/> depending on the current <see cref="CreationMode"/>.
        /// </summary>
        private void CreateItemInstance()
        {
            switch( this.CreationMode )
            {
                case ItemInstanceCreationMode.Default:
                    this.ItemInstance = this.Item.CreateInstance( this.serviceProvider.Rand );
                    break;

                case ItemInstanceCreationMode.AllowAffixes:
                    this.ItemInstance = Zelda.Items.ItemCreationHelper.Create( this.Item, this.serviceProvider.Rand );
                    break;

                default:
                    throw new NotImplementedException();
            }
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

            context.Write( this.Item != null ? this.Item.Name : string.Empty );

            context.Write( this.Price );
            context.Write( this.InitialStockCount );
            context.Write( (int)this.MinimumReputation );
            context.Write( (int)this.CreationMode );

            if( this.RestockMethod != null )
            {
                context.Write( this.RestockMethod.GetType().GetTypeName() );
                this.RestockMethod.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }
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

            this.serviceProvider = context.ServiceProvider;

            string itemName = context.ReadString();            
            if( itemName.Length > 0 )
            {
                var itemManager = context.ServiceProvider.ItemManager;
                this.Item = itemManager.Get( itemName );
            }

            this.Price = context.ReadInt32();
            this.InitialStockCount = context.ReadInt32();
            this.MinimumReputation = (ReputationLevel)context.ReadInt32();
            this.CreationMode = (ItemInstanceCreationMode)context.ReadInt32();

            this.DeserializeRestockMode( context );
        }
        
        /// <summary>
        /// Called when the Merchant that owns this <see cref="MerchantItem"/>
        /// got added to a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnMerchantAddedToScene( object sender, ZeldaScene scene )
        {
            this.ReceiveDataStorage( scene.Status );
        }

        /// <summary>
        /// Receives the StockCount IntegerStorage from the SceneStatus.
        /// </summary>
        /// <param name="sceneStatus">
        /// The current status of the ZeldaScene.
        /// </param>
        private void ReceiveDataStorage( Zelda.Saving.SceneStatus sceneStatus )
        {
            if( sceneStatus == null || this.Item == null )
                return;

            var dataStore = sceneStatus.DataStore;
            string identifier = this.GetDataStorageIdentifier();
            this.dataStorage = dataStore.TryGet<MerchantItemStorage>( identifier );

            if( this.dataStorage == null )
            {
                this.dataStorage = new MerchantItemStorage() {
                    StockCount = this.InitialStockCount
                };

                dataStore.AddOrReplace( identifier, this.dataStorage );
            }
        }

        /// <summary>
        /// Gets the identifier that is used to store the StockCount in the
        /// save file.
        /// </summary>
        /// <returns>
        /// The unique indifier.
        /// </returns>
        private string GetDataStorageIdentifier()
        {
            return this.Merchant.Name + '.' + this.Item.Name;
        }

        /// <summary>
        /// Deserializes the RestockMode.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void DeserializeRestockMode( Zelda.Saving.IZeldaDeserializationContext context )
        {
            string typeName = context.ReadString();
            if( typeName.Length == 0 )
                return;

            Type type = Type.GetType( typeName );
            this.RestockMethod = (IRestockMethod)Activator.CreateInstance( type );
            this.RestockMethod.Deserialize( context );
        }

        /// <summary>
        /// Overridden to return the name of the underlying Item.
        /// </summary>
        /// <returns>
        /// The name of the underlying item.
        /// </returns>
        public override string ToString()
        {
            return this.Item != null ? this.Item.Name : string.Empty;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field of the InitialStockCount property.
        /// </summary>
        private int _initialStockCount;

        /// <summary>
        /// The storage field of the <see cref="RestockMethod"/> property.
        /// </summary>
        private IRestockMethod _restockMethod;

        /// <summary>
        /// The storage field of the <see cref="Merchant"/> property.
        /// </summary>
        private readonly IMerchant merchant;

        /// <summary>
        /// The place in which the data that is saved within the SaveFile is stored.
        /// </summary>
        private MerchantItemStorage dataStorage;
        
        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ Classes ]

        /// <summary>
        /// Defines the storage place for the data that is saved
        /// within the SaveFile for a MerchantItem.
        /// </summary>
        private sealed class MerchantItemStorage : IStorage
        {
            /// <summary>
            /// Gets or sets the number of items the Merchant has currently in stock
            /// of this MerchantItem.
            /// </summary>
            public int StockCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the ItemInstance that is currently sold for this MerchantItem.
            /// </summary>
            public ItemInstance ItemInstance
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="IStorage"/> that holds data saved about
            /// the current state of the restocking process to the save file.
            /// </summary>
            public IStorage RestockDataStorage
            {
                get;
                set;
            }
            
            /// <summary>
            /// Serializes the data required to descripe this IStorage.
            /// </summary>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void SerializeStorage( Zelda.Saving.IZeldaSerializationContext context )
            {
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                context.Write( this.StockCount );
                this.SerializeItemInstance( context );
                this.SerializeRestockDataStorage( context );
            }

            /// <summary>
            /// Serializes the <see cref="ItemInstance"/> data field.
            /// </summary>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            private void SerializeItemInstance( Zelda.Saving.IZeldaSerializationContext context )
            {
                if( this.MustSerializeItemInstance() )
                {
                    context.Write( true );
                    this.ItemInstance.Serialize( context );
                }
                else
                {
                    context.Write( false );
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="ItemInstance"/> must be
            /// serialized into this IStorage.
            /// </summary>
            /// <returns>
            /// true if the ItemInstance should be serialized (into the SaveFile);
            /// otherwise false.
            /// </returns>
            private bool MustSerializeItemInstance()
            {
                return this.ItemInstance != null && this.ItemInstance is Zelda.Items.Affixes.IAffixedItemInstance;
            }      

            /// <summary>
            /// Serializes the data stored in the <see cref="RestockDataStorage"/>.
            /// </summary>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            private void SerializeRestockDataStorage( Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteStorage( this.RestockDataStorage );
            }     
                        
            /// <summary>
            /// Deserializes the data required to descripe this IStorage.
            /// </summary>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void DeserializeStorage( Zelda.Saving.IZeldaDeserializationContext context )
            {
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

                this.StockCount = context.ReadInt32();
                this.DeserializeItemInstance( context );

                if( version >= 2 )
                {
                    this.DeserializeRestockDataStorage( context );
                }
            }

            /// <summary>
            /// Deserializes the ItemInstance.
            /// </summary>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            private void DeserializeItemInstance( Zelda.Saving.IZeldaDeserializationContext context )
            {
                if( context.ReadBoolean() )
                {
                    this.ItemInstance = Zelda.Items.ItemInstance.Read( context );
                }
            }

            /// <summary>
            /// Deserializes the RestockDataStorage.
            /// </summary>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            private void DeserializeRestockDataStorage( Zelda.Saving.IZeldaDeserializationContext context )
            {
                this.RestockDataStorage = context.ReadStorage<IStorage>();
            } 
        }

        #endregion
    }
}
