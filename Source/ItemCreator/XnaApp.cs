// <copyright file="XnaApp.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.XnaApp class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.ItemCreator
{
    using Atom;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Wpf;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Difficulties;
    
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
        /// Gets the ITexture2DLoader object that provides a mechanism for loading Texture2D assets. 
        /// </summary>
        public ITexture2DLoader TextureLoader
        {
            get
            {
                return this.textureLoader;
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
        /// Gets the ISpriteSheetLoader object that provides a mechanism for loading ISpriteSheet assets. 
        /// </summary>
        public ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return this.spriteSheetLoader;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XnaApp"/> class.
        /// </summary>
        public XnaApp()
        {
            this.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            GameDifficulty.Current = DifficultyId.Easy;
            this.textureLoader = new Texture2DLoader( this.Services );
            
            this.spriteSheetLoader = new SpriteSheetLoader( this.spriteLoader );

            this.Services.AddService<ISpriteSheetLoader>( this.spriteSheetLoader );
            this.Services.AddService<ITexture2DLoader>( this.textureLoader );
            this.Services.AddService<ISpriteLoader>( this.spriteLoader );
            this.Services.AddService<ISpriteSource>( this.spriteLoader );

            this.Services.AddService<IEffectLoader>( EffectLoader.Create( this.Services ) );
            this.Services.AddService<IRenderTarget2DFactory>( new RenderTarget2DFactory( new Atom.Math.Point2(1,1), this.Graphics ) );

            this.Services.AddService<Zelda.BubbleTextManager>( new Zelda.BubbleTextManager() );
            this.Services.AddService<Zelda.Weather.Creators.IWeatherCreatorMap>( new Zelda.Weather.Creators.WeatherCreatorMap() );

            var dialogFactory = new Atom.Wpf.Design.ItemSelectionDialogFactory();
            this.Services.AddService( typeof( Atom.Design.IItemSelectionDialogFactory ), dialogFactory );
            GlobalServices.Container.AddService( typeof( Atom.Design.IItemSelectionDialogFactory ), dialogFactory );

            var existingItemCollectionEditorFormFactory = new Atom.Wpf.Design.ExistingItemCollectionEditorFormFactory();
            this.Services.AddService( typeof( Atom.Design.IExistingItemCollectionEditorFormFactory ), existingItemCollectionEditorFormFactory );
            GlobalServices.Container.AddService<Atom.Design.IExistingItemCollectionEditorFormFactory>( existingItemCollectionEditorFormFactory );
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
        /// Provides a mechanism for loading ISpriteSheet assets. 
        /// </summary>
        private readonly SpriteSheetLoader spriteSheetLoader;

        /// <summary>
        /// Provides a mechanism for loading Texture2D assets. 
        /// </summary>
        private readonly ITexture2DLoader textureLoader;
    }
}
