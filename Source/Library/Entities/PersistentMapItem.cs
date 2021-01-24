// <copyright file="PersistentMapItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.PersistentMapItem class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Math;
    using Zelda.Items;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents a <see cref="MapItem"/> that also is an <see cref="IPersistentEntity"/>;
    /// which allows it to be removed from the Scene forever.
    /// </summary>
    public class PersistentMapItem : MapItem, IPersistentEntity
    {
        /// <summary>
        /// Gets or sets a value indicating whether
        /// the persistance of this PersistantMapItem is
        /// removed when the player has picked it up.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool IsRemovingPersistanceOnPickUp
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentMapItem"/> class.
        /// </summary>
        public PersistentMapItem()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentMapItem"/> class.
        /// </summary>
        /// <param name="itemInstance">
        /// The <see cref="ItemInstance"/>.
        /// </param>
        public PersistentMapItem( ItemInstance itemInstance )
            : base( itemInstance )
        {
            this.IsRemovingPersistanceOnPickUp = true;
        }

        /// <summary>
        /// Tries to pickup this <see cref="PersistentMapItem"/>.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that tries t pick it up.
        /// </param>
        /// <returns>
        /// true if it has been picked up;
        /// otherwise false.
        /// </returns>
        public override bool PickUp( PlayerEntity player )
        {
            var scene = this.Scene;

            if( base.PickUp( player ) )
            {
                if( this.IsRemovingPersistanceOnPickUp )
                {
                    scene.Status.RemovePersistantEntity( this );
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a clone of this <see cref="MapItem"/>.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new PersistentMapItem();

            this.SetupClone( clone );

            return clone;
        }
               
        /// <summary>
        /// Setups the given <see cref="ZeldaEntity"/> object
        /// to be a clone of this <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to clone.
        /// </param>
        protected void SetupClone( PersistentMapItem entity )
        {
            base.SetupClone( entity );
            this.IsRemovingPersistanceOnPickUp = entity.IsRemovingPersistanceOnPickUp;
        }

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="PersistentMapItem"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<PersistentMapItem>
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
            public override void Serialize( PersistentMapItem entity, IZeldaSerializationContext context )
            {
                context.WriteHeader( 2 );

                // Write Basic Properties
                context.Write( entity.Name );
                context.Write( entity.IsVisible );
                context.Write( entity.FloorNumber );
                context.Write( (byte)entity.FloorRelativity ); // New in v2

                // Write Position
                context.Write( entity.Transform.Position.X );
                context.Write( entity.Transform.Position.Y );

                // Write Item Info
                context.Write( entity.ItemInstance == null ? string.Empty : entity.ItemInstance.Item.Name );

                // Write Peristence Info.
                context.Write( entity.IsRemovingPersistanceOnPickUp );
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
            public override void Deserialize( PersistentMapItem entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                int version = context.ReadHeader( 2, this.EntityType );
                
                // Read Basic Properties
                entity.Name        = context.ReadString();
                entity.IsVisible   = context.ReadBoolean();
                entity.FloorNumber = context.ReadInt32();

                if( version >= 2 ) 
                {
                    entity.FloorRelativity = (EntityFloorRelativity)context.ReadByte();
                }

                // Read Position
                float x = context.ReadSingle();
                float y = context.ReadSingle();
                entity.Transform.Position = new Vector2( x, y );

                // Read Item Info
                string itemName = context.ReadString();

                if( itemName.Length != 0 )
                {
                    try
                    {
                        var item = serviceProvider.ItemManager.Load( itemName );
                        entity.ItemInstance = item.CreateInstance();
                    }
                    catch( System.IO.FileNotFoundException exc )
                    {
                        serviceProvider.Log.WriteLine( Atom.Diagnostics.LogSeverities.Error, exc.ToString() );
                    }
                }

                // Read Peristence Info.
                entity.IsRemovingPersistanceOnPickUp = context.ReadBoolean();
            }
        }
    }
}
