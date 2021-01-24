// <copyright file="IPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.IPredicate interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates
{
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Represents a yes/no question against a specific value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the IPredicate{T} acts on.
    /// </typeparam>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IPredicate<T> : ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether this IPredicate{T}
        /// holds on the specified value.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        /// <returns>
        /// true if this IPredicate{T} holds on the specified value;
        /// -or- otherwise false.
        /// </returns>
        bool Holds( T value );
    }
}
