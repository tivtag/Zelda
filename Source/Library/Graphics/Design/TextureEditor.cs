// <copyright file="TextureEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Design.TextureEditor class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Design
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="Texture2D"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class TextureEditor : System.Drawing.Design.UITypeEditor
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
                Title            = Resources.DialogTitle_SelectTexture,
                Filter           = Resources.Filter_BinaryTextureResources,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Textures\" ),
                RestoreDirectory = true
            };

            if( dialog.ShowDialog() == DialogResult.OK )
            {
                return LoadTexture( dialog.FileName );
            }

            return value;
        }

        /// <summary>
        /// Tries to load the sprite with the given fileName.
        /// </summary>
        /// <param name="fileName">
        /// The filename that has been selected by the user.
        /// </param>
        /// <returns>
        /// The loaded Texture2D.
        /// </returns>
        private static Texture2D LoadTexture( string fileName )
        {
            IZeldaServiceProvider serviceProvider = Zelda.Design.DesignTime.Services;
            Atom.Xna.ITexture2DLoader textureLoader = serviceProvider.TextureLoader;

            string relativeFileName = fileName.Replace( AppDomain.CurrentDomain.BaseDirectory, string.Empty );
            string assetName = Path.ChangeExtension( relativeFileName, null );

            return textureLoader.Load( assetName );
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

        /// <summary>
        /// The initial relative directory path in which texture files are assumed to be localed.
        /// </summary>
        private const string BasePath = "Content/Textures/";
    }
}
