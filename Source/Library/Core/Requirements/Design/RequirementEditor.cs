// <copyright file="RequirementEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Requirements.Design.RequirementEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Requirements.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism
    /// that allows the user to create <see cref="IRequirement"/> instances.
    /// </summary>
    internal sealed class RequirementEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this IsUseableEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return RequirementEditor.types;
        }

        /// <summary>
        /// The types that can be constructed by the IsUseableEditor.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( Zelda.Core.Useability.IsUseableInSpecificScene ),
            typeof( SpecificTimeOfDayRequirement ),
            typeof( MinimumDifficultyRequirement )
        };
    }
}
