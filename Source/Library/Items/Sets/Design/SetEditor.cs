// <copyright file="SetEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.Design.SetEditor class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets.Design
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Atom;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="ISet"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SetEditor : System.Drawing.Design.UITypeEditor
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
                Title            = Zelda.Resources.DialogTitle_SelectSet,
                Filter           = Zelda.Resources.Filter_SetResources,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Items\Sets\" ),
                RestoreDirectory = true
            };

            if( dialog.ShowDialog() == DialogResult.OK )
            {
                return LoadSet( dialog.FileName );
            }

            return value;
        }

        /// <summary>
        /// Tries to load the ISet with the given fileName.
        /// </summary>
        /// <param name="fileName">
        /// The filename that has been selected by the user.
        /// </param>
        /// <returns>
        /// The loaded ISet.
        /// </returns>
        private static ISet LoadSet( string fileName )
        {
            var setDatabase = Zelda.Design.DesignTime.Services.GetService<ISetDatabase>();
            string setName = System.IO.Path.GetFileNameWithoutExtension( fileName );

            return setDatabase.Get( setName );
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
