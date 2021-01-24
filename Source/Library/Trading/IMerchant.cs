// <copyright file="IMerchant.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.IMerchant interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Trading
{
    using System.Collections.Generic;
    using Zelda.Entities;
    using Zelda.Factions;

    /// <summary>
    /// An IMerchant sells various Items to the player.
    /// </summary>
    public interface IMerchant : IZeldaEntity, IUseable
    {
        /// <summary>
        /// Gets the localized name of this IMerchant.
        /// </summary>
        LocalizableText LocalizedName
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="IMerchantSellList"/> that provides access to the <see cref="MerchantItem"/> s this IMerchant sells.
        /// </summary>
        IMerchantSellList SellList
        {
            get;
        }

        /// <summary>
        /// Gets or sets the <see cref="Faction"/> this IMerchant is part of.
        /// </summary>
        Faction Faction
        {
            get;
            set;
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
        IEnumerable<MerchantItem> GetAvailableItems( PlayerEntity buyer );

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
        float GetDiscountModifier( PlayerEntity buyer );
    }
}
