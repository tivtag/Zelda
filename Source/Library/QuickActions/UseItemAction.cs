// <copyright file="UseItemAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuickActions.UseItemAction class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuickActions
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Xna;
    using Zelda.Items;

    /// <summary>
    /// Represents an <see cref="IQuickAction"/> that uses an <see cref="Item"/>.
    /// </summary>
    public sealed class UseItemAction : IQuickAction, IDisposable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this IQuickAction is active;
        /// and as such executeable.
        /// </summary>
        public bool IsActive
        {
            get 
            {
                return this.hasItem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction is executeable.
        /// </summary>
        public bool IsExecuteable
        {
            get
            {
                return this.hasItem && this.useEffect.IsReady;
            }
        }

        /// <summary>
        /// Gets the time left (in seconds) until this IQuickAction can be executed
        /// again.
        /// </summary>
        public float CooldownLeft
        {
            get 
            {
                return this.useEffect.Cooldown.TimeLeft;
            }
        }

        /// <summary>
        /// Gets the time (in seconds) this IQuickAction can't be executed again after executing it.
        /// </summary>
        public float CooldownTotal
        {
            get
            {
                return this.useEffect.Cooldown.TotalTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction executeability is only
        /// limited by the cooldown.
        /// </summary>
        public bool IsOnlyLimitedByCooldown
        {
            get
            { 
                return true;
            }
        }

        /// <summary>
        /// Gets the symbol associated with this IQuickAction.
        /// </summary>
        public Atom.Xna.ISprite Symbol
        {
            get
            { 
                return this.sprite;
            }
        }

        /// <summary>
        /// Gets the Color the <see cref="Symbol"/> of this IQuickAction is tinted in.
        /// </summary>
        public Microsoft.Xna.Framework.Color SymbolColor 
        {
            get
            {
                return this.item.SpriteColor;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the UseItemAction class.
        /// </summary>
        /// <param name="item">
        /// The item to use.
        /// </param>
        /// <param name="inventory">
        /// The Inventory that contains the Item to use.
        /// </param>
        public UseItemAction( Item item, Inventory inventory )
        {
            this.Setup( item, inventory );
        }

        /// <summary>
        /// Initializes a new instance of the UseItemAction class;
        /// used for deserialization.
        /// </summary>
        public UseItemAction()
        {
        }

        /// <summary>
        /// Setups a new instance of the UseItemAction class.
        /// </summary>
        /// <param name="item">
        /// The item to use.
        /// </param>
        /// <param name="inventory">
        /// The Inventory that contains the Item to use.
        /// </param>
        private void Setup( Item item, Inventory inventory )
        {
            Contract.Requires<ArgumentException>( item.UseEffect != null );

            this.item = item;
            this.sprite = item.Sprite.CreateInstance();
            this.useEffect = item.UseEffect;
            this.inventory = inventory;

            this.inventory.Added += OnItemAdded;
            this.inventory.Removed += OnItemRemoved;

            this.hasItem = inventory.Contains( item );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Executes this UseItemQuickAction.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wants to use this IQuickAction.
        /// </param>
        /// <returns>
        /// Whether this IQuickAction has been executed.
        /// </returns>
        public bool Execute( Zelda.Entities.PlayerEntity user )
        {
            if( !this.IsExecuteable )
                return false;
            
            if( this.inventory.UseItem( this.item ) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when an item was added to the Inventory.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="item">The item that was removed from the Inventory.</param>
        private void OnItemAdded( object sender, ItemInstance item )
        {
            if( !this.hasItem && item.BaseItem == this.item )
            {
                this.hasItem = true;
            }
        }

        /// <summary>
        /// Called when an item was removed from the Inventory.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="item">The item that was added to the Inventory.</param>
        private void OnItemRemoved( object sender, ItemInstance item )
        {
            if( this.hasItem && item.BaseItem == this.item )
            {
                this.hasItem = this.inventory.Contains( this.item.Name );
            }
        }

        /// <summary>
        /// Serializes this IQuickAction using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.item.Name );
        }

        /// <summary>
        /// Deserializes this IQuickAction using the given BinaryReader.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity whose action is executed by this IQuickAction.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Entities.PlayerEntity player, Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            string itemName = context.ReadString();

            var itemManager = context.ServiceProvider.ItemManager;
            Item item = itemManager.Get( itemName );

            this.Setup( item, player.Inventory );
        }

        /// <summary>
        /// Disposes the (managed) resources aquired by this UseItemQuickAction.
        /// </summary>
        public void Dispose()
        {
            if( this.inventory != null )
            {
                this.inventory.Added -= this.OnItemAdded;
                this.inventory.Removed -= this.OnItemRemoved;
            }

            this.item = null;
            this.useEffect = null;
            this.inventory = null;
            this.hasItem = false;
            this.sprite = null;
        }

        /// <summary>
        /// Updates this UseItemAction.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            var updateable = this.sprite as IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the Item associated with this UseItemQuickAction.
        /// </summary>
        private Item item;

        /// <summary>
        /// The sprite used to draw this UseItemAction.
        /// </summary>
        private ISprite sprite;

        /// <summary>
        /// Identifies the ItemUseEffect associated with the Item.
        /// </summary>
        private ItemUseEffect useEffect;

        /// <summary>
        /// States whether the inventory currently contains atleast one Item.
        /// </summary>
        private bool hasItem;

        /// <summary>
        /// Identifies the Inventory that contains the Item to be used.
        /// </summary>
        private Inventory inventory;

        #endregion
    }
}
