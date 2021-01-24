// <copyright file="FactionSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.Dialogs.FactionSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.NpcCreator.Dialogs
{
    /// <summary>
    /// Defines a dialog that allows the user to select the Faction use.
    /// </summary>
    internal sealed class FactionSelectionDialog : SelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactionSelectionDialog"/> class.
        /// </summary>
        public FactionSelectionDialog()
            : base( Factions.FactionList.Known )
        {
        }
    }
}
