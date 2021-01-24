// <copyright file="GoalSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.Dialogs.GoalSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuestCreator.Dialogs
{
    using System;
    using Zelda.Quests.Goals;

    /// <summary>
    /// Defines a dialog that asks the user to select a IQuestGoal type.
    /// </summary>
    internal sealed class GoalSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoalSelectionDialog"/> class.
        /// </summary>
        public GoalSelectionDialog()
            : base( Types )
        {
            this.Title = Properties.Resources.DialogTitle_GoalSelection;
        }

        /// <summary>
        /// The types available to be selected in the <see cref="GoalSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Types = new Type[2] {
            typeof( KillEnemiesGoal ),
            typeof( GetItemsGoal )
        };
    }
}
