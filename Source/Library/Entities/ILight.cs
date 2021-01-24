// <copyright file="ILight.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ILight interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;

    /// <summary>
    /// Represents a light entity that is drawn seperately from
    /// other entities in the so called "Light Pass" which is later
    /// recombined with the normal pass.
    /// </summary>
    public interface ILight : IZeldaEntity
    {
        /// <summary>
        /// Gets a value indicating whether only the DrawLight method of this ILight is called
        /// during the light drawing pass;
        /// -or- also the Draw method during the normal drawing pass.
        /// </summary>
        bool IsLightOnly
        {
            get;
        }

        /// <summary>
        /// Draws this Light. This method is called during the "Light-Drawing-Pass".
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        void DrawLight( ZeldaDrawContext drawContext );
    }
}
