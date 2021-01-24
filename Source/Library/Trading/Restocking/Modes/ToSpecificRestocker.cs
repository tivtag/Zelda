// <copyright file="ToSpecficRestocker.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.ToSpecficRestocker class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Trading.Restocking
{
    using Atom.Diagnostics.Contracts;

    /// <summary>
    /// Implements an <see cref="IRestocker"/> that sets
    /// the restock count of the MerchantItem to the initial stock count.s
    /// </summary>
    public sealed class ToSpecficRestocker : IRestocker
    {
        /// <summary>
        /// Gets or sets the stock will be restocked to.
        /// </summary>
        public int Stock
        {
            get
            {
                return this.stock;
            }

            set
            {
                Contract.Requires( value >= 0 );

                this.stock = value;
            }
        }

        /// <summary>
        /// Restocks the given MerchantItem.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem to restock.
        /// </param>
        public void Restock( MerchantItem item )
        {
            if( this.Stock > item.StockCount )
            {
                item.StockCount = this.Stock;
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
            context.Write( this.stock );
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
            this.stock = context.ReadInt32();
        }

        /// <summary>
        /// Represents the storage field of the Stock property.
        /// </summary>
        private int stock = 1;
    }
}
