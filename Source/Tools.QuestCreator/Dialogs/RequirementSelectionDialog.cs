// <copyright file="RequirementSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.Dialogs.RequirementSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuestCreator.Dialogs
{
    using System;
    using Zelda.Quests.Requirements;

    /// <summary>
    /// Defines a dialog that asks the user to select a IQuestRequirement type.
    /// </summary>
    internal sealed class RequirementSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementSelectionDialog"/> class.
        /// </summary>
        public RequirementSelectionDialog()
            : base( Types )
        {
            this.Title = Properties.Resources.DialogTitle_RequirementSelection;
        }
        
        /// <summary>
        /// The types available to be selected in the <see cref="RequirementSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Types = new Type[1] {
            typeof( QuestCompletedRequirement )
        };
    }
}
