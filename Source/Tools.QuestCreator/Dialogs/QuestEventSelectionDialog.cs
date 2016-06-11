// <copyright file="QuestEventSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuestCreator.Dialogs.QuestEventSelectionDialog class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.QuestCreator.Dialogs
{
    using System;
    using Zelda.Quests.Events;

    /// <summary>
    /// Defines a dialog that asks the user to select a IQuestEvent type.
    /// </summary>
    internal sealed class QuestEventSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestEventSelectionDialog"/> class.
        /// </summary>
        public QuestEventSelectionDialog()
            : base( Types )
        {
            this.Title = Properties.Resources.DialogTitle_CompletionEventSelection;
        }

        /// <summary>
        /// The types available to be selected in the <see cref="QuestEventSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Types = new Type[] {
            typeof( GiveQuestItemEvent ),
            typeof( ExecuteActionEvent ),
            typeof( RemovePersistentObjectEvent ),
            typeof( LearnSongEvent )
        };
    }
}
