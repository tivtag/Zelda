// <copyright file="MerchantWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Trading.MerchantWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Trading
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Zelda.Items;
    using Zelda.Trading;
    using Zelda.UI.Items;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Provides the UI required to interact with an IMerchant.s
    /// </summary>
    internal sealed class MerchantWindow : IngameWindow
    {
        #region [ Constants ]
        
        /// <summary>
        /// The maximum size of one stack of items.
        /// A new stack is created when this value is exceeded.
        /// </summary>
        private const float MaximumStackHeight = 160;

        /// <summary>
        /// The offset between individual elements in a stack.
        /// </summary>
        private const int OffsetBetweenElements = 1;

        /// <summary>
        /// The horizontal offset between two stacks.
        /// </summary>
        private const int OffsetBetweenStacks = 16, OffsetBetweenStacksSmall = 5;

        /// <summary>
        /// The time in seconds two clicks must be within to still count as a double-click.
        /// </summary>
        private const float DoubleClickTime = 0.40f;

        /// <summary>
        /// The time in seconds for which double-clicking is disabled
        /// after the player has bought an item via double-clicking.
        /// </summary>
        private const float TimeDoubleClickDisabledAfterAction = 0.32f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Sets the Merchant visualized by this MerchantWindow.
        /// </summary>
        public IMerchant Merchant
        {
            //get
            //{
            //    return this.merchant;
            //}
            // ^ is unused.
            set
            {
                this.merchant = value;

                this.CreateContent();
                this.RefreshWindowTitle();
            }
        }

        /// <summary>
        /// Gets or sets the Entity that wants to buy from the Merchant.
        /// </summary>
        public PlayerEntity Buyer
        {
            get
            {
                return this.Player;
            }
            
            set
            {
                this.Player = value;
                this.itemInfoDisplay.Player = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal MerchantWindow( IZeldaServiceProvider serviceProvider )
        {
            this.Size = serviceProvider.ViewSize;
            this.serviceProvider = serviceProvider;

            this.itemInfoDisplay = new ItemInfoDisplay( serviceProvider.GetService<IItemInfoVisualizer>() );
        }

        /// <summary>
        /// Creates the content of this MerchantWindow.
        /// </summary>
        private void CreateContent()
        {
            this.DestroyElements();
            this.CreateElements();
            this.ArrangeElements();
            this.AddShowEnableElements();
        }

        /// <summary>
        /// Adds, shows and enables all MerchantItemElements. 
        /// </summary>
        private void AddShowEnableElements()
        {
            foreach( var element in this.elements )
            {
                element.ShowAndEnable();
                this.Owner.AddElement( element );
            }
        }

        /// <summary>
        /// Destroys, hides and removes all MerchantItemElements.
        /// </summary>
        private void DestroyElements()
        {
            this.UnselectElement();

            foreach( var element in this.elements )
            {
                element.HideAndDisable();
                this.Owner.RemoveElement( element );
            }

            this.elements.Clear();
        }

        /// <summary>
        /// Creates the MerchantItemElements required to visualize the 
        /// items the Merchant is selling.
        /// </summary>
        private void CreateElements()
        {
            foreach( var item in this.merchant.GetAvailableItems( this.Buyer ) )
            {
                var element = new MerchantItemElement( this.serviceProvider ) {
                    Buyer = this.Buyer,
                    MerchantItem = item
                };

                this.elements.Add( element );
            }
        }

        /// <summary>
        /// Arranges the MerchantItemElements.
        /// </summary>
        private void ArrangeElements()
        {
            float greatestStackHeight;
            var elementStacks = GetArrangedInStacks( this.elements, out greatestStackHeight );

            bool isFullyStocked = elementStacks.Count >= 4;
            int offsetBetweenStacks = isFullyStocked ? OffsetBetweenStacksSmall : OffsetBetweenStacks;
            int elementWidth = isFullyStocked ? MerchantItemElement.ElementWidthSmall : MerchantItemElement.ElementWidthDefault;
            
            int totalWidth = GetTotalWidthOfStacks( elementStacks.Count, offsetBetweenStacks, elementWidth );
            int startY = (int)((this.Height / 2.0f) - (greatestStackHeight / 2.0f)); 

            float x = (int)((this.Size.X / 2.0f) - (totalWidth / 2.0f));
            float y = startY;

            foreach( IList<MerchantItemElement> stack in elementStacks )
            {
                foreach( MerchantItemElement itemElement in stack )
                {
                    itemElement.ElementWidth = elementWidth;
                    itemElement.PositionAt( new Atom.Math.Vector2( x, y ) );
                    y += itemElement.Size.Y + OffsetBetweenElements;
                }

                x += elementWidth;
                x += offsetBetweenStacks;
                y = startY;
            }
        }

        /// <summary>
        /// Arranges the given list of MerchantItemElements into stacks
        /// that don't overflow this MerchantWindow.
        /// </summary>
        /// <param name="elements">
        /// The elemetns to arrange into stacks.
        /// </param>
        /// <param name="greatestStackHeight">
        /// Will contain the height of the greatest stack.
        /// </param>
        /// <returns></returns>
        private static IList<IList<MerchantItemElement>> GetArrangedInStacks( 
            IList<MerchantItemElement> elements, 
            out float greatestStackHeight )
        {
            var stacks = new List<IList<MerchantItemElement>>();
            var currentStack = new List<MerchantItemElement>();

            float currentStackHeight = 0.0f;
            greatestStackHeight = 0.0f;
            stacks.Add( currentStack );
            
            foreach( MerchantItemElement element in elements )
            {
                float elementSize = element.Size.Y + OffsetBetweenElements;
                float newStackHeight = currentStackHeight + elementSize;

                if( newStackHeight > MaximumStackHeight )
                {
                    greatestStackHeight = MaximumStackHeight;

                    currentStack = new List<MerchantItemElement>();
                    currentStack.Add( element );
                    currentStackHeight = elementSize;

                    stacks.Add( currentStack );
                }
                else
                {
                    if( newStackHeight > greatestStackHeight )
                    {
                        greatestStackHeight = newStackHeight;
                    }

                    currentStack.Add( element );
                    currentStackHeight = newStackHeight;
                }                
            }

            return stacks;
        }

        /// <summary>
        /// Gets the total width the given number of MerchantItemElements
        /// stacks would cower.
        /// </summary>
        /// <param name="stackCount">
        /// The number of stacks.
        /// </param>
        /// <param name="offsetBetweenStacks">
        /// The number of pixels between two stacks.
        /// </param>
        /// <param name="elementWidth">
        /// The width of a single element of a stack in pixels.
        /// </param>
        /// <returns>
        /// The total width in pixels.
        /// </returns>
        private static int GetTotalWidthOfStacks( int stackCount, int offsetBetweenStacks, int elementWidth )
        {
            int offsetSize = (stackCount - 1) * offsetBetweenStacks;
            int stackSize = stackCount * elementWidth;

            return stackSize + offsetSize;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Sells the selected item to the buyer.
        /// </summary>
        /// <param name="sellFullStock">
        /// States whether
        /// </param>
        private void SellSelectedToBuyer( bool sellFullStock )
        {
            if( this.selectedElement == null )
                return;

            var merchantItem = this.selectedElement.MerchantItem;
            
            if( sellFullStock ? this.SellFullStock( merchantItem ) : this.Sell( merchantItem ) )
            {
                this.OnItemSold( this.selectedElement );
            }
        }

        /// <summary>
        /// Called when atleast one item of the given MerchantItem has been sold.
        /// </summary>
        /// <param name="itemElement">
        /// The MerchantItemElement that has been sold.
        /// </param>
        private void OnItemSold( MerchantItemElement itemElement )
        {
            var merchantItem = itemElement.MerchantItem;

            if( merchantItem.StockCount == 0 )
            {
                this.OnItemStockSoldOut( itemElement );
            }

            PlaySoldSound( merchantItem );
        }

        /// <summary>
        /// Called when the item stock of the given MerchantItemElement 
        /// has been completely sold out.
        /// </summary>
        /// <param name="itemElement">
        /// The MerchantItemElement that has been sold.
        /// </param>
        private void OnItemStockSoldOut( MerchantItemElement itemElement )
        {
            int index = this.elements.IndexOf( itemElement );

            this.CreateContent();

            // Reaquire selection.
            if( this.elements.Count > 0 )
            {
                index = index % this.elements.Count;

                if( this.IsValidElementIndex( index ) )
                {
                    this.SelectElement( index );
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given index
        /// into the elements list is valid.
        /// </summary>
        /// <param name="index">
        /// The index to verify.
        /// </param>
        /// <returns></returns>
        private bool IsValidElementIndex( int index )
        {
            return index >= 0 && index < this.elements.Count;
        }

        /// <summary>
        /// Sells the full stock of items to the buyer. 
        /// </summary>
        /// <param name="merchantItem">
        /// The MerchantItem to sell.
        /// </param>
        /// <returns>
        /// true if atleast one item was sold;
        /// otherwise false.
        /// </returns>
        private bool SellFullStock( MerchantItem merchantItem )
        {
            bool soldAtleastOne = false;

            while( this.Sell( merchantItem ) ) 
            {
                soldAtleastOne = true;
            }

            return soldAtleastOne;
        }

        /// <summary>
        /// Tries to sell the given MerchantItem to the Buyer.
        /// </summary>
        /// <param name="merchantItem">
        /// The MerchantItem to sell.
        /// </param>
        /// <returns>
        /// true if the item has been sold;
        /// otherwise false.
        /// </returns>
        private bool Sell( MerchantItem merchantItem )
        {
            ItemInstance soldItem = merchantItem.SellTo( this.Buyer );
            if( soldItem == null )
                return false;

            this.Buyer.Inventory.FailSafeInsert( soldItem );
            return true;
        }

        /// <summary>
        /// Plays the sound associated with selling
        /// the given ItemInstance.
        /// </summary>
        /// <param name="merchantItem">
        /// The item that has been sold.
        /// </param>
        private static void PlaySoldSound( MerchantItem merchantItem )
        {
            ItemSounds.PlayPickUp( merchantItem.Item );
        }

        /// <summary>
        /// Refershes the title that is shown in this MerchantWindow.
        /// </summary>
        private void RefreshWindowTitle()
        {
            if( this.merchant == null )
            {
                this.title = string.Empty;
            }
            else
            {
                this.title = this.GetMerchantName();
            }
        }

        /// <summary>
        /// Gets the localized name of the Merchant.
        /// </summary>
        /// <returns></returns>
        private string GetMerchantName()
        {
            return this.merchant.LocalizedName.LocalizedText;
        }

        /// <summary>
        /// Selects the given MerchantItemElement.
        /// </summary>
        /// <param name="element">
        /// The MerchantItemElement to select.
        /// </param>
        private void SelectElement( MerchantItemElement element )
        {
            this.UnselectElement();

            this.selectedElement = element;

            if( this.selectedElement != null )
            {
                this.selectedElement.IsSelected = true;
            }
        }

        /// <summary>
        /// Unselects the currently selected MerchantItemElement.
        /// </summary>
        private void UnselectElement()
        {
            if( this.selectedElement != null )
            {
                this.selectedElement.IsSelected = false;
                this.selectedElement = null;
            }
        }

        /// <summary>
        /// Called when this <see cref="CharacterWindow"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, (int)this.Width, (int)this.Height ),
                UIColors.LightWindowBackground,
                0.0f
            );

            // Draw Title Background
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, (int)this.Width, 20 ),
                new Xna.Color( 0, 0, 0, 200 ),
                0.0015f
            );

            // Draw Title
           this.fontTitle.Draw(                
                this.title,
                new Vector2( this.Width / 2.0f, 0.0f ),
                TextAlign.Center,
                Xna.Color.White,
                0.002f,
                drawContext
            );
        }

        /// <summary>
        /// Called when MouseInput happens that is related to this Atom.Xna.UI.UIElement; aka. inside the Element.
        /// </summary>
        /// <param name="mouseState">
        /// The state of the Microsoft.Xna.Framework.Input.Mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the Microsoft.Xna.Framework.Input.Mouse one frame ago.
        /// </param>
        /// <returns>
        /// Returns true if input should be passed to elements that are behind the Atom.Xna.UI.UIElement, otherwise false.
        /// </returns>
        protected override bool HandleRelatedMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                this.OnMouseClick( mouseState.X, mouseState.Y );
            }

            UpdateMouseHoveredItem();
            return false;
        }

        /// <summary>
        /// Updates the currently shown item instance.
        /// </summary>
        private void UpdateMouseHoveredItem()
        {
            int x = this.Owner.MouseState.X;
            int y = this.Owner.MouseState.Y;

            this.itemInfoDisplay.ItemInstance = this.GetMouseOveredItem( x, y );
            this.itemInfoDisplay.Position = new Atom.Math.Vector2( x, y );
        }

        /// <summary>
        /// Gets the item that is at the given position; if any.
        /// </summary>
        /// <param name="x">The position on the x-axis.</param>
        /// <param name="y">The position on the y-axis.</param>
        /// <returns>
        /// The item at the give position;
        /// or null.
        /// </returns>
        private Zelda.Items.ItemInstance GetMouseOveredItem( int x, int y )
        {
            MerchantItemElement hoveredContainer = elements.Find(
                (element) => {
                    return element.ContainerArea.Contains( x, y );
                }
            );

            if( hoveredContainer == null )
            {
                return null;
            }

            ItemInstance itemInstance = hoveredContainer.MerchantItem.ItemInstance;
            itemInstance = InventoryBaseWindow.GetMouseOverItemOrEquippedItemForComparison( itemInstance, this.Buyer, this.Owner );
            return itemInstance;
        }

        /// <summary>
        /// Called when the user has clicked the left mouse-button.
        /// </summary>
        /// <param name="x">
        /// The position of the mouse on the x-axis.
        /// </param>
        /// <param name="y">
        /// The position of the mouse on the y-axis.
        /// </param>
        private void OnMouseClick( int x, int y )
        {
            var clickedElement = this.elements.Find(
               ( element ) => element.ClientArea.Contains( x, y )
            );

            if( clickedElement != null )
            {
                this.SelectElement( clickedElement );
            }

            if( this.isCapturingForDoubleClick )
            {
                this.SellSelectedToBuyer( this.OnActionShouldSellFullStock() );
                this.timeLeftDoubleClickDisabled = TimeDoubleClickDisabledAfterAction;
            }
            else
            {
                this.isCapturingForDoubleClick = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the full stock
        /// should be sold.
        /// </summary>
        /// <returns></returns>
        private bool OnActionShouldSellFullStock()
        {
            return this.Owner.IsShiftDown || this.Owner.IsControlDown;
        }

        /// <summary>
        /// Called when this <see cref="CharacterWindow"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // Ruby
            RubyDisplay rubyDisplay = this.Owner.GetElement<RubyDisplay>();
            rubyDisplay.ShowRubyValueThisFrame = true;

            // Double Click
            if( this.timeLeftDoubleClickDisabled > 0.0f )
            {
                this.timeLeftDoubleClickDisabled -= updateContext.FrameTime;
                this.isCapturingForDoubleClick = false;
                return;
            }

            if( this.isCapturingForDoubleClick )
            {
                this.timeSinceLastClick += updateContext.FrameTime;

                if( this.timeSinceLastClick >= DoubleClickTime )
                {
                    this.isCapturingForDoubleClick = false;
                    this.timeSinceLastClick = 0.0f;
                }
            }
        }

        /// <summary>
        /// Called when this MerchantWindow is opening.
        /// </summary>
        protected override void Opening()
        {
            this.Owner.FocusedElement = this;
        }

        /// <summary>
        /// Called when this MerchantWindow is closing.
        /// </summary>
        protected override void Closing()
        {
            this.Owner.FocusedElement = null;
        }
        
        /// <summary>
        /// Handles keyboard input for the MerchantWindow.
        /// </summary>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        protected override void HandleKeyInput( ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            if( keyState.IsKeyDown( Keys.Enter ) &&
                oldKeyState.IsKeyUp( Keys.Enter ) )
            {
                this.SellSelectedToBuyer( OnActionShouldSellFullStock() );
            }

            else if( keyState.IsKeyDown( Keys.Up ) && oldKeyState.IsKeyUp( Keys.Up ) )
            {
                this.MoveSelectionUp();
            }

            else if( keyState.IsKeyDown( Keys.Down ) && oldKeyState.IsKeyUp( Keys.Down ) )
            {
                this.MoveSelectionDown();
            }

            if( InventoryBaseWindow.ShouldRefreshItemInfoDisplayBasedOnKeyInput( this.Owner ) )
            {
                UpdateMouseHoveredItem();
            }
        }

        /// <summary>
        /// Moves the current element selection up by one index.
        /// </summary>
        private void MoveSelectionUp()
        {
            if( this.elements.Count == 0 )
                return;

            int index = this.GetSelectedElementIndex();    
                   
            int newIndex = (index - 1) % this.elements.Count;
            if( newIndex < 0 )
            {
                newIndex = this.elements.Count - 1;
            }

            this.SelectElement( newIndex );
        }

        /// <summary>
        /// Moves the current element selection down by one index.
        /// </summary>
        private void MoveSelectionDown()
        {
            if( this.elements.Count == 0 )
                return;

            int index = this.GetSelectedElementIndex();
            int newIndex = (index + 1) % this.elements.Count;

            this.SelectElement( newIndex );
        }

        /// <summary>
        /// Selects the MerchantItemElement at the given elements index.
        /// </summary>
        /// <param name="elementIndex">
        /// The zero-based index into the elements list.
        /// </param>
        private void SelectElement( int elementIndex )
        {
            var newElement = this.elements[elementIndex];

            this.UnselectElement();
            this.SelectElement( newElement );
        }

        /// <summary>
        /// Gets the zero-based index into the elements list
        /// of the currently selected element.
        /// </summary>
        /// <returns></returns>
        private int GetSelectedElementIndex()
        {
            return this.elements.IndexOf( this.selectedElement );
        }

        /// <summary>
        /// Called when the IsVisible state of this MerchantWindow has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            foreach( var element in this.elements )
            {
                element.IsVisible = this.IsVisible;
            }

            this.itemInfoDisplay.IsVisible = this.IsVisible;
        }

        /// <summary>
        /// Called when the IsEnabled state of this MerchantWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            foreach( var element in this.elements )
            {
                element.IsEnabled = this.IsEnabled;
            }

            this.itemInfoDisplay.IsEnabled = this.IsEnabled;
        }

        /// <summary>
        /// Callde when this MerchantWindow has been added to an UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.itemInfoDisplay );
        }
        
        /// <summary>
        /// Callde when this MerchantWindow has been removed from an UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.itemInfoDisplay );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field of the <see cref="Merchant"/> property.
        /// </summary>
        private IMerchant merchant;

        /// <summary>
        /// The currently selected MerchantItemElement.
        /// </summary>
        private MerchantItemElement selectedElement;

        /// <summary>
        /// The list of MerchantItemElements this MerchantWindow consists of.
        /// </summary>
        private readonly List<MerchantItemElement> elements = new List<MerchantItemElement>();

        /// <summary>
        /// States whether the MerchantWindow is currently capturing double-clicks.
        /// </summary>
        private bool isCapturingForDoubleClick;

        /// <summary>
        /// States the time the last left mouse-click.
        /// Used to capture double clicks.
        /// </summary>
        private float timeSinceLastClick;

        /// <summary>
        /// The time left for which double-clicking is disabled.
        /// </summary>
        private float timeLeftDoubleClickDisabled;
        
        /// <summary>
        /// The ItemInfoDisplay that is responsible for drawing the Item Information
        /// when the user mouse-overs an item.
        /// </summary>
        private readonly ItemInfoDisplay itemInfoDisplay;

        /// <summary>
        /// The font used to draw the title string.
        /// </summary>
        private readonly IFont fontTitle = UIFonts.TahomaBold11;

        /// <summary>
        /// The cached title that should be displayed for the Merchant.
        /// </summary>
        private string title = string.Empty;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
