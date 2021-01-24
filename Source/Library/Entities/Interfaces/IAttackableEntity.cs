// <copyright file="IAttackableEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IAttackableEntity interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities
{
    using Zelda.Entities.Components;

    /// <summary>
    /// Represents an entity that can attack and/or be attacked.
    /// </summary>
    public interface IAttackableEntity
    {
        /// <summary>
        /// Gets the <see cref="Attackable"/> component of this IAttackingEntity.
        /// </summary>
        Attackable Attackable
        {
            get;
        }
    }
}
