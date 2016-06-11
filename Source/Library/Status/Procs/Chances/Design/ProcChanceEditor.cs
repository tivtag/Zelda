// <copyright file="ProcChanceEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.Design.ProcChanceEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Procs.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism that
    /// allows the user to create a <see cref="IProcChance"/>.
    /// </summary>
    internal sealed class ProcChanceEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this StatusEffectEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return ProcChanceEditor.types;
        }

        /// <summary>
        /// The types that can created by a ProcChanceEditor.
        /// </summary>
        private static readonly Type[] types = new Type[2] {
            typeof( FixedProcChance ),
            typeof( PerMinuteAttackSpeedRelatedProcChance )
        };
    }
}
