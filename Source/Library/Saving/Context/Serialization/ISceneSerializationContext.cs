// <copyright file="ISceneSerializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Saving.ISceneSerializationContext interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides access to the objects required during serialization of a <see cref="ZeldaScene"/>.
    /// </summary>
    public interface ISceneSerializationContext : IZeldaSerializationContext
    {
        /// <summary>
        /// Gets the ZeldaScene that is currently beeing serialized.
        /// </summary>
        ZeldaScene Scene
        {
            get;
        }
    }
}
