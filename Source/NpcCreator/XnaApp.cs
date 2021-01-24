// <copyright file="XnaApp.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.NpcCreator.XnaApp class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.NpcCreator
{
    using Atom;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Wpf;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Handles all Xna-related logic of the ItemCreator.
    /// </summary>
    internal sealed class XnaApp : HiddenXnaGame
    {
        /// <summary>
        /// Gets the ISpriteLoader object that provides a mechanism for loading ISprite assets. 
        /// </summary>
        public ISpriteLoader SpriteLoader
        {
            get
            {
                return this.spriteLoader;
            }
        }

        /// <summary>
        /// Gets the ISpriteSource that provides access to all ISprite assets that might be used.
        /// </summary>
        public ISpriteSource SpriteSource
        {
            get
            {
                return this.spriteLoader;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XnaApp"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public XnaApp( IZeldaServiceProvider serviceProvider )
        {
            this.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.textureLoader = new Texture2DLoader( this.Services );

            this.Services.AddService<ITexture2DLoader>( this.textureLoader );
            this.Services.AddService<ISpriteLoader>( this.spriteLoader );
            this.Services.AddService<ISpriteSource>( this.spriteLoader );
            this.Services.AddService<Zelda.Items.Sets.ISetDatabase>( new Zelda.Items.Sets.SetDatabase( serviceProvider ) );
            this.Services.AddService<IEffectLoader>( EffectLoader.Create( this.Services ) );
            this.Services.AddService<IRenderTarget2DFactory>( new RenderTarget2DFactory( new Atom.Math.Point2( 1, 1 ), this.Graphics ) );

            GlobalServices.Container.AddService<Atom.Design.IItemSelectionDialogFactory>( new Atom.Wpf.Design.ItemSelectionDialogFactory() );
            GlobalServices.Container.AddService<Atom.Design.IExistingItemCollectionEditorFormFactory>( new Atom.Wpf.Design.ExistingItemCollectionEditorFormFactory() );
        }

        /// <summary>
        /// Loads the resources required by this XnaApp.
        /// </summary>
        protected override void LoadContent()
        {
            this.LoadSprites();
        }

        /// <summary>
        /// Loads the sprites that might be used for items.
        /// </summary>
        private void LoadSprites()
        {            
            this.spriteLoader.Insert( @"Content\Sprites\Mixed.sdb", this.textureLoader );
        }

        /// <summary>
        /// Provides a mechanism for loading ISprite assets. 
        /// </summary>
        private readonly SpriteLoader spriteLoader = new SpriteLoader();

        /// <summary>
        /// Provides a mechanism for loading Texture2D assets. 
        /// </summary>
        private readonly ITexture2DLoader textureLoader;
    }
}
