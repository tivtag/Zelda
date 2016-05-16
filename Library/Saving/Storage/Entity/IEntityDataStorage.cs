// <copyright file="IEntityDataStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.IEntityDataStorage interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving.Storage
{
    using Atom.Components;
    using Zelda.Entities;

    /// <summary>
    /// Represents a storage of entity related data.
    /// </summary>
    public interface IEntityDataStorage : IStorage
    {      
        /// <summary>
        /// Applies this IEntityDataStorage on the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to apply the data stored in this IEntityDataStorage on.
        /// </param>
        void ApplyOn( IEntity entity );

        /// <summary>
        /// Receives the data that is stored in this IEntityDataStorage about
        /// the specified IEntity and then stores it.
        /// </summary>
        /// <param name="entity">
        /// The entity from which the data should be received from.
        /// </param>
        void ReceiveFrom( IEntity entity );
    }
}
