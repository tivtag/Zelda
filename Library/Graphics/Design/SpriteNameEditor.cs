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
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that loads <see cref="ISprite"/>
    /// assets.
    /// </summary>
    public class SpriteNameEditor : SpriteEditor
    {
        /// <summary>
        /// Overriden to return the name of the <see cref="ISpriteAsset"/>.
        /// </summary>
        /// <param name="asset">
        /// The ISpriteAsset that has been selected by the user.
        /// </param>
        /// <returns>
        /// The name of the specified ISpriteAsset.
        /// </returns>
        protected override object CreateActual( ISpriteAsset asset )
        {
            return asset != null ? asset.Name : null;
        }
    }
}
