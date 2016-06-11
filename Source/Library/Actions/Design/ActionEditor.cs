// <copyright file="ActionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.Design.ActionEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Actions.Design
{
    using System;

    /// <summary>
    /// Implements an ObjectSelectionEditor that provides a mechanism
    /// that allows the user to select a Faction.
    /// </summary>
    public sealed class ActionEditor : Atom.Design.BaseItemCreationEditor
    {
        /// <summary>
        /// Gets the types that the user can select in this BaseTypeSelectionEditor.
        /// </summary>
        /// <returns>
        /// The types the user can select.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return ActionEditor.types;
        }

        /// <summary>
        /// Enumerates the types that can be created by this ActionEditor.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( UI.OpenSharedChestAction ),
            typeof( Player.StatResetAction ),
            typeof( Player.TalentResetAction ),
            typeof( Player.TeachSharedChestTierAction ),
            typeof( Scene.ActivateEntityAction ),
            typeof( Scene.ExecuteEventAction )
        };
    }
}
