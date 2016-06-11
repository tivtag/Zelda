// <copyright file="FactionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Factions.Design.FactionEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Factions.Design
{
    /// <summary>
    /// Implements an ObjectSelectionEditor that provides a mechanism
    /// that allows the user to select a Faction.
    /// </summary>
    internal sealed class FactionEditor : Atom.Design.BaseItemSelectionEditor<Faction>
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this FactionEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Faction> GetSelectableItems()
        {
            return FactionList.Known;
        }
    }
}
