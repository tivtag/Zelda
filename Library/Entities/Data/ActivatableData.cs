// <copyright file="ActivatableData.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Data.ActivatableData class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Data
{
    using Zelda.Saving;
    using Zelda.Saving.Storage;

    /// <summary>
    /// Represents an <see cref="IActivatable"/> state of an entity that is stored in the save file.
    /// </summary>
    public sealed class ActivatableData : BaseEntityDataStorage<IActivatable>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is currently active.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Applies this IEntityDataStorage on the specified constrained entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to apply the data stored in this IEntityDataStorage on.
        /// </param>
        protected override void ApplyOn( IActivatable entity )
        {
            entity.IsActive = this.IsActive;
        }

        /// <summary>
        /// Receives the data that is stored in this IEntityDataStorage about
        /// the specified constrained entity and then stores it.
        /// </summary>
        /// <param name="entity">
        /// The entity from which the data should be received from.
        /// </param>
        protected override void ReceiveFrom( IActivatable entity )
        {
            this.IsActive = entity.IsActive;
        }

        /// <summary>
        /// Serializes the data required to descripe this IEntityDataStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void SerializeStorage( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.IsActive );
        }

        /// <summary>
        /// Deserializes the data required to descripe this IEntityDataStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void DeserializeStorage( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.IsActive = context.ReadBoolean();
        }
    }
}
