// <copyright file="IZeldaEventSerializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IZeldaEventSerializationContext interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using Atom.Events;

    /// <summary>
    /// Provides access to the objects required during serialization of event data.
    /// </summary>
    public interface IZeldaEventSerializationContext : IZeldaSerializationContext, IEventSerializationContext
    {
    }
}
