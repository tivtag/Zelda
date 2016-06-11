// <copyright file="DoubleFullTileChangeEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.DoubleFullTileChangeEvent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Events
{
    using System;
    using Atom.Events;

    /// <summary>
    /// Represents an <see cref="Event"/> that when triggered
    /// changes two kind of tiles on a specific TileMapLayer 
    /// into a different kind of tiles. 
    /// The tile of the ActionLayer may also be changed.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DoubleFullTileChangeEvent : Event
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the depth layer edited by the event.
        /// </summary>
        public int FloorNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the TileMapLayer to edit.
        /// </summary>
        public int LayerIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id that uniquely identifies the first tile to change.
        /// </summary>
        public int SourceTileA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id that uniquely identifies the second tile to change.
        /// </summary>
        public int SourceTileB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tile id any tile with the <see cref="SourceTileA"/> changes to.
        /// </summary>
        public int TargetTileA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tile id any tile with the <see cref="SourceTileB"/> changes to.
        /// </summary>
        public int TargetTileB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action id that will be set for all changed tiles 
        /// that had the <see cref="SourceTileA"/>.
        /// </summary>
        /// <value>
        /// The action layer will not be edited if this value is -1. 
        /// </value>
        public int TargetActionTileA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action id that will be set for all changed tiles 
        /// that had the <see cref="SourceTileB"/>.
        /// </summary>
        /// <value>
        /// The action layer will not be edited if this value is -1. 
        /// </value>
        public int TargetActionTileB
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DoubleFullTileChangeEvent class.
        /// </summary>
        public DoubleFullTileChangeEvent()
        {
            this.SourceTileA = -1;
            this.SourceTileB = -1;

            this.TargetTileA = -1;
            this.TargetTileB = -1;  

            this.TargetActionTileA = -1;
            this.TargetActionTileB = -1;            
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Triggers this DoubleFullTileChangeEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this DoubleFullTileChangeEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var entity = obj as Entities.ZeldaEntity;
            if( entity == null )
                return;

            var scene = entity.Scene;
            if( scene == null )
                return;

            scene.Map.ChangeTiles( this.FloorNumber, this.LayerIndex, this.SourceTileA, this.TargetTileA, this.TargetActionTileA, this.SourceTileB, this.TargetTileB, this.TargetActionTileB );
        }

        #region > Storage <

        /// <summary>
        /// Serializes this DoubleFullTileChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            context.Write( this.LayerIndex  );
            context.Write( this.FloorNumber );

            context.Write( this.SourceTileA );
            context.Write( this.SourceTileB );

            context.Write( this.TargetTileA );
            context.Write( this.TargetTileB );

            context.Write( this.TargetActionTileA );
            context.Write( this.TargetActionTileB );
        }

        /// <summary>
        /// Deserializes this DoubleFullTileChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            this.LayerIndex  = context.ReadInt32();
            this.FloorNumber = context.ReadInt32();

            this.SourceTileA = context.ReadInt32();
            this.SourceTileB = context.ReadInt32();

            this.TargetTileA = context.ReadInt32();
            this.TargetTileB = context.ReadInt32();

            this.TargetActionTileA = context.ReadInt32();
            this.TargetActionTileB = context.ReadInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]
        #endregion
    }
}
