// <copyright file="MerchantSellList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.MerchantSellList class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Zelda.Factions;

    /// <summary>
    /// Represents the list of items an <see cref="IMerchant"/> has
    /// available for sale.
    /// This class can't be inherited.
    /// </summary>
    public sealed class MerchantSellList : IMerchantSellList
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="IMerchant"/> that owns this MerchantSellList.
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
        /// Gets the internal list of MerchantItems the IMerchant that owns this MerchantSellList sells.
        /// </summary>
        /// <remarks>
        /// This property is exposed for design-time support.
        /// </remarks>
        [Editor( "Zelda.Trading.Design.MerchantSellListItemsEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public IList<MerchantItem> Items
        {
            get
            {
                return this.items;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MerchantSellList class.
        /// </summary>
        /// <param name="merchant">
        /// The IMerchant that owns the new MerchantSellList.
        /// </param>
        internal MerchantSellList( IMerchant merchant )
        {
            this.merchant = merchant;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the <see cref="MerchantItem"/>s that have are currently
        /// available to be bought by the given potential buyer.
        /// </summary>
        /// <param name="buyer">
        /// The potential buyer.
        /// </param>
        /// <returns>
        /// The available MerchantItems.
        /// </returns>
        public IEnumerable<MerchantItem> GetAvailable( Zelda.Entities.PlayerEntity buyer )
        {
            var reputationLevel = buyer.FactionStates.GetReputationLevelTowards( this.merchant.Faction );
            return this.GetAvailable( reputationLevel );
        }

        /// <summary>
        /// Gets the <see cref="MerchantItem"/>s that have are currently
        /// available to be bought at the given ReputationLevel.
        /// </summary>
        /// <param name="reputationLevel">
        /// The ReputationLevel of the potential buyer.
        /// </param>
        /// <returns>
        /// The available MerchantItems.
        /// </returns>
        private IEnumerable<MerchantItem> GetAvailable( ReputationLevel reputationLevel )
        {
            return from item in this.items
                   where item.StockCount >= 1 &&
                         item.HasRequiredReputation( reputationLevel )
                   select item;
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

            context.Write( this.items.Count );

            for( int i = 0; i < this.items.Count; ++i )
            {
                this.items[i].Serialize( context );
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

            int itemCount = context.ReadInt32();

            for( int i = 0; i < itemCount; ++i )
            {
                var merchantItem = new MerchantItem( this.merchant );

                merchantItem.Deserialize( context );
                this.items.Add( merchantItem );
            }
        }

        /// <summary>
        /// Creates the ItemInstances of the MerchantItems in this MerchantSellList;
        /// if required.
        /// </summary>
        public void ReadyUp()
        {
            foreach( var item in this.items )
            {
                item.CreateItemInstanceIfRequired();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The IMerchant that owns this MerchantSellList.
        /// </summary>
        private readonly IMerchant merchant;

        /// <summary>
        /// The internal list of MerchantItems.
        /// </summary>
        private readonly List<MerchantItem> items = new List<MerchantItem>();

        #endregion
    }
}