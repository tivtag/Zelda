// <copyright file="WrappingPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.WrappingPredicate{T} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Predicates
{
    using System;
    using Atom.Diagnostics.Contracts;
    
    /// <summary>
    /// Represents an IPredicate{T} that wraps around another IPredicate{T}.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the Predicate{T} acts on.
    /// </typeparam>
    public abstract class WrappingPredicate<T> : Predicate<T>
    {
        /// <summary>
        /// Initializes a new instance of the WrappingPredicate class.
        /// </summary>
        /// <param name="predicate">
        /// The IPredicate{T} the new WrappingPredicate{T} wraps around.
        /// </param>
        protected WrappingPredicate( IPredicate<T> predicate )
        {
            Contract.Requires<ArgumentNullException>( predicate != null );

            this.predicate = predicate;
        }
        
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
        public override bool Holds( T value )
        {
            return this.predicate.Holds( value );
        }

        /// <summary>
        /// The IPredicate{T} this WrappingPredicate{T} wraps around.
        /// </summary>
        private readonly IPredicate<T> predicate;
    }
}
