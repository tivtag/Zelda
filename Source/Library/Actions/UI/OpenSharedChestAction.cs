// <copyright file="OpenSharedChestAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.UI.OpenSharedChestAction class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Actions.UI
{
    using Atom;
    using Zelda.Items;
    using Zelda.UI;

    /// <summary>
    /// Implements an action that when executed opens the UI for the <see cref="SharedChest"/>.
    /// </summary>
    public sealed class OpenSharedChestAction : BaseUserInterfaceAction
    {
        /// <summary>
        /// Executes this OpenSharedChestAction.
        /// </summary>
        public override void Execute()
        {
            var openInventoryService = this.UserInterface.GetService<IInventoryService>();
            if( openInventoryService == null )
                return;

            openInventoryService.Open<SharedChest>();
        }

        /// <summary>
        /// Deferredly undoes this IAction.
        /// </summary>
        public override void Dexecute()
        {
            var openInventoryService = this.UserInterface.GetService<IInventoryService>();
            if( openInventoryService == null )
                return;

            openInventoryService.Close<SharedChest>();
        }
    }
}
