// <copyright file="InventoryWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.InventoryWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Items;

    /// <summary>
    /// Defines the <see cref="IngameWindow"/> that is used to
    /// visualize the <see cref="Inventory"/> of the Player.
    /// </summary>
    internal sealed class InventoryWindow : InventoryBaseWindow
    {
        /// <summary>
        /// Gets or sets the SideBar UIElement that is part of the User Interface.
        /// </summary>
        public SideBar SideBar { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryWindow"/> class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Provides a mechanism to visualize the cooldown on the ItemUseEffect of an item.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal InventoryWindow( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
            : base( cooldownVisualizer, serviceProvider )
        {
            this.Size = new Atom.Math.Vector2( 
                CellSize * Inventory.DefaultGridWidth,
                CellSize * Inventory.DefaultGridHeight
            );

            this.Position = serviceProvider.ViewSize / 2 - this.Size / 2;
        }
        
        /// <summary>
        /// Called when this <see cref="InventoryWindow"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.player == null )
                return;

            this.DrawCellsAndItems( drawContext );
            this.DrawPickedUpItemIndicator(
                this.PickedUpItem,
                this.MousePosition,
                (ZeldaDrawContext)drawContext
            );
        }
        
        /// <summary>
        /// Handles mouse input related to this <see cref="InventoryWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        /// <returns>Whether input should be passed to elements behind this UIElement.</returns>
        protected override bool HandleRelatedMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( this.Player == null )
                return true;
            
            Point2 cell = GetMouseCellPosition( ref mouseState );
            
            var inventory = this.Player.Inventory;
            if( inventory.IsValidCell( cell ) )
            {
                // Handle click
                ItemInstance itemInstance = inventory.GetItemAt( cell );
                if( mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released )
                {
                    inventory.HandleLeftClick( cell.X, cell.Y, this.Owner.IsShiftDown );
                }
                else if( mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released )
                {
                    inventory.HandleRightClick( cell.X, cell.Y, this.Owner.IsControlDown );
                }

                RefreshItemInfoDisplay();
            }

            return false;
        }

        /// <summary>
        /// Handles mouse input related for this <see cref="InventoryWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            base.HandleMouseInput( ref mouseState, ref oldMouseState );

            if( !this.ClientArea.Contains( mouseState.X, mouseState.Y ) )
            {
                this.ItemInfoDisplay.ItemInstance = null;

                if( mouseState.LeftButton == ButtonState.Released && 
                    oldMouseState.LeftButton == ButtonState.Pressed )
                {
                    var actionSlotsDisplay = this.Owner.ActionSlotsDisplay;
                    if( actionSlotsDisplay.ManuallyClick( ref mouseState, ref oldMouseState ) )
                    {
                        return;
                    }

                    if( !this.SideBar.HasButtonAt( new Point2( mouseState.X, mouseState.Y ) ) )
                    {
                        this.Player.PickedupItemContainer.Drop();
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets called when this <see cref="IngameWindow"/> is opening.
        /// </summary>
        protected override void Opening()
        {
            this.ItemInfoVisualizer.ResetCache();
        }

        /// <summary>
        /// Gets called when the PlayerEntity whose Inventory is visualized by this InventoryWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            if( this.player != null )
            {
                this.player.Statable.LevelUped -= this.OnPlayerLevelUp;
            }

            this.player = this.Player;

            if( this.player != null )
            {
                this.player.Statable.LevelUped += this.OnPlayerLevelUp;
            }

            base.OnPlayerChanged();
        }

        /// <summary>
        /// Called when the player got a levelup.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnPlayerLevelUp( object sender, System.EventArgs e )
        {
            this.ItemInfoVisualizer.ResetCache();
        }

        /// <summary>
        /// Gets the Inventory that gets visualized by this InventoryWindow.
        /// </summary>
        /// <returns>
        /// An Inventory instance.
        /// </returns>
        protected override Inventory GetInventory()
        {
            return this.player.Inventory;
        }

        /// <summary>
        /// Identifies the PlayerEntity whose Inventory is visualized using this InventoryWindow.
        /// </summary>
        private Zelda.Entities.PlayerEntity player;
    }
}