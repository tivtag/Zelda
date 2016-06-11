// <copyright file="ItemEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Design.ItemEditor class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Design
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    
    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="Item"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ItemEditor : System.Drawing.Design.UITypeEditor
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
        [System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust" )]
        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            var dialog = new OpenFileDialog() {
                Title            = Resources.DialogTitle_SelectItem,
                Filter           = Resources.Filter_ItemResources,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Items\" ),
                RestoreDirectory = true
            };

            if( dialog.ShowDialog() == DialogResult.OK )
            {
                return LoadItem( dialog.FileName );
            }

            return value;
        }

        /// <summary>
        /// Tries to load the Item with the given fileName.
        /// </summary>
        /// <param name="fileName">
        /// The filename that has been selected by the user.
        /// </param>
        /// <returns>
        /// The loaded Item.
        /// </returns>
        private static Item LoadItem( string fileName )
        {
            var serviceProvider = Zelda.Design.DesignTime.Services;
            var itemManager = serviceProvider.ItemManager;

            string itemName = System.IO.Path.GetFileNameWithoutExtension( fileName );
            return itemManager.Get( itemName );
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
        [System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust" )]
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
    }
}
