// <copyright file="KnownPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Design.KnownPredicate class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates.Design
{
    using System;
    using Zelda.Core.Predicates.Entity;

    /// <summary>
    /// Enumerates all IPredicate{T}s that are available at design-time.
    /// </summary>
    public static class KnownPredicate
    {
        /// <summary>
        /// Gets the IPredicate{T} types supported by the design-time editors.
        /// </summary>
        public static Type[] Types
        {
            get
            {
                return KnownPredicate.types;
            }
        }

        /// <summary>
        /// The list of predicate types supported by the design-time editors.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( InShallowOrDeepWaterPredicate )
        };
    }
}
