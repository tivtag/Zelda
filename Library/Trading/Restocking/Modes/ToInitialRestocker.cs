// <copyright file="ToInitialRestocker.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.ToInitialRestocker class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Trading.Restocking
{
    /// <summary>
    /// Implements an <see cref="IRestocker"/> that sets
    /// the restock count of the MerchantItem to the initial stock count.
    /// </summary>
    public sealed class ToInitialRestocker : IRestocker
    {
        /// <summary>
        /// Restocks the given MerchantItem.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem to restock.
        /// </param>
        public void Restock( MerchantItem item )
        {
            item.StockCount = item.InitialStockCount;
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
