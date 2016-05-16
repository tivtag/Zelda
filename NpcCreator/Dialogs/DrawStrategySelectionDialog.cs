// <copyright file="DrawStrategySelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.Dialogs.DrawStrategySelectionDialog class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator.Dialogs
{
    /// <summary>
    /// Defines a dialog that allows the user to select the Draw Strategy to use.
    /// </summary>
    internal sealed class DrawStrategySelectionDialog : SelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawStrategySelectionDialog"/> class.
        /// </summary>
        /// <param name="drawStrategyManager">
        /// The DrawStrategyManager that is used to populate the list of IDrawStrategies.
        /// </param>
        public DrawStrategySelectionDialog( Zelda.Entities.Drawing.DrawStrategyManager drawStrategyManager )
            : base( drawStrategyManager.KnownStrategies )
        {
        }
    }
}
