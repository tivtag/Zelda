// <copyright file="QuickActionSlotsDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.QuickActionSlotsDisplay class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Zelda.QuickActions;

    /// <summary>
    /// Visualizes and encapsulates all Quick Action Slots of the Player.
    /// </summary>
    internal sealed class QuickActionSlotsDisplay : UIContainerElement
    {
        #region [ Properties ]

        /// <summary>
        /// Sets the PlayerEntity whose QuickActionSlots are visualized by this QuickActionSlotsDisplay.
        /// </summary>
        public PlayerEntity Player
        {
            //get 
            //{ 
            //    return this.player; 
            //}
            // ^ is unused.
            set
            {
                this.player = value;
                this.SetupSlots( value );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the QuickActionSlotsDisplay class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Allows the visualization of <see cref="Cooldown"/> data.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public QuickActionSlotsDisplay( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
            : base( QuickActionSlotList.Size )
        {
            this.slots = new QuickActionSlotElement[QuickActionSlotList.Size];
            this.slotDrawer = new QuickActionSlotDrawer( cooldownVisualizer, serviceProvider );

            this.pickedUp = new PickedupQuickActionDisplay( this.slotDrawer );
            this.AddChild( this.pickedUp );

            this.CreateSlots( serviceProvider );
            this.ShowAndEnable();
        }

        /// <summary>
        /// Creates the QuickActionSlotElements of this QuickActionSlotsDisplay.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void CreateSlots( IZeldaServiceProvider serviceProvider )
        {
            Point2 viewSize = serviceProvider.ViewSize;

            for( int index = 0; index < this.slots.Length; ++index )
            {
                var slot = this.CreateSlot( index, viewSize );

                this.slots[index] = slot;
                this.AddChild( slot );
            }
        }

        /// <summary>
        /// Creates a single QuickActionSlotElement.
        /// </summary>
        /// <param name="index">
        /// The index of the slot to create.
        /// </param>
        /// <param name="viewSize">
        /// The view size of the game.
        /// </param>
        /// <returns>
        /// A newly created QuickActionSlotElement.
        /// </returns>
        private QuickActionSlotElement CreateSlot( int index, Point2 viewSize )
        {
            var slot = new QuickActionSlotElement( this.slotDrawer ) {
                Position = GetSlotPosition( index, viewSize )
            };

            slot.Clicked += this.OnQuickSlotClicked;

            return slot;
        }

        /// <summary>
        /// Gets the position of a new QuickActionSlotElement.
        /// </summary>
        /// <param name="index">
        /// The index of the slot.
        /// </param>
        /// <param name="viewSize">
        /// The view size of the game.
        /// </param>
        /// <returns>
        /// The requested position.
        /// </returns>
        private static Atom.Math.Vector2 GetSlotPosition( int index, Atom.Math.Point2 viewSize )
        {
            if( index >= (QuickActionSlotList.Size / 2) )
            {
                index -= (QuickActionSlotList.Size / 2);
                return new Atom.Math.Vector2(
                    (22 * index) + 5,
                    viewSize.Y - (19 * 2) - 6
                );
            }
            else
            {
                return new Atom.Math.Vector2(
                    (22 * index) + 5,
                    viewSize.Y - 19 - 5
                );
            }
        }

        /// <summary>
        /// Setups the QuickActionSlotElements of this QuickActionSlotsDisplay to visualize the slots
        /// of the given PlayerEntity.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        private void SetupSlots( PlayerEntity player )
        {
            if( player == null )
                return;

            for( int index = 0; index < slots.Length; ++index )
            {
                this.slots[index].Slot = player.QuickActionSlots.GetSlotAt( index );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Restores the currently picked-up Quick Action.
        /// </summary>
        public void RestorePickedUp()
        {
            if( player != null )
            {
                pickedUp.Restore( player.QuickActionSlots );
            }
        }

        /// <summary>
        /// Called when this QuickActionSlotsDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( this.ShouldDrawEmptySlots() )
            {
                this.DrawEmptySlots( (ZeldaDrawContext)drawContext );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the empty slots should be drawn.
        /// </summary>
        /// <returns></returns>
        private bool ShouldDrawEmptySlots()
        {
            return this.pickedUp.Action != null || this.IsAllowedOrNoWindowOpenAndHasPickedUpUseableItem();
        }

        /// <summary>
        /// Gets a value indicating whether no ne of the allowed IngameWindow is 
        /// currently open and that the player currently has picked-up an useable item.
        /// </summary>
        /// <returns></returns>
        private bool IsAllowedOrNoWindowOpenAndHasPickedUpUseableItem()
        {
            return this.IsAllowedOrNoWindowOpen() && this.HasPickedUpUseableItem();
        }

        /// <summary>
        /// Gets a value indicating whether no or one of the allowed IngameWindow is currently open.
        /// </summary>
        /// <returns></returns>
        private bool IsAllowedOrNoWindowOpen()
        {
            var userInterface = (IngameUserInterface)this.Owner;
            return userInterface.OpenWindow == null || (userInterface.OpenWindow is InventoryWindow);
        }

        /// <summary>
        /// Gets a value indicating whether the player currently has picked-up an useable item.
        /// </summary>
        /// <returns></returns>
        private bool HasPickedUpUseableItem()
        {
            var container = player.PickedupItemContainer;
            var item = container.Item;

            return item != null && item.Item.UseEffect != null;
        }

        /// <summary>
        /// Draws an indicator for each slot that has no action.
        /// </summary>
        /// <param name="zeldaDrawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawEmptySlots( ZeldaDrawContext zeldaDrawContext )
        {
            var batch = zeldaDrawContext.Batch;
            var colorEmptySlot = new Microsoft.Xna.Framework.Color( 255, 255, 255, 144 );
            var colorEmptySlotMouseOver = new Microsoft.Xna.Framework.Color( 255, 255, 255, 200 );
            var colorTextEmpty = new Microsoft.Xna.Framework.Color( 255, 255, 255, 144 );

            foreach( var slot in this.slots )
            {
                if( slot.QuickAction == null )
                {
                    batch.DrawRect(
                        slot.ClientArea,
                        slot.ClientArea.Contains( this.Owner.MousePosition ) ? colorEmptySlotMouseOver : colorEmptySlot,
                        0.1f
                    );

                    if( slot.Slot.Key != Keys.None )
                    {
                        if( slot.Slot.TopRow )
                        {
                            UIFonts.Tahoma7.Draw( "mod", slot.ClientArea.Center - new Vector2( -1, 9 ), Atom.Xna.Fonts.TextAlign.Center, colorTextEmpty, zeldaDrawContext );
                        }

                        UIFonts.Tahoma7.Draw( slot.Slot.Key.ToString(), slot.ClientArea.Center + new Vector2( 1.0f, 0.0f ), Atom.Xna.Fonts.TextAlign.Center, colorTextEmpty, zeldaDrawContext );
                    }
                }
            }
        }

        /// <summary>
        /// Called when this QuickActionSlotsDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            if( player != null )
            {
                this.player.QuickActionSlots.Update( (ZeldaUpdateContext)updateContext );
            }
        }

        /// <summary>
        /// Manually handles clicking on any of the QuickActionSlots.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        /// <returns>
        /// Returns true if any of the QuickActionSlots was clicked;
        /// otherwise false.
        /// </returns>
        public bool ManuallyClick( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            foreach( var slot in this.slots )
            {
                if( slot.ClientArea.Contains( mouseState.X, mouseState.Y ) )
                {
                    return this.HandleClick( slot, ref mouseState, ref oldMouseState );
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the player has clicked on one of the Action Quick Slots.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnQuickSlotClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            this.HandleClick( (QuickActionSlotElement)sender, ref mouseState, ref oldMouseState );
        }

        /// <summary>
        /// Handles clicking of the given QuickActionSlotElement.
        /// </summary>
        /// <param name="slotElement">The QuickActionSlotElement that was clicked.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        /// <returns>
        /// true if input should be passed to elements
        /// behind the clicked element;
        /// otherwise false.
        /// </returns>
        private bool HandleClick( QuickActionSlotElement slotElement, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( player.IsDead )
                return false;

            QuickActionSlot slot = slotElement.Slot;

            if( mouseState.RightButton == ButtonState.Pressed &&
                oldMouseState.RightButton == ButtonState.Released )
            {
                this.HandleRightClick( slot );
                return true;
            }

            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                this.HandleLeftMouseClick( slot, slotElement, ref mouseState );
                return true;
            }

            if( mouseState.LeftButton == ButtonState.Released &&
                oldMouseState.LeftButton == ButtonState.Pressed )
            {
                return this.HandleLeftMouseRelease( slot, slotElement, ref mouseState );
            }

            return false;
        }

        /// <summary>
        /// Handles right-clicking of a Quick Action Slot.
        /// </summary>
        /// <param name="slot">The slot that has been clicked.</param>
        private void HandleRightClick( QuickActionSlot slot )
        {
            if( slot.Action != null )
            {
                slot.Action.Execute( this.player );
            }
        }

        /// <summary>
        /// Handles left-clicking of a Quick Action Slot.
        /// </summary>
        /// <param name="slot">The slot that has been clicked.</param>
        /// <param name="slotElement">The slot UIElement that has been clicked.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        private void HandleLeftMouseClick( QuickActionSlot slot, QuickActionSlotElement slotElement, ref MouseState mouseState )
        {
            if( slot.Action != null )
            {
                this.pickedUp.Take( slot );

                this.pickedUp.DrawOffset = new Point2(
                    mouseState.X - (int)slotElement.Position.X,
                    mouseState.Y - (int)slotElement.Position.Y
                );
            }
            else
            {
                this.StorePickedUpItem( slot );
                this.ignoreReleaseOnce = true;
            }
        }

        /// <summary>
        /// Handles releasing of the left mouse button above a Quick Action Slot.
        /// </summary>
        /// <param name="slot">The slot that has been clicked.</param>
        /// <param name="slotElement">The slot UIElement that has been clicked.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <returns>
        /// true if input should be passed to elements
        /// behind the clicked element;
        /// otherwise false.
        /// </returns>
        private bool HandleLeftMouseRelease(
            QuickActionSlot slot,
            QuickActionSlotElement slotElement,
            ref MouseState mouseState )
        {
            if( this.ignoreReleaseOnce )
            {
                this.ignoreReleaseOnce = false;
                return false;
            }

            this.pickedUp.DrawOffset = new Point2(
                mouseState.X - (int)slotElement.Position.X,
                mouseState.Y - (int)slotElement.Position.Y
            );

            if( slot.Action != null )
            {
                if( this.pickedUp.Action != null )
                {
                    this.pickedUp.Exchange( slot );
                }
                else
                {
                    // Insert into 
                    this.pickedUp.Action = slot.Action;
                    this.pickedUp.OriginalSlot = slot;
                    slot.Action = null;
                }
            }
            else
            {
                if( this.pickedUp.Action != null )
                {
                    // Insert it
                    slot.Action = pickedUp.Action;

                    pickedUp.Action = null;
                    pickedUp.OriginalSlot = null;
                }
                else
                {
                    return this.StorePickedUpItem( slot );
                }
            }

            return false;
        }

        /// <summary>
        /// Stores the currently picked-up Item in the given QuickActionSlot.
        /// </summary>
        /// <param name="slot">
        /// The QuickActionSlot to store the item in.
        /// </param>
        /// <returns>
        /// true if the picked-up item has been stored
        /// in the given QuickActionSlot; otherwise false.
        /// </returns>
        private bool StorePickedUpItem( QuickActionSlot slot )
        {
            var pickedUpItem = player.PickedupItemContainer.Item;

            if( pickedUpItem != null )
            {
                if( pickedUpItem.Item.UseEffect != null )
                {
                    if( player.PickedupItemContainer.Restore() )
                    {
                        slot.Action = new UseItemAction( pickedUpItem.BaseItem, player.Inventory );
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Handles mouse input for this QuickActionSlotsDisplay.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Released &&
                oldMouseState.LeftButton == ButtonState.Pressed )
            {
                this.RestorePickedUp();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The PlayerEntity whose QuickActionSlots are visualized by this QuickActionSlotsDisplay.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// States whether the next mouse release should be ignored.
        /// </summary>
        private bool ignoreReleaseOnce;

        /// <summary>
        /// Holds and visualizes the IQuickAction the player has picked-up.
        /// </summary>
        private readonly PickedupQuickActionDisplay pickedUp;

        /// <summary>
        /// The QuickActionSlotElements that visualize the individual QuickActionSlots of the PlayerEntity.
        /// </summary>
        private readonly QuickActionSlotElement[] slots;

        /// <summary>
        /// The QuickActionSlotDrawer that encapsulates the drawing process and data.
        /// </summary>
        private readonly QuickActionSlotDrawer slotDrawer;

        #endregion

        #region [ class PickedupQuickActionDisplay ]

        /// <summary>
        /// Visualizes the currently picked up IQuickAction.
        /// </summary>
        private sealed class PickedupQuickActionDisplay : UIElement
        {
            /// <summary>
            /// Gets or sets the draw offset applied when drawing the Picked Up Action.
            /// </summary>
            public Point2 DrawOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the IQuickAction that has been picked-up.
            /// </summary>
            public IQuickAction Action
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the QuickActionSlot the IQuickAction that has been picked-up was originally in.
            /// </summary>
            public QuickActionSlot OriginalSlot
            {
                get;
                set;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PickedupQuickActionDisplay"/> class.
            /// </summary>
            /// <param name="slotDrawer">
            /// Provides a mechanism to draw a QuickActionSlot.
            /// </param>
            public PickedupQuickActionDisplay( QuickActionSlotDrawer slotDrawer )
            {
                this.FloorNumber = 1;
                this.PassInputToSubElements = true;

                this.slotDrawer = slotDrawer;
            }

            /// <summary>
            /// Restores the currently picked-up Quick Action.
            /// </summary>
            /// <param name="actionSlots">
            /// The list of quick slots.
            /// </param>
            public void Restore( QuickActionSlotList actionSlots )
            {
                if( this.Action != null )
                {
                    // Special Case of UseItem actions.
                    var useItemAction = this.Action as UseItemAction;

                    if( useItemAction != null )
                    {
                        useItemAction.Dispose();

                        this.OriginalSlot = null;
                        this.Action = null;
                        return;
                    }

                    this.RestoreInOriginalSlot( actionSlots );
                }
            }

            /// <summary>
            /// Restores the Action to its OriginalSlot.
            /// </summary>
            /// <param name="actionSlots">
            /// The list of quick slots.
            /// </param>
            private void RestoreInOriginalSlot( QuickActionSlotList actionSlots )
            {
                QuickActionSlot slot = this.OriginalSlot;

                if( slot == null || slot.Action != null )
                {
                    slot = actionSlots.GetFreeSlot();
                }

                if( slot != null )
                {
                    slot.Action = this.Action;
                }

                this.Action = null;
                this.OriginalSlot = null;
            }

            /// <summary>
            /// Exchanges the picked-up action with the action in the given QuickActionSlot.
            /// </summary>
            /// <param name="slot">
            /// The QuickActionSlot to exchange the IQuickAction with.
            /// </param>
            internal void Exchange( QuickActionSlot slot )
            {
                if( this.OriginalSlot == null || slot == null )
                    return;

                var newAction = slot.Action;
                slot.Action = this.Action;
                this.OriginalSlot.Action = newAction;

                this.Action = null;
                this.OriginalSlot = null;
            }

            /// <summary>
            /// Takes the action of the given QuickActionSlot.
            /// </summary>
            /// <param name="slot">
            /// The QuickActionSlot to take.
            /// </param>
            internal void Take( QuickActionSlot slot )
            {
                var oldPickedUpAction = this.Action;

                this.Action = slot.Action;
                this.OriginalSlot = slot;
                slot.Action = oldPickedUpAction;
            }

            /// <summary>
            /// Called when this <see cref="PickedupQuickActionDisplay"/> is drawing itself.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            protected override void OnDraw( ISpriteDrawContext drawContext )
            {
                // Draw picked up skill
                if( this.Action != null )
                {
                    Point2 drawPositon = this.Owner.MousePosition - this.DrawOffset;
                    const float LayerDepth = 0.2f;

                    this.slotDrawer.DrawSymbol( this.Action, drawPositon, LayerDepth, (ZeldaDrawContext)drawContext );
                }
            }

            /// <summary>
            /// Called when this <see cref="PickedupQuickActionDisplay"/> is updating itself.
            /// </summary>
            /// <param name="updateContext">
            /// The current IUpdateContext.
            /// </param>
            protected override void OnUpdate( Atom.IUpdateContext updateContext )
            {
            }

            /// <summary>
            /// Provides a mechanism to draw a QuickActionSlot.
            /// </summary>
            private readonly QuickActionSlotDrawer slotDrawer;
        }

        #endregion
    }
}
