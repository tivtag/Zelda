// <copyright file="ItemInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemInstance class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items
{
    using System;
using System.Diagnostics.Contracts;
using Atom;
using Atom.Xna;
using Microsoft.Xna.Framework.Graphics;
using Zelda.Items.Affixes;
using Zelda.Saving;

    /// <summary>
    /// An <see cref="ItemInstance"/> is a single instance of an <see cref="Item"/>.
    /// This is needed to allow multiple <see cref="Item"/>s of the same type.
    /// </summary>
    public class ItemInstance
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Item"/> that is encapsulated by the <see cref="ItemInstance"/>.
        /// </summary>
        public Item Item
        {
            get
            {
                return this.modifiedItem;
            }
        }

        /// <summary>
        /// Gets the <see cref="Item"/> template that is encapsulated by the <see cref="ItemInstance"/>.
        /// </summary>
        public Item BaseItem
        {
            get
            {
                return this.baseItem;
            }
        }

        /// <summary>
        /// Gets or sets the number of <see cref="Item"/>s on the 'Item Stack'.
        /// </summary>
        /// <value>The default value is 1.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Set: If the given value is out of valid range. 0 &lt;= value &lt; Item.StackSize
        /// </exception>
        public int Count
        {
            get
            { 
                return this.count;
            }

            set
            {
                if( value < 0 || value > baseItem.StackSize )
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value, 
                        Resources.Error_GivenItemCountOutOfValidStackRange
                    );
                }

                this.count = value;
            }
        }

        /// <summary>
        /// Gets the ISprite instance of the ISpriteAsset of the Item.
        /// </summary>
        public ISprite Sprite
        {
            get
            {
                return this.sprite;
            }
        }

        /// <summary>
        /// Gets the SpriteColor of the Item.
        /// </summary>
        public Microsoft.Xna.Framework.Color SpriteColor
        {
            get
            {
                return this.baseItem.SpriteColor;
            }
        }

        /// <summary>
        /// Gets or sets the factor by which the power of this ItemInstance varies compared to the base Item.
        /// </summary>
        public float PowerFactor
        {
            get
            {
                return this.powerFactor;
            }

            set
            {
                this.powerFactor = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInstance"/> class.
        /// </summary>
        /// <param name="baseItem">
        /// The underlying <see cref="Item"/>.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of this ItemInstance varies compared to the base Item.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseItem"/> is null.
        /// </exception>
        internal ItemInstance( Item baseItem, float powerFactor )
        {
            Contract.Requires<ArgumentNullException>( baseItem != null );

            this.baseItem = baseItem;
            
            this.modifiedItem = baseItem.Clone();
            this.modifiedItem.ModifyPowerBy( powerFactor );
            
            this.powerFactor = powerFactor;
            this.sprite = baseItem.Sprite != null ? baseItem.Sprite.CreateInstance() : null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this ItemInstance; animating the sprite.
        /// </summary>
        /// <param name="updateContext">
        /// The current update context.
        /// </param>
        public void Update( IUpdateContext updateContext )
        {
            var updateable = this.sprite as IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( (int)ItemType.Item );

            context.WriteDefaultHeader(); // New in File V4
            context.Write( baseItem.Name );
            context.Write( this.count );
            context.Write( this.powerFactor );
        }

        /// <summary>
        /// Deserializes the data required to create an <see cref="ItemInstance"/>.
        /// Warning! this reads *only* Items. Use ItemInstance.Read to read any item type.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new ItemInstance object.
        /// </returns>
        public static ItemInstance ReadItem( Zelda.Saving.IZeldaDeserializationContext context )
        {
            // The typename has been readen at this point.
            var itemManager = context.ServiceProvider.ItemManager;

            int version;

            if( context.Version >= 4 )
            {
                context.ReadDefaultHeader( "ItemInstance.ReadItem" );
                version = 1;
            }
            else
            {
                version = 0;
            }

            string itemName = context.ReadString();
            int itemCount = context.ReadInt32();
            float powerFactor;

            if( version >= 1 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            Item item = itemManager.Get( itemName );
            if( item == null )
                return null;

            return new ItemInstance( item, powerFactor ) {
                count = itemCount
            };
        }

        /// <summary>
        /// Reads the next <see cref="ItemInstance"/>, <see cref="EquipmentInstance"/>, 
        /// <see cref="WeaponInstance"/> or <see cref="GemInstance"/> from the specified BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new ItemInstance object.
        /// </returns>
        public static ItemInstance Read( Zelda.Saving.IZeldaDeserializationContext context )
        {
            ItemType itemType = (ItemType)context.ReadInt32();

            switch( itemType )
            {
                case ItemType.Item:
                    return ItemInstance.ReadItem( context );

                case ItemType.Equipment:
                    return EquipmentInstance.ReadEquipment( context );

                case ItemType.Weapon:
                    return WeaponInstance.ReadWeapon( context );
              
                case ItemType.Gem:
                    return GemInstance.ReadGem( context );

                case ItemType.AffixedEquipment:
                    return AffixedEquipmentInstance.ReadAffixedEquipment( context );

                case ItemType.AffixedWeapon:
                    return AffixedWeaponInstance.ReadAffixedWeapon( context );

                default:
                    throw new System.NotImplementedException();
            }
        }
        
        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of <see cref="Item"/>s.
        /// </summary>
        private int count = 1;

        /// <summary>
        /// The factor by which the power of this ItemInstance varies compared to the base Item.
        /// </summary>
        private float powerFactor;

        /// <summary>
        /// The item template that is encapsulated by this <see cref="ItemInstance"/>.
        /// </summary>
        private readonly Item baseItem;

        /// <summary>
        /// The item that is encapsulated by this <see cref="ItemInstance"/>.
        /// </summary>
        private readonly Item modifiedItem;

        /// <summary>
        /// The ISprite instance of the ISpriteAsset of the Item.
        /// </summary>
        private readonly ISprite sprite;

        #endregion
    }
}