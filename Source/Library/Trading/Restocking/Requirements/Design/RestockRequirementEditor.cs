// <copyright file="RestockRequirementEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.Design.RestockRequirementEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Trading.Restocking.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism that
    /// allows the user to create instances of the <see cref="IRestockRequirement"/> interface.
    /// </summary>
    internal sealed class RestockRequirementEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this StatusEffectEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return RestockRequirementEditor.types;
        }

        /// <summary>
        /// The list of Types the RestockRequirementEditor can create.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( SharedChestHasSpecificTierRestockRequirement )
        };
    }
}
