// <copyright file="SharedChestWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.SharedChestWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Items;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines the <see cref="IngameWindow"/> that is used to
    /// visualize the <see cref="SharedChest"/> of the Player.
    /// </summary>
    internal sealed class SharedChestWindow : InventoryBaseWindow
    {
        /// <summary>
        /// Initializes a new instance of the SharedChestWindow class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Provides a mechanism to visualize the cooldown on the ItemUseEffect of an item.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        public SharedChestWindow( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
            : base( cooldownVisualizer, serviceProvider )
        {
            this.viewSize = serviceProvider.ViewSize;

            // Load Content
            var spriteLoader = serviceProvider.SpriteLoader;
        }

        /// <summary>
        /// Updates the position and size of the cell grid.
        /// </summary>
        private void UpdateGridTransform()
        {
            if( this.Player == null )
                return;

            var inventory = this.GetInventory();

            if( this.currentGridSize == inventory.GridSize )
            {
                return;
            }
            else
            {
                this.currentGridSize = inventory.GridSize;

                this.Size = new Atom.Math.Vector2(
                    CellSize * inventory.GridWidth,
                    CellSize * inventory.GridHeight
                );

                this.Position = new Vector2(
                    (this.viewSize.X / 2) - (CellSize * inventory.GridWidth / 2),
                    (this.viewSize.Y / 2) - (CellSize * inventory.GridHeight / 2)
                );
            }
        }

        /// <summary>
        /// Called when this CraftingCubeWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;

            this.UpdateGridTransform();
            var batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, viewSize.X, viewSize.Y ),
                UIColors.DarkWindowBackground,
                0.0f
            );

            // Draw Title Background
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, viewSize.X, 20 ),
                UIColors.LightWindowBackground,
                0.0015f
            );

            // Draw Title
            UIFonts.TahomaBold11.Draw(
                Resources.ChestOfWinds,
                new Vector2( viewSize.X / 2.0f, 0.0f ),
                TextAlign.Center,
                Xna.Color.White,
                0.002f,
                drawContext
            );

            this.DrawCellsAndItems( drawContext );
            this.DrawPickedUpItemIndicator( 
                this.Player.PickedupItemContainer.Item,
                this.MousePosition,
                (ZeldaDrawContext)drawContext
            );
        }

        /// <summary>
        /// Gets called when the PlayerEntity whose Inventory is visualized by this InventoryBaseWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.UpdateGridTransform();
            base.OnPlayerChanged();
        }

        /// <summary>
        /// Gets the Inventory that gets visualized by this InventoryBaseWindow.
        /// </summary>
        /// <returns>
        /// An Inventory instance.
        /// </returns>
        protected override Zelda.Items.Inventory GetInventory()
        {
            return this.Player.SharedChest;
        }

        /// <summary>
        /// Handles mouse input related for this <see cref="InventoryBaseWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            if( !this.ClientArea.Contains( mouseState.X, mouseState.Y ) )
            {
                this.ItemInfoDisplay.ItemInstance = null;
            }

            base.HandleMouseInput( ref mouseState, ref oldMouseState );
        }

        /// <summary>
        /// Handles mouse input related to this <see cref="InventoryBaseWindow"/>.
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
            var chest = this.Player.SharedChest;

            if( chest.IsValidCell( cell ) )
            {
                ItemInstance itemInstance = chest.GetItemAt( cell );

                if( mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released )
                {
                    chest.HandleLeftClick( cell.X, cell.Y, this.Owner.IsShiftDown );
                }
                else if( mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released )
                {
                    chest.HandleRightClick( cell.X, cell.Y, this.Owner.IsControlDown );
                }

                RefreshItemInfoDisplay();
            }

            return false;
        }

        /// <summary>
        /// The size of the currently displayed cell grid (in cell space).
        /// </summary>
        private Point2 currentGridSize;

        /// <summary>
        /// The size of the ingame-view window.
        /// </summary>
        private readonly Point2 viewSize;
    }
}
