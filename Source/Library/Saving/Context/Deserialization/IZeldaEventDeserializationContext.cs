// <copyright file="IZeldaEventDeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IZeldaEventDeserializationContext interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    using Atom.Events;

    /// <summary>
    /// Provides access to the objects required during deserialization of event data.
    /// </summary>
    public interface IZeldaEventDeserializationContext : IZeldaDeserializationContext, IEventDeserializationContext
    {
    }
}
