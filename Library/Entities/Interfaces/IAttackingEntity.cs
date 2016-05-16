// <copyright file="IAttackingEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IAttackingEntity interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Provides a mechanism to receive a value that 
    /// indicates whether the Entity is currently attacking.
    /// </summary>
    public interface IAttackingEntity
    {      
        /// <summary>
        /// Gets a value indicating whether this Enemy is currently attacking.
        /// </summary>
        bool IsAttacking 
        { 
            get; 
        }
    }
}
