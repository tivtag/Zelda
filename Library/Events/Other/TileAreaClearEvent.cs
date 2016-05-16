// <copyright file="TileAreaClearEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.TileAreaClearEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Zelda.Entities;
    
    /// <summary>
    /// Defines an event that when triggered sets all tiles in a specific
    /// area to 0.
    /// </summary>
    public class TileAreaClearEvent : Atom.Events.Event
    {
        /// <summary>
        /// Gets or sets the area that should be deleted.
        /// </summary>
        public Rectangle Area
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ground layer should be ignored.
        /// </summary>
        public bool IgnoreGroundLayer
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this AreaTileDeletionEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that wants to trigger this AreaTileDeletionEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var entity = (IZeldaEntity)obj;
            var layersToClean = this.GetLayersToClean( entity.Scene );
            var area = this.Area;

            int endX = area.X + area.Width;
            int endY = area.Y + area.Height;

            for( int x = area.X; x < endX; ++x )
            {
                for( int y = area.Y; y < endY; ++y )
                {
                    ClearTile( x, y, layersToClean );
                }
            }
        }

        /// <summary>
        /// Gets the TileMapDataLayers that should be cleaned when
        /// this AreaTileDeletionEvent is triggered.
        /// </summary>
        /// <param name="scene">
        /// The ZeldaScene that should be cleaned.
        /// </param>
        /// <returns>
        /// The layers to clean.
        /// </returns>
        private List<TileMapDataLayer> GetLayersToClean( ZeldaScene scene )
        {
            var list = new List<TileMapDataLayer>();

            foreach( var floor in scene.Map.Floors )
            {
                var layers = floor.Layers;

                for( int layerIndex = 0; layerIndex < layers.Count; ++layerIndex )
                {
                    var layer = layers[layerIndex];

                    if( ShouldCleanLayer( layerIndex, layer ) )
                    {
                        list.Add( layer );
                    }
                }

                if( ShouldCleanActionLayerOf( floor ) )
                {
                    list.Add( floor.ActionLayer );
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a value indicating whether the specified layer should be cleaned.
        /// </summary>
        /// <param name="layerIndex">
        /// The index of the layer.
        /// </param>
        /// <param name="layer">
        /// The actual layer.
        /// </param>
        /// <returns>
        /// true if the layer should be cleaned;
        /// otherwise false.
        /// </returns>
        protected virtual bool ShouldCleanLayer( int layerIndex, TileMapDataLayer layer )
        {
            // Ignore ground layer?
            if( layerIndex == 0 && layer.Floor.FloorNumber == 0 )
            {
                return !this.IgnoreGroundLayer;
            }

            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the action layer of the specified TileMapFloor
        /// should be cleaned.
        /// </summary>
        /// <param name="floor">
        /// The floor whose action layer might be cleaned.
        /// </param>
        /// <returns>
        /// true if the action layer should also be cleaned;
        /// otherwise false.
        /// </returns>
        protected virtual bool ShouldCleanActionLayerOf( TileMapFloor floor )
        {
            return true;
        }

        /// <summary>
        /// Sets the tile of all specified layers at the specified position to 0.
        /// </summary>
        /// <param name="x">
        /// The position of the tile to cleanse on the x-axis. (tile-space)
        /// </param>
        /// <param name="y">
        /// The position of the tile to cleanse on the y-axis. (tile-space)
        /// </param>
        /// <param name="layers">
        /// The layers to cleanse.
        /// </param>
        private static void ClearTile( int x, int y, List<TileMapDataLayer> layers )
        {
            foreach( var layer in layers )
            {
                layer.SetTile( x, y, 0 );
            }
        }

        /// <summary>
        /// Serializes this AreaTileClearEvent.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Area.X );
            context.Write( this.Area.Y );
            context.Write( this.Area.Width );
            context.Write( this.Area.Height );
            context.Write( this.IgnoreGroundLayer );
        }

        /// <summary>
        /// Deserializes this AreaTileClearEvent.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, typeof( TileAreaClearEvent ) );

            int x = context.ReadInt32();
            int y = context.ReadInt32();
            int width = context.ReadInt32();
            int height = context.ReadInt32();
            this.Area = new Rectangle( x, y, width, height );

            this.IgnoreGroundLayer = context.ReadBoolean();
        }
    }
}
