// <copyright file="AggregatePredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.AggregatePredicate class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Predicates
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom.Collections;

    /// <summary>
    /// Represents an IPredicate{T} that acts on multiple other IPredicate{T}s.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the IPredicate{T}s act on.
    /// </typeparam>
    public abstract class AggregatePredicate<T> : Predicate<T>
    {
        /// <summary>
        /// Gets the list of IPredicate{T}s this AggregatePredicate{T} acts on.
        /// </summary>
        [Editor( "Zelda.Core.Predicates.Design.PredicateListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public IList<IPredicate<T>> Predicates
        {
            get
            {
                return this.predicates;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AggregatePredicate{T} class.
        /// </summary>
        protected AggregatePredicate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AggregatePredicate{T} class.
        /// </summary>
        /// <param name="predicates">
        /// The IPredicate{T}s the new AggregatePredicate{T} should act on.
        /// </param>
        protected AggregatePredicate( params IPredicate<T>[] predicates )
        {
            this.predicates.AddRange( predicates );
        }

        /// <summary>
        /// The IPredicate{T}s this AggregatePredicate{T} acts on.
        /// </summary>
        private readonly IList<IPredicate<T>> predicates = new NonNullList<IPredicate<T>>();
    }
}
