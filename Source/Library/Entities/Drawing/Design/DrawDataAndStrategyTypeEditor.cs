// <copyright file="DrawDataAndStrategyTypeEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.Design.DrawDataAndStrategyTypeEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Drawing.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Atom.Design;
    using Zelda.Design;

    /// <summary>
    /// Implements an <see cref="System.Drawing.Design.UITypeEditor"/> that
    /// allows the user to select from a list of IDrawDataAndStrategy types.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DrawDataAndStrategyTypeEditor : System.Drawing.Design.UITypeEditor
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
            IItemSelectionDialogFactory factory = DesignTime.GetService<IItemSelectionDialogFactory>();
            IEnumerable<Type> types = DesignTime.Services.DrawStrategyManager.KnownStrategies;

            IEnumerable<NameableObjectWrapper<Type>> wrappedTypes = types.Select( t => new NameableObjectWrapper<Type>( t, tt => tt.Name ) );
            IItemSelectionDialog<NameableObjectWrapper<Type>> dialog       = factory.Build<NameableObjectWrapper<Type>>( wrappedTypes );
            
            var type = value as Type;

            if( type != null )
            {
                dialog.SelectedItem = wrappedTypes.FirstOrDefault( wrapper => wrapper.Object.Equals( type ) );
            }

            if( dialog.ShowDialog() )
            {
                type = dialog.SelectedItem != null ? dialog.SelectedItem.Object : null;
            }

            return type;
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
