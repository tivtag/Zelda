// <copyright file="ZeldaTypeActivator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaTypeActivator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using Atom;

    /// <summary>
    /// Provides a mechanism that creates new objects given a type name.
    /// </summary>
    public sealed class ZeldaTypeActivator : ITypeActivator
    {
        /// <summary>
        /// Represents an instance of the ZeldaTypeActivator class.
        /// </summary>
        public static readonly ITypeActivator Instance = new ZeldaTypeActivator();

        /// <summary>
        /// Prevents the creation of ZeldaTypeActivator instances.
        /// </summary>
        private ZeldaTypeActivator()
        {
        }

        /// <summary>
        /// Creates an instance of the type with the given typeName.
        /// </summary>
        /// <param name="typeName">
        /// The name that uniquely identifies the type to initiate.
        /// </param>
        /// <returns>
        /// The object that has been created.
        /// </returns>
        public object CreateInstance( string typeName )
        {
            //if( typeName.StartsWith( "Acid" ) )
            //{
            //    typeName = typeName.Replace( "Acid", "Atom" );
            //}

            return this.actualTypeActivator.CreateInstance( typeName );
        }

        /// <summary>
        /// The ITypeActivator that is actually doing the the activation logic.
        /// </summary>
        private readonly TypeActivator actualTypeActivator = TypeActivator.Instance;
    }
}
