// <copyright file="PickedupItemDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.PickedupItemDisplay class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System.Globalization;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Items;

    /// <summary>
    /// Enables the visualization of the item the PlayerEntity has currently picked up.
    /// </summary>
    internal sealed class PickedupItemDisplay : UIElement
    {
        /// <summary>
        /// Gets or sets the PlayerEntity whos currently PickedUpItem (if any)
        /// is visualized by the <see cref="PickedupItemDisplay"/>.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player 
        {
            get; 
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PickedupItemDisplay"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal PickedupItemDisplay( IZeldaServiceProvider serviceProvider )
        {
            this.PassInputToSubElements = true;
            
            // Draw above all other UIElements,
            // such as the IngameWindows, the HeartBar, etc...
            this.FloorNumber = 6;
        }
        
        /// <summary>
        /// Called when this <see cref="PickedupItemDisplay"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;

            ItemInstance itemInstance = this.Player.PickedupItemContainer.Item;
            if( itemInstance == null )
                return;

            Item item = itemInstance.Item;
            ISprite sprite = itemInstance.Sprite;

            if( sprite == null )
                return;

            int width = item.InventoryWidth * InventoryWindow.CellSize;
            int height = item.InventoryHeight * InventoryWindow.CellSize;
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            
            // Draw Item Sprite 
            Vector2 cellPosition = new Vector2(
                this.X - halfWidth,
                this.Y - halfHeight
            );

            Vector2 spritePosition = new Vector2(
                cellPosition.X + halfWidth - sprite.Width / 2,
                cellPosition.Y + halfHeight - sprite.Height / 2
            );

            sprite.Draw( spritePosition, item.SpriteColor, drawContext.Batch );

            // Draw Item Count
            if( itemInstance.Count > 1 )
            {
                string countString = itemInstance.Count.ToString( CultureInfo.CurrentCulture );
                var countStringSize = fontItemCount.MeasureString( countString );

                var countDrawPosition = new Vector2(
                    cellPosition.X + width - countStringSize.X - 2,
                    cellPosition.Y + height - 11
                );

                this.fontItemCount.Draw(
                    countString,
                    countDrawPosition,
                    Microsoft.Xna.Framework.Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    1.0f,
                    drawContext
                );
            }
        }

        /// <summary>
        /// Called when this <see cref="PickedupItemDisplay"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            if( this.Player == null )
                return;

            var item = this.Player.PickedupItemContainer.Item;
            if( item == null )
                return;

            var updateable = item.Sprite as IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        /// <summary>
        /// Gets called once per frame to handle mouse input.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState, 
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            this.Position = new Vector2( mouseState.X, mouseState.Y );
        }
        
        /// <summary>
        /// The IFont that is used when drawing the Item Count.
        /// </summary>
        private readonly IFont fontItemCount = UIFonts.Tahoma7;
    }
}
