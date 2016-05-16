// <copyright file="ISceneDeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ISceneDeserializationContext interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides access to the objects required during deserialization of a <see cref="ZeldaScene"/>.
    /// </summary>
    public interface ISceneDeserializationContext : IZeldaDeserializationContext, IWorldStatusProvider
    {
        /// <summary>
        /// Gets the ZeldaScene that is currently beeing deserialized.
        /// </summary>
        ZeldaScene Scene
        {
            get;
        }
    }
}
