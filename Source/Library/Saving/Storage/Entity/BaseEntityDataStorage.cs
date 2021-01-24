// <copyright file="BaseEntityDataStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.BaseEntityDataStorage{TConstraint} class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving.Storage
{
    using Atom.Components;
    using Zelda.Entities;

    /// <summary>
    /// Represents an abstract base implementation of the IEntityDataStorage interface
    /// that can be applied on an entity with a special constraint.
    /// </summary>
    public abstract class BaseEntityDataStorage<TConstraint> : IEntityDataStorage
    {
        /// <summary>
        /// Applies this IEntityDataStorage on the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to apply the data stored in this IEntityDataStorage on.
        /// </param>
        public void ApplyOn( IEntity entity )
        {
            this.ApplyOn( (TConstraint)entity );
        }

        /// <summary>
        /// Receives the data that is stored in this IEntityDataStorage about
        /// the specified IEntity and then stores it.
        /// </summary>
        /// <param name="entity">
        /// The entity from which the data should be received from.
        /// </param>
        public void ReceiveFrom( IEntity entity )
        {
            this.ReceiveFrom( (TConstraint)entity );
        }

        /// <summary>
        /// Applies this IEntityDataStorage on the specified constrained entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to apply the data stored in this IEntityDataStorage on.
        /// </param>
        protected abstract void ApplyOn( TConstraint entity );

        /// <summary>
        /// Receives the data that is stored in this IEntityDataStorage about
        /// the specified constrained entity and then stores it.
        /// </summary>
        /// <param name="entity">
        /// The entity from which the data should be received from.
        /// </param>
        protected abstract void ReceiveFrom( TConstraint entity );

        /// <summary>
        /// Serializes the data required to descripe this IEntityDataStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void SerializeStorage( Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes the data required to descripe this IEntityDataStorage.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void DeserializeStorage( Zelda.Saving.IZeldaDeserializationContext context );
    }
}
