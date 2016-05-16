// <copyright file="Merchant.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Merchant class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Diagnostics;
    using Zelda.Entities;

    /// <summary>
    /// A Merchant sells various Items to the player.
    /// </summary>
    public sealed class Merchant : FriendlyNpc, IMerchant
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the localized name of this Merchant.
        /// </summary>
        public LocalizableText LocalizedName
        {
            get
            {
                return this.localizedName;
            }
        }

        /// <summary>
        /// Gets the <see cref="IMerchantSellList"/> that provides access to the <see cref="MerchantItem"/> s this Merchant sells.
        /// </summary>
        public IMerchantSellList SellList
        {
            get 
            {
                return this.sellList;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the Merchant class.
        /// </summary>
        public Merchant()
        {
            this.sellList = new MerchantSellList( this );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to use this IUseable object.
        /// </summary>
        /// <param name="user">
        /// The object which tries to use this IUseable.
        /// </param>
        /// <returns>
        /// true if this IUseable object has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( PlayerEntity user )
        {
            if( !this.Collision.Intersects( user.Collision ) )
                return false;
            
            if( this.AnalyzeAndLoadQuests( user ) )
                return true;

            if( this.HasItemsToSellFor( user ) )
            {
                this.ReadyUpItems();

                var windowService = this.GetMerchantWindowService();                
                return windowService.Show( this, user );
            }
            else
            {
                return this.ShowUncompletedQuestsOrDefaultText( user );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given PlayerEntity
        /// can buy any item from this Merchant.
        /// </summary>
        /// <param name="user">
        /// The player that wishes to buy MerchantItems.
        /// </param>
        /// <returns>
        /// true if any items can be bought;
        /// otherwise false.
        /// </returns>
        private bool HasItemsToSellFor( PlayerEntity user )
        {
            return this.GetAvailableItems( user ).Any();
        }

        /// <summary>
        /// Gets the MerchantItems that available to be bought
        /// by the given PlayerEntity.
        /// </summary>
        /// <param name="buyer">
        /// The player that wishes to buy MerchantItems.
        /// </param>
        /// <returns>
        /// The available MerchantItems.
        /// </returns>
        public IEnumerable<MerchantItem> GetAvailableItems( PlayerEntity buyer )
        {
            return this.sellList.GetAvailable( buyer );
        }

        /// <summary>
        /// Prepares this Merchant for serving items to the player.
        /// </summary>
        private void ReadyUpItems()
        {
            this.sellList.ReadyUp();
        }

        /// <summary>
        /// Gets the discount the given PlayerEntity would get when
        /// buying items at this IMerchant.
        /// </summary>
        /// <param name="buyer">
        /// The player that wishes to buy MerchantItems.
        /// </param>
        /// <returns>
        /// The multiplier that should be applied to the sell price of an item.
        /// </returns>
        public float GetDiscountModifier( PlayerEntity buyer )
        {
            var reputationLevel = buyer.FactionStates.GetReputationLevelTowards( this.Faction );

            switch( reputationLevel )
            {
                case Zelda.Factions.ReputationLevel.Exalted:
                    return 0.80f;

                case Zelda.Factions.ReputationLevel.Revered:
                    return 0.90f;

                case Zelda.Factions.ReputationLevel.Honored:
                    return 0.95f;

                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Gets the <see cref="UI.IMerchantWindowService"/> object.
        /// </summary>
        /// <returns>
        /// The UI.IMerchantWindowService object.
        /// </returns>
        private UI.IMerchantWindowService GetMerchantWindowService()
        {
            return this.Scene.UserInterface.GetService<UI.IMerchantWindowService>();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The localizable name of this Merchant.
        /// </summary>
        private readonly LocalizableText localizedName = new LocalizableText();

        /// <summary>
        /// The list of items this Merchant sells.
        /// </summary>
        private readonly IMerchantSellList sellList;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="Merchant"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<Merchant>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReaderWriter"/> class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services. 
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given entity using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( Merchant entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // Header
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                // Main
                context.Write( entity.Name );
                context.Write( entity.localizedName.Id );

                entity.sellList.Serialize( context );

                // Components
                entity.Collision.Serialize( context );
                entity.Moveable.Serialize( context );
                entity.Behaveable.Serialize( context );
                entity.Visionable.Serialize( context );
                entity.Talkable.Serialize( context );
                entity.QuestsGiveable.Serialize( context );

                // Write DDaS
                if( entity.DrawDataAndStrategy != null )
                {
                    context.Write( Zelda.Entities.Drawing.DrawStrategyManager.GetName( entity.DrawDataAndStrategy ) );
                    entity.DrawDataAndStrategy.Serialize( context );
                }
                else
                {
                    context.Write( string.Empty );
                }

                if( entity.Faction != null )
                {
                    context.Write( entity.Faction.Name );
                }
                else
                {
                    context.Write( string.Empty );
                }
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( Merchant entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

                // Main
                entity.Name = context.ReadString();
                entity.localizedName.Id = context.ReadString();

                entity.sellList.Deserialize( context );

                // Components
                entity.Collision.Deserialize( context );
                entity.Moveable.Deserialize( context );
                entity.Behaveable.Deserialize( context );
                entity.Visionable.Deserialize( context );
                entity.Talkable.Deserialize( context );
                entity.QuestsGiveable.Deserialize( context );

                // Read draw data and strategy:
                string ddsName = context.ReadString();

                // -- need to refactor this --
                if( ddsName.Length != 0 )
                {
                    var dds = serviceProvider.DrawStrategyManager.GetStrategyClone( ddsName, entity );
                    dds.Deserialize( context );

                    try
                    {
                        dds.Load( serviceProvider );
                    }
                    catch( System.IO.FileNotFoundException exc )
                    {
                        serviceProvider.Log.WriteLine( exc.ToString() );
                    }

                    entity.DrawDataAndStrategy = dds;
                }

                string factionName = context.ReadString();

                if( factionName.Length != 0 )
                {
                    entity.Faction = Factions.FactionList.Get( factionName );
                }
            }
        }

        #endregion
    }
}