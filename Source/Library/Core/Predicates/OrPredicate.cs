// <copyright file="OrPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.OrPredicate class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates
{
    using System.Collections.Generic;
    using Atom.Collections;

    /// <summary>
    /// Represents an AggregatePredicate{T} that holds true if any of its
    /// IPredicate{T}s holds true.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the IPredicate{T}s acts on.
    /// </typeparam>
    public class OrPredicate<T> : AggregatePredicate<T>
    {
        /// <summary>
        /// Initializes a new instance of the OrPredicate{T} class.
        /// </summary>
        public OrPredicate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrPredicate{T} class.
        /// </summary>
        /// <param name="predicates">
        /// The IPredicate{T}s the new OrPredicate{T} should act on.
        /// </param>
        public OrPredicate( params IPredicate<T>[] predicates )
            : base( predicates )
        {
        }

        /// <summary>
        /// Gets a value indicating whether this OrPredicate{T}
        /// holds on the specified value.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        /// <returns>
        /// true if this IPredicate{T} holds on the specified value;
        /// -or- otherwise false.
        /// </returns>
        public override bool Holds( T value )
        {
            foreach( var predicate in this.Predicates )
            {
                if( predicate.Holds( value ) )
                {
                    return true;
                }                
            }

            return false;
        }
    }
}
