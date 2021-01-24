// <copyright file="SpriteAssetEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.Design.SpriteAssetEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Graphics.Design
{
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="ISpriteAsset"/>s.
    /// </summary>
    public sealed class SpriteAssetEditor : SpriteEditor
    {
        /// <summary>
        /// Tries to load the sprite with the given fileName.
        /// </summary>
        /// <param name="asset">
        /// The asset to load.
        /// </param>
        /// <returns>
        /// The loaded ISprite.
        /// </returns>
        protected override object CreateActual( ISpriteAsset asset )
        {
            return asset;
        }
    }
}
