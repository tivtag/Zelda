// <copyright file="IMoveableEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IMoveableEntity interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities
{
    using Zelda.Entities.Components;

    /// <summary>
    /// Represents an <see cref="IZeldaEntity"/> that can move by
    /// providing the <see cref="Moveable"/> component.
    /// </summary>
    public interface IMoveableEntity : IZeldaEntity
    {
        /// <summary>
        /// Gets the <see cref="Moveable"/> component of this IMoveableEntity.
        /// </summary>
        Moveable Moveable
        {
            get;
        }
    }
}
