// <copyright file="ItemTooltipGroup.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemTooltipGroup class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;
    using Atom.Xna.UI;
    using Zelda.Items;

    /// <summary>
    /// Represents a group of ItemTooltips to which mouse input data is only passed
    /// when the mouse input data lies within the client area of the ItemTooltipGroup.
    /// </summary>
    internal sealed class ItemTooltipGroup : UIElement, IEnumerable<ItemTooltip>
    {
        /// <summary>
        /// Initializes a new instance of the ItemTooltipGroup class.
        /// </summary>
        /// <param name="tooltipDrawElement">
        /// The IooltipDrawElement responsible for drawing the actual item information
        /// when player moves the mouse over any of the ItemTooltip the new ItemTooltipGroup contains.
        /// </param>
        public ItemTooltipGroup( ItemTooltipDrawElement tooltipDrawElement )
        {
            Contract.Requires<ArgumentNullException>( tooltipDrawElement != null );

            this.tooltipDrawElement = tooltipDrawElement;
        }

        /// <summary>
        /// Called when this ItemTooltipGroup is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            this.tooltips.ForEach( tooltip => tooltip.Draw( drawContext ) );
        }

        /// <summary>
        /// Gets called before this ItemTooltipGroup is updated.
        /// </summary>
        protected override void OnPreUpdate()
        {
            this.tooltipDrawElement.Tooltip = null;
        }

        /// <summary>
        /// Called when this ItemTooltipGroup is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            this.tooltips.ForEach( tooltip => tooltip.Update( updateContext ) );
        }

        /// <summary>
        /// Called once per frame to handle mouse input.
        /// </summary>
        /// <param name="mouseState">
        /// The state of the <see cref="Microsoft.Xna.Framework.Input.Mouse"/>.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the <see cref="Microsoft.Xna.Framework.Input.Mouse"/> one frame ago.
        /// </param>
        protected override void HandleMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( this.ClientArea.Contains( mouseState.X, mouseState.Y ) )
            {
                foreach( var tooltip in this.tooltips )
                {
                    if( tooltip.ClientArea.Contains( mouseState.X, mouseState.Y ) )
                    {
                        tooltip.HandleMouseInputCore( ref mouseState, ref oldMouseState );
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new ItemTooltip to this ItemTooltipGroup.
        /// </summary>
        /// <param name="item">
        /// The Item for which a new ItemTooltip should be created.
        /// </param>
        /// <returns>
        /// The newly created ItemTooltip.
        /// </returns>
        internal ItemTooltip AddNew( Item item )
        {
            Contract.Requires<ArgumentNullException>( item != null );

            var tooltip = new ItemTooltip( this.tooltipDrawElement ) {
                FloorNumber = this.FloorNumber,
                RelativeDrawOrder = this.RelativeDrawOrder,
                Item = item
            };

            this.tooltips.Add( tooltip );
            return tooltip;
        }
        
        /// <summary>
        /// Removes all ItemTooltips from this ItemTooltipGroup.
        /// </summary>
        public void Clear()
        {
            this.tooltips.Clear();
        }
        
        /// <summary>
        /// Gets an enumerator that iterates over the ItemTooltips this ItemTooltipGroup contains.
        /// </summary>
        /// <returns>
        /// A new enumerator.
        /// </returns>
        public IEnumerator<ItemTooltip> GetEnumerator()
        {
            return this.tooltips.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates over the ItemTooltips this ItemTooltipGroup contains.
        /// </summary>
        /// <returns>
        /// A new enumerator.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The ItemTooltips that are part of this ItemTooltipGroup.
        /// </summary>
        private readonly List<ItemTooltip> tooltips = new List<ItemTooltip>();

        /// <summary>
        /// The IooltipDrawElement responsible for drawing the actual item information
        /// when player moves the mouse over this ItemTooltip.
        /// </summary>
        private readonly ItemTooltipDrawElement tooltipDrawElement;
    }
}
