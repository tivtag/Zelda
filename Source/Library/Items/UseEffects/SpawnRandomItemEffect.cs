// <copyright file="SpawnRandomItemEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.SpawnRandomItemEffect class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.UseEffects
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Atom.Math;
    
    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that when triggered 
    /// spawns a random item.
    /// </summary>
    /// <remarks>
    /// Usually used by treasure chests.
    /// </remarks>
    public sealed class SpawnRandomItemEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the minimum allowed level an item spawned by this
        /// ItemUseEffectSpawnRandomItem can have.
        /// </summary>
        public int MinimumItemLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum allowed level an item spawned by this
        /// ItemUseEffectSpawnRandomItem can have.
        /// </summary>
        /// <value>
        /// Any item can be spawned it the MaximumItemLevel is set to 0.
        /// </value>
        public int MaximumItemLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ItemType of the items spawned by this SpawnRandomItemEffect are limited to.
        /// </summary>
        /// <value>The default value is ItemType.None; this means no limitation.</value>
        public ItemType ItemType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Zelda.Status.Statable statable )
        {
            if( this.ItemType == ItemType.Gem )
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    ItemResources.IUE_SpawnRandomGemMinLevelXMaxLevelY,
                    this.MinimumItemLevel.ToString( CultureInfo.CurrentCulture ),
                    this.MaximumItemLevel.ToString( CultureInfo.CurrentCulture )
                );
            }

            if( this.MaximumItemLevel != 0 )
            {
                return string.Format( 
                    CultureInfo.CurrentCulture,
                    ItemResources.IUE_SpawnRandomItemMinLevelXMaxLevelY,
                    this.MinimumItemLevel.ToString( CultureInfo.CurrentCulture ),
                    this.MaximumItemLevel.ToString( CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return ItemResources.IUE_SpawnRandomItem;
            }
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get 
            {
                return this.MaximumItemLevel == 0 ? 50 : this.MaximumItemLevel * 0.8;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Zelda.Entities.PlayerEntity user )
        {
            var item = this.GetRandomItem( user );
            if( item == null )
                return false;

            this.CreateAndSpawnItem( item, user );     
            return true;
        }

        /// <summary>
        /// Randomly picks an item and returns it.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>The Item that has been picked.</returns>
        private Item GetRandomItem( Zelda.Entities.PlayerEntity user )
        {
            var itemFiles = this.GetItemFiles();

            Item item;
            bool foundItem = false;

            do
            {
                // Randomly pick an item name.
                item = this.PickItem( itemFiles );

                // Verify.
                foundItem = this.Verify( item );

                // Additionaly check whether the player 
                // fulfills the requirements for the item.
                if( foundItem )
                {
                    var requirement = item.DropRequirement;

                    if( requirement != null )
                    {
                        foundItem = requirement.IsFulfilledBy( user );
                    }
                }
            } 
            while( !foundItem );

            return item;
        }

        /// <summary>
        /// Gets the item filenames from which an item should be randomly picked.
        /// </summary>
        /// <returns>
        /// An array of file names.
        /// </returns>
        private string[] GetItemFiles()
        {
            string[] files = Directory.GetFiles( "Content\\Items" );

            // Optimize in the special case of Gems.
            // This reduces the search space dramatically.
            if( this.ItemType == ItemType.Gem )
            {
                return QueryGemFiles( files );
            }

            return files;
        }

        /// <summary>
        /// Queries the given array of item files for Gems.
        /// </summary>
        /// <param name="files">
        /// The array that contains all item files that exist in the game.
        /// </param>
        /// <returns>
        /// An array that only contains the gem description files from the given files array.
        /// </returns>
        private static string[] QueryGemFiles( string[] files )
        {
            var query = 
                from file in files
                where file.StartsWith( "Content\\Items\\Gem", System.StringComparison.Ordinal )
                select file;
            return query.ToArray();
        }

        /// <summary>
        /// Randomply picks and loads an item from the given list items.
        /// </summary>
        /// <param name="itemFiles">
        /// The list of item files.
        /// </param>
        /// <returns>
        /// The loaded Item.
        /// </returns>
        private Item PickItem( string[] itemFiles )
        {
            string itemName = this.GetRandomItemName( itemFiles );
            return this.itemManager.Get( itemName );
        }

        /// <summary>
        /// Randomly extract the name of an Item from the given list of item files.
        /// </summary>
        /// <param name="itemFiles">
        /// The list of item files.
        /// </param>
        /// <returns>
        /// A random Item name.
        /// </returns>
        private string GetRandomItemName( string[] itemFiles )
        {
            int index = this.rand.RandomRange( 0, (itemFiles.Length - 1) );

            string itemName = itemFiles[index];
                   itemName = itemName.Replace( "Content\\Items\\", string.Empty );
                   itemName = System.IO.Path.ChangeExtension( itemName, null );
            return itemName;
        }

        /// <summary>
        /// Verifies whether the given Item can be randomly dropped.
        /// </summary>
        /// <param name="item">
        /// The item to verify.
        /// </param>
        /// <returns>
        /// Whether the item is allowed to be randomly dropped.
        /// </returns>
        private bool Verify( Item item )
        {
            return item.StackSize > 0 && this.VerifyType( item ) && VerifyQuality( item ) && this.VerifyLevel( item );
        }

        /// <summary>
        /// Verifies the ItemType of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item to verify.
        /// </param>
        /// <returns>
        /// Whether the item is allowed to be randomly dropped, based its the ItemType.
        /// </returns>
        private bool VerifyType( Item item )
        {
            if( this.ItemType != ItemType.None )
            {
                return item.ItemType == this.ItemType;
            }

            return true;
        }

        /// <summary>
        /// Verifies the ItemQuality of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item to verify.
        /// </param>
        /// <returns>
        /// Whether the item is allowed to be randomly dropped, based its the ItemQuality.
        /// </returns>
        private static bool VerifyQuality( Item item )
        {
            return item.Quality != ItemQuality.Quest && item.Quality != ItemQuality.Legendary;
        }

        /// <summary>
        /// Verifies the level of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item to verify.
        /// </param>
        /// <returns>
        /// Whether the item is allowed to be randomly dropped, based its the level.
        /// </returns>
        private bool VerifyLevel( Item item )
        {
            return (this.MaximumItemLevel == 0) || (item.Level >= this.MinimumItemLevel && item.Level <= this.MaximumItemLevel);
        }
        
        /// <summary>
        /// Creates and spawns the given Item.
        /// </summary>
        /// <param name="item">
        /// The Item to create and then spawn.
        /// </param>
        /// <param name="user">
        /// The PlayerEntity that used this ItemUseEffect.
        /// </param>
        private void CreateAndSpawnItem( Item item, Zelda.Entities.PlayerEntity user )
        {
            var itemInstance = ItemCreationHelper.Create( item, this.rand );
            SpawnItem( itemInstance, user );
        }

        /// <summary>
        /// Spawns the given ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The ItemInstance to spawn.
        /// </param>
        /// <param name="user">
        /// The PlayerEntity that used this ItemUseEffect.
        /// </param>
        private static void SpawnItem( ItemInstance itemInstance, Zelda.Entities.PlayerEntity user )
        {
            if( user.PickedupItemContainer.Item == null )
            {
                user.PickedupItemContainer.Pick( itemInstance );
            }
            else
            {
                user.Inventory.FailSafeInsert( itemInstance );
            }
        }

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( this.MinimumItemLevel );
            context.Write( this.MaximumItemLevel );
            context.Write( (byte)this.ItemType );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            // Set Fields.
            this.rand        = context.ServiceProvider.Rand;
            this.itemManager = context.ServiceProvider.ItemManager;

            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            // Read Data.
            this.MinimumItemLevel = context.ReadInt32();
            this.MaximumItemLevel = context.ReadInt32();

            if( version >= 2 )
            {
                this.ItemType = (ItemType)context.ReadByte();
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        /// <summary>
        /// The Zelda ItemManager that is used to load the spawned item.
        /// </summary>
        private ItemManager itemManager;

        #endregion
    }
}