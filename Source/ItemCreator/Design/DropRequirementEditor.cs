// <copyright file="DropRequirementEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.Design.DropRequirementEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.ItemCreator.Design
{
    using System;
    using System.ComponentModel;
    using Zelda.Core.Requirements;

    /// <summary>
    /// Defines an <see cref="UITypeEditor"/> that asks the user to select an IItemDropRequirement.
    /// </summary>
    internal sealed class DropRequirementEditor : System.Drawing.Design.UITypeEditor
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
            Type type = value != null ? value.GetType() : null;

            var dialog = new Dialogs.DropRequirementSelectionDialog() {
                SelectedType = type
            };

            if( dialog.ShowDialog() == true )
            {
                if( dialog.SelectedType == type )
                    return value;

                type = dialog.SelectedType;

                if( type != null )
                {
                    // Create instance:
                    var requirement = (IRequirement)Activator.CreateInstance( type );

                    // Allow setup of objects: :-p
                    var editNodifier = requirement as IManualEditNotifier;
                    if( editNodifier != null )
                        editNodifier.StartManualEdit();

                    value = requirement;
                }
                else
                {
                    value = null;
                }
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
