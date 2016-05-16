// <copyright file="DeserializationContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.DeserializationContext class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Provides access to the objects required during deserialization.
    /// </summary>
    public class DeserializationContext : Atom.Storage.BinaryDeserializationContext, IZeldaDeserializationContext
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
        /// Gets or sets the version of the safe file beeing deserialized.
        /// </summary>
        public int Version 
        { 
            get;
            set; 
        }

        /// <summary>
        /// Initializes a new instance of the DeserializationContext class.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> from which is read during the deserialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        public DeserializationContext( Stream stream, IZeldaServiceProvider serviceProvider )
            : base( stream )
        {
            Debug.Assert( serviceProvider != null );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the DeserializationContext class.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="BinaryReader"/> that should be used during the deserialization process.
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="IZeldaServiceProvider"/> that provides fast access to game-related services.
        /// </param>
        public DeserializationContext( BinaryReader reader, IZeldaServiceProvider serviceProvider )
            : base( reader )
        {
            Debug.Assert( serviceProvider != null );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">
        /// An object that specifies the type of service object to get.
        /// </param>
        /// <returns>
        /// A service object of type serviceType.
        /// -or- null if there is no service object of type serviceType.
        /// </returns>
        public object GetService( System.Type serviceType )
        {
            return this.serviceProvider.GetService( serviceType );
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
