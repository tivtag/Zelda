// <copyright file="SelectedSpriteContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.SelectedSpriteContainer structure.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Tile
{
    using Atom.Xna;

    /// <summary>
    /// Stores data about the currently selected sprite.
    /// </summary>
    public struct SelectedSpriteContainer
    {
        /// <summary>
        /// Identifies the currently selected <see cref="ISprite"/>.
        /// </summary>
        public ISprite Sprite
        { 
            get
            {
                return this.sprite; 
            }
        }

        /// <summary>
        /// The index of the Sprite into the SpriteSheet.
        /// This value is stored within into the TileMapDataLayer.
        /// </summary>
        public int Index
        {
            get 
            { 
                return this.index;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedSpriteContainer"/> class.
        /// </summary>
        /// <param name="sprite">The currently selected sprite.</param>
        /// <param name="index">The index into the spritesheet of the currently selected sprite.</param>
        public SelectedSpriteContainer( ISprite sprite, int index )
        {
            this.sprite = sprite;
            this.index  = index;
        }

        private int index;
        private ISprite sprite;
    }
}
