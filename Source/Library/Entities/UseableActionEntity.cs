// <copyright file="UseableActionEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.UseableActionEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom;
    using Atom.Math;
    using Zelda.Actions;
    using Zelda.Core.Requirements;
    using Zelda.Saving;

    /// <summary>
    /// Represents a generic entity that has a visual and when used by the player
    /// executes a specific <see cref="IAction"/>.
    /// </summary>
    public sealed class UseableActionEntity : ZeldaEntity, IUseable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the player has to face the entity
        /// to use it.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        public bool HasToFace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAction"/> to execute when this entity is used.
        /// </summary>
        public IAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the UseableActionEntity class.
        /// </summary>
        public UseableActionEntity()
        {
            this.HasToFace = true;
        }

        /// <summary>
        /// Tries to use this <see cref="IUseable"/> object.
        /// </summary>
        /// <param name="user">
        /// The object which tries to use this <see cref="IUseable"/>.
        /// </param>
        /// <returns>
        /// true if this IUseable object has been used;
        /// otherwise false.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.Collision.Intersects( user.Collision ) )
            {
                return false;
            }

            if( this.HasToFace && !this.Transform.IsFacing( user.Transform ) )
            {
                return false;                
            }

            this.Action.Execute();
            return true;
        }
        
        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="UseableActionEntity"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<UseableActionEntity>
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
            public override void Serialize( UseableActionEntity entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // Header:
                context.WriteDefaultHeader();

                // Properties:
                context.Write( entity.Name );
                context.Write( entity.HasToFace );
                context.Write( entity.FloorNumber );
                context.Write( entity.Transform.Position );
                context.Write( (byte)entity.Transform.Direction );
                context.WriteStoreObject( entity.Action );
                
                // Components
                entity.Collision.Serialize( context );

                // Visual
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
            public override void Deserialize( UseableActionEntity entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                context.ReadDefaultHeader( this.GetType() );

                // Properties
                entity.Name = context.ReadString();
                entity.HasToFace = context.ReadBoolean();
                entity.FloorNumber = context.ReadInt32();
                entity.Transform.Position = context.ReadVector2();
                entity.Transform.Direction = (Direction4)context.ReadByte();
                entity.Action = context.ReadStoreObject<IAction>();

                // Components
                entity.Collision.Deserialize( context );

                // Visual
                entity.DrawDataAndStrategy = context.ReadDrawStrategy( entity );
            }
        }
    }
}
