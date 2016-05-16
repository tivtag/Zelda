// <copyright file="IMerchantWindowService.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.IMerchantWindowService interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading.UI
{    
    /// <summary>
    /// Provides a mechanism that allows the player to
    /// interact with the offerings of an IMerchant.
    /// </summary>
    public interface IMerchantWindowService
    {
        /// <summary>
        /// Shows the UIElement responsible for allowing the
        /// player to interact with the offerings of the given IMerchant.
        /// </summary>
        /// <param name="merchant">
        /// The <see cref="IMerchant"/> whose offerings should be presented
        /// to the buyer.
        /// </param>
        /// <param name="buyer">
        /// The entity that wants to buy from the merchant.
        /// </param>
        /// <returns>
        /// true if the window has been opened;
        /// otherwise false.
        /// </returns>
        bool Show( IMerchant merchant, Zelda.Entities.PlayerEntity buyer );                
    }
}
