// <copyright file="IPickupableEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IPickupableEntity interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Defines a mechanism that when implemented allows to pickup the entity.
    /// </summary>
    public interface IPickupableEntity
    {        
        /// <summary>
        /// Tries to pickup this <see cref="IPickupableEntity"/>.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that tries t pick it up.
        /// </param>
        /// <returns>
        /// true if it has been picked up;
        /// otherwise false.
        /// </returns>
        bool PickUp( PlayerEntity player );
    }
}
