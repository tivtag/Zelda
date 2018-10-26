// <copyright file="SharedChest.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.SharedChest class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items
{
    using System;
    using Atom.Diagnostics.Contracts;
    using System.IO;
    using Atom;
    using Atom.Math;
    using Atom.Storage;
    using Zelda.Entities;
    using Zelda.Saving;

    /// <summary>
    /// Represents the chest that is shared over -all- characters.
    /// The main use of the chest is that it allows transfering of items
    /// between characters.
    /// </summary>
    /// <remarks>
    /// The players is allowed to buy tier upgrades that increase the size of the chest.
    /// </remarks>
    public sealed class SharedChest : Inventory
    {
        /// <summary>
        /// Gets the full path at which the SharedChest is saved.
        /// </summary>
        private static string SharedChestSavePath
        {
            get
            {
                return Path.Combine( GameFolders.Profiles, "Chest" );
            }
        }

        /// <summary>
        /// Gets the SharedChestTier of this SharedChest.
        /// </summary>
        public SharedChestTier Tier
        {
            get
            {
                return this.tier;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SharedChest class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new SharedChest.
        /// </param>
        /// <param name="tier">
        /// The expension level of the new SharedChest. Determines the grid size.
        /// </param>
        private SharedChest( PlayerEntity player, SharedChestTier tier )
            : this( player, tier, GetGridSize( tier ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharedChest class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new SharedChest.
        /// </param>
        /// <param name="tier">
        /// The expension level of the new SharedChest.
        /// </param>
        /// <param name="gridSize">
        /// The number of cells on the x-axis and y-axis.
        /// </param>
        private SharedChest( PlayerEntity player, SharedChestTier tier, Point2 gridSize )
            : base( player, gridSize.X, gridSize.Y )
        {
            this.tier = tier;
        }

        /// <summary>
        /// Loads the current SharedChest.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that should own the new SharedChest.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly loaded SharedChest.
        /// </returns>
        public static SharedChest Load( PlayerEntity player, IZeldaServiceProvider serviceProvider )
        {
            try
            {
                using( var stream = File.OpenRead( SharedChestSavePath ) )
                {
                    var context = new Zelda.Saving.DeserializationContext( stream, serviceProvider ) {
                        Version = 4
                    };
                    return Load( player, context );
                }
            }
            catch( Exception exc )
            {
                serviceProvider.Log.WriteLine( exc.ToString() );
                return Create( player, SharedChestTier.Tier1 );
            }
        }

        /// <summary>
        /// Loads the SharedChest using the specified IZeldaDeserializationContext.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that should own the new SharedChest.
        /// </param>
        /// <param name="context">
        /// The context that requires access to all objects required during the deserialization process.
        /// </param>
        /// <returns>
        /// The newly created SharedChest.
        /// </returns>
        private static SharedChest Load( PlayerEntity player, IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, typeof( SharedChest ) );

            var tier = (SharedChestTier)context.ReadInt32();
            var chest = Create( player, tier );

            chest.Deserialize( context );

            return chest;
        }

        /// <summary>
        /// Saves the specified SharedChest.
        /// </summary>
        /// <param name="chest">
        /// The SharedChest to serialize.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public static void Save( SharedChest chest, IZeldaServiceProvider serviceProvider )
        {
            using( var stream = new MemoryStream() )
            {
                var context = new SerializationContext( stream, serviceProvider );
                Serialize( chest, context );

                StorageUtilities.CopyToFile( stream, SharedChestSavePath );
            }
        }

        /// <summary>
        /// Serializes the specified SharedChest using the specified IZeldaSerializationContext.
        /// </summary>
        /// <param name="chest">
        /// The SharedChest to serialize.
        /// </param>
        /// <param name="context">
        /// The context that requires access to all objects required during the serialization process.
        /// </param>
        private static void Serialize( SharedChest chest, IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( (int)chest.Tier );
            chest.Serialize( context );
        }

        /// <summary>
        /// Creates a new SharedChest for the specified Player and tier.
        /// </summary>
        /// <param name="tier">
        /// The upgrade tier of the chest to create. Determines the size of the chest.
        /// </param>
        /// <param name="player">
        /// The PlayerEntity that should own the new SharedChest.
        /// </param>
        /// <returns>
        /// The newly created SharedChest.
        /// </returns>
        private static SharedChest Create( PlayerEntity player, SharedChestTier tier )
        {
            return new SharedChest( player, tier );
        }

        /// <summary>
        /// Upgrades the tier of the specified SharedChest.
        /// </summary>
        /// <param name="chest">
        /// The SharedChest to upgrade.
        /// </param>
        internal static SharedChest UpgradeTier( SharedChest chest )
        {
            Contract.Requires<ArgumentNullException>( chest != null );

            if( chest.Tier == SharedChestTier.Tier4 )
            {
                return chest;
            }

            var nextTier = GetNextTier( chest.tier );
            var newChest = Create( chest.Owner, nextTier );

            foreach( var item in chest.Items.ToArray() )
            {
                newChest.FailSafeInsertAt( item.ItemInstance, item.ParentCell.Position );
            }

            return newChest;
        }

        /// <summary>
        /// Gets the SharedChestTier that follows the specified SharedChestTier.
        /// </summary>
        /// <param name="tier">
        /// The current SharedChestTier.
        /// </param>
        /// <returns>
        /// The next SharedChestTier.
        /// </returns>
        public static SharedChestTier GetNextTier( SharedChestTier tier )
        {
            return (SharedChestTier)((int)tier + 1);
        }

        /// <summary>
        /// Gets the size of the SharedChest at the given SharedChestTier.
        /// </summary>
        /// <param name="tier">
        /// The tier of the chest.
        /// </param>
        /// <returns>
        /// The number of grid cells on the x-axis and y-axis.
        /// </returns>
        private static Point2 GetGridSize( SharedChestTier tier )
        {
            switch( tier )
            {
                case SharedChestTier.Tier1:
                    return new Point2( 4, 4 );

                case SharedChestTier.Tier2:
                    return new Point2( 8, 4 );

                case SharedChestTier.Tier3:
                    return new Point2( 8, 8 );

                case SharedChestTier.Tier4:
                    return new Point2( DefaultGridWidth, DefaultGridHeight );

                default:
                    throw new NotImplementedException();
            }
        }      
        
        /// <summary>
        /// Handles the case of the user left-clicking on an item in the CraftingBottle
        /// while the Shift key is down.
        /// </summary>
        /// <remarks>
        /// The default behaviour is to move the item into the inventory.
        /// </remarks>
        /// <param name="item">The related item.</param>
        /// <param name="cellX">The original position of the <paramref name="item"/> (in cell-space).</param>
        protected override void SwapItemsOnShiftLeftClick( ItemInstance item, Point2 cell )
        {
            if( !this.Owner.Inventory.Insert( item ) )
            {
                if( !this.Owner.CraftingBottle.Insert( item ) )
                {
                    this.InsertAt( item, cell );
                }
            }
        }

        /// <summary>
        /// Stores the SharedChestTier of this SharedChest. The chest must be re-created to upgrade the tier.
        /// </summary>
        private readonly SharedChestTier tier;
    }
}
