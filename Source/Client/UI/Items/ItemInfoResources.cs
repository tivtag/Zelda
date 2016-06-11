// <copyright file="ItemInfoResources.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfoResources class. 
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items
{
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Provides access to the resources required to visualize items.
    /// </summary>
    internal sealed class ItemInfoResources : IZeldaSetupable
    {
        /// <summary>
        /// The <see cref="IFont"/> that is used to draw an Item's info-text.
        /// </summary>
        public IFont FontText
        {
            get
            {
                return this.fontText;
            }
        }

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw most of the info text.
        /// </summary>
        public IFont FontSmallText
        {
            get
            {
                return this.fontSmallText;
            }
        }

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw various important things.
        /// </summary>
        public IFont FontSmallBoldText
        {
            get
            {
                return this.fontSmallBoldText;
            }
        }

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw an Item's name.
        /// </summary>
        public IFont FontLargeText
        {
            get
            {
                return this.fontLargeText;
            }
        }

        /// <summary>
        /// Gets the sprite that is used to visualize.
        /// </summary>
        public Sprite SpriteRuby
        {
            get
            {
                return this.spriteRuby;
            }
        }

        /// <summary>
        /// Setups and loads the ItemVisualizationResources.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.spriteRuby = serviceProvider.SpriteLoader.LoadSprite( "RubyUI2" );

            this.fontText = UIFonts.Tahoma9;
            this.fontSmallText = UIFonts.Tahoma7;
            this.fontLargeText = UIFonts.TahomaBold10;      
            this.fontSmallBoldText = UIFonts.TahomaBold8;     
        }

        /// <summary>
        /// Gets a UserInterface Ruby Sprite.
        /// </summary>
        private Sprite spriteRuby;
        
        /// <summary>
        /// The <see cref="IFont"/> that is used to draw an Item's info-text.
        /// </summary>
        private IFont fontText;

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw most of the info text.
        /// </summary>
        private IFont fontSmallText;

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw various important things.
        /// </summary>
        private IFont fontSmallBoldText;

        /// <summary>
        /// The <see cref="IFont"/> that is used to draw an Item's name.
        /// </summary>
        private IFont fontLargeText;
    }
}
