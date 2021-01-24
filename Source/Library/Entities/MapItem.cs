// <copyright file="MapItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.MapItem class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom;
    using Atom.Math;
    using Zelda.Entities.Components;
    using Zelda.Items;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="ZeldaEntity"/> that allows
    /// to place an <see cref="ItemInstance"/> in a <see cref="ZeldaScene"/>.
    /// </summary>
    public class MapItem : ZeldaEntity, IPickupableEntity, ISceneStatusStorableEntity
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemInstance"/> displayed by this <see cref="MapItem"/>.
        /// </summary>
        public ItemInstance ItemInstance
        {
            get
            {
                return this.itemInstance;
            }

            set
            {
                this.itemInstance = value;
                this.Setup();
            }
        }

        public Components.SceneStatusStoreable Storeable
        {
            get
            {
                return storeable;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapItem"/> class.
        /// </summary>
        public MapItem()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapItem"/> class.
        /// </summary>
        /// <param name="itemInstance">
        /// The <see cref="ItemInstance"/>.
        /// </param>
        public MapItem( ItemInstance itemInstance )
        {
            this.ItemInstance = itemInstance;

            // Items that part of the Scene get (by default) drawn below characters
            this.FloorRelativity = EntityFloorRelativity.IsBelow;
            this.Collision.IsSolid = false;

            this.Components.BeginSetup();
            {
                this.Components.Add( storeable );
            }
            this.Components.EndSetup();
        }

        public static MapItem SpawnUnder( ZeldaEntity entity, ItemInstance itemInstance )
        {
            MapItem mapItem = new MapItem( itemInstance ) {
                FloorNumber = entity.FloorNumber,
            };

            mapItem.Transform.Position = entity.Transform.Position;
            entity.Scene.Add( mapItem );
            entity.Scene.Status.AddEntity( mapItem );
            return mapItem;
        }

        /// <summary>
        /// Setups the <see cref="MapItem"/> to visualize a new <see cref="ItemInstance"/>.
        /// </summary>
        private void Setup()
        {
            if( itemInstance == null || itemInstance.Item.Sprite == null )
            {
                this.Collision.Size = new Atom.Math.Vector2();
            }
            else
            {
                this.Collision.Size = new Atom.Math.Vector2(
                    itemInstance.Item.Sprite.Width,
                    itemInstance.Item.Sprite.Height
                );
            }
        }

        /// <summary>
        /// Draws this <see cref="MapItem"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( this.itemInstance == null )
                return;

            var sprite = this.itemInstance.Sprite;

            if( sprite != null )
            {
                Vector2 position;
                position.X = (int)this.Transform.X;
                position.Y = (int)this.Transform.Y;

                sprite.Draw( position, this.itemInstance.SpriteColor, drawContext.Batch );
            }
        }

        /// <summary>
        /// Updates this <see cref="MapItem"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.itemInstance == null )
                return;

            var updateable = this.itemInstance.Sprite as IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        /// <summary>
        /// Tries to pickup this <see cref="MapItem"/>.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that tries t pick it up.
        /// </param>
        /// <returns>
        /// true if it has been picked up;
        /// otherwise false.
        /// </returns>
        public virtual bool PickUp( PlayerEntity player )
        {
            if( this.itemInstance == null || this.Scene == null )
                return false;

            if( player.Collision.Intersects( this.Collision ) )
            {
                if( player.Collect( this.itemInstance ) )
                {
                    ItemSounds.PlayPickUp( this.itemInstance.Item );

                    this.Scene.Status.RemoveEntity( this );
                    this.RemoveFromScene();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a clone of this <see cref="MapItem"/>.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new MapItem();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="MapItem"/> object
        /// to be a clone of this <see cref="MapItem"/>.
        /// </summary>
        /// <param name="clone">
        /// The entity to clone.
        /// </param>
        protected void SetupClone( MapItem clone )
        {
            base.SetupClone( clone );

            clone.ItemInstance = this.ItemInstance;
        }

        /// <summary>
        /// The <see cref="ItemInstance"/> visualized by this <see cref="MapItem"/>.
        /// </summary>
        private ItemInstance itemInstance;
        private readonly SceneStatusStoreable storeable = new SceneStatusStoreable();

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="MapItem"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<MapItem>
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
            public override void Serialize( MapItem entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteHeader( 3 );

                // Write Basic Properties
                context.Write( entity.Name ?? string.Empty );
                context.Write( entity.IsVisible );
                context.Write( entity.FloorNumber );

                // Write Position
                context.Write( entity.Transform.Position );

                // Write Item Info - rewritten in V3
                if( entity.itemInstance != null )
                {
                    context.Write( true );
                    entity.itemInstance.Serialize( context );
                }
                else
                {
                    context.Write( false );
                }

                // Write Stored state
                entity.storeable.Serialize( context );
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <exception cref="InvalidCastException">
            /// If the type of the given entity is invalid for the <see cref="IEntityReaderWriter"/>.
            /// </exception>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( MapItem entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                int version = context.ReadHeader( 3, this.GetType() );

                // Read Basic Properties
                entity.Name = context.ReadString();
                entity.IsVisible = context.ReadBoolean();
                entity.FloorNumber = context.ReadInt32();

                // Transform
                entity.Transform.Position = context.ReadVector2();

                if( version >= 3 )
                {
                    if( context.ReadBoolean() )
                    {
                        entity.ItemInstance = ItemInstance.Read( context );
                    }
                }
                else
                {
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
                }

                if( version >= 2 )
                {
                    entity.storeable.Deserialize( context );
                }
            }

            public override bool ShouldSave( MapItem entity )
            {
                return entity.itemInstance != null;
            }
        }
    }
}
