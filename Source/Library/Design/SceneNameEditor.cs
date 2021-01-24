// <copyright file="SceneNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Design.SceneNameEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Design
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that
    /// allows the user to select an existing ZeldaScene.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SceneNameEditor : System.Drawing.Design.UITypeEditor
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
        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            var dialog = new OpenFileDialog() {
                Title            = Resources.DialogTitle_SelectScene,
                Filter           = Resources.Filter_SceneResources,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Scenes\" ),
                RestoreDirectory = true
            };

            if( dialog.ShowDialog() == DialogResult.OK )
            {
                return System.IO.Path.GetFileNameWithoutExtension( dialog.FileName );
            }

            return value;
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
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
    }
}
