// <copyright file="IZeldaSetupable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IZeldaSetupable interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda
{
    /// <summary>
    /// Provides a method that allows to setup the object.
    /// </summary>
    public interface IZeldaSetupable
    {
        /// <summary>
        /// Setups this IZeldaSetupable.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        void Setup( IZeldaServiceProvider serviceProvider );
    }
}
