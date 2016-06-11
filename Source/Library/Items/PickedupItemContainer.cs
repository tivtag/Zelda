// <copyright file="PickedupItemContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.PickedupItemContainer class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using Atom;
    using Atom.Math;
    using Zelda.Entities;
    
    /// <summary>
    /// Stores the Item which the player currently has picked up.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PickedupItemContainer
    {
        /// <summary>
        /// Raised when a picked-up Item has been dropped.
        /// </summary>
        public event RelaxedEventHandler<Item> Dropped;

        /// <summary>
        /// Gets the <see cref="ItemInstance"/> the <see cref="PlayerEntity"/> has currently picked-up.
        /// </summary>
        /// <value>The default value is null.</value>
        public ItemInstance Item
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the position of the Item in the Inventory before it was picked-up.
        /// </summary>
        private Atom.Math.Point2 OriginalPosition
        {
            get;
            set; 
        }

        /// <summary>
        /// Gets a value indicating whether this PickedUpItemContainer is empty;
        /// and as such doesn't contain an Item.
        /// </summary>
        public bool IsEmpty 
        {
            get 
            {
                return this.Item == null;
            } 
        }

        /// <summary>
        /// The PlayerEntity that owns this PickedupItemContainer.
        /// </summary>
        public readonly PlayerEntity Owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickedupItemContainer"/> class.
        /// </summary>
        /// <param name="owner">The owner of the new PickedupItemContainer.</param>
        public PickedupItemContainer( Zelda.Entities.PlayerEntity owner )
        {
            if( owner == null )
                throw new ArgumentNullException( "owner" );

            this.Owner = owner;
        }

        /// <summary>
        /// Picks-up the speceified ItemInstance; disgarding
        /// the currently picked-up ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance that should be picked-up.
        /// </param>
        public void Pick( ItemInstance itemInstance )
        {
            this.Pick( itemInstance, Inventory.InvalidCell );
        }

        /// <summary>
        /// Picks-up the speceified ItemInstance; disgarding
        /// the currently picked-up ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance that should be picked-up.
        /// </param>
        /// <param name="originalPosition">
        /// The position of the inventory cell the ItemInstance was placed
        /// in before it was picked-up.
        /// </param>
        public void Pick( ItemInstance itemInstance, Point2 originalPosition )
        {
            this.Item = itemInstance;
            this.OriginalPosition = originalPosition;
        }

        /// <summary>
        /// Drops the currently picked-up <see cref="Item"/> onto the floor.
        /// </summary>
        public void Drop()
        {
            var itemInstance = this.Item;
            if( itemInstance == null )
                return;

            this.DropMapItem( itemInstance );

            this.Dropped.Raise( this, itemInstance.Item );
            this.Item = null;
        }

        /// <summary>
        /// Drops a MapItem under the feet of the player.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to drop on the map.
        /// </param>
        private void DropMapItem( ItemInstance itemInstance )
        {
            ItemSounds.PlayPutDown( itemInstance.Item );
            MapItem.SpawnUnder( this.Owner, itemInstance );
        }

        /// <summary>
        /// Restores the currently picked-up <see cref="Item"/> back into the Inventory.
        /// </summary>
        /// <returns>
        /// Returns whether the Item was sucessfully restored.
        /// </returns>
        public bool Restore()
        {
            if( this.Item == null )
                return false;

            if( this.TryRestoreAtOriginalPosition() || this.Owner.Inventory.Insert( this.Item ) )
            {
                this.Item = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the picked-up item if its Count is zero.
        /// </summary>
        internal void DestroyIfEmpty()
        {
            if( this.Item == null )
                return;

            if( this.Item.Count == 0 )
            {
                this.Destroy();
            }
        }

        /// <summary>
        /// Removes the currently picked-up item.
        /// </summary>
        internal void Destroy()
        {
            this.Item = null;
            this.OriginalPosition = Inventory.InvalidCell;
        }

        /// <summary>
        /// Tries to restore the picked-up item to its original position in the inventory.
        /// </summary>
        /// <returns>
        /// true if it was restored;
        /// otherwise false.
        /// </returns>
        private bool TryRestoreAtOriginalPosition()
        {
            if( this.OriginalPosition != Inventory.InvalidCell )
            {
                return this.Owner.Inventory.InsertAt( this.Item, this.OriginalPosition );
            }

            return false;
        }
    }
}
