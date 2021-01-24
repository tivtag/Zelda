// <copyright file="StandingOrSwimmingInWaterPredicate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Predicates.Entity.InShallowOrDeepWaterPredicate class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Predicates.Entity
{
    using Atom.Components;
    using Zelda.Entities.Components;

    /// <summary>
    /// Represents an IPredicate{IEntity} that holds true when the entity
    /// is currently standing in shallow water or swimming in deep water.
    /// </summary>
    public sealed class InShallowOrDeepWaterPredicate : WrappingPredicate<IEntity>
    {
        /// <summary>
        /// Initializes a new instance of the InShallowOrDeepWaterPredicate class.
        /// </summary>
        public InShallowOrDeepWaterPredicate()
            : base( new OrPredicate<IEntity>( new InDeepWaterPredicate(), new InShallowWaterPredicate() ) )
        {
        }
    }
}
