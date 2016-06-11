// <copyright file="Decoration.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Decoration class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom.Math;
    using Zelda.Saving;

    /// <summary>
    /// Represents a purely decorative entity, whose lone purpose is to
    /// be drawn using its DrawDataAndStrategy.
    /// </summary>
    public sealed class Decoration : ZeldaEntity
    {
        /// <summary>
        /// Initializes a new instance of the Decoration class.
        /// </summary>
        public Decoration()
        {
            this.Collision.IsSolid = false;
            this.Collision.Size = new Vector2( 16.0f, 16.0f );
        }

        /// <summary>
        /// Creates a clone of this <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new Decoration();
            this.SetupClone( clone );
            return clone;
        }

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="Decoration"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<Decoration>
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
            public override void Serialize( Decoration entity, IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                context.Write( entity.Name );
                context.Write( entity.FloorNumber );
                context.Write( (byte)entity.FloorRelativity );

                context.Write( entity.Transform.Position );
                context.Write( entity.Transform.Scale );
                entity.Collision.Serialize( context );

                context.WriteDrawStrategy( entity.DrawDataAndStrategy );               
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
            public override void Deserialize( Decoration entity, IZeldaDeserializationContext context )
            {
                context.ReadDefaultHeader( this.GetType() );

                entity.Name = context.ReadString();
                entity.FloorNumber = context.ReadInt32();
                entity.FloorRelativity = (EntityFloorRelativity)context.ReadByte();

                entity.Transform.Position = context.ReadVector2();
                entity.Transform.Scale = context.ReadVector2();
                entity.Collision.Deserialize( context );

                entity.DrawDataAndStrategy = context.ReadDrawStrategy( entity );
            }
        }
    }
}
