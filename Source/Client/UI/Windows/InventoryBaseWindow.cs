// <copyright file="InventoryBaseWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.InventoryBaseWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System.Globalization;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Items;
    using Zelda.UI.Items;
    using Xna = Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Factors out common functionality used by the <see cref="InventoryWindow"/>
    /// and the <see cref="Zelda.UI.Crafting.CraftingBottleWindow"/> class.
    /// </summary>
    internal abstract class InventoryBaseWindow : IngameWindow
    {
        #region [ Constants ]

        /// <summary>
        /// The size of a single cell.
        /// </summary>
        internal const int CellSize = UIConstants.CellSize;

        /// <summary>
        /// Specifies the color of the rectangle that is drawn behind items.
        /// </summary>
        private readonly Xna.Color ColorItemBackground = new Xna.Color( 255, 255, 255, 100 );

        /// <summary>
        /// Specifies the color of the visualization of an item's on-use-effect cooldown.
        /// </summary>
        private readonly Xna.Color ColorItemOnUseCooldown = new Xna.Color( 0, 0, 0, 154 );

        /// <summary>
        /// Specifies the color of the rectangle that fills the cell under the currently picked-up item.
        /// </summary>
        private static readonly Xna.Color ColorPickedUpItemBackground = Microsoft.Xna.Framework.Color.Yellow.WithAlpha( 110 );

        private const float CellDepth       = 0.005f;
        private const float FilledCellDepth = 0.015f;
        private const float ItemDepth       = 0.025f;
        private const float ItemCountDepth  = 0.045f; 

        #endregion

        #region [ Properties ]

        protected ItemInstance PickedUpItem
        {
            get
            {
                return this.GetInventory().Owner.PickedupItemContainer.Item;
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemInfoDisplay"/> used by this InventoryBaseWindow to display Item Information.
        /// </summary>
        protected ItemInfoDisplay ItemInfoDisplay
        {
            get 
            {
                return this.itemInfoDisplay;
            }
        }

        /// <summary>
        /// Gets the <see cref="IItemInfoVisualizer"/> used by this InventoryBaseWindow to display Item Information.
        /// </summary>
        protected IItemInfoVisualizer ItemInfoVisualizer
        {
            get
            {
                return this.itemInfoVisualizer;
            }
        }

        /// <summary>
        /// Gets the last captured position of the mouse.
        /// </summary>
        protected Point2 MousePosition
        {
            get 
            {
                return this.mousePosition;
            }
        }

        /// <summary>
        /// Gets the <see cref="IngameUserInterface"/> that owns this IngameWindow.
        /// </summary>
        public new IngameUserInterface Owner
        {
            get
            {
                return (IngameUserInterface)base.Owner;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryBaseWindow"/> class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Provides a mechanism to visualize the cooldown on the ItemUseEffect of an item.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected InventoryBaseWindow( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
        {
            this.cooldownVisualizer = cooldownVisualizer;
            
            this.itemInfoVisualizer = serviceProvider.GetService<IItemInfoVisualizer>();
            this.itemInfoDisplay = new ItemInfoDisplay( itemInfoVisualizer );

            var spriteLoader = serviceProvider.SpriteLoader;
            this.spriteCell = spriteLoader.LoadSprite( "InventoryCell" );
            this.socketRenderer = new ItemSocketRenderer( spriteLoader );
        }

        #endregion

        #region [ Methods ]
                
        protected Point2 GetMouseCellPosition( ref Microsoft.Xna.Framework.Input.MouseState mouseState )
        {
            Point2 point;
            ItemInstance pickedUpItem = this.PickedUpItem;
            
            if( pickedUpItem == null )
            {
                point = (mouseState.Position() - (Point2)this.Position) / CellSize;
            }
            else
            {
                Item item = pickedUpItem.Item;
                Point2 mousePosition = mouseState.Position();
                Vector2 real = mousePosition - this.Position;

                // Find the correct cell by scaling down.
                // And then get the drawing position by scaling up again.
                Vector2 cellReal = real / (float)CellSize - item.InventorySize / 2.0f;

                Point2 position = new Point2(
                    (int)((int)System.Math.Round( cellReal.X ) * CellSize),
                    (int)((int)System.Math.Round( cellReal.Y ) * CellSize)
                );

                point = position / CellSize;
            }
            
            return point;
        }

        /// <summary>
        /// Called when this InventoryBaseWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( IUpdateContext updateContext )
        {
            foreach( ItemInstance item in this.GetInventory().ContainedItemInstances )
            {
                item.Update( updateContext );
            }

            this.Player.Equipment.Update();
        }

        /// <summary>
        /// Draws the Inventory Cells and Items.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected void DrawCellsAndItems( ISpriteDrawContext drawContext )
        {
            Inventory inventory = this.GetInventory();
            var batch    = drawContext.Batch;
            var position = (Point2)this.Position;

            for( int cellY = 0, drawY = position.Y;
                 cellY < inventory.GridHeight;
                 ++cellY, drawY += CellSize )
            {
                for( int cellX = 0, drawX = position.X;
                     cellX < inventory.GridWidth; ++cellX,
                     drawX += CellSize )
                {
                    // First draw the empty cell:
                    this.spriteCell.Draw( new Vector2( drawX, drawY ), CellDepth, batch );
                } // for x
            } // for y

            for( int i = 0; i < inventory.ItemCount; ++i )
            {
                // Ontop of that the Item, if there is any in the cell:
                int cellX, cellY;
                ItemInstance itemInstance = inventory.GetItem( i, out cellX, out cellY );

                if( itemInstance != null )
                {
                    this.DrawItem( itemInstance, cellX, cellY, drawContext );
                }
            }
        }

        /// <summary>
        /// Draws the given ItemInstance at the given cell position.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to draw.
        /// </param>
        /// <param name="cellX">
        /// The position of the starting cell on the x-axis.
        /// </param>
        /// <param name="cellY">
        /// The position of the starting cell on the y-axis.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext
        /// </param>
        protected void DrawItem( ItemInstance itemInstance, int cellX, int cellY, ISpriteDrawContext drawContext )
        {
            var position = (Point2)this.Position;
            var item   = itemInstance.Item;
            var sprite = itemInstance.Sprite;
            var batch = drawContext.Batch;

            Vector2 drawPosition = new Vector2(
                (cellX * CellSize) + position.X,
                (cellY * CellSize) + position.Y
            );

            // Draw Rectangle under the item.
            batch.DrawRect(
                new Rectangle(
                    (int)drawPosition.X,
                    (int)drawPosition.Y,
                    item.InventoryWidth * CellSize,
                    item.InventoryHeight * CellSize
                ),
                ColorItemBackground,
                FilledCellDepth
            );

            if( sprite != null )
            {
                // Draw the item's sprite centered:
                Vector2 itemDrawPosition = new Vector2(
                    drawPosition.X + (item.InventoryWidth * CellSize / 2) - (sprite.Width / 2),
                    drawPosition.Y + (item.InventoryHeight * CellSize / 2) - (sprite.Height / 2)
                );

                float actualItemDepth = ItemDepth - (drawPosition.X / this.Width / 10000.0f) + (drawPosition.Y / this.Height / 10000.0f);
                sprite.Draw( itemDrawPosition, item.SpriteColor, actualItemDepth, batch );

                this.socketRenderer.DrawSockets( itemInstance, drawPosition, actualItemDepth, batch );
                this.DrawItemCount( itemInstance, drawPosition, drawContext );
            }

            this.DrawUseCooldown( item, drawPosition );
        }

        /// <summary>
        /// Draws the number of items that are on the same stack.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance whose Count should be visualized.
        /// </param>
        /// <param name="drawPositon">
        /// The original drawing position.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawItemCount( ItemInstance itemInstance, Vector2 drawPositon, ISpriteDrawContext drawContext )
        {
            if( itemInstance.Count <= 1 )
                return;

            string strItemCount = itemInstance.Count.ToString( CultureInfo.CurrentCulture );
            Vector2 strSize = fontItemCount.MeasureString( strItemCount );
            Item item = itemInstance.Item;

            var itemCountDrawPosition = new Vector2(
                drawPositon.X + (item.InventoryWidth * CellSize) - strSize.X - 2,
                drawPositon.Y + (item.InventoryHeight * CellSize) - 11
            );

            this.fontItemCount.Draw(
                strItemCount,
                itemCountDrawPosition,
                Xna.Color.White,
                ItemCountDepth,
                drawContext
            );
        }

        /// <summary>
        /// Draws the cooldown of the UseEffect of the given Item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <param name="drawPosition">The drawing position of the currently processed item.</param>
        protected void DrawUseCooldown( Item item, Vector2 drawPosition )
        {
            if( item.UseEffect != null )
            {
                var cooldown = item.UseEffect.Cooldown;

                if( cooldown != null && !cooldown.IsReady )
                {
                    this.cooldownVisualizer.PushCooldown(
                        cooldown,
                        drawPosition,
                        new Vector2( item.InventoryWidth * CellSize, item.InventoryHeight * CellSize ),
                        ColorItemOnUseCooldown,
                        false
                    );
                }
            }
        }

        /// <summary>
        /// Draws a rectangle below the given item to indicate
        /// where the item would go into the inventory.
        /// </summary>
        /// <param name="pickedUpItem">
        /// The currently picked up item.
        /// </param>
        /// <param name="mousePosition">
        /// The current position of the mouse.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        protected void DrawPickedUpItemIndicator( 
            ItemInstance     pickedUpItem, 
            Vector2          mousePosition,
            ZeldaDrawContext drawContext )
        {
            if( pickedUpItem == null )
                return;

            var item = pickedUpItem.Item;

            if( !this.ClientArea.Contains( mousePosition ) )
                return;

            Vector2 real = mousePosition - this.Position;

            // Find the correct cell by scaling down.
            // And then get the drawing position by scaling up again.
            Vector2 cellReal = real / (float)CellSize - item.InventorySize / 2.0f;
          
            Point2 position = new Point2(
                (int)((int)System.Math.Round(cellReal.X) * CellSize + Position.X),
                (int)((int)System.Math.Round(cellReal.Y) * CellSize + Position.Y)
            );
            
            var rectangle = new Rectangle(
                position.X,
                position.Y,
                item.InventoryWidth * CellSize,
                item.InventoryHeight * CellSize
            );

            if( !this.ClientArea.Contains( rectangle ) )
                return;

            drawContext.Batch.DrawRect(
                rectangle,
                ColorPickedUpItemBackground,
                0.05f
            );
        }

        /// <summary>
        /// Gets a value indicating whether the Item Information should be shown for the given ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance that is supposed to be shown.
        /// </param>
        /// <returns>
        /// Returns true if the information about the ItemInstance should be shown;
        /// otherwise false.
        /// </returns>
        protected bool ShouldShowItemInfoFor( ItemInstance itemInstance )
        {
            if( itemInstance == null )
                return false;

            if( this.Player.PickedupItemContainer.Item is GemInstance )
            {
                var equimentInstance = itemInstance as EquipmentInstance;

                if( equimentInstance != null )
                {
                    var socketProperties = equimentInstance.SocketProperties;
                    return socketProperties.EmptySocketCount == 0;
                }
            }

            return true;
        }

        protected void RefreshItemInfoDisplay()
        {
            var mouseState = this.Owner.MouseState;
            Point2 cell = GetMouseCellPosition( ref mouseState );

            Inventory inventory = GetInventory();
            if( inventory.IsValidCell( cell ) )
            {
                ItemInstance itemInstance = inventory.GetItemAt( cell );

                if( this.ShouldShowItemInfoFor( itemInstance ) )
                {
                    this.ItemInfoDisplay.ItemInstance = GetMouseOverItemOrEquippedItemForComparison( itemInstance, this.Player, this.Owner );
                }
                else
                {
                    this.ItemInfoDisplay.ItemInstance = null;
                }
            }
        }
        
        /// <summary>
        /// Gets the given item or the equipped item that fits into the same slot as the given item if the user has pressed the right ALT/CTRL modifier keys for item comparison.
        /// </summary>
        /// <param name="itemInstance">
        /// The item instance that the user has mouse-hovered.
        /// </param>
        /// <param name="player">
        /// The PlayerEntity.
        /// </param>
        /// <param name="userInterface">
        /// The UI object.
        /// </param>
        /// <returns>
        /// The item to show to the user.
        /// </returns>
        public static ItemInstance GetMouseOverItemOrEquippedItemForComparison( ItemInstance itemInstance, Zelda.Entities.PlayerEntity player, Atom.Xna.UI.UserInterface userInterface )
        {
            if( userInterface.IsAltDown )
            {
                EquipmentInstance equipmentInstance = itemInstance as EquipmentInstance;

                if( equipmentInstance != null )
                {
                    EquipmentStatusSlot slot = player.Equipment.GetSlotForItem( equipmentInstance.Equipment.Slot, userInterface.IsControlDown );
                    EquipmentInstance equippedInstance = player.Equipment.Get( slot );

                    if( equippedInstance != null )
                    {
                        itemInstance = equippedInstance;
                    }
                }
            }

            return itemInstance;
        }

        public static bool ShouldRefreshItemInfoDisplayBasedOnKeyInput( Atom.Xna.UI.UserInterface userInterface )
        {
            return userInterface.IsAltDown != userInterface.WasAltDown || userInterface.IsControlDown != userInterface.WasControlDown;
        }

        protected override void HandleKeyInput( ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            if( ShouldRefreshItemInfoDisplayBasedOnKeyInput( this.Owner ) )
            {
                RefreshItemInfoDisplay();
            }

            base.HandleKeyInput( ref keyState, ref oldKeyState );
        }

        /// <summary>
        /// Handles mouse input related for this <see cref="InventoryWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.mousePosition = new Point2( mouseState.X, mouseState.Y );
            this.ItemInfoDisplay.Position = this.mousePosition;
        }
        
        /// <summary>
        /// Gets the Inventory that gets visualized by this InventoryBaseWindow.
        /// </summary>
        /// <returns>
        /// An Inventory instance.
        /// </returns>
        protected abstract Inventory GetInventory();

        /// <summary>
        /// Gets called when the PlayerEntity whose Inventory is visualized by this InventoryBaseWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.itemInfoDisplay.Player = this.Player;
        }

        #region > State & Events <

        /// <summary>
        /// Gets called when the IsEnabled state of this CraftingBottleWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.itemInfoDisplay.IsEnabled = this.IsEnabled;
        }

        /// <summary>
        /// Gets called when the IsVisible state of this CraftingBottleWindow has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            this.itemInfoDisplay.IsVisible = this.IsVisible;
        }

        /// <summary>
        /// Gets called when this InventoryWindow has been added to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.itemInfoDisplay );
        }

        /// <summary>
        /// Gets called when this InventoryWindow has been removed from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.itemInfoDisplay );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Captures tThe mouse's current position.
        /// </summary>
        private Point2 mousePosition;

        /// <summary>
        /// The ItemInfoDisplay that is responsible for drawing the Item Information
        /// when the user mouse-overs an item.
        /// </summary>
        private readonly ItemInfoDisplay itemInfoDisplay;

        /// <summary>
        /// Provides a mechanism for visualizing an item.
        /// </summary>
        private readonly IItemInfoVisualizer itemInfoVisualizer;

        /// <summary>
        /// The sprite that is used to visualize a single cell of the inventory's grid.
        /// </summary>
        private readonly Sprite spriteCell;

        /// <summary>
        /// The IFont that is used when drawing the Item Count.
        /// </summary>
        private readonly IFont fontItemCount = UIFonts.Tahoma7;
        
        /// <summary>
        /// Provides a mechanism to visualize the cooldown on the ItemUseEffect of an item.
        /// </summary>
        private readonly CooldownVisualizer cooldownVisualizer;

        /// <summary>
        /// Responsible for rendering the sockets and gems of an EquipmentInstance.
        /// </summary>
        private readonly ItemSocketRenderer socketRenderer;

        #endregion
    }
}