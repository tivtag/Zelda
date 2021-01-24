// <copyright file="EntityTemplateNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Design.EntityTemplateNameEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Design
{
    using System;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    /// <summary>
    /// Defines the UITypeEditor (used in a property grid) that
    /// asks the user to select a Entity Template resource.
    /// </summary>
    internal sealed class EntityTemplateNameEditor : FileNameEditor
    {
        /// <summary>
        /// Initializes the open file dialog when it is created.
        /// </summary>
        /// <param name="openFileDialog">
        /// The System.Windows.Forms.OpenFileDialog to use to select a file name.
        /// </param>
        protected override void InitializeDialog( OpenFileDialog openFileDialog )
        {
            openFileDialog.Title            = Properties.Resources.DialogTitle_SelectEntityTemplate;
            openFileDialog.InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Objects\" );
            openFileDialog.RestoreDirectory = true;
        }
    }
}
