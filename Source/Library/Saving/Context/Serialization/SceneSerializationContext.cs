// <copyright file="SceneSerializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.SceneSerializationContext class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    using System.Diagnostics;

    /// <summary>
    /// Provides access to the objects required during serialization of a ZeldaScene.
    /// </summary>
    public sealed class SceneSerializationContext : SerializationContext, ISceneSerializationContext
    {
        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> that is currently beeing serialized.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SceneSerializationContext class.
        /// </summary>
        /// <param name="scene">
        /// The ZeldaScene that is going to be serialized.
        /// </param>
        /// <param name="writer">
        /// The <see cref="System.IO.BinaryWriter"/> that should be used during the serialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        internal SceneSerializationContext( ZeldaScene scene, System.IO.BinaryWriter writer, IZeldaServiceProvider serviceProvider )
            : base( writer, serviceProvider )
        {
            Debug.Assert( scene != null );

            this.scene = scene;
        }

        /// <summary>
        /// The <see cref="ZeldaScene"/> that is currently beeing deserialized.
        /// </summary>
        private readonly ZeldaScene scene;
    }
}
