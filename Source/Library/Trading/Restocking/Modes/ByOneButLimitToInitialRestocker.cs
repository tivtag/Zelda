﻿
namespace Zelda.Trading.Restocking
{
    /// <summary>
    /// Implements an <see cref="IRestocker"/> that increases
    /// the stock count of a MerchantItem by 1, to a maximum
    /// of the InitialStockCount.
    /// </summary>
    public sealed class ByOneButLimitToInitialRestocker : IRestocker
    {
        /// <summary>
        /// Restocks the given MerchantItem.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem to restock.
        /// </param>
        public void Restock( MerchantItem item )
        {
            if( item.StockCount >= item.InitialStockCount )
            {
                item.StockCount = item.InitialStockCount;
            }
            else
            {
                item.StockCount += 1;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
        }
    }
}
