// <copyright file="IUseable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IUseable interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Defines a mechanism that allows a PlayerEntity to use the implementing object.
    /// </summary>
    public interface IUseable
    {
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
        bool Use( Zelda.Entities.PlayerEntity user );
    }
}
