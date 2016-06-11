// <copyright file="IEntityUpdateLogic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IEntityUpdateLogic{TEntity} interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Provides a mechanism that updates a ZeldaEntity.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity.
    /// </typeparam>
    public interface IEntityUpdateLogic<TEntity>
        where TEntity : ZeldaEntity
    {
        /// <summary>
        /// Updates the given entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to update.
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void Update( TEntity entity, ZeldaUpdateContext updateContext );
    }
}
