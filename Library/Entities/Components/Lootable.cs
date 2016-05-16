// <copyright file="Lootable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Lootable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Atom.Math;
    using Zelda.Items;

    /// <summary>
    /// Defines the <see cref="ZeldaComponent"/> that makes a <see cref="ZeldaEntity"/> lootable (by the player).
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// This component must be Setup before it can be used correctly.
    /// </remarks>
    public sealed class Lootable : ZeldaComponent, IZeldaSetupable
    {
        #region [ Initialization ]

        /// <summary>
        /// Setups this <see cref="Lootable"/> component.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( this.loot == null )
            {
                this.loot = new LootTable( serviceProvider.ItemManager, serviceProvider.Rand );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value that indicates how many items 
        /// the lootable <see cref="ZeldaEntity"/> may drop.
        /// </summary>
        public ItemDropMode DropMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the radius of the circle, centering at the collision center of the parent entity,
        /// in which items will drop.
        /// </summary>
        /// <value>The default value is 5.</value>
        public float DropRange
        {
            get { return this.dropRange; }
            set { this.dropRange = value; }
        }

        /// <summary>
        /// Gets the <see cref="LootTable"/> of the lootable ZeldaEntity.
        /// </summary>
        public LootTable Loot
        {
            get { return loot; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Drops loot of the <see cref="Lootable"/> ZeldaEntity.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        public void DropLoot( RandMT rand )
        {
            Debug.Assert( rand != null );
            if( this.Scene == null || this.Loot == null )
                return;

            int dropCount = this.GetItemDropCount( rand );
            if( dropCount == 0 )
                return;

            this.DropItems( dropCount, rand );
        }

        /// <summary>
        /// Drops N random items.
        /// </summary>
        /// <param name="dropCount">
        /// The number of items to drop.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        private void DropItems( int dropCount, RandMT rand )
        {
            var player = this.Scene.Player;
            if( player == null )
                return;

            this.loot.MagicFindModifier = player.Statable.MagicFind;

            Vector2 dropPosition = this.Owner.Collision.Center;

            for( int i = 0; i < dropCount; ++i )
            {
                Item item = this.Loot.Get();
                if( item == null )
                    continue;

                if( item.DropRequirement != null )
                {
                    if( !item.DropRequirement.IsFulfilledBy( player ) )
                    {
                        --i;
                        continue;
                    }
                }

                this.SpawnItem( ItemCreationHelper.Create( item, rand ), dropPosition, rand );
            }
        }

        /// <summary>
        /// Spawns the given Item at the given position.
        /// </summary>
        /// <param name="item">
        /// The item to spawn.
        /// </param>
        /// <param name="dropPosition">
        /// The position the item should drop at.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        private void SpawnItem( ItemInstance item, Vector2 dropPosition, RandMT rand )
        {
            MapItem mapItem = new MapItem( item ) {
                FloorNumber = this.Owner.FloorNumber
            };

            mapItem.Transform.Position = FindItemSpawnPosition( mapItem, dropPosition, rand );
            this.Scene.Add( mapItem );
        }

        /// <summary>
        /// Attempts to find a reachable spawn position for an item.
        /// </summary>
        /// <param name="mapItem">
        /// The item that will spawn.
        /// </param>
        /// <param name="dropPosition">
        /// The central drop position.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// The position at which the item should spawn.
        /// </returns>
        private Vector2 FindItemSpawnPosition( MapItem mapItem, Vector2 dropPosition, RandMT rand )
        {
            const int MaximumTryCount = 5;

            int tryCount = 0;
            Vector2 position;
            Vector2 offset = mapItem.Collision.Size / 2.0f;

            var actionLayer = this.Scene.GetActionLayer( mapItem.FloorNumber );
            Moveable moveable = this.Scene.Player.Moveable;
            
            do
            {
                position = new Vector2(
                   dropPosition.X + rand.RandomRange( -dropRange, dropRange ),
                   dropPosition.Y + rand.RandomRange( -dropRange, dropRange )
                );

                if( moveable.IsWalkableAt( position + offset, actionLayer ) )
                {
                    // Found good position
                    break;
                }

                ++tryCount;
            }
            while( tryCount < MaximumTryCount );

            return position;
        }

        /// <summary>
        /// Receives a value that states how many <see cref="Item"/>s the lootable ZeldaEntity should drop.
        /// </summary>
        /// <param name="rand">A random number generator.</param>
        /// <returns>
        /// How many <see cref="Item"/>s the lootable ZeldaEntity should drop.
        /// </returns>
        private int GetItemDropCount( RandMT rand )
        {
            float value = rand.RandomSingle;

            switch( this.DropMode )
            {
                case ItemDropMode.Bad:
                    // 60% 0, 40% 1
                    if( value <= 0.6f )
                        return 0;
                    return 1;

                case ItemDropMode.Normal:
                    // 35% 0, 35% 1, 20% 2, 10% 3
                    if( value <= 0.35f )
                        return 0;
                    if( value <= 0.70f )
                        return 1;
                    if( value <= 0.90f )
                        return 2;
                    return 3;

                case ItemDropMode.Better:
                    // 40% 1, 30% 2, 30% 3
                    if( value <= 0.40f )
                        return 1;
                    if( value <= 0.70f )
                        return 2;
                    return 3;

                case ItemDropMode.Special:
                    // 25% 1, 35% 2, 30% 3, 10% 4
                    if( value <= 0.25f )
                        return 1;
                    if( value <= 0.60f )
                        return 2;
                    if( value <= 0.90f )
                        return 3;
                    return 4;

                case ItemDropMode.Boss:
                    // 30% 3, 40% 4, 20% 5, 10% 6
                    if( value <= 0.30f )
                        return 3;
                    if( value <= 0.70f )
                        return 4;
                    if( value <= 0.90f )
                        return 5;
                    return 6;

                default:
                    throw new NotImplementedException();
            }
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given Lootable component to be a clone
        /// of this Lootable component.
        /// </summary>
        /// <param name="lootable">
        /// The Lootable component to setup as a clone.
        /// </param>
        public void SetupClone( Lootable lootable )
        {
            lootable.DropMode = this.DropMode;
            lootable.loot = this.loot;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 3;
            context.Write( CurrentVersion );

            // Write Drop Mode:
            context.Write( (int)this.DropMode );
            context.Write( this.dropRange ); // New in Version 3.

            // Write Loot:
            if( this.loot == null )
                context.Write( (int)0 );
            else
                context.Write( this.loot.Count );

            for( int i = 0; i < this.loot.Count; ++i )
            {
                var lootEntry = this.loot[i];

                context.Write( lootEntry.Data );
                context.Write( lootEntry.OriginalWeight );
                context.Write( lootEntry.Id );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            Contract.Requires<InvalidOperationException>( this.Loot != null, Resources.Error_TheComponentNeedsSetup );

            const int CurrentVersion = 3;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read Settings:
            this.DropMode = (ItemDropMode)context.ReadInt32();
            this.dropRange = context.ReadSingle();

            // Read Loot:
            int itemCount = context.ReadInt32();

            for( int i = 0; i < itemCount; ++i )
            {
                string itemName = context.ReadString();
                float dropChance = context.ReadSingle();
                int itemTypeId = context.ReadInt32();

                this.loot.Insert( itemName, dropChance, itemTypeId );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The radius of the circle, centering at the collision center of the parent entity,
        /// in which items will drop.
        /// </summary>
        private float dropRange = 5.0f;

        /// <summary>
        /// Stores the <see cref="LootTable"/> of the lootable ZeldaEntity.
        /// </summary>
        private LootTable loot;

        #endregion
    }
}