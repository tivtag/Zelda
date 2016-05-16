// <copyright file="DropRequirementSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.Dialogs.DropRequirementSelectionDialog class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.ItemCreator.Dialogs
{
    using System;
    using Zelda.Core.Requirements;
    using Zelda.Items.DropRequirements;

    /// <summary>
    /// Defines a dialog that asks the user to select a IItemDropRequirements type.
    /// </summary>
    internal sealed class DropRequirementSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropRequirementSelectionDialog"/> class.
        /// </summary>
        public DropRequirementSelectionDialog()
            : base( Effects )
        {
        }
        
        /// <summary>
        /// The drop requirement available to be selected in the <see cref="DropRequirementSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Effects = new Type[] {
            typeof( QuestNotActiveNorCompletedRequirement ),
            typeof( DoesntHaveItemsRequirement ),
            typeof( DoesntKnowSongRequirement ),
            typeof( MinimumDifficultyRequirement ),
            typeof( NeverRequirement )
        };
    }
}
