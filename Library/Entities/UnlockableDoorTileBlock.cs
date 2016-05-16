// <copyright file="UnlockableDoorTileBlock.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.UnlockableDoorTileBlock class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom.Events;
    using Atom.Math;
    using Zelda.Items;
    using Zelda.Saving;

    /// <summary>
    /// Represents a <see cref="TileBlock"/> that can be 'removed' from the Scene
    /// by the player by unlocking it using a key <see cref="Item"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class UnlockableDoorTileBlock : TileBlock, IUseable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies the key <see cref="Item"/>
        /// that is required to unlock this UnlockableDoorTileBlock. 
        /// </summary>
        public string RequiredKeyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// the persistance of this UnlockableDoorTileBlock is
        /// removed when the player unlocks it.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool IsRemovingPersistanceOnUnlock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Event"/> that is
        /// triggered when the player unlocks this UnlockableDoorTileBlock.
        /// </summary>
        /// <value>The default value is null.</value>
        public Event UnlockedEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Event"/> that is
        /// triggered when the player didn't manage to unlock this UnlockableDoorTileBlock
        /// (because he was missing the key).
        /// </summary>
        /// <value>The default value is null.</value>
        public Event NotUnlockedEvent
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockableDoorTileBlock"/> class.
        /// </summary>
        public UnlockableDoorTileBlock()
        {
            this.IsRemovingPersistanceOnUnlock = true;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this UnlockableDoorTileBlock, trying to unlock it.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that tried to unlock this UnlockableDoorTileBlock.
        /// </param>
        /// <returns>
        /// Whether the UnlockableDoorTileBlock has been unlocked;
        /// and as such removed from the Scene.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.Collision.IntersectsUnstrict( user.Collision ) )
                return false;
            if( !this.Transform.IsFacing( user.Transform ) )
                return false;
            
            if( this.Unlock( user ) )
            {
                this.OnUnlocked( user );
                return true;
            }
            else
            {
                this.OnNotUnlocked( user );
                return false;
            }
        }

        private bool Unlock(PlayerEntity user )
        {
            if( string.IsNullOrEmpty( this.RequiredKeyName ) )
            {
                return true;
            }
            else
            {
                var inventory = user.Inventory;
                return inventory.Remove( this.RequiredKeyName );
            }
        }

        /// <summary>
        /// Called when this UnlockableDoorTileBlock has been unlocked
        /// by the player.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that has unlocked this UnlockableDoorTileBlock.
        /// </param>
        private void OnUnlocked( PlayerEntity user )
        {
            if( this.IsRemovingPersistanceOnUnlock )
            {
                this.Scene.Status.RemovePersistantEntity( this );
            }

            if( this.UnlockedEvent != null && this.UnlockedEvent.CanBeTriggeredBy( user ) )
            {
                this.UnlockedEvent.Trigger( user );
            }

            this.RemoveFromScene();
        }
        
        /// <summary>
        /// Called when the player tried to unlock this UnlockableDoorTileBlock,
        /// but failed.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that tried to unlock this UnlockableDoorTileBlock.
        /// </param>
        private void OnNotUnlocked( PlayerEntity user )
        {
            if( this.NotUnlockedEvent != null && this.NotUnlockedEvent.CanBeTriggeredBy( user ) )
            {
                this.NotUnlockedEvent.Trigger( user );
            }
        }

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="UnlockableDoorTileBlock"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<UnlockableDoorTileBlock>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( UnlockableDoorTileBlock entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteHeader( 3 );

                // Main Data
                context.Write( entity.Name );
                context.Write( entity.IsSolid );
                context.Write( entity.RequiredKeyName ?? string.Empty );
                context.Write( entity.IsRemovingPersistanceOnUnlock );

                // Events
                context.Write( entity.UnlockedEvent != null ? entity.UnlockedEvent.Name : string.Empty );
                context.Write( entity.NotUnlockedEvent != null ? entity.NotUnlockedEvent.Name : string.Empty );

                // Transform
                context.Write( entity.Transform.Position );
                context.Write( entity.FloorNumber ); // new in v2.

                // Drawing
                context.Write( (byte)entity.FloorRelativity ); // new in v3.
                entity.DrawDataAndStrategy.Serialize( context );
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( UnlockableDoorTileBlock entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                int version = context.ReadHeader( 3, this.GetType() );

                // Main Data
                entity.Name            = context.ReadString();
                entity.IsSolid         = context.ReadBoolean();
                entity.RequiredKeyName = context.ReadString();
                entity.IsRemovingPersistanceOnUnlock = context.ReadBoolean();

                // Events
                string unlockedEventName = context.ReadString();
                if( unlockedEventName.Length > 0 )
                    entity.UnlockedEvent = entity.Scene.EventManager.GetEvent( unlockedEventName );

                string notUnlockedEventName = context.ReadString();
                if( notUnlockedEventName.Length > 0 )
                    entity.NotUnlockedEvent = entity.Scene.EventManager.GetEvent( notUnlockedEventName );

                // Transform
                entity.Transform.Position = context.ReadVector2();

                if( version >= 2 )
                {
                    entity.FloorNumber = context.ReadInt32();
                }

                // Drawing
                if( version >= 3 )
                {
                    entity.FloorRelativity = (EntityFloorRelativity)context.ReadByte();
                }

                entity.DrawDataAndStrategy.Deserialize( context );
                entity.DrawDataAndStrategy.Load( serviceProvider );

                // Post Init
                entity.RefreshActionTile();
            }
        }

        #endregion
    }
}
