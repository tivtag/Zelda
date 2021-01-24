// <copyright file="TileBlock.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.TileBlock class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom.Math;
    using Zelda.Entities.Drawing;
    using Zelda.Saving;

    /// <summary>
    /// Represents a simple tile-sized entity that can be used
    /// to make a tile solid.
    /// </summary>
    public class TileBlock : ZeldaEntity, IPersistentEntity
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TileBlock"/> is solid,
        /// and as such unpassable by moving entities.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool IsSolid
        {
            get { return this.Collision.IsSolid; }
            set 
            {
                if( value == this.IsSolid )
                    return;

                this.Collision.IsSolid = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="OneDirDrawDataAndStrategy"/> of this <see cref="TileBlock"/>.
        /// </summary>
        public new OneDirDrawDataAndStrategy DrawDataAndStrategy
        {
            get 
            { 
                return (OneDirDrawDataAndStrategy)base.DrawDataAndStrategy;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the TileBlock class.
        /// </summary>
        public TileBlock()
        {
            this.IsSolid = false;

            this.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            base.DrawDataAndStrategy = new OneDirDrawDataAndStrategy( this );

            this.Added   += OnAdded;
            this.Removed += OnRemoved;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the action tile that is related to this TileBlock.
        /// </summary>
        internal void RefreshActionTile()
        {
            var floor = this.Scene.Map.GetFloor( this.FloorNumber );
            if( floor == null )
                return;

            var actionLayer = floor.ActionLayer;
            var tile        = this.Transform.PositionTile;

            actionLayer.SetTile( tile.X, tile.Y, this.IsSolid ? (int)ActionTileId.Solid : (int)ActionTileId.Normal );
        }

        /// <summary>
        /// Called when this TileBlock gets added to the specified ZeldaScene.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="scene">The related ZeldaScene.</param>
        private void OnAdded( object sender, ZeldaScene scene )
        {
            if( ZeldaScene.EditorMode )
                return;

            if( this.IsSolid )
            {
                var floor = scene.Map.GetFloor( this.FloorNumber );
                if( floor == null )
                    return;

                var actionLayer = floor.ActionLayer;
                var tile        = this.Transform.PositionTile;

                actionLayer.SetTile( tile.X, tile.Y, (int)ActionTileId.Solid );
            }
        }

        /// <summary>
        /// Called when this TileBlock gets removed from the specified ZeldaScene.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="scene">The related ZeldaScene.</param>
        private void OnRemoved( object sender, ZeldaScene scene )
        {
            if( ZeldaScene.EditorMode )
                return;

            if( this.IsSolid )
            {
                var floor = scene.Map.GetFloor( this.FloorNumber );
                if( floor == null )
                    return;

                var actionLayer = floor.ActionLayer;
                var tile        = this.Transform.PositionTile;

                actionLayer.SetTile( tile.X, tile.Y, (int)ActionTileId.Normal );
            }
        }

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="TileBlock"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<TileBlock>
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
            public override void Serialize( TileBlock entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // Header
                context.WriteHeader( 2 );

                // Main Data
                context.Write( entity.Name );
                context.Write( entity.IsSolid );
                context.Write( (int)entity.FloorRelativity ); // Version 2

                // Transform
                context.Write( entity.Transform.Position.X );
                context.Write( entity.Transform.Position.Y );

                // Drawing
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
            public override void Deserialize( TileBlock entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                int version = context.ReadHeader( 2, this.GetType() );

                // Main Data
                entity.Name    = context.ReadString();
                entity.IsSolid = context.ReadBoolean();

                if( version == 2 )
                {
                    entity.FloorRelativity = (EntityFloorRelativity)context.ReadInt32();
                }

                // Transform
                float x = context.ReadSingle();
                float y = context.ReadSingle();
                entity.Transform.Position = new Vector2( x, y );

                // Drawing
                entity.DrawDataAndStrategy.Deserialize( context );
                entity.DrawDataAndStrategy.Load( serviceProvider );
                
                // Post Init
                entity.RefreshActionTile();
            }
        }

        #endregion
    }
}
