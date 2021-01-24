// <copyright file="IMerchantSellList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.IMerchantSellList interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Trading
{
    using System.Collections.Generic;
    using System.ComponentModel;
    
    /// <summary>
    /// Represents the list of <see cref="MerchantItem"/> an <see cref="IMerchant"/> has
    /// available for sale.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IMerchantSellList : Saving.ISaveable
    {
        /// <summary>
        /// Gets the <see cref="IMerchant"/> that owns this IMerchantSellList.
        /// </summary>
        IMerchant Merchant 
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="MerchantItem"/>s that are part of this IMerchantSellList.
        /// </summary>
        IList<MerchantItem> Items
        { 
            get;
        }

        /// <summary>
        /// Gets all <see cref="MerchantItem"/>s that are available to be sold to the
        /// specified PlayerEntity.
        /// </summary>
        /// <param name="buyer">
        /// The potential buyer.
        /// </param>
        /// <returns>
        /// The MerchantItem that might be sold.
        /// </returns>
        IEnumerable<MerchantItem> GetAvailable( Zelda.Entities.PlayerEntity buyer );

        /// <summary>
        /// Makes sure that this IMerchantSellList is ready to be used.
        /// </summary>
        void ReadyUp();
    }
}
