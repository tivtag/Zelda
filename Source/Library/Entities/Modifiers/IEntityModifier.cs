// <copyright file="IEntityModifier.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.IEntityModifier interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Modifiers
{
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Represents some logic that modifies an <see cref="ZeldaEntity"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IEntityModifier : ISaveable
    {
        /// <summary>
        /// Applies this IEntityModifier on the specified ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity to modify.
        /// </param>
        void Apply( ZeldaEntity entity );
    }
}
