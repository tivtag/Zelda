// <copyright file="SerializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.SerializationContext class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Provides access to the objects required during serialization.
    /// </summary>
    public class SerializationContext : Atom.Storage.BinarySerializationContext, IZeldaSerializationContext
    {
        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> that provides fast access
        /// to game-related services.
        /// </summary>
        [DebuggerHidden]
        public IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SerializationContext class.
        /// </summary>
        /// <param name="stream">
        /// The Stream to which is written during the serialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        public SerializationContext( Stream stream, IZeldaServiceProvider serviceProvider )
            : base( stream )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the SerializationContext class.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="System.IO.BinaryWriter"/> that should be used during the serialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        public SerializationContext( BinaryWriter writer, IZeldaServiceProvider serviceProvider )
            : base( writer )
        {
            Debug.Assert( serviceProvider != null );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
