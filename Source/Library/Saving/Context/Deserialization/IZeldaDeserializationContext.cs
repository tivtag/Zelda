// <copyright file="IZeldaDeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IZeldaDeserializationContext class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides access to the objects required during deserialization.
    /// </summary>
    public interface IZeldaDeserializationContext : Atom.Storage.IUnsignedBinaryDeserializationContext, IServiceProvider
    {
        /// <summary>
        /// Gets or sets the version of the serialized data beeing deserialized.
        /// </summary>
        int Version
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> that provides fast access
        /// to game-related services.
        /// </summary>
        [DebuggerHidden]
        IZeldaServiceProvider ServiceProvider
        {
            get;
        }
    }
}
