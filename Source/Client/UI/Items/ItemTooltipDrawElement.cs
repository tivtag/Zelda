// <copyright file="ItemTooltipDrawElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemTooltipDrawElement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom.Xna.UI.Tooltips;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ItemTooltipDrawElement : BaseTooltipDrawElement
    {
        /// <summary>
        /// Initializes a new instance of the ItemTooltipDrawElement class.
        /// </summary>
        /// <param name="itemInfoDisplay">
        /// Represents the UI element that executes the actual item information drawing logic.
        /// </param>
        public ItemTooltipDrawElement( ItemInfoDisplay itemInfoDisplay )
        {
            Contract.Requires<ArgumentNullException>( itemInfoDisplay != null );
            
            this.itemInfoDisplay = itemInfoDisplay;
        }

        /// <summary>
        /// Called when the Tooltip drawn by this ItemTooltipDrawElement has changed.
        /// </summary>
        protected override void OnTooltipChanged()
        {
            var itemTooltip = this.Tooltip as ItemTooltip;

            if( itemTooltip == null )
                this.itemInfoDisplay.ItemInstance = null;
            else
                this.itemInfoDisplay.ItemInstance = itemTooltip.ItemInstance;
        }

        /// <summary>
        /// Represents the UI element that executes the actual item information drawing logic.
        /// </summary>
        private readonly ItemInfoDisplay itemInfoDisplay;
    }
}
