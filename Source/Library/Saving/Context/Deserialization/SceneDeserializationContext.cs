// <copyright file="SceneDeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Saving.SceneDeserializationContext class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Saving
{
    using System.Diagnostics;

    /// <summary>
    /// Provides access to the objects required during deserialization of a ZeldaScene.
    /// </summary>
    public sealed class SceneDeserializationContext : DeserializationContext, ISceneDeserializationContext
    {
        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> that is currently
        /// beeing deserialized.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        /// <summary>
        /// Gets the object that stores the state of the game world.
        /// Might be null.
        /// </summary>
        public WorldStatus WorldStatus
        {
            get
            {
                return this.worldStatus;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SceneDeserializationContext class.
        /// </summary>
        /// <param name="scene">
        /// The ZeldaScene that is going to be deserialized.
        /// </param>
        /// <param name="worldStatus">
        /// The object that stores the state of the game world.
        /// Can be null.
        /// </param>
        /// <param name="reader">
        /// The <see cref="System.IO.BinaryReader"/> that should be used during the deserialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        internal SceneDeserializationContext( 
            ZeldaScene scene,
            WorldStatus worldStatus, 
            System.IO.BinaryReader reader, 
            IZeldaServiceProvider serviceProvider )
            : base( reader, serviceProvider )
        {
            Debug.Assert( scene != null );

            this.scene = scene;
            this.worldStatus = worldStatus;
        }

        /// <summary>
        /// The <see cref="ZeldaScene"/> that is currently beeing deserialized.
        /// </summary>
        private readonly ZeldaScene scene;

        /// <summary>
        /// Stores the state of the game world.
        /// </summary>
        private readonly WorldStatus worldStatus;
    }
}
