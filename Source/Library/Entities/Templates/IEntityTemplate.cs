// <copyright file="IEntityTemplate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IEntityTemplate interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom;

    /// <summary>
    /// Provides a mechanism for creating instances of ZeldaEntities based on a template ZeldaEntity.
    /// </summary>
    public interface IEntityTemplate : IReadOnlyLocalizedNameable, IReadOnlyNameable
    {
        /// <summary>
        /// Creates an instance of this <see cref="EntityTemplate"/>.
        /// </summary>
        /// <returns>
        /// The newly created ZeldaEntity.
        /// </returns>
        ZeldaEntity CreateInstance();
    }
}
