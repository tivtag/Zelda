// <copyright file="IEntityOwned.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IEntityOwned interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Represents an object that is owned by a <see cref="ZeldaEntity"/>.
    /// </summary>
    public interface IEntityOwned
    {
        /// <summary>
        /// Gets the <see cref="ZeldaEntity"/> that owns this IEntityOwned object.
        /// </summary>
        ZeldaEntity Owner
        {
            get;
        }
    }
}
