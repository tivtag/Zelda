// <copyright file="IStatefulEntityUpdateLogic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IStatefulEntityUpdateLogic{TEntity} interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    using System;

    /// <summary>
    /// Provides a mechanism that updates a ZeldaEntity;
    /// each entity requires its own instance of the update logic.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity.
    /// </typeparam>
    public interface IStatefulEntityUpdateLogic<TEntity> : IEntityUpdateLogic<TEntity>, ICloneable
        where TEntity : ZeldaEntity
    {
    }
}
