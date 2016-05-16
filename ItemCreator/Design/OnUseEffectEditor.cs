// <copyright file="OnUseEffectEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.Design.OnUseEffectEditor class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.ItemCreator.Design
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines an <see cref="UITypeEditor"/> that asks the user to select the OnUseEffect.
    /// </summary>
    internal sealed class OnUseEffectEditor : System.Drawing.Design.UITypeEditor
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

            var dialog = new Dialogs.UseEffectSelectionDialog() {
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
                    var effect = (Zelda.Items.ItemUseEffect)Activator.CreateInstance( type );

                    // Allow setup of objects: :-p
                    var editNodifier = effect as IManualEditNotifier;
                    if( editNodifier != null )
                        editNodifier.StartManualEdit();

                    // Every Effect has a cooldown!
                    effect.Cooldown = new Cooldown();

                    value = effect;
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
