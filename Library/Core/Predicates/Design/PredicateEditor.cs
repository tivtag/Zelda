// <copyright file="PredicateEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Design.PredicateEditor{T} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Predicates.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism
    /// that allows the user to create <see cref="IPredicate{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value the IPredicate{T}s acts on.
    /// </typeparam>
    internal sealed class PredicateEditor<T> : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this IsUseableEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return KnownPredicate.Types;
        }
    }
}
