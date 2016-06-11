// <copyright file="OnMerchantCreatedRestockMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Restocking.OnMerchantCreatedRestockMethod class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading.Restocking
{
    /// <summary>
    /// Defines an <see cref="IRestockMethod"/> that restocks a
    /// <see cref="MerchantItem"/> when the IMerchant that owns it
    /// has been added to a ZeldaScene.
    /// </summary>
    public sealed class OnMerchantCreatedRestockMethod : RestockMethod
    {
        /// <summary>
        /// Called when this RestockMethod has been hooked up with the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected override void OnHooked( MerchantItem merchantItem )
        {
            merchantItem.Merchant.Added += this.OnMerchantAddedToScene;
        }

        /// <summary>
        /// Called when this RestockMethod has been unhooked from the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected override void OnUnhooked( MerchantItem merchantItem )
        {
            merchantItem.Merchant.Added -= this.OnMerchantAddedToScene;
        }

        /// <summary>
        /// Called when the Merchant that owns the <see cref="MerchantItem"/>
        /// this IRestockMethod got hooked onto got added to a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnMerchantAddedToScene( object sender, ZeldaScene scene )
        {
            this.Restock();
        }
    }
}
