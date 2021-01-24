// <copyright file="IRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Requirements.IRequirement interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Requirements
{
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Represents a serializeable predicate that returns a value indicating
    /// whether something can be used or executed.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IRequirement : ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether this IRequirement is fulfilled.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity for which this IRequirement is checked against.
        /// </param>
        /// <returns>
        /// true if it is fulfilled;
        /// otherwise false.
        /// </returns>
        bool IsFulfilledBy( Zelda.Entities.PlayerEntity player );
    }
}
