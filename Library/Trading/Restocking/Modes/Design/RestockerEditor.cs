// <copyright file="RestockerEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.Design.RestockerEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Trading.Restocking.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism that
    /// allows the user to create instances of the <see cref="IRestocker"/> interface.
    /// </summary>
    internal sealed class RestockerEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this StatusEffectEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return RestockerEditor.types;
        }

        /// <summary>
        /// The list of types the RestockerEditor can create.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( ByOneRestocker ),
            typeof( ToInitialRestocker ),
            typeof( ByOneButLimitToInitialRestocker ),
            typeof( ToSpecficRestocker ),
        };
    }
}
