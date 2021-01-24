// <copyright file="RewardSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.Dialogs.RewardSelectionDialog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuestCreator.Dialogs
{
    using System;
    using Zelda.Quests.Rewards;

    /// <summary>
    /// Defines a dialog that asks the user to select a IQuestReward type.
    /// </summary>
    internal sealed class RewardSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RewardSelectionDialog"/> class.
        /// </summary>
        public RewardSelectionDialog()
            : base( Types )
        {
            this.Title = Properties.Resources.DialogTitle_RewardSelection;
        }

        /// <summary>
        /// The types available to be selected in the <see cref="RewardSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Types = new Type[4] {
            typeof( ExperienceReward ),
            typeof( ReputationReward ),            
            typeof( RubyReward ),
            typeof( ItemReward )
        };
    }
}
