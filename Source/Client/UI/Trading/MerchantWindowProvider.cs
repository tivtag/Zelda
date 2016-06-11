// <copyright file="MerchantWindowProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.UI.Trading.MerchantWindowProvider class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Trading
{
    /// <summary>
    /// Provides a mechanism that allows the player to
    /// interact with the offerings of an IMerchant.
    /// </summary>
    internal sealed class MerchantWindowProvider : Zelda.Trading.UI.IMerchantWindowService
    {
        /// <summary>
        /// Initializes a new instance of the MerchantWindowProvider class.
        /// </summary>
        /// <param name="userInterface">
        /// The IngameUserInterface that provides the new MerchantWindowProvider.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public MerchantWindowProvider( IngameUserInterface userInterface, IZeldaServiceProvider serviceProvider )
        {
            this.window = new MerchantWindow( serviceProvider );

            this.userInterface = userInterface;
            this.userInterface.AddElement( this.window );
        }

        /// <summary>
        /// Shows the UIElement responsible for allowing the
        /// player to interact with the offerings of the given IMerchant.
        /// </summary>
        /// <param name="merchant">
        /// The <see cref="Zelda.Trading.IMerchant"/> whose offerings should be presented
        /// to the player.
        /// </param>
        /// <param name="buyer">
        /// The entity that wants to buy from the merchant.
        /// </param>
        /// <returns>
        /// true if the window has been opened;
        /// otherwise false.
        /// </returns>
        public bool Show( Zelda.Trading.IMerchant merchant, Zelda.Entities.PlayerEntity buyer  )
        {
            if( this.userInterface.ToggleWindow( this.window ) )
            {
                this.window.Buyer = buyer;
                this.window.Merchant = merchant;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The IngameWindow that is used to visualize the Merchant.
        /// </summary>
        private readonly MerchantWindow window;

        /// <summary>
        /// The IngameUserInterface that provides the new MerchantWindowProvider.
        /// </summary>
        private readonly IngameUserInterface userInterface;
    }
}
