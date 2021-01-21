// <copyright file="SpriteEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.Design.SpriteEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Atom;
    using Atom.Design;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="ISprite"/>
    /// assets.
    /// </summary>
    public class SpriteEditor : System.Drawing.Design.UITypeEditor
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
            IItemSelectionDialog<ISpriteAsset> dialog = BuildDialog( value );

            if( dialog.ShowDialog() )
            {
                return this.CreateActual( dialog.SelectedItem );
            }

            return value;
        }

        /// <summary>
        /// Builds the dialog that allows the user to select an ISpriteAsset.
        /// </summary>
        /// <param name="value">
        /// The value of the property beeing edited.
        /// </param>
        /// <returns>
        /// A newly created IItemSelectionDialog{ISpriteAsset}.
        /// </returns>
        private IItemSelectionDialog<ISpriteAsset> BuildDialog( object value )
        {
            IZeldaServiceProvider serviceProvider = Zelda.Design.DesignTime.Services;
            ISpriteSource spriteSource = serviceProvider.GetService<ISpriteSource>();

            IItemSelectionDialogFactory dialogFactory = serviceProvider.GetService<IItemSelectionDialogFactory>();
            IEnumerable<ISpriteAsset> assets = spriteSource.Sprites.Concat<ISpriteAsset>( spriteSource.AnimatedSprites );
            IItemSelectionDialog<ISpriteAsset> dialog = dialogFactory.Build<ISpriteAsset>( assets );

            dialog.SelectedItem = GetSelectedAsset( value, assets );
            return dialog;
        }

        /// <summary>
        /// Gets the ISpriteAsset the user had previously selected.
        /// </summary>
        /// <param name="value">
        /// The value the user had selected.
        /// </param>
        /// <param name="assets">
        /// The assets the user is allowed to select.
        /// </param>
        /// <returns>
        /// The ISpriteAsset that should be pre-selected.
        /// </returns>
        private static ISpriteAsset GetSelectedAsset( object value, IEnumerable<ISpriteAsset> assets )
        {
            var animation = value as SpriteAnimation;
         
            if( animation != null )
            {
                return animation.AnimatedSprite;
            }
            else
            {
                string name = value as string;

                if( name != null )
                {
                    return assets.FirstOrDefault( asset => asset.Name.Equals( name, StringComparison.Ordinal ) );
                }
                else
                {
                    return value as ISpriteAsset;
                }
            }
        }

        /// <summary>
        /// Tries to load the sprite with the given fileName.
        /// </summary>
        /// <param name="asset">
        /// The asset to load.
        /// </param>
        /// <returns>
        /// The loaded ISprite.
        /// </returns>
        protected virtual object CreateActual( ISpriteAsset asset )
        {
            return asset.CreateInstance();
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
