// <copyright file="ItemTooltipArea.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemTooltipArea class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI.Tooltips;
    using Zelda.Items;

    /// <summary>
    /// 
    /// </summary>
    internal class ItemTooltip : Tooltip
    {
        /// <summary>
        /// Gets or sets the Item that is shown in this ItemTooltip.
        /// </summary>
        public Item Item
        {
            get
            {
                return this.item;
            }

            set
            {
                this.item = value;
                this.itemInstance = null;

                this.RefreshClientArea();
            }
        }

        /// <summary>
        /// Gets the ItemInstance that is shown by this ItemTooltip.
        /// </summary>
        public ItemInstance ItemInstance
        {
            get
            {
                if( this.itemInstance == null && this.item != null )
                {
                    this.itemInstance = this.item.CreateInstance();
                    this.itemSprite = this.itemInstance.Sprite;
                }
                
                return this.itemInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemTooltip class.
        /// </summary>
        /// <param name="tooltipDrawElement">
        /// The IooltipDrawElement responsible for drawing the actual item information
        /// when player moves the mouse over this ItemTooltip.
        /// </param>
        public ItemTooltip( ItemTooltipDrawElement tooltipDrawElement )
            : base( tooltipDrawElement )
        {
            this.HideAndDisable();
        }

        /// <summary>
        /// Refreshes the area the tooltip takes up.
        /// </summary>
        private void RefreshClientArea()
        {
            if( this.item != null )
            {
                var size = new Vector2( this.item.Sprite.Width, this.item.Sprite.Height );
                this.SetTransform( this.Position, -(size / 2.0f), size );
            }
            else
            {
                this.SetTransform( this.Position, Vector2.Zero, Vector2.Zero );
            }
        }

        /// <summary>
        /// Draws the item's sprite at the center of this ItemTooltip.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        public void DrawItemSprite( ISpriteDrawContext drawContext )
        {
            if( this.ItemInstance != null )
            {
                Vector2 position = new Vector2(
                    this.X - (this.itemSprite.Width / 2),
                    this.Y - (this.itemSprite.Height / 2)
                );

                position.X = (int)position.X;
                position.Y = (int)position.Y;
                this.itemSprite.Draw( position, this.Item.SpriteColor, this.RelativeDrawOrder, drawContext.Batch );
            }
        }       
        
        /// <summary>
        /// Called when this ItemTooltip is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( IUpdateContext updateContext )
        {
            IUpdateable updateable = this.itemSprite as IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        /// <summary>
        /// Represents the storage field of Item property.
        /// </summary>
        private Item item;
        
        /// <summary>
        /// Represents the instance of the Item that has been created to draw this ItemTooltip.
        /// </summary>
        private ItemInstance itemInstance;

        /// <summary>
        /// Represents the sprite of the item.
        /// </summary>
        private ISprite itemSprite;
    }
}
