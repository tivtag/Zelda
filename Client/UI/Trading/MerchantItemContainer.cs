// <copyright file="MerchantItemContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.UI.Trading.MerchantItemContainer class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Trading
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Zelda.Items;
    using Zelda.Trading;
    
    /// <summary>
    /// Represents the place in which the item of a MerchantItemElement is actually
    /// stored.
    /// </summary>
    internal sealed class MerchantItemContainer : UIElement
    {
        /// <summary>
        /// Gets or sets the MerchantItem this MerchantItemContainer contains.
        /// </summary>
        public MerchantItem MerchantItem
        {
            get
            {
                return this.merchantItem;
            }

            set
            {
                this.merchantItem = value;
                this.InitializeForItem();
            }
        }

        /// <summary>
        /// Initializes this MerchantItemContainer for the MerchantItem.
        /// </summary>
        private void InitializeForItem()
        {
            if( this.merchantItem == null )
            {
                this.Size = Vector2.Zero;
            }
            else
            {
                this.Size = new Vector2(
                    this.merchantItem.Item.InventoryWidth * spriteCell.Width,
                    this.merchantItem.Item.InventoryHeight * spriteCell.Height
                );
            }
        }

        /// <summary>
        /// Initializes a new instance of the MerchantItemContainer class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public MerchantItemContainer( IZeldaServiceProvider serviceProvider )
        {
            this.FloorNumber = 4;

            this.spriteCell = serviceProvider.SpriteLoader.LoadSprite( "InventoryCell" );
        }

        /// <summary>
        /// Called when this <see cref="MerchantItemContainer"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            this.DrawCells( drawContext );
            this.DrawItemSprite( drawContext );
        }

        /// <summary>
        /// Draws the cells of this MerchantItemContainer.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawCells( ISpriteDrawContext drawContext )
        {
            Item item = this.merchantItem.Item;
            Point2 size = item.InventorySize;

            for( int cellX = 0; cellX < size.X; ++cellX )
            {
                for( int cellY = 0; cellY < size.Y; ++cellY )
                {
                    var cellPosition = new Vector2(
                        this.Position.X + (cellX * this.spriteCell.Width),
                        this.Position.Y + (cellY * this.spriteCell.Height)
                    );

                    this.spriteCell.Draw( cellPosition, drawContext.Batch );
                }
            }
        }

        /// <summary>
        /// Draws the sprite of the MerchantItem.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawItemSprite( ISpriteDrawContext drawContext )
        {
            var itemInstance = this.merchantItem.ItemInstance;
            if( itemInstance == null )
                return;

            Item item = itemInstance.Item;
            var sprite = itemInstance.Sprite;

            var spritePosition = this.Position + (this.Size / 2.0f) - (sprite.Size / 2);
            sprite.Draw( spritePosition, item.SpriteColor, 0.1f, drawContext.Batch );
        }

        /// <summary>
        /// Called when this <see cref="MerchantItemContainer"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            ItemInstance itemInstance = this.merchantItem.ItemInstance;
            if( itemInstance == null )
                return;
            
            itemInstance.Update( updateContext );        
        }

        /// <summary>
        /// The MerchantItem this MerchantItemContainer contains. 
        /// </summary>
        private MerchantItem merchantItem;

        /// <summary>
        /// The sprite that is used to visualize a single InventoryCell.
        /// </summary>
        private readonly Sprite spriteCell;
    }
}
