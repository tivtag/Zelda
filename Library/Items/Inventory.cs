// <copyright file="Inventory.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Inventory class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Items.Affixes;
    using Zelda.Status;

    /// <summary>
    /// Represents the backpack of the player that stores 
    /// all items the player owns but doesn't have equipped.
    /// </summary>
    public class Inventory
    {
        #region [ Constants ]

        /// <summary>
        /// The default size of the grid (in cell space) of the <see cref="Inventory"/>.
        /// </summary>
        public const int DefaultGridWidth = 18, DefaultGridHeight = 10;

        /// <summary>
        /// Represents an invalid Inventory Cell position.
        /// </summary>
        public static readonly Point2 InvalidCell = new Point2( -1, -1 );

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when an <see cref="Item"/> has been added to this Inventory.
        /// </summary>
        public event RelaxedEventHandler<ItemInstance> Added;

        /// <summary>
        /// Fired when an <see cref="Item"/> has been removed from this Inventory.
        /// </summary>
        public event RelaxedEventHandler<ItemInstance> Removed;

        #endregion

        #region [ Properties ]

        public PlayerEntity Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets the width of the grid (in cell space).
        /// </summary>
        public int GridWidth
        {
            get
            {
                return this.gridWidth;
            }
        }

        /// <summary>
        /// Gets the height of the grid (in cell space).
        /// </summary>
        public int GridHeight
        {
            get
            {
                return this.gridHeight;
            }
        }

        /// <summary>
        /// Gets the size of the grid (in cell space).
        /// </summary>
        public Point2 GridSize
        {
            get
            {
                return new Point2( this.gridWidth, this.gridHeight );
            }
        }

        /// <summary>
        /// Gets a value that represents how many Item (stacks) are in the Inventory.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return this.items.Count;
            }
        }

        /// <summary>
        /// Gets the list of InventoryItems in this Inventory.
        /// </summary>
        protected List<InventoryItem> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets an enumeration over all Items of this Inventory.
        /// </summary>
        public IEnumerable<Item> ContainedItems
        {
            get
            {
                foreach( var item in this.items )
                {
                    yield return item.Item;
                }
            }
        }

        /// <summary>
        /// Gets an enumeration over all ItemInstances of this Inventory.
        /// </summary>
        public IEnumerable<ItemInstance> ContainedItemInstances
        {
            get
            {
                foreach( var item in this.items )
                {
                    yield return item.ItemInstance;
                }
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="player">The PlayerEntity that owns the Inventory.</param>
        public Inventory( PlayerEntity player )
            : this( player, DefaultGridWidth, DefaultGridHeight )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new Inventory.
        /// </param>
        /// <param name="gridWidth">
        /// The number of cells on the x-axis.
        /// </param>
        /// <param name="gridHeight">
        /// The number of cells on the y-axis.
        /// </param>
        protected internal Inventory( PlayerEntity player, int gridWidth, int gridHeight )
        {
            Debug.Assert( player != null );

            this.owner = player;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;

            this.cells = new Cell[GridWidth][];

            for( int x = 0; x < GridWidth; ++x )
            {
                Cell[] cellY = new Cell[GridHeight];
                this.cells[x] = cellY;

                for( int y = 0; y < GridHeight; ++y )
                    cellY[y] = new Cell( x, y );
            }

            player.Spawnable.Spawned += OnOwnerSpawned;
        }

        #endregion

        #region [ Methods ]

        #region > Insert <

        /// <summary>
        /// Tries to insert the given ItemInstance into the <see cref="Inventory"/>.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to insert into the Inventory.
        /// </param>
        /// <returns>
        /// Returns true if the ItemInstance has been inserted;
        /// otherwise false.
        /// </returns>
        public bool Insert( ItemInstance itemInstance )
        {
            if( itemInstance == null )
                throw new ArgumentNullException( "itemInstance" );

            Item item = itemInstance.Item;

            // Try to add it ontop of an existing stack 
            // If the Item is stackable:
            if( this.TryAddToStack( itemInstance ) )
            {
                return true;
            }

            // Try to find a new cell 
            // for the item:
            Cell cell = this.FindSpaceForItem( item );
            if( cell == null )
                return false;

            this.SetAndAdd( itemInstance, cell );
            return true;
        }

        /// <summary>
        /// Tries to add the given ItemInstance to an existing Item Stack.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to try to add to an existing Item Stack.
        /// </param>
        /// <returns>
        /// Returns whether the given ItemInstance was completely added to an ItemStack.
        /// </returns>
        private bool TryAddToStack( ItemInstance itemInstance )
        {
            Item item = itemInstance.Item;
            if( !item.IsStackable )
                return false;

            InventoryItem stack = this.FindItemStack( item.Name, itemInstance.Count );

            if( stack != null )
            {
                Cell parentCell = stack.ParentCell;

                // Can we add all to the the stack in the cell?
                if( parentCell.ItemCount + itemInstance.Count <= item.StackSize )
                {
                    parentCell.ItemCount += itemInstance.Count;
                    itemInstance.Count = 0;

                    this.Added.Raise( this, itemInstance );
                    return true;
                }
                else
                {
                    // Simply fill what we can:
                    int allowedAmount = item.StackSize - parentCell.ItemCount;

                    if( allowedAmount > 0 )
                    {
                        parentCell.ItemCount = item.StackSize;
                        itemInstance.Count -= allowedAmount;

                        this.Added.Raise( this, itemInstance );
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to insert the specified <see cref="ItemInstance"/> at the given cell.
        /// </summary>
        /// <param name="itemInstance">The ItemInstance to insert.</param>
        /// <param name="cellPosition">The position of the starting cell (in cell space).</param>
        /// <returns>
        /// true if it has been inserted; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="itemInstance"/> is null.</exception>
        internal bool InsertAt( ItemInstance itemInstance, Point2 cellPosition )
        {
            return this.InsertAt( itemInstance, cellPosition.X, cellPosition.Y );
        }

        /// <summary>
        /// Tries to insert the specified <see cref="ItemInstance"/> at the given cell.
        /// </summary>
        /// <param name="itemInstance">The ItemInstance to insert.</param>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <returns>true if it has been inserted; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="itemInstance"/> is null.</exception>
        public bool InsertAt( ItemInstance itemInstance, int cellX, int cellY )
        {
            if( itemInstance == null )
                throw new ArgumentNullException( "itemInstance" );

            if( !IsValidCell( cellX, cellY ) )
                return false;

            if( this.HasFreeSpace( itemInstance, cellX, cellY ) )
            {
                this.SetAndAdd( itemInstance, cellX, cellY );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to insert the specified <see cref="ItemInstance"/> at the given cell.
        /// </summary>
        /// <param name="itemInstance">The ItemInstance to insert.</param>
        /// <param name="cellX">
        /// The position of the cell on the x-axis (in cell space).
        /// </param>
        /// <param name="cellY">
        /// The position of the cell on the y-axis (in cell space).
        /// </param>
        /// <param name="oldItemInstance">
        /// Will contain the ItemInstance that has been replaced with the given <paramref name="itemInstance"/>.
        /// </param>
        /// <returns>true if it has been inserted; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="itemInstance"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="cellX"/> or <paramref name="cellY"/> is invalid.
        /// </exception>
        public bool InsertAt( ItemInstance itemInstance, int cellX, int cellY, out ItemInstance oldItemInstance )
        {
            if( itemInstance == null )
                throw new ArgumentNullException( "itemInstance" );

            int cellWidth = itemInstance.Item.InventoryWidth;
            int cellHeight = itemInstance.Item.InventoryHeight;

            // Verify.
            if( !this.IsValidCellArea( cellX, cellY, cellWidth, cellHeight ) )
            {
                oldItemInstance = null;
                return false;
            }

            // Get first item in area.
            var existingItem = this.GetInventoryItemIn( cellX, cellY, cellWidth, cellHeight );

            if( existingItem == null )
            {
                this.SetAndAdd( itemInstance, cellX, cellY );

                oldItemInstance = null;
                return true;
            }
            else
            {
                // Remove the existing item
                // and then try to insert the given item.
                if( this.RemoveItem( existingItem ) )
                {
                    if( this.InsertAt( itemInstance, cellX, cellY ) )
                    {
                        oldItemInstance = existingItem.ItemInstance;
                        return true;
                    }
                    else
                    {
                        // Still failed to insert even after removing the existing item.
                        // -> Reinsert existing item.
                        var cellPosition = existingItem.ParentCell.Position;
                        this.FailSafeInsertAt( existingItem.ItemInstance, cellPosition.X, cellPosition.Y );
                    }
                }

                oldItemInstance = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to insert the given ItemInstance into this Inventory at the given cell position.
        /// If this fails other mechanism are used to rescue the item.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to insert into this Inventory.
        /// </param>
        /// <param name="cellPosition">
        /// The position of the cell.
        /// </param>
        public void FailSafeInsertAt( ItemInstance itemInstance, Point2 cellPosition )
        {
            this.FailSafeInsertAt( itemInstance, cellPosition.X, cellPosition.Y );
        }

        /// <summary>
        /// Tries to insert the given ItemInstance into this Inventory at the given cell position.
        /// If this fails other mechanism are used to rescue the item.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to insert into this Inventory.
        /// </param>
        /// <param name="cellX">The position of the cell on the x-axis.</param>
        /// <param name="cellY">The position of the cell on the y-axis.</param>
        public void FailSafeInsertAt( ItemInstance itemInstance, int cellX, int cellY )
        {
            if( !this.InsertAt( itemInstance, cellX, cellY ) )
            {
                this.FailSafeInsert( itemInstance );
            }
        }

        /// <summary>
        /// Tries to insert the given ItemInstance into this Inventory.
        /// If this fails other mechanism are used to rescue the item.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to insert into this Inventory.
        /// </param>
        public void FailSafeInsert( ItemInstance itemInstance )
        {
            if( !this.Insert( itemInstance ) )
            {
                if( this.owner.PickedupItemContainer.Item != null )
                {
                    if( this.owner.Scene != null )
                    {
                        MapItem.SpawnUnder( this.owner, itemInstance );
                    }
                    else
                    {
                        itemBackup.Add( itemInstance );
                    }
                }
                else
                {
                    this.owner.PickedupItemContainer.Pick( itemInstance );
                }
            }
        }

        private void OnOwnerSpawned( Entities.Components.Spawnable sender, Entities.Spawning.ISpawnPoint e )
        {
            ProcessItemBackupStore();
        }

        private void ProcessItemBackupStore()
        {
            foreach( var item in this.itemBackup )
            {
                MapItem.SpawnUnder( this.owner, item );
            }

            itemBackup.Clear();
        }

        #endregion

        #region > Find <

        #region - FindSpaceForItem -

        /// <summary>
        /// Tries to find a place the specified item fits in.
        /// </summary>
        /// <param name="item">
        /// The Item to find space for.
        /// </param>
        /// <returns>
        /// The parent cell or null if no space found.
        /// </returns>
        protected Cell FindSpaceForItem( Item item )
        {
            if( item == null )
                return null;

            int itemWidth = item.InventoryWidth;
            int itemHeight = item.InventoryHeight;

            for( int x = 0; x < GridWidth; ++x )
            {
                for( int y = 0; y < GridHeight; ++y )
                {
                    Cell cell = cells[x][y];

                    // if the cell contains no item
                    if( cell.Item == null )
                    {
                        // it fits into this cell
                        if( itemWidth == 1 && itemHeight == 1 )
                            return cell;

                        if( this.HasSpaceAt( x, y, itemWidth, itemHeight ) )
                            return cell;
                    }
                }
            }

            return null;
        }

        #endregion

        #region - FindItemStack -

        /// <summary>
        /// Tries to find an <see cref="InventoryItem"/> whos Item
        /// has the given name and that isn't full.
        /// </summary>
        /// <param name="name"> The name of the item that should be stacked.</param>
        /// <param name="amount"> The amount of items that should be placeable on the stack. </param>
        /// <returns> The <see cref="InventoryItem"/> object or null if no stack could be found. </returns>
        protected InventoryItem FindItemStack( string name, int amount )
        {
            if( name == null )
                return null;

            InventoryItem bestStack = null;
            int bestStackSpace = 0;

            foreach( var inventoryItem in this.items )
            {
                var itemInstance = inventoryItem.ItemInstance;
                var item = itemInstance.Item;

                if( name.Equals( item.Name, StringComparison.Ordinal ) )
                {
                    if( itemInstance.Count + amount <= item.StackSize )
                    {
                        // Yay, got a stack that fits ALL!
                        return inventoryItem;
                    }
                    else
                    {
                        int stackSpace = item.StackSize - itemInstance.Count;

                        if( bestStackSpace < stackSpace )
                        {
                            bestStack = inventoryItem;
                            bestStackSpace = stackSpace;
                        }
                    }
                }
            }

            return bestStack;
        }

        #endregion

        #endregion

        #region > Has <

        #region - HasSpace -

        /// <summary>
        /// Gets whether there is space enough space in the <see cref="Inventory"/>
        /// for the specified <see cref="Item"/>.
        /// </summary>
        /// <param name="item">
        /// The related Item.
        /// </param>
        /// <param name="amount">
        /// The number of items of the given Item.
        /// </param>
        /// <returns>
        /// Returns true if there exists enough space in the Inventory for the specified Item;
        /// otherwise false.
        /// </returns>
        public bool HasSpace( Item item, int amount )
        {
            if( item == null )
                return false;

            if( item.IsStackable )
            {
                InventoryItem stack = FindItemStack( item.Name, amount );

                if( stack != null )
                {
                    if( stack.ItemInstance.Count + amount <= stack.Item.StackSize )
                        return true;
                }
            }

            int itemWidth = item.InventoryWidth,
                itemHeight = item.InventoryHeight;
            Debug.Assert( itemWidth >= 1 );
            Debug.Assert( itemHeight >= 1 );

            for( int x = 0; x < GridWidth; ++x )
            {
                for( int y = 0; y < GridHeight; ++y )
                {
                    Cell cell = cells[x][y];

                    // If the cell contains no item:
                    if( cell.Item == null )
                    {
                        if( itemWidth == 1 && itemHeight == 1 )
                            return true; // It fits into this cell

                        bool fits = true;

                        #region Check whether it fits

                        for( int x2 = x; x2 < x + itemWidth; ++x2 )
                        {
                            if( x2 >= GridWidth )
                            {
                                fits = false;
                                break; // out of range
                            }

                            for( int y2 = y; y2 < y + itemHeight; ++y2 )
                            {
                                if( y2 >= GridHeight )
                                {
                                    fits = false;
                                    break; // out of range
                                }

                                if( cells[x2][y2].Item != null )
                                {
                                    fits = false;
                                    break;
                                }
                            }
                        }

                        #endregion

                        if( fits )
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets whether an item of the given size fits into the inventory
        /// at the given position
        /// </summary>
        /// <param name="cellX">
        /// The position of the starting cell on the x-axis.
        /// </param>
        /// <param name="cellY">
        /// The position of the starting cell on the y-axis.
        /// </param>
        /// <param name="itemWidth">
        /// The width of the item (in cell space).
        /// </param>
        /// <param name="itemHeight">
        /// The height of the item (in cell space).
        /// </param>
        /// <returns>
        /// True if no other item is in the way of the item;
        /// otherwise false.
        /// </returns>
        private bool HasSpaceAt( int cellX, int cellY, int itemWidth, int itemHeight )
        {
            int endX = cellX + itemWidth;
            if( endX > this.GridWidth )
                return false;

            int endY = cellY + itemHeight;
            if( endY > this.GridHeight )
                return false;

            for( int x = cellX; x < endX; ++x )
            {
                for( int y = cellY; y < endY; ++y )
                {
                    if( cells[x][y].Item != null )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region - HasFreeSpace -

        /// <summary>
        /// Helper method that checks whether the there is free space
        /// for the given item at the given cell. The paramters are supposed to be valid.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to test.
        /// </param>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space). Must be a valid value.</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space). Must be a valid value.</param>
        /// <returns>
        /// Returns true if there exists enough space in the Inventory to insert the given ItemInstance;
        /// otherwise false.
        /// </returns>
        private bool HasFreeSpace( ItemInstance itemInstance, int cellX, int cellY )
        {
            var item = itemInstance.Item;
            return this.HasSpaceAt( cellX, cellY, item.InventoryWidth, item.InventoryHeight );
        }

        #endregion

        #endregion

        #region > Contains <

        /// <summary>
        /// Gets a value indicating whether this Inventory contains the given Item.
        /// </summary>
        /// <param name="item">
        /// The Item to check for.
        /// </param>
        /// <returns>
        /// True if this Inventory contains the Item;
        /// otherwise false.
        /// </returns>
        public bool Contains( Item item )
        {
            foreach( var inventoryItem in this.items )
            {
                if( inventoryItem.BaseItem == item )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Inventory"/> contains the specified item.
        /// </summary>
        /// <param name="itemName">
        /// The name that uniquely identifies the item to look for.
        /// </param>
        /// <returns>
        /// True if the Inventory contains atleast one Item that has the specified <paramref name="itemName"/>;
        /// otherwise false.
        /// </returns>
        public bool Contains( string itemName )
        {
            int dummy;
            return this.Contains( itemName, 1, out dummy );
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Inventory"/> contains
        /// the specific amount of a specific item.
        /// </summary>
        /// <param name="itemName">
        /// The name that uniquely identifies the item to look for.
        /// </param>
        /// <param name="amount">
        /// The number of items the Inventory should contain atleast.
        /// </param>
        /// <returns>
        /// True if the Inventory contains atleast the specified <paramref name="amount"/>
        /// of Items that have the specified <paramref name="itemName"/>.
        /// </returns>
        public bool Contains( string itemName, int amount )
        {
            int dummy;
            return this.Contains( itemName, amount, out dummy );
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Inventory"/> contains
        /// the specific amount of a specific item.
        /// </summary>
        /// <param name="itemName">
        /// The name that uniquely identifies the item to look for.
        /// </param>
        /// <param name="amount">
        /// The number of items the Inventory should contain atleast.
        /// </param>
        /// <param name="amountFound">Will store the number of found items.</param>
        /// <returns>
        /// True if the Inventory contains atleast the specified <paramref name="amount"/>
        /// of Items that have the specified <paramref name="itemName"/>.
        /// </returns>
        public bool Contains( string itemName, int amount, out int amountFound )
        {
            return Contains( itemName, amount, false, out amountFound );
        }

        public bool Contains( Predicate<Item> predicate, int amount, out int amountFound )
        {
            return Contains( predicate, amount, false, out amountFound );
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Inventory"/> contains
        /// the specific amount of a specific item.
        /// </summary>
        /// <param name="itemName">
        /// The name that uniquely identifies the item to look for.
        /// </param>
        /// <param name="amount">
        /// The number of items the Inventory should contain atleast.
        /// </param>
        /// <param name="ignoreAffixes">
        /// States whether the affixes of affixed item should be ignored;
        /// or not.
        /// </param>
        /// <returns>
        /// True if the Inventory contains atleast the specified <paramref name="amount"/>
        /// of Items that have the specified <paramref name="itemName"/>.
        /// </returns>
        public bool Contains( string itemName, int amount, bool ignoreAffixes )
        {
            int dummy;
            return this.Contains( itemName, amount, ignoreAffixes, out dummy );
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Inventory"/> contains
        /// the specific amount of a specific item.
        /// </summary>
        /// <param name="itemName">
        /// The name that uniquely identifies the item to look for.
        /// </param>
        /// <param name="amount">
        /// The number of items the Inventory should contain atleast.
        /// </param>
        /// <param name="ignoreAffixes">
        /// States whether the affixes of affixed item should be ignored;
        /// or not.
        /// </param>
        /// <param name="amountFound">
        /// Will store the number of found items.
        /// </param>
        /// <returns>
        /// True if the Inventory contains atleast the specified <paramref name="amount"/>
        /// of Items that have the specified <paramref name="itemName"/>.
        /// </returns>
        public bool Contains( string itemName, int amount, bool ignoreAffixes, out int amountFound )
        {
            amountFound = 0;
            if( string.IsNullOrEmpty( itemName ) )
                return false;

            return Contains( item => itemName.Equals( item.Name, StringComparison.Ordinal ), amount, ignoreAffixes, out amountFound );
        }

        public bool Contains( Predicate<Item> predicate, int amount, bool ignoreAffixes, out int amountFound )
        {
            amountFound = 0;
            if( amount < 0 )
                return false;

            if( ignoreAffixes )
            {
                foreach( InventoryItem inventoryItem in this.items )
                {
                    var itemInstance = inventoryItem.ItemInstance;
                    var item = itemInstance.Item;

                    if( predicate( item ) )
                    {
                        amountFound += itemInstance.Count;
                    }
                    else
                    {
                        var affixedInstance = itemInstance as IAffixedItemInstance;

                        if( affixedInstance != null )
                        {
                            var baseItem = affixedInstance.AffixedItem.BaseItem;

                            if( predicate( baseItem ) )
                            {
                                amountFound += itemInstance.Count;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach( InventoryItem inventoryItem in this.items )
                {
                    var itemInstance = inventoryItem.ItemInstance;
                    var item = itemInstance.Item;

                    if( predicate( item ) )
                    {
                        amountFound += itemInstance.Count;
                    }
                }
            }

            if( amount == 0 )
                return amountFound == 0;
            else
                return amountFound >= amount;
        }

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the ItemInstance at the given position of the Inventory-Grid.
        /// </summary>
        /// <param name="cellX">The position of the cell in cell space.</param>
        /// <returns>
        /// The <see cref="ItemInstance"/> in the cell, or null if the cell contains no Item.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        public ItemInstance GetItemAt( Point2 cell )
        {
            return GetItemAt( cell.X, cell.Y );
        }

        /// <summary>
        /// Gets the ItemInstance at the given position of the Inventory-Grid.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <returns>
        /// The <see cref="ItemInstance"/> in the cell, or null if the cell contains no Item.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        public ItemInstance GetItemAt( int cellX, int cellY )
        {
            ThrowOnInvalidCell( cellX, cellY );

            var inventoryItem = this.cells[cellX][cellY].Item;
            if( inventoryItem == null )
                return null;

            return inventoryItem.ItemInstance;
        }

        /// <summary>
        /// Gets the InventoryItem at the given position of the Inventory-Grid.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <returns>
        /// The <see cref="ItemInstance"/> in the cell, or null if the cell contains no Item.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        private InventoryItem GetInventoryItemAt( int cellX, int cellY )
        {
            ThrowOnInvalidCell( cellX, cellY );
            return this.cells[cellX][cellY].Item;
        }

        /// <summary>
        /// Gets the ItemInstance at the specified index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the given index is invalid.</exception>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <param name="cellX">Will contain the position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">Will contain the position of the cell on the y-axis (in cell space).</param>
        /// <returns>The <see cref="ItemInstance"/> in the cell.</returns>
        public ItemInstance GetItem( int index, out int cellX, out int cellY )
        {
            var inventoryItem = this.items[index];
            Point2 cellPosition = inventoryItem.ParentCell.Position;

            cellX = cellPosition.X;
            cellY = cellPosition.Y;

            return inventoryItem.ItemInstance;
        }

        /// <summary>
        /// Gets the first item in the given cell area.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="cellWidth">The width of the area to search (in cell space).</param>
        /// <param name="cellHeight">The height of the area to search (in cell space).</param>
        /// <returns>The <see cref="ItemInstance"/> in the cell.</returns>
        private InventoryItem GetInventoryItemIn( int cellX, int cellY, int cellWidth, int cellHeight )
        {
            ThrowOnInvalidCell( cellX, cellY );

            int endX = cellX + cellWidth;
            int endY = cellY + cellHeight;
            ThrowOnInvalidCell( endX - 1, endY - 1 );

            for( int x = cellX; x < endX; ++x )
            {
                for( int y = cellY; y < endY; ++y )
                {
                    var item = this.cells[x][y].Item;
                    if( item != null )
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first InventoryItem that consists of the given Item.
        /// </summary>
        /// <param name="item">
        /// The template item.
        /// </param>
        /// <returns>
        /// The requested InventoryItem; or null if no such item exists in the Inventory.
        /// </returns>
        private InventoryItem GetInventoryItem( Item item )
        {
            for( int i = 0; i < this.items.Count; ++i )
            {
                var inventoryItem = this.items[i];

                if( inventoryItem.BaseItem == item )
                {
                    return inventoryItem;
                }
            }

            return null;
        }

        #endregion

        #region > Use <

        /// <summary>
        /// Tries to use an ItemInstance of the given Item that is placed in this Inventory. 
        /// </summary>
        /// <param name="item">
        /// The Item to use.
        /// </param>
        /// <returns>
        /// Returns whether the item was actually used.
        /// </returns>
        public bool UseItem( Item item )
        {
            var inventoryItem = this.GetInventoryItem( item );
            if( inventoryItem == null )
                return false;

            return this.UseItem( inventoryItem );
        }

        /// <summary>
        /// Helper method that uses the ItemUseEffect of the specified <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="inventoryItem">
        /// The InventoryItem whose ItemUseEffect should be used.
        /// </param>
        /// <returns>
        /// Returns whether the item was actually used.
        /// </returns>
        private bool UseItem( InventoryItem inventoryItem )
        {
            ItemInstance itemInstance = inventoryItem.ItemInstance;

            if( itemInstance != null )
            {
                Item item = itemInstance.Item;
                ItemUseEffect useEffect = item.UseEffect;

                if( useEffect != null )
                {
                    if( useEffect.Use( this.owner ) )
                    {
                        if( useEffect.DestroyItemOnUse )
                        {
                            if( itemInstance.Count > 0 )
                                itemInstance.Count -= 1;

                            if( itemInstance.Count == 0 )
                            {
                                this.RemoveItem( inventoryItem );
                            }
                            else
                            {
                                this.Removed.Raise( this, itemInstance );
                            }
                        }

                        // All went great
                        ItemSounds.PlayRandomPickUpOrDown( itemInstance.Item, volumneMultiplicatorRange: new FloatRange( 0.8f, 1.0f ) );
                        return true;
                    }
                }
            }

            return false;
        }

        #region - UseRestoringItem -

        /// <summary>
        /// Uses the best fitting life restoring item
        /// that is currently in the inventory.
        /// </summary>
        public void UseLifeRestoringItem()
        {
            this.UsePowerRestoringItem( LifeMana.Life );
        }

        /// <summary>
        /// Uses the best fitting mana restoring item
        /// that is currently in the inventory.
        /// </summary>
        public void UseManaRestoringItem()
        {
            this.UsePowerRestoringItem( LifeMana.Mana );
        }

        /// <summary>
        /// Uses the best fitting power restoring item
        /// that is currently in the inventory.
        /// </summary>
        /// <param name="powerType">
        /// The power type to restore.
        /// </param>
        private void UsePowerRestoringItem( LifeMana powerType )
        {
            int healingNeeded = this.owner.Statable.GetRestoreNeeded( powerType );
            if( healingNeeded == 0 )
                return;

            var healingItems = this.GetRestoringItems( powerType );
            if( healingItems.Count == 0 )
                return;

            var bestItem = this.FindBestFittingRestoringItem( healingNeeded, powerType, healingItems );
            this.UseItem( bestItem );
        }

        /// <summary>
        /// Finds the best fitting life or mana restoring item from the given list of items.
        /// </summary>
        /// <param name="healingNeeded">
        /// The amount of healing needed to fully restore the entity.
        /// </param>
        /// <param name="powerType">
        /// The power type to restore.
        /// </param>
        /// <param name="healingItems">
        /// The list of items that can restore the entity.
        /// </param>
        /// <returns>
        /// The best suited restoring item.
        /// </returns>
        private InventoryItem FindBestFittingRestoringItem(
            int healingNeeded,
            LifeMana powerType,
            List<InventoryItem> healingItems )
        {
            var statable = this.owner.Statable;

            InventoryItem bestFittingItem = healingItems[0];
            int bestFittingItemHealingAverage = ((UseEffects.IRestoreEffect)bestFittingItem.Item.UseEffect).GetAverageAmountRestored( powerType, statable );
            int bestHealingDelta = healingNeeded - bestFittingItemHealingAverage;

            const float MissingHealingFitnessPenality = 1.5f;
            int bestHealingFitness = bestHealingDelta <= 0 ?
                -bestHealingDelta : (int)(bestHealingDelta * MissingHealingFitnessPenality);

            foreach( var item in healingItems )
            {
                var effect = (UseEffects.IRestoreEffect)item.Item.UseEffect;

                int healingAverage = effect.GetAverageAmountRestored( powerType, statable );
                int healingDelta = healingNeeded - healingAverage;
                int healingFitness = healingDelta <= 0 ?
                    -healingDelta : (int)(healingDelta * MissingHealingFitnessPenality);

                // HealingNeeded 100
                //                     Delta   Fitness
                // Item A heals  125   -25        25
                // Item B heals  80     20        20 * MissingHealingFitnessPenality => 30
                //                      ------> A gets choosen
                // lower fitness is better
                if( healingFitness < bestHealingFitness )
                {
                    // found a better fitting healing item
                    bestFittingItem = item;
                    bestHealingDelta = healingDelta;
                    bestHealingFitness = healingFitness;
                    bestFittingItemHealingAverage = healingAverage;
                }
            }

            return bestFittingItem;
        }

        /// <summary>
        /// Gets all items that restore the given power type.
        /// </summary>
        /// <param name="powerType">
        /// The power type to restore.
        /// </param>
        /// <returns>
        /// The list of matching items.
        /// </returns>
        private List<InventoryItem> GetRestoringItems( LifeMana powerType )
        {
            var statable = this.owner.Statable;

            return this.items.FindAll(
                ( inventoryItem ) => {
                    var item = inventoryItem.Item;

                    var effect = item.UseEffect as UseEffects.IRestoreEffect;
                    if( effect == null || !effect.IsFulfilledBy( this.owner ) )
                        return false;

                    return effect.GetAverageAmountRestored( powerType, statable ) > 0;
                }
            );
        }

        #endregion

        #endregion

        #region > Set <

        #region - SetNull -

        /// <summary>
        /// Helper method that sets the items in the area the specified InventoryItem covers to null.
        /// </summary>
        /// <param name="inventoryItem">
        /// The InventoryItem to 'erase' from the grid.
        /// </param>
        private void SetNull( InventoryItem inventoryItem )
        {
            Cell cell = inventoryItem.ParentCell;
            Item item = inventoryItem.Item;

            this.SetNull( cell.Position, item.InventorySize );
        }

        /// <summary>
        /// Helper method that sets the items of the grid in the specified area to null.
        /// </summary>
        /// <param name="start">The start position in grid-space.</param>
        /// <param name="size">The size of the area to set to null.</param>
        private void SetNull( Point2 start, Point2 size )
        {
            this.SetNull( start.X, start.Y, size.X, size.Y );
        }

        /// <summary>
        /// Helper method that sets the items of the grid in the specified area to null.
        /// </summary>
        /// <param name="startX">The start index on the x-axis in grid-space.</param>
        /// <param name="startY">The start index on the y-axis in grid-space.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        private void SetNull( int startX, int startY, int width, int height )
        {
            int endX = startX + width - 1;
            int endY = startY + height - 1;

            Debug.Assert( width >= 1 );
            Debug.Assert( height >= 1 );
            Debug.Assert( startX > 0 && startX < GridWidth );
            Debug.Assert( startY > 0 && startY < GridHeight );
            Debug.Assert( endX > 0 && endX < GridWidth );
            Debug.Assert( endY > 0 && endY < GridHeight );

            for( int x = startX; x <= endX; ++x )
            {
                for( int y = startY; y <= endY; ++y )
                {
                    cells[x][y].Item = null;
                }
            }
        }

        #endregion

        #region - SetAndAdd -

        /// <summary>
        /// Helper method that sets the item at the specified cell, also adding the item
        /// to the inventory's item-list.
        /// </summary>
        /// <param name="itemInstance">The item to set. May not be null.</param>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space). Must be a valid value.</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space). Must be a valid value.</param>
        private void SetAndAdd( ItemInstance itemInstance, int cellX, int cellY )
        {
            this.SetAndAdd( itemInstance, this.cells[cellX][cellY] );
        }

        /// <summary>
        /// Helper method that sets the item at the specified cell, also adding the item
        /// to the inventory's item-list.
        /// </summary>
        /// <param name="itemInstance">The item to set. May not be null.</param>
        /// <param name="cell">The starting cell to set the item at.</param>
        private void SetAndAdd( ItemInstance itemInstance, Cell cell )
        {
            this.SetAndAdd( new InventoryItem( itemInstance, cell ), cell );
        }

        /// <summary>
        /// Helper method that sets the item at the specified cell, 
        /// also adding the item to the inventory's item-list.
        /// </summary>
        /// <param name="inventoryItem">The item to set.</param>
        /// <param name="cell">The starting cell.</param>
        private void SetAndAdd( InventoryItem inventoryItem, Cell cell )
        {
            this.SetCells( inventoryItem, cell );
            this.AddToList( inventoryItem );
        }

        #endregion

        /// <summary>
        /// Sets the cells starting at the given Cell to the given InventoryItem.
        /// </summary>
        /// <param name="inventoryItem">The item to set.</param>
        /// <param name="cell">The starting cell.</param>
        private void SetCells( InventoryItem inventoryItem, Cell cell )
        {
            Item item = inventoryItem.Item;
            Debug.Assert( item.InventoryWidth >= 1 );
            Debug.Assert( item.InventoryHeight >= 1 );

            int cellX = cell.Position.X;
            int cellY = cell.Position.Y;
            int endX = cellX + item.InventoryWidth - 1;
            int endY = cellY + item.InventoryHeight - 1;

            Debug.Assert( endX < this.GridWidth );
            Debug.Assert( endY < this.GridHeight );

            for( int x = cellX; x <= endX; ++x )
            {
                for( int y = cellY; y <= endY; ++y )
                {
                    cells[x][y].Item = inventoryItem;
                }
            }
        }

        #endregion

        #region > Remove <

        #region - Remove -

        /// <summary>
        /// Tries to remove one item with the given <paramref name="itemName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemName"/> is null.
        /// </exception>
        /// <param name="itemName">
        /// The name of the item to remove.
        /// </param>
        /// <returns>
        /// Returns true if the items have been removed;
        /// or otherwise false if there don't exist
        /// enough items with the given <paramref name="itemName"/>.
        /// </returns>
        public bool Remove( string itemName )
        {
            return this.Remove( itemName, 1, null );
        }

        /// <summary>
        /// Tries to remove x <paramref name="amount"/> of the items
        /// with the given <paramref name="itemName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than zero.
        /// </exception>
        /// <param name="itemName">The name of the item to remove.</param>
        /// <param name="amount">The number of items to remove.</param>
        /// <returns>
        /// Returns true if the items have been removed;
        /// or otherwise false if there don't exist
        /// enough items with the given <paramref name="itemName"/>.
        /// </returns>
        public bool Remove( string itemName, int amount )
        {
            return this.Remove( itemName, amount, null );
        }

        /// <summary>
        /// Tries to remove x <paramref name="amount"/> of the items
        /// with the given <paramref name="itemName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than zero.
        /// </exception>
        /// <param name="itemName">The name of the item to remove.</param>
        /// <param name="amount">The number of items to remove.</param>
        /// <param name="modifiedItems">
        /// Will contain the ItemInstances that have been modified.
        /// </param>
        /// <returns>
        /// Returns true if the items have been removed;
        /// or otherwise false if there don't exist
        /// enough items with the given <paramref name="itemName"/>.
        /// </returns>
        public bool Remove( string itemName, int amount, IList<ItemInstance> modifiedItems )
        {
            #region - Verify -

            if( itemName == null )
                throw new ArgumentNullException( "itemName" );

            if( amount < 0 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "amount" );

            if( amount == 0 )
                return true;

            #endregion

            if( !this.Contains( itemName, amount ) )
                return false;

            for( int i = 0; i < this.items.Count; ++i )
            {
                if( amount == 0 )
                    break;

                InventoryItem inventoryItem = this.items[i];
                ItemInstance itemInstance = inventoryItem.ItemInstance;
                Item item = itemInstance.Item;

                if( itemName.Equals( item.Name, StringComparison.Ordinal ) )
                {
                    if( amount <= itemInstance.Count )
                    {
                        itemInstance.Count -= amount;
                        amount = 0;

                        if( itemInstance.Count == 0 )
                        {
                            this.RemoveItem( inventoryItem );
                            i = -1; // Restart the Items Loop.
                        }
                    }
                    else
                    {
                        amount -= itemInstance.Count;
                        itemInstance.Count = 0;

                        this.RemoveItem( inventoryItem );
                        i = -1;  // Restart the Items Loop.
                    }

                    if( modifiedItems != null )
                        modifiedItems.Add( itemInstance );
                }
            }

            Debug.Assert( amount == 0 );
            return true;
        }

        public bool Remove( ItemInstance itemInstance )
        {
            for( int i = 0; i < this.items.Count; ++i )
            {
                InventoryItem inventoryItem = this.items[i];

                if( inventoryItem.ItemInstance == itemInstance )
                {
                    this.RemoveItem( inventoryItem );
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to remove x <paramref name="amount"/> of the items
        /// with the given <paramref name="itemName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than zero.
        /// </exception>
        /// <param name="itemName">
        /// The name of the item to remove.
        /// </param>
        /// <param name="amount">
        /// The number of items to remove.
        /// </param>
        /// <param name="ignoreAffixed">
        /// States whether affixed items should be ignored, 
        /// and instead their base item should be checked.
        /// </param>
        /// <returns>
        /// Returns true if the items have been removed;
        /// or otherwise false if there don't exist
        /// enough items with the given <paramref name="itemName"/>.
        /// </returns>
        public bool Remove( string itemName, int amount, bool ignoreAffixed )
        {
            return this.Remove( itemName, amount, ignoreAffixed, null );
        }

        /// <summary>
        /// Tries to remove x <paramref name="amount"/> of the items
        /// with the given <paramref name="itemName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than zero.
        /// </exception>
        /// <param name="itemName">
        /// The name of the item to remove.
        /// </param>
        /// <param name="amount">
        /// The number of items to remove.
        /// </param>
        /// <param name="ignoreAffixed">
        /// States whether affixed items should be ignored, 
        /// and instead their base item should be checked.
        /// </param>
        /// <param name="modifiedItems">
        /// Will contain the ItemInstances that have been modified.
        /// </param>
        /// <returns>
        /// Returns true if the items have been removed;
        /// or otherwise false if there don't exist
        /// enough items with the given <paramref name="itemName"/>.
        /// </returns>
        public bool Remove( string itemName, int amount, bool ignoreAffixed, IList<ItemInstance> modifiedItems )
        {
            if( !ignoreAffixed )
                return this.Remove( itemName, amount, modifiedItems );

            #region - Verify -

            if( itemName == null )
                throw new ArgumentNullException( "itemName" );

            if( amount < 0 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "amount" );

            if( amount == 0 )
                return true;

            #endregion

            if( !this.Contains( itemName, amount, ignoreAffixed ) )
                return false;

            for( int i = 0; i < this.items.Count; ++i )
            {
                if( amount == 0 )
                    break;

                var inventoryItem = this.items[i];
                var itemInstance = inventoryItem.ItemInstance;
                var item = itemInstance.Item;

                bool removeItem = false;

                if( itemName.Equals( item.Name, StringComparison.Ordinal ) )
                {
                    removeItem = true;
                }
                else
                {
                    var affixedInstance = itemInstance as IAffixedItemInstance;

                    if( affixedInstance != null )
                    {
                        var baseItem = affixedInstance.AffixedItem.BaseItem;

                        if( baseItem.Name.Equals( itemName, StringComparison.Ordinal ) )
                        {
                            removeItem = true;
                        }
                    }
                }

                if( removeItem )
                {
                    if( amount <= itemInstance.Count )
                    {
                        itemInstance.Count -= amount;
                        amount = 0;

                        if( itemInstance.Count == 0 )
                        {
                            this.RemoveItem( inventoryItem );
                            i = -1; // Restart the Items Loop.
                        }
                    }
                    else
                    {
                        amount -= itemInstance.Count;
                        itemInstance.Count = 0;

                        this.RemoveItem( inventoryItem );
                        i = -1;  // Restart the Items Loop.
                    }

                    if( modifiedItems != null )
                        modifiedItems.Add( itemInstance );
                }
            }

            Debug.Assert( amount == 0 );
            return true;
        }

        public bool Remove( Predicate<Item> predicate, int amount, bool ignoreAffixed, IList<ItemInstance> modifiedItems )
        {
            #region - Verify -

            if( predicate == null )
                throw new ArgumentNullException( "predicate" );

            if( amount < 0 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "amount" );

            if( amount == 0 )
                return true;

            #endregion

            int amountFound;
            if( !this.Contains( predicate, amount, ignoreAffixed, out amountFound ) )
                return false;

            for( int i = 0; i < this.items.Count; ++i )
            {
                if( amount == 0 )
                    break;

                var inventoryItem = this.items[i];
                var itemInstance = inventoryItem.ItemInstance;
                var item = itemInstance.Item;

                bool removeItem = false;

                if( predicate( item ) )
                {
                    removeItem = true;
                }
                else if( ignoreAffixed )
                {
                    var affixedInstance = itemInstance as IAffixedItemInstance;

                    if( affixedInstance != null )
                    {
                        var baseItem = affixedInstance.AffixedItem.BaseItem;

                        if( predicate( baseItem ) )
                        {
                            removeItem = true;
                        }
                    }
                }

                if( removeItem )
                {
                    if( amount <= itemInstance.Count )
                    {
                        itemInstance.Count -= amount;
                        amount = 0;

                        if( itemInstance.Count == 0 )
                        {
                            this.RemoveItem( inventoryItem );
                            i = -1; // Restart the Items Loop.
                        }
                    }
                    else
                    {
                        amount -= itemInstance.Count;
                        itemInstance.Count = 0;

                        this.RemoveItem( inventoryItem );
                        i = -1;  // Restart the Items Loop.
                    }

                    if( modifiedItems != null )
                        modifiedItems.Add( itemInstance );
                }
            }

            Debug.Assert( amount == 0 );
            return true;
        }

        public void RemoveAll( IEnumerable<ItemInstance> items )
        {
            foreach( var item in items )
            {
                this.Remove( item );
            }
        }

        #endregion

        #region - RemoveAt -

        /// <summary>
        /// Tries to remove the item at the given cell.
        /// </summary>
        /// <param name="cellX">The position on the x-axis of the cell. (in cell-space)</param>
        /// <param name="cellY">The position on the y-axis of the cell. (in cell-space)</param>
        /// <param name="removedItemInstance">Will contain the removed item.</param>
        /// <returns>
        /// true if the Item at the given Cell has been removed;
        /// false if not.
        /// </returns>
        /// <exception cref="ArgumentException">If the given cell indices are invalid.</exception>
        public bool RemoveAt( int cellX, int cellY, out ItemInstance removedItemInstance )
        {
            ThrowOnInvalidCell( cellX, cellY );

            var inventoryItem = cells[cellX][cellY].Item;

            if( inventoryItem != null )
            {
                this.SetNull( inventoryItem );
                this.RemoveFromList( inventoryItem );

                removedItemInstance = inventoryItem.ItemInstance;
                return true;
            }
            else
            {
                removedItemInstance = null;
                return false;
            }
        }

        #endregion

        #region - RemoveItem -

        /// <summary>
        /// Helper method that removes the specified <see cref="InventoryItem"/> from the Inventory.
        /// </summary>
        /// <param name="inventoryItem">
        /// The InventoryItem to remove from this Inventory.
        /// </param>
        /// <returns>
        /// Returns true if the specified InventoryItem has been removed;
        /// otherwise false.
        /// </returns>
        private bool RemoveItem( InventoryItem inventoryItem )
        {
            if( inventoryItem == null )
                return false;

            if( this.RemoveFromList( inventoryItem ) )
            {
                this.SetNull( inventoryItem );
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region - Exchange -

        /// <summary>
        /// Tries to exchange the item at the given cell with the given <paramref name="itemInstance"/>.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to exchange with the ItemInstance at the given cell.
        /// </param>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="exchangedItemInstance">
        /// Will contain the <see cref="ItemInstance"/> that has been
        /// exchanged by the given <paramref name="itemInstance"/>.
        /// </param>
        /// <returns>
        /// Returns true if the ItemInstances have been successfully exchanged;
        /// otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="itemInstance"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        /// <exception cref="Atom.NotFoundException">If no item is in the given cell.</exception>
        public bool Exchange(
            ItemInstance itemInstance,
            int cellX,
            int cellY,
            out ItemInstance exchangedItemInstance )
        {
            InventoryItem exchangedInventoryItem;

            if( this.Exchange( itemInstance, cellX, cellY, out exchangedInventoryItem ) )
            {
                exchangedItemInstance = exchangedInventoryItem.ItemInstance;
                return true;
            }
            else
            {
                exchangedItemInstance = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to exchange the item at the given cell with the given <paramref name="itemInstance"/>.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to exchange with the ItemInstance at the given cell.
        /// </param>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="exchangedInventoryItem">
        /// Will contain the <see cref="InventoryItem"/> that has been
        /// exchanged by the given <paramref name="itemInstance"/>.
        /// </param>
        /// <returns>
        /// Returns true if the ItemInstances have been successfully exchanged;
        /// otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="itemInstance"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        /// <exception cref="Atom.NotFoundException">If no item is in the given cell.</exception>
        private bool Exchange(
            ItemInstance itemInstance,
            int cellX,
            int cellY,
            out InventoryItem exchangedInventoryItem )
        {
            exchangedInventoryItem = null;
            if( itemInstance == null )
                throw new ArgumentNullException( "itemInstance" );

            ThrowOnInvalidCell( cellX, cellY );

            var cell = this.cells[cellX][cellY];
            var existingItem = cell.Item;

            if( existingItem == null )
                throw new Atom.NotFoundException( Zelda.Resources.Error_NoItemIsInGivenCell );

            Cell existingItemParentCell = existingItem.ParentCell;

            // First lets remove the existing item in the cell
            this.SetNull( existingItem );

            // And then we try to insert the given item.
            if( this.InsertAt( itemInstance, cellX, cellY ) )
            {
                // Sucess !
                this.RemoveFromList( existingItem );
                exchangedInventoryItem = existingItem;

                return true;
            }
            else
            {
                // This failed, lets reinsert the existing item.
                this.SetCells( existingItem, existingItemParentCell );

                Debug.Assert( existingItemParentCell.Item == existingItem );
                return false;
            }
        }

        #endregion

        #region - Clear -

        /// <summary>
        /// Removes all items from the <see cref="Inventory"/>.
        /// </summary>
        protected void Clear()
        {
            this.items.Clear();

            for( int x = 0; x < this.GridWidth; ++x )
            {
                for( int y = 0; y < this.GridHeight; ++y )
                {
                    cells[x][y].Item = null;
                }
            }
        }

        #endregion

        #region > Organization <

        /// <summary>
        /// Adds the given InventoryItem to the list of items.
        /// </summary>
        /// <param name="inventoryItem">
        /// The InventoryItem to add.
        /// </param>
        private void AddToList( InventoryItem inventoryItem )
        {
            this.items.Add( inventoryItem );
            this.Added.Raise( this, inventoryItem.ItemInstance );
        }

        /// <summary>
        /// Tries to remove the given InventoryItem from the list of items.
        /// </summary>
        /// <param name="inventoryItem">
        /// The InventoryItem to remove.
        /// </param>
        /// <returns>
        /// true if the specified item has been removed from the internal
        /// list of items; otherwise false.
        /// </returns>
        private bool RemoveFromList( InventoryItem inventoryItem )
        {
            if( this.items.Remove( inventoryItem ) )
            {
                this.Removed.Raise( this, inventoryItem.ItemInstance );
                return true;
            }

            return false;
        }

        #endregion

        #region > Helper <

        /// <summary>
        /// Helper method that returns the 'other' Inventory.
        /// </summary>
        /// <returns>
        /// If the current inventory is the CraftingBottle then 
        /// the main Inventory is returned;
        /// otherwise if the current inventory is the main Inventory then 
        /// the CraftingBottle is returned.
        /// </returns>
        protected Inventory GetOtherInventory()
        {
            if( this == this.owner.CraftingBottle )
            {
                return this.owner.Inventory;
            }
            else
            {
                return this.owner.CraftingBottle;
            }
        }

        /// <summary>
        /// Gets whether the given cell position is valid.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <returns>
        /// true if the cell indices are valid;
        /// otherwise false.
        /// </returns>
        public bool IsValidCell( int cellX, int cellY )
        {
            if( cellX >= GridWidth || cellX < 0 )
                return false;
            if( cellY >= GridHeight || cellY < 0 )
                return false;

            return true;
        }

        /// <summary>
        /// Gets whether the given cell position is valid.
        /// </summary>
        /// <param name="cell">
        /// The position of the cell (in cell space).
        /// </param>
        /// <returns>
        /// true if the cell indices are valid;
        /// otherwise false.
        /// </returns>
        public bool IsValidCell( Point2 cell )
        {
            return IsValidCell( cell.X, cell.Y );
        }

        #region - IsValidCellArea -

        /// <summary>
        /// Gets whether the given cell position is valid.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="cellWidth">The width of the cell area (in cell space).</param>
        /// <param name="cellHeight">The height of the cell area (in cell space).</param>
        /// <returns>
        /// true if the cell indices are valid;
        /// otherwise false.
        /// </returns>
        public bool IsValidCellArea( int cellX, int cellY, int cellWidth, int cellHeight )
        {
            if( (cellX + cellWidth) > this.GridWidth || cellX < 0 )
                return false;
            if( (cellY + cellHeight) > this.GridHeight || cellY < 0 )
                return false;

            return true;
        }

        #endregion

        #region - ThrowOnInvalidCell -

        /// <summary>
        /// Helper method that throws <see cref="ArgumentOutOfRangeException"/>
        /// if any of the given cell indices is invalid.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ThrowOnInvalidCell( int cellX, int cellY )
        {
            if( cellX > GridWidth || cellX < 0 )
                throw new ArgumentOutOfRangeException( "cellX", cellX, Zelda.Resources.Error_GivenCellIndexIsInvalid );

            if( cellY > GridHeight || cellY < 0 )
                throw new ArgumentOutOfRangeException( "cellY", cellY, Zelda.Resources.Error_GivenCellIndexIsInvalid );
        }

        #endregion

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes/Writes the data of this <see cref="Inventory"/>
        /// using the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );
            context.Write( this.items.Count );

            foreach( InventoryItem item in items )
            {
                Cell cell = item.ParentCell;

                context.Write( cell.Position.X );
                context.Write( cell.Position.Y );
                item.ItemInstance.Serialize( context );
            }
        }

        /// <summary>
        /// Deserializes/Reads the data of this <see cref="Inventory"/>
        /// using the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.Clear();

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, "Zelda.Items.Inventory" );

            int itemCount = context.ReadInt32();
            this.items.Capacity = itemCount;

            for( int i = 0; i < itemCount; ++i )
            {
                int cellPositionX = context.ReadInt32();
                int cellPositionY = context.ReadInt32();
                var itemInstance = ItemInstance.Read( context );

                this.FailSafeInsertAt( itemInstance, cellPositionX, cellPositionY );
            }
        }

        #endregion

        #region > Handle Input <

        #region - HandleLeftClick -

        /// <summary>
        /// Handles the left click on the given Inventory Cell.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="isShiftDown">States whether the player is pressing the Shift key.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        public void HandleLeftClick( int cellX, int cellY, bool isShiftDown )
        {
            if( this.owner.IsDead )
                return;

            InventoryItem cellInventoryItem = this.GetInventoryItemAt( cellX, cellY );
            ItemInstance cellItemInstance = cellInventoryItem != null ? cellInventoryItem.ItemInstance : null;

            ItemInstance pickedUpItemInstance = this.owner.PickedupItemContainer.Item;

            /* On Left Click the following can happen:
             * a) player has no Item picked up, and there is no Item in the cell.
             *    --> do nothing.
             * b) player has an Item picked up, and there is no Item in the cell.
             *    --> insert item at the cell, if possible.
             * c) player has no Item picked up, and there is an Item in the cell.
             *    --> pickup the item from the inventory
             * d) player has an Item picked up, and there is an Item in the cell.
             *  1.--> see whether the items are the same and whether we could put some
             *        from the 'hand' onto the stack.
             *  2.--> if the picked-up item is a gem and the cell item has a free socket,
             *        then place the gem in the socket.
             *  3.--> Exchange the items if possible.
             */

            // case a)
            if( pickedUpItemInstance == null && cellItemInstance == null )
                return;

            // case b)
            if( pickedUpItemInstance != null && cellItemInstance == null )
            {
                ItemInstance oldItemInstance;
                if( this.InsertAt( pickedUpItemInstance, cellX, cellY, out oldItemInstance ) )
                {
                    ItemSounds.PlayPutDown( pickedUpItemInstance.Item );

                    // Pickedup Item was inserted into the this.
                    this.owner.PickedupItemContainer.Pick( oldItemInstance );
                }

                return;
            }

            // case c)
            if( pickedUpItemInstance == null && cellItemInstance != null )
            {
                ItemInstance dummy;
                if( this.RemoveAt( cellX, cellY, out dummy ) )
                {
                    Debug.Assert( dummy == cellItemInstance );

                    if( isShiftDown )
                    {
                        SwapItemsOnShiftLeftClick( dummy, cellInventoryItem.ParentCell.Position );
                    }
                    else
                    {
                        this.owner.PickedupItemContainer.Pick( dummy, cellInventoryItem.ParentCell.Position );
                    }

                    ItemSounds.PlayPickUp( dummy.Item );
                }

                return;
            }

            // case d) 
            if( pickedUpItemInstance != null && cellItemInstance != null )
            {
                Item pickedUpItem = pickedUpItemInstance.Item;
                Item cellItem = cellItemInstance.Item;

                if( pickedUpItemInstance == cellItemInstance )
                    return;

                // case d1)
                if( pickedUpItem.Name.Equals( cellItem.Name, StringComparison.Ordinal ) )
                {
                    System.Diagnostics.Debug.Assert( pickedUpItem == cellItem );

                    int totalCount = cellItemInstance.Count + pickedUpItemInstance.Count;
                    int allowedToAdd = cellItem.StackSize - cellItemInstance.Count;
                    int added = (allowedToAdd >= pickedUpItemInstance.Count) ?
                                       pickedUpItemInstance.Count : allowedToAdd;

                    cellItemInstance.Count += added;
                    pickedUpItemInstance.Count -= added;

                    ItemSounds.PlayPutDown( pickedUpItemInstance.Item );
                    this.owner.PickedupItemContainer.DestroyIfEmpty();
                }
                else
                {
                    var gemInstance = pickedUpItemInstance as GemInstance;
                    var cellEquipmentInstance = cellItemInstance as EquipmentInstance;

                    if( gemInstance != null && cellEquipmentInstance != null )
                    {
                        // case d2)
                        Point2 socketPosition = this.GetRelativeSocketPositionAt( cellX, cellY );

                        var socketProperties = cellEquipmentInstance.SocketProperties;
                        if( socketProperties.Insert( socketPosition, gemInstance, this.owner.Statable ) )
                        {
                            this.owner.PickedupItemContainer.Destroy();
                            ItemSounds.PlayPutDown( gemInstance.Item );
                            return;
                        }
                    }

                    // case d3)
                    InventoryItem dummy;

                    if( this.Exchange( pickedUpItemInstance, cellX, cellY, out dummy ) )
                    {
                        ItemSounds.PlayPutDown( pickedUpItemInstance.Item );

                        System.Diagnostics.Debug.Assert( dummy.ItemInstance == cellItemInstance );
                        this.owner.PickedupItemContainer.Pick( cellItemInstance, dummy.ParentCell.Position );
                    }
                }

                return;
            }
        }

        /// <summary>
        /// Tries to get the relative Socket position in the cell with the given position.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis.</param>
        /// <param name="cellY">The position of the cell on the y-axis.</param>
        /// <returns>
        /// The requested Socket position; or (-1; -1).
        /// </returns>
        private Point2 GetRelativeSocketPositionAt( int cellX, int cellY )
        {
            Cell cell = this.cells[cellX][cellY];

            var inventoryItem = cell.Item;
            if( inventoryItem == null )
                return -Point2.One;

            var equipmentInstance = inventoryItem.ItemInstance as EquipmentInstance;
            if( equipmentInstance == null )
                return -Point2.One;

            return cell.Position - inventoryItem.ParentCell.Position;
        }

        /// <summary>
        /// Handles the case of the user left-clicking on an item in the Inventory
        /// while the Shift key is down.
        /// </summary>
        /// <remarks>
        /// The default behaviour is to move the item into the magic crafting bottle.
        /// </remarks>
        /// <param name="item">The related item.</param>
        /// <param name="cell">The original position of the <paramref name="item"/> (in cell-space).</param>
        protected virtual void SwapItemsOnShiftLeftClick( ItemInstance item, Point2 cell )
        {
            if( !this.owner.CraftingBottle.Insert( item ) )
                this.InsertAt( item, cell );
        }

        #endregion

        #region - HandleRightClick -

        /// <summary>
        /// Handles the right click on given Inventory Cell.
        /// </summary>
        /// <param name="cellX">The position of the cell on the x-axis (in cell space).</param>
        /// <param name="cellY">The position of the cell on the y-axis (in cell space).</param>
        /// <param name="isCtrlDown">States whether the control modifier key is pressed.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the given cell indices are invalid.</exception>
        public void HandleRightClick( int cellX, int cellY, bool isCtrlDown )
        {
            if( this.owner.IsDead )
                return;

            ThrowOnInvalidCell( cellX, cellY );

            Cell cell = cells[cellX][cellY];
            InventoryItem cellInventoryItem = cell.Item;
            if( cellInventoryItem == null )
                return;

            ItemInstance cellItemInstance = cellInventoryItem.ItemInstance;

            /* On Right Click the following can happen:
             * a) There is no Item in the cell.
             *    --> do nothing.
             * b) Item is in the cell
             *    --> use the item if possible
             *    --> or try to equip it
             */

            // case a)
            if( cellItemInstance == null )
                return;

            // case b)
            // if( cellItemInstance != null )
            {
                if( this.UseItem( cellInventoryItem ) )
                    return;

                // Try to equip it if it is an equipment.
                var equipInstance = cellItemInstance as EquipmentInstance;

                if( equipInstance != null )
                {
                    this.TryEquip( equipInstance, cell, isCtrlDown );
                }
            }
        }

        /// <summary>
        /// Tries to equip the given EquipmentInstance that is part of this Inventory.
        /// </summary>
        /// <param name="equipInstance">The EquipmentInstance to equip.</param>
        /// <param name="cell">The cell that contains the EquipmentInstance.</param>
        /// <param name="preferSecondSlot">
        /// States whether the equipment should be equiped into the secondary slot if possible.
        /// </param>
        private void TryEquip( EquipmentInstance equipInstance, Cell cell, bool preferSecondSlot )
        {
            var equipmentStatus = this.owner.Equipment;

            EquipmentStatusSlot slot = equipmentStatus.GetEmptySlotForItem( equipInstance.Equipment.Slot, preferSecondSlot );

            EquipmentInstance old;
            if( equipmentStatus.Equip( equipInstance, slot, out old ) )
            {
                this.RemoveItem( cell.Item );

                if( old != null && !this.Insert( old ) )
                {
                    // Doh! This is going bad.

                    // Lets try the easiest solution first:
                    if( this.owner.PickedupItemContainer.Item != null )
                    {
                        this.owner.PickedupItemContainer.Pick( old );
                    }
                    else
                    {
                        // As a last resort try to undo the exchange.

                        // Reequip
                        EquipmentInstance dummy;
                        if( !equipmentStatus.Equip( old, slot, out dummy ) )
                        {
                            Debug.Fail( "Critical error occured while trying to swap items, couldn't recover." );
                        }

                        if( !this.Insert( equipInstance ) )
                        {
                            Debug.Fail( "Critical error occured while trying to swap items, couldn't recover." );
                        }
                    }
                }
                else
                {
                    // All went great
                    ItemSounds.PlayPutDown( equipInstance.Item, volumneMultiplicator: 0.8f );
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dimensions of the cells (in cell space).
        /// </summary>
        private readonly int gridWidth, gridHeight;

        /// <summary>
        /// The inventory cells.
        /// </summary>
        private readonly Cell[][] cells;

        /// <summary>
        /// Reference to the <see cref="PlayerEntity"/> that owns 
        /// the <see cref="Inventory"/>.
        /// </summary>
        private readonly PlayerEntity owner;

        /// <summary>
        /// Contains the items currently in the inventory.
        /// </summary>
        private readonly List<InventoryItem> items = new List<InventoryItem>();

        /// <summary>
        /// Holds the items that couldn't be inserted into the ivnentory; even when they should have.
        /// It's a backup so that the player doesn't lose items. E.g. when changing the inventory size of an item.
        /// </summary>
        private readonly List<ItemInstance> itemBackup = new List<ItemInstance>();

        #endregion

        #region [ class Cell ]

        /// <summary>
        /// Defines a single cell in the <see cref="Inventory"/>'s grid.
        /// </summary>
        protected class Cell
        {
            /// <summary>
            /// The position of the <see cref="Cell"/> in the <see cref="Inventory"/>'s grid.
            /// This is a read-only field.
            /// </summary>
            public readonly Point2 Position;

            /// <summary>
            /// Gets or sets the item this <see cref="Cell"/> contains, if any.
            /// </summary>
            public InventoryItem Item
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of items on the item stack in this cell. 
            /// </summary>
            public int ItemCount
            {
                get
                {
                    return this.Item == null ? 0 : this.Item.ItemInstance.Count;
                }

                set
                {
                    if( this.Item == null )
                        throw new System.InvalidOperationException( Zelda.Resources.Error_ItemIsNull );

                    this.Item.ItemInstance.Count = value;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cell"/> class. 
            /// </summary>
            /// <param name="x">
            /// The position of the <see cref="Cell"/> in
            /// the <see cref="Inventory"/>'s grid on the x-axis.
            /// </param>
            /// <param name="y">
            /// The position of the <see cref="Cell"/> in
            /// the <see cref="Inventory"/>'s grid on the y-axis.
            /// </param>
            public Cell( int x, int y )
            {
                this.Position = new Point2( x, y );
            }
        }

        #endregion

        #region [ class IventoryItem ]

        /// <summary>
        /// Defines the container object that stores
        /// an <see cref="ItemInstance"/> that is
        /// inside of the <see cref="Inventory"/>.
        /// </summary>
        protected class InventoryItem
        {
            /// <summary>
            /// The underlying <see cref="ItemInstance"/> object.
            /// </summary>
            public readonly ItemInstance ItemInstance;

            /// <summary>
            /// The parent cell this item is in. (upper left corner)
            /// </summary>
            public readonly Cell ParentCell;

            /// <summary>
            /// Gets the definition Item of the <see cref="ItemInstance"/>.
            /// </summary>
            public Item BaseItem
            {
                get { return ItemInstance.BaseItem; }
            }

            /// <summary>
            /// Gets the Item of the <see cref="ItemInstance"/>.
            /// </summary>
            public Item Item
            {
                get { return ItemInstance.Item; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InventoryItem"/> class.
            /// </summary>
            /// <exception cref="ArgumentNullException">
            /// If <paramref name="itemInstance"/> or <paramref name="parentCell"/> is null.
            /// </exception>
            /// <param name="itemInstance">
            /// The ItemInstance that is part of the new InventoryItem.
            /// </param>
            /// <param name="parentCell">
            /// The Cell the new InventoryItem is placed in.
            /// </param>
            public InventoryItem( ItemInstance itemInstance, Cell parentCell )
            {
                if( itemInstance == null )
                    throw new ArgumentNullException( "itemInstance" );
                if( parentCell == null )
                    throw new ArgumentNullException( "parentCell" );

                this.ItemInstance = itemInstance;
                this.ParentCell = parentCell;
            }

            public override string ToString()
            {
                return string.Format( "{0} ({1})", this.Item.Name, this.ParentCell.Position );
            }
        }

        #endregion
    }
}