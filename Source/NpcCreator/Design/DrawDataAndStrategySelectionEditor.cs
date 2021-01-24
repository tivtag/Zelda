// <copyright file="DrawDataAndStrategySelectionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.Design.DrawDataAndStrategySelectionEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.NpcCreator.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    /// <summary>
    /// Defines an UITypeEditor that allows the user to select an DrawDataAndStrategy type.
    /// </summary>
    internal sealed class DrawDataAndStrategySelectionEditor : UITypeEditor
    {
        /// <summary>
        /// Edits the value of the specified object using the editor style indicated
        /// by the System.Drawing.Design.UITypeEditor.GetEditStyle() method.
        /// </summary>
        /// <param name="context">
        /// An System.ComponentModel.ITypeDescriptorContext that can be used to gain
        /// additional context information.
        /// </param>
        /// <param name="provider">
        ///  An System.IServiceProvider that this editor can use to obtain services.
        ///  </param>
        /// <param name="value">  
        /// The object to edit.
        /// </param>
        /// <returns>
        /// The new value of the object.
        /// </returns>
        public override object EditValue( 
            ITypeDescriptorContext context,
            IServiceProvider provider, 
            object value )
        {
            var dialog = new Dialogs.DrawStrategySelectionDialog( App.Current.DrawStrategyManager );

            dialog.ShowDialog();

            return dialog.SelectedType;
        }

        /// <summary>
        /// Gets the editor style used by the 
        /// System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)
        /// method.
        /// </summary>
        /// <param name="context">
        /// An System.ComponentModel.ITypeDescriptorContext that can be used to gain
        /// additional context information.
        /// </param>
        /// <returns>
        /// Returns UITypeEditorEditStyle.Modal.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
