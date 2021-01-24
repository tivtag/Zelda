// <copyright file="ZeldaEventTypeActivator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaEventTypeActivator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using Atom;

    /// <summary>
    /// Provides a mechanism that creates new objects given a type name.
    /// </summary>
    public sealed class ZeldaEventTypeActivator : ITypeActivator
    {
        /// <summary>
        /// Represents an instance of the ZeldaEventTypeActivator class.
        /// </summary>
        public static readonly ZeldaEventTypeActivator Instance = new ZeldaEventTypeActivator();

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
            // Support the old Acid framework.

            if( typeName.StartsWith( "Acid", StringComparison.Ordinal ) )
            {
                typeName = typeName.Replace( "Acid", "Atom" );

                int index = typeName.IndexOf( ',' );
                string type = typeName.Substring( 0, index );
                string assembly = typeName.Substring( index + 1, typeName.Length - index - 1 );

                if( type.StartsWith( "Atom.Events", StringComparison.Ordinal ) )
                {
                    assembly = "Atom.Game";
                }

                typeName = type + "," + assembly;
            }

            return this.actualTypeActivator.CreateInstance( typeName );
        }

        /// <summary>
        /// The ITypeActivator that is actually doing the the activation logic.
        /// </summary>
        private readonly TypeActivator actualTypeActivator = TypeActivator.Instance;
    }
}
