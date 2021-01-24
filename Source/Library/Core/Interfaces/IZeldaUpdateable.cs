// <copyright file="IZeldaUpdateable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IZeldaUpdateable interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda
{
    /// <summary>
    /// Provides a mechanism that updates the object. 
    /// </summary>
    /// <remarks>
    /// <see cref="Update"/> is usually called once per frame.
    /// </remarks>
    public interface IZeldaUpdateable
    {
        /// <summary>
        /// Updates this IZeldaUpdateable.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void Update( ZeldaUpdateContext updateContext );
    }
}
