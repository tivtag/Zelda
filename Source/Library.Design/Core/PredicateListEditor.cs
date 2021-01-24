// <copyright file="PredicateListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Design.PredicateListEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for lists of IPredicates.
    /// This class can't be inherited.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the IPredicate{T}s acts on.
    /// </typeparam>
    public sealed class PredicateListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateListEditor{T}"/> class.
        /// </summary>
        public PredicateListEditor()
            : base( typeof( IList<IPredicate<object>> ) )
        {
        }

        /// <summary>
        /// Gets the type created by this PredicateListEditor{T}.
        /// </summary>
        /// <returns>The data type of the items in the collection.</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof( IPredicate<object> );
        }

        /// <summary>
        /// Gets the data types that this collection editor can contain.
        /// </summary>
        /// <returns>
        /// An array of data types that this collection can contain.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return KnownPredicate.Types;
        }
    }
}
