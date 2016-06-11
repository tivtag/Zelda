// <copyright file="FactionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Factions.Design.FactionEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Modifiers.Design
{
    using System;

    /// <summary>
    /// Implements an ObjectSelectionEditor that provides a mechanism
    /// that allows the user to select a Faction.
    /// </summary>
    public sealed class EntityModifierEditor : Atom.Design.BaseItemCreationEditor
    {
        /// <summary>
        /// Gets the Types of the objects that can be created by this EntityModifierEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return EntityModifierEditor.types;
        }

        /// <summary>
        /// Enumerates the types this EntityModifierEditor can create.
        /// </summary>
        private static readonly Type[] types = new Type[] { 
            typeof( SetBehaviourEntityModifier )
        };
    }
}
