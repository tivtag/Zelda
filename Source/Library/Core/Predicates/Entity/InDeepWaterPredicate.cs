// <copyright file="InDeepWaterPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Entity.InDeepWaterPredicate class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Predicates.Entity
{
    using Atom.Components;
    using Zelda.Entities.Components;

    /// <summary>
    /// Represents an IPredicate{IEntity} that holds true when the entity
    /// is currently swimming in deep water.
    /// </summary>
    public sealed class InDeepWaterPredicate : Predicate<IEntity>
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
        public override bool Holds( IEntity value )
        {
            var moveable = value.Components.Get<Moveable>();

            if( moveable == null )
            {
                return false;
            }

            return moveable.IsSwimming;
        }
    }
}
