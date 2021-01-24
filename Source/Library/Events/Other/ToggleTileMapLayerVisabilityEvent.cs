// <copyright file="ToggleTileMapLayerVisabilityEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.ToggleTileMapLayerVisabilityEvent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Events
{
    using System;
    using Zelda.Core;
    using Atom;

    /// <summary>
    /// Represents an Event that when triggered
    /// tiggles the visability state of an <see cref="Atom.Scene.Tiles.TileMapDataLayer"/>.
    /// </summary>
    public sealed class ToggleTileMapLayerVisabilityEvent : ZeldaEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="ToggleMode"/> that determines how
        /// the TileMapLayer's Visability should be toggled.
        /// </summary>
        public ToggleMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number that uniquely identifies the TileMapFloor
        /// to manipulate.
        /// </summary>
        public int FloorNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the <see cref="Atom.Scene.Tiles.TileMapDataLayer"/> to manipulate.
        /// </summary>
        public int LayerIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this ToggleTileMapLayerVisabilityEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this ToggleTileMapLayerVisabilityEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var floor = this.Scene.Map.GetFloor( this.FloorNumber );

            if( floor != null )
            {
                var layer = floor.GetLayer( this.LayerIndex );

                if( layer != null )
                {
                    this.Toggle( layer );
                }
            }
        }

        /// <summary>
        /// Toggles the visablity of the given TileMapDataLayer
        /// based on the <see cref="Mode"/> that has been set.
        /// </summary>
        /// <param name="layer">
        /// The TileMapDataLayer to manipulate.
        /// </param>
        private void Toggle( Atom.Scene.Tiles.TileMapDataLayer layer )
        {
            switch( this.Mode )
            {
                case ToggleMode.Invert:
                    layer.IsVisible = !layer.IsVisible;
                    break;

                case ToggleMode.Off:
                    layer.IsVisible = false;
                    break;

                case ToggleMode.On:
                    layer.IsVisible = true;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Serializes this ToggleTileMapLayerVisabilityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( (byte)this.Mode );
            context.Write( this.FloorNumber );
            context.Write( this.LayerIndex );
        }

        /// <summary>
        /// Deserializes this ToggleTileMapLayerVisabilityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );
            
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Mode = (ToggleMode)context.ReadByte();
            this.FloorNumber = context.ReadInt32();
            this.LayerIndex = context.ReadInt32();
        }
    }
}