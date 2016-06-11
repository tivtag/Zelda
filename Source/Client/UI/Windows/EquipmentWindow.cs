// <copyright file="EquipmentWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.EquipmentWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Items;
    using Zelda.UI.Items;
    using XnaF = Microsoft.Xna.Framework;

    /// <summary>
    /// The <see cref="EquipmentWindow"/> is used to visualize the
    /// player's equipment status information.
    /// </summary>
    internal sealed class EquipmentWindow : IngameWindow
    {
        #region [ Constants ]

        /// <summary>
        /// The color of the rectangle that is drawn above items
        /// whose requirements are currently not fullfilled.
        /// </summary>
        private static readonly XnaF.Color ColorRequirementsNotFullfilled = new XnaF.Color( 255, 0, 0, 150 );

        #endregion

        #region [ Initialization ]
                
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal EquipmentWindow( IZeldaServiceProvider serviceProvider )
        {
            this.Size = serviceProvider.ViewSize;
            this.itemInfoDisplay = new ItemInfoDisplay( serviceProvider.GetService<IItemInfoVisualizer>() );
            
            var content  = serviceProvider.Content;
            int middleX = (int)this.Size.X / 2;
            int middleY = (int)(this.Size.Y / 2) - 39;

            this.fields = new EquipmentField[20];

            var spriteLoader = serviceProvider.SpriteLoader;

            int i = -1;            
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Head, middleX - 12, middleY - 64 + 22, spriteLoader.LoadSprite( "EquipmentCell_Head" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Cloak, middleX - 12, middleY - 38 + 22, spriteLoader.LoadSprite( "EquipmentCell_Cloak" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Chest,  middleX - 12, middleY - 12 + 22, spriteLoader.LoadSprite( "EquipmentCell_Chest" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Belt, middleX - 12, middleY + 16 + 22, spriteLoader.LoadSprite( "EquipmentCell_Belt" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Boots, middleX - 12, middleY + 42 + 22, spriteLoader.LoadSprite( "EquipmentCell_Boots" ) );

            fields[++i]  = new EquipmentField( EquipmentStatusSlot.Necklace1, middleX - 12 - 18 - 6, middleY - 16, spriteLoader.LoadSprite( "EquipmentCell_Necklace" ) );
            fields[++i]  = new EquipmentField( EquipmentStatusSlot.Necklace2,  middleX + 12 + 6,      middleY - 16,      spriteLoader.LoadSprite( "EquipmentCell_Necklace" ) );
            fields[++i]  = new EquipmentField( EquipmentStatusSlot.Ring1, middleX - 12 - 18 - 6, middleY + 10, spriteLoader.LoadSprite( "EquipmentCell_Ring" ) );
            fields[++i]  = new EquipmentField( EquipmentStatusSlot.Ring2, middleX + 12 + 6, middleY + 10, spriteLoader.LoadSprite( "EquipmentCell_Ring" ) );
            fields[++i]  = new EquipmentField( EquipmentStatusSlot.Trinket1, middleX - 12 - 18 - 6, middleY + 38, spriteLoader.LoadSprite( "EquipmentCell_Trinket" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Trinket2, middleX + 12 + 6, middleY + 38, spriteLoader.LoadSprite( "EquipmentCell_Trinket" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Relic1, middleX - 12 - 18 - 6, middleY + 64, spriteLoader.LoadSprite( "EquipmentCell_Relic" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Relic2, middleX + 12 + 6, middleY + 64, spriteLoader.LoadSprite( "EquipmentCell_Relic" ) );

            int weaponRowY = middleY + 84 + 22;
            fields[++i] = new EquipmentField( EquipmentStatusSlot.WeaponHand, middleX - 38 + (26 * -1), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_WeaponHand" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.ShieldHand, middleX - 38 + (26 * 0), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_ShieldHand" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Gloves, middleX - 38 + (26 * 1), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_Gloves" ) );

            fields[++i] = new EquipmentField( EquipmentStatusSlot.Ranged, middleX - 38 + (26 * 2), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_Ranged" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Staff, middleX - 38 + (26 * 3), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_Staff" ) );

            fields[++i] = new EquipmentField( EquipmentStatusSlot.Bag1, middleX - 38 + (26 * 5), weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_Bag" ) );
            fields[++i] = new EquipmentField( EquipmentStatusSlot.Bag2, middleX - 38 + (26 * 5) + 20, weaponRowY, spriteLoader.LoadSprite( "EquipmentCell_Bag" ) );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="EquipmentWindow"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect( 
                this.ClientArea,
                UIColors.LightWindowBackground,
                0.001f
            );
            
            // Draw Quest Log String Background
            batch.DrawRect(
                new XnaF.Rectangle( 0, 0, (int)this.Width, 20 ),
                UIColors.WindowTitleBackground,
                0.0015f
            );

            // Draw Quest Log String
            this.fontTitle.Draw( 
                Zelda.Resources.EquipmentStatus,
                new Vector2( this.Width / 2.0f, 0.0f ),
                TextAlign.Center,
                XnaF.Color.White,
                0.002f,
                drawContext
            );

            if( this.Player == null )
                return;

            var equipment = this.Player.Equipment;

            // Draw the individual equipment slots/fields.
            foreach( var field in this.fields )
            {
                field.Draw( equipment, zeldaDrawContext );
            }

            // Draw information about item the player currently has mouse-hovered.
            if( fieldMouseOver != null )
            {
                this.itemInfoDisplay.ItemInstance = equipment.Get( fieldMouseOver.Slot );
                this.itemInfoDisplay.Position = mousePosition;
            }
            else
            {
                this.itemInfoDisplay.ItemInstance = null;
            }
        }

        /// <summary>
        /// Called when this <see cref="EquipmentWindow"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            var equipment = this.Player.Equipment;

            foreach( var field in this.fields )
            {
                var instance = equipment.Get( field.Slot );

                if( instance != null )
                {
                    var updateable = instance.Sprite as IUpdateable;

                    if( updateable != null )
                    {
                        updateable.Update( updateContext );
                    }
                }
            }

            equipment.Update();
        }

        /// <summary>
        /// Handles mouse input for this EquipmentWindow.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            this.fieldMouseOver = null;
            this.mousePosition  = new Point2( mouseState.X, mouseState.Y );

            foreach( var field in this.fields )
            {
                if( field.IsInside( mousePosition.X, mousePosition.Y ) )
                {
                    this.fieldMouseOver = field;
                    break;
                }
            }

            if( mouseState.LeftButton == ButtonState.Pressed && 
                oldMouseState.LeftButton == ButtonState.Released )
            {
                this.HandleLeftClickOnField( fieldMouseOver );
            }
        }

        /// <summary>
        /// Called when a simple left click occurs on the specified <see cref="EquipmentField"/>.
        /// </summary>
        /// <param name="field">The field that was clicked on, if any.</param>
        private void HandleLeftClickOnField( EquipmentField field )
        {
            if( field == null || this.Player.IsDead )
                return;

            var equipment     = this.Player.Equipment;
            var itemContainer = this.Player.PickedupItemContainer;

            ItemInstance      itemInstance       = itemContainer.Item;
            EquipmentInstance fieldEquipInstance = equipment.Get( field.Slot );

            // case a)
            // No picked up item, but an item in the field.
            // --> Unequip item in the field and pick it up.
            if( itemInstance == null && fieldEquipInstance != null )
            {
                var unequipedItem = equipment.Unequip( field.Slot );

                ItemSounds.PlayPickUp( unequipedItem.Item );
                itemContainer.Pick( unequipedItem );
                return;
            }

            // case b)
            // gem picked up and socket-able equipment in field.
            var gemInstance = itemInstance as GemInstance;

            if( gemInstance != null )
            {
            }
            else
            {
                var equipInstance = itemInstance as EquipmentInstance;
                if( equipInstance == null )
                    return; // Non Equipment can't be equiped.
                
                // case c)
                // Has picked up equipment, but no item in the field.
                //     --> Equip the equipment on the hand.
                // and case d)
                // Has picked up equipment, and an item in the field.
                //     --> Swap the equipments.
                EquipmentInstance old = null;
                if( equipment.Equip( equipInstance, field.Slot, out old ) )
                {
                    System.Diagnostics.Debug.Assert( old == fieldEquipInstance );

                    ItemSounds.PlayPutDown( equipInstance.Item );
                    itemContainer.Pick( old );
                    return;
                }
            }
        }

        /// <summary>
        /// Called when the PlayerEntity that owns this IngameWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.itemInfoDisplay.Player = this.Player;
        }

        #region > Events <

        /// <summary>
        /// Gets called when the IsEnabled state of this InventoryWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.itemInfoDisplay.IsEnabled = this.IsEnabled;
        }

        /// <summary>
        /// Gets called when the IsVisible state of this InventoryWindow has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            this.itemInfoDisplay.IsVisible = this.IsVisible;
        }

        #endregion

        #region > State <
        
        /// <summary>
        /// Adds the child elements of this IngameWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.itemInfoDisplay );
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.itemInfoDisplay );
        }

        #endregion

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// The list of <see cref="EquipmentField"/>s.
        /// </summary>
        private readonly EquipmentField[] fields;

        /// <summary>
        /// Stores the <see cref="EquipmentField"/> the player has his mouse
        /// currently over, if any.
        /// </summary>
        private EquipmentField fieldMouseOver;

        /// <summary>
        /// The position of the mouse.
        /// </summary>
        private Point2 mousePosition;

        /// <summary>
        /// Used to visualize the information about an item.
        /// </summary>
        private readonly ItemInfoDisplay itemInfoDisplay;

        /// <summary>
        /// The IFonts used in the Equipment Window.
        /// </summary>
        private readonly IFont fontTitle = UIFonts.TahomaBold11;

        #endregion

        #region [ class EquipmentField ]

        /// <summary>
        /// The EquipmentField stores the information needed to draw
        /// a single EquipmentStatusSlot.
        /// </summary>
        private sealed class EquipmentField
        {
            /// <summary>
            /// The EquipmentStatusSlot this EquipmentField is related to.
            /// </summary>
            public readonly EquipmentStatusSlot Slot;

            /// <summary>
            /// The area this EquipmentField covers.
            /// </summary>
            private readonly Rectangle area;

            /// <summary>
            /// The sprite of this EquipmentField.
            /// </summary>
            private readonly Sprite slotSprite;

            /// <summary>
            /// Initializes a new instance of the <see cref="EquipmentField"/> class.
            /// </summary>
            /// <param name="slot">
            /// The EquipmentStatusSlot the new EquipmentField is related to.
            /// </param>
            /// <param name="x">The position of the new EquipmentField on the x-axis.</param>
            /// <param name="y">The position of the new EquipmentField on the y-axis.</param>
            /// <param name="sprite">The background sprite of the new EquipmentField.</param>
            public EquipmentField( 
                EquipmentStatusSlot slot,
                int x,
                int y,
                Sprite sprite )
            {
                this.Slot         = slot;
                this.slotSprite   = sprite;
                this.area         = new Rectangle( x, y, slotSprite.Width, slotSprite.Height );
            }

            /// <summary>
            /// Draws the <see cref="EquipmentField"/>.
            /// </summary>
            /// <param name="equipment">
            /// The related EquipmentStatus object.
            /// </param>
            /// <param name="drawContext">
            /// The current ZeldaDrawContext.
            /// </param>
            public void Draw( EquipmentStatus equipment, ZeldaDrawContext drawContext )
            {
                var batch = drawContext.Batch;
                slotSprite.Draw( new Point2( area.X, area.Y ), 0.7f, batch );

                EquipmentInstance equipInstance = equipment.Get( Slot );

                if( equipInstance != null )
                {
                    Equipment equip  = equipInstance.Equipment;
                    ISprite sprite = equipInstance.Sprite;

                    if( sprite != null )
                    {
                        // Tell the user if the requirments are not fulfilled for that slot:
                        if( equipment.IsRequirementFulfilled( this.Slot ) == false )
                        {
                            drawContext.Batch.DrawRect(
                                area,
                                ColorRequirementsNotFullfilled,
                                0.79f
                            );
                        }

                        var drawPosition = new Vector2( 
                            area.X + (slotSprite.Width / 2) - (sprite.Width / 2),
                            area.Y + (slotSprite.Height / 2) - (sprite.Height / 2)
                        );

                        // Draw the item's sprite centered:
                        sprite.Draw(
                            drawPosition,
                            equip.SpriteColor,
                            0.8f,
                            batch
                        );
                    }
                }
            }

            /// <summary>
            /// Gets whether the specified points is inside of the <see cref="EquipmentField"/>.
            /// </summary>
            /// <param name="x">The position of the point on the x-axis.</param>
            /// <param name="y">The position of the point on the y-axis.</param>
            /// <returns>
            /// true if the given point is inside this EquipmentField's area;
            /// otherwise false.
            /// </returns>
            public bool IsInside( int x, int y )
            {
                return this.area.Contains( x, y );
            }
        }

        #endregion
    }
}