// <copyright file="ICollisionWithPlayerNotifier.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ICollisionWithPlayerNotifier interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Provides a mechanism to inform a <see cref="ZeldaEntity"/>
    /// that it is/was colliding the the <see cref="PlayerEntity"/>.
    /// </summary>
    /// <remarks>
    /// Implementing this interface on an Entity is all
    /// that is needed to get the functionallity to function.
    /// </remarks>
    public interface ICollisionWithPlayerNotifier
    {
        /// <summary>
        /// Notifies the object that implements this interface
        /// that the <paramref name="player"/> is colliding with the object.
        /// </summary>
        /// <param name="player">
        /// The corresponding <see cref="PlayerEntity"/>.
        /// </param>
        void NotifyCollisionWithPlayer( PlayerEntity player );
    }
}
