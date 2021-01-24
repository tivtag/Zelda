// <copyright file="OpenInventoryService.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.OpenInventoryService class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using Zelda.Crafting;
    using Zelda.Items;

    /// <summary>
    /// Implements a mechanism for opening/closing the UI for the various Inventory implementations.
    /// </summary>
    internal sealed class InventoryService : IInventoryService
    {
        /// <summary>
        /// Initializes a new instance of the OpenInventoryService class.
        /// </summary>
        /// <param name="userInterface">
        /// The UserInterface that contains the related UI elements.
        /// </param>
        public InventoryService( IngameUserInterface userInterface )
        {
            this.userInterface = userInterface;
        }

        /// <summary>
        /// Opens the inventory of the specified type.
        /// </summary>
        /// <typeparam name="TInventory">
        /// The exact type of the Inventory to open.
        /// </typeparam>
        public void Open<TInventory>()
            where TInventory : Inventory
        {
            IngameWindow window = this.GetWindow<TInventory>();

            if( window != null && !window.IsOpen )
            {
                if( window is SharedChestWindow )
                {
                    this.PlayOpenChestSound();
                }

                this.userInterface.ToggleWindow( window );
            }
        }

        /// <summary>
        /// Closes the inventory of the specified type.
        /// </summary>
        /// <typeparam name="TInventory">
        /// The exact type of the Inventory to close.
        /// </typeparam>
        public void Close<TInventory>()
            where TInventory : Inventory
        {
            IngameWindow window = this.GetWindow<TInventory>();

            if( window != null && window.IsOpen )
            {
                userInterface.ToggleWindow( window );
            }
        }

        /// <summary>
        /// Gets the IngameWindow associated with the inventory of the specified type.
        /// </summary>
        /// <typeparam name="TInventory">
        /// The exact type of the Inventory.
        /// </typeparam>
        /// <returns>
        /// The associated IngameWindow.
        /// </returns>
        private IngameWindow GetWindow<TInventory>() where TInventory : Inventory
        {
            var type = typeof( TInventory );
            
            if( type == typeof( SharedChest ) )
            {
                return userInterface.GetElement<SharedChestWindow>();                
            }
            else if( type == typeof( CraftingBottle ) )
            {
                return userInterface.GetElement<Zelda.UI.Crafting.CraftingBottleWindow>();
            }
            else
            {
                return userInterface.GetElement<InventoryWindow>();
            }
        }

        /// <summary>
        /// Plays the sound sample for opening a chest.
        /// </summary>
        private void PlayOpenChestSound()
        {
            var sample = this.serviceProvider.AudioSystem.GetSample( "OpenChest.ogg" );

            if( sample != null )
            {
                sample.LoadAsSample( false );
                sample.Play( volume : 0.3f );
            }
        }

        /// <summary>
        /// Setups this OpenInventoryService.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
        
        /// <summary>
        /// The UserInterface that contains the related UI elements.
        /// </summary>
        private readonly IngameUserInterface userInterface;
    }
}
