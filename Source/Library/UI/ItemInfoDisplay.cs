// <copyright file="ItemInfoDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfoDisplay class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using System.Diagnostics;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Zelda.Entities;
    using Zelda.Items;

    /// <summary>
    /// Enables the visualization of any <see cref="ItemInstance"/>.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class ItemInfoDisplay : UIElement
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemInstance"/> whose ItemInfo gets visualized by this ItemInfoDisplay.
        /// </summary>
        public ItemInstance ItemInstance
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="PlayerEntity"/> that wants to see the item info.
        /// </summary>
        public PlayerEntity Player
        {
            get;
            set; 
        }

        /// <summary>
        /// Gets the alpha color of the ItemInfoDisplay.
        /// </summary>
        private float Alpha
        {
            get
            {
                return this.Player.PickedupItemContainer.IsEmpty ? 1.0f : 0.85f;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemInfoDisplay class.
        /// </summary>
        /// <param name="itemInfoVisualizer">
        /// The <see cref="IItemInfoVisualizer"/> that provides a mechanism to draw
        /// the Item information.
        /// </param>
        public ItemInfoDisplay( IItemInfoVisualizer itemInfoVisualizer )
        {
            this.HideAndDisableNoEvent();
            this.PassInputToSubElements = true;

            this.FloorNumber       = 15;
            this.RelativeDrawOrder = 0.95f;

            this.visualizer = itemInfoVisualizer;
        }

        /// <summary>
        /// Called when this ItemInfoDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.ItemInstance == null )
                return;

            Debug.Assert( this.Player != null );

            this.visualizer.Draw(
                new ItemInfoVisualizationDrawContext() {
                    PositionX = (int)this.X,
                    PositionY = (int)this.Y,
                    Depth = this.RelativeDrawOrder,
                    Alpha = this.Alpha,
                    ItemInstance = this.ItemInstance,
                    Statable = this.Player.Statable,
                    EquipmentStatus = this.Player.Equipment,
                    DrawContext = (ZeldaDrawContext)drawContext
                }
            ); 
        }

        /// <summary>
        /// Called when this ItemInfoDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // no op.
        }

        /// <summary>
        /// Provides a mechanism that allows the drawing of the exact information 
        /// that descripes what an Item does.
        /// </summary>
        private readonly IItemInfoVisualizer visualizer;
    }
}
