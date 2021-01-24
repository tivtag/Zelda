// <copyright file="ISubEntityBehaviourContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.ISubEntityBehaviourContainer interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours
{
    using System;

    /// <summary>
    /// Provides a mechanism to receive a sub behaviour
    /// from the object that implements this interface.
    /// </summary>
    public interface ISubEntityBehaviourContainer
    {
        /// <summary>
        /// Tries to get the sub <see cref="IEntityBehaviour"/>
        /// of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the IEntityBehaviour to get.
        /// </param>
        /// <returns>
        /// The requested IEntityBehaviour;
        /// or null if there exists no IEntityBehaviour of the given <paramref name="type"/>.
        /// </returns>
        IEntityBehaviour GetSubBehaviour( Type type );
    }
}
