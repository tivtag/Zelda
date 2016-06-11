// <copyright file="IZeldaSerializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IZeldaSerializationContext interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides access to the objects required during serialization.
    /// </summary>
    public interface IZeldaSerializationContext : Atom.Storage.IUnsignedBinarySerializationContext
    {
        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> that provides fast access
        /// to game-related services.
        /// </summary>
        IZeldaServiceProvider ServiceProvider
        {
            get;
        }
    }
}
