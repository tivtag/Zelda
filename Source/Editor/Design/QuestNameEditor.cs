// <copyright file="QuestNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Design.QuestNameEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Design
{
    using System;
    using System.Windows.Forms.Design;

    /// <summary>
    /// Defines the UITypeEditor (used in a property grid) that
    /// asks the user to select a <see cref="Zelda.Quests.Quest"/> resource.
    /// </summary>
    internal sealed class QuestNameEditor : FileNameEditor
    {
        /// <summary>
        /// Initializes the open file dialog when it is created.
        /// </summary>
        /// <param name="openFileDialog">
        /// The System.Windows.Forms.OpenFileDialog to use to select a file name.
        /// </param>
        protected override void InitializeDialog( System.Windows.Forms.OpenFileDialog openFileDialog )
        {
            openFileDialog.Title            = Properties.Resources.DialogTitle_SelectQuest;
            openFileDialog.Filter           = Properties.Resources.Filter_QuestResources;
            openFileDialog.InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Quests\" );
            openFileDialog.RestoreDirectory = true;
        }
    }
}
