// <copyright file="IOpenInventoryService.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.IInventoryService interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Zelda.Items;

    /// <summary>
    /// Provides a mechanism for opening/closing the UI for the various Inventory implementations.
    /// </summary>
    public interface IInventoryService : IZeldaSetupable
    {
        /// <summary>
        /// Opens the inventory of the specified type.
        /// </summary>
        /// <typeparam name="TInventory">
        /// The exact type of the Inventory to open.
        /// </typeparam>
        void Open<TInventory>()
            where TInventory : Inventory;

        /// <summary>
        /// Closes the inventory of the specified type.
        /// </summary>
        /// <typeparam name="TInventory">
        /// The exact type of the Inventory to open.
        /// </typeparam>
        void Close<TInventory>()
            where TInventory : Inventory;
    }
}
