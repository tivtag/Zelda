// <copyright file="ILaternOwner.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ILaternOwner interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Represents an IZeldaEntity that owns a <see cref="Latern"/>.
    /// </summary>
    public interface ILaternOwner : IZeldaEntity
    {
        /// <summary>
        /// Gets the <see cref="Zelda.Latern"/> this ILaternOwner owns.
        /// </summary>
        Latern Latern
        {
            get;
        }
    }
}
