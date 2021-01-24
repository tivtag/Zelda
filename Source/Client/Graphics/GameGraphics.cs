// <copyright file="GameGraphics.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameGraphics class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Fonts;
    using Atom.Xna.Particles;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.UI;

    /// <summary>
    /// Encapsulates the graphics related initialization and settings.
    /// </summary>
    internal sealed class GameGraphics
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="IResolutionService"/> which provides access
        /// to the current resolution settings.
        /// </summary>
        public IResolutionService Resolution
        {
            get
            {
                return this.resolution;
            }
        }

        /// <summary>
        /// Gets the currently used <see cref="IDrawingPipeline"/>.
        /// </summary>
        public IDrawingPipeline Pipeline
        {
            get
            {
                return this.drawingPipeline;
            }
        }

        /// <summary>
        /// Gets the <see cref="BloomDrawingPipeline"/>; which may not be supported
        /// on the current hardware.
        /// </summary>
        public BloomDrawingPipeline BloomPipeline
        {
            get
            {
                return this.bloomDrawingPipeline;
            }
        }

        /// <summary>
        /// Gets the Atom.Xna.Particles.ParticleRenderer object.
        /// </summary>
        public Atom.Xna.Particles.ParticleRenderer ParticleRenderer
        {
            get
            {
                return this.particleRenderer;
            }
        }

        /// <summary>
        /// Gets the xna GraphicsDevice that is required for drawing operations.
        /// </summary>
        public GraphicsDevice Device
        {
            get
            {
                return this.graphics.GraphicsDevice;
            }
        }

        /// <summary>
        /// Gets the ZeldaDrawContext that is used by default.
        /// </summary>
        public ZeldaDrawContext DrawContext
        {
            get
            {
                return this.drawContext;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading ISprite assets.
        /// </summary>
        public ISpriteLoader SpriteLoader
        {
            get
            {
                return this.spriteLoader;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading Texture2D assets.
        /// </summary>
        public ITexture2DLoader TextureLoader
        {
            get
            {
                return this.textureLoader;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading ISpriteSheet assets.
        /// </summary>
        public ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return this.spriteSheetLoader;
            }
        }
        
        public bool SupportsHighDef
        {
            get
            {
                return this.Device.Adapter.IsProfileSupported( GraphicsProfile.HiDef );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the GameGraphics class.
        /// </summary>
        /// <param name="game">
        /// The main game object that provides access to all game-services.
        /// </param>
        public GameGraphics( ZeldaGame game )
        {
            this.game = game;
            this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager( game ) {
                SynchronizeWithVerticalRetrace = Settings.Instance.VSync
            };

            this.resolution = new ResolutionService( graphics, game.Log );

            this.RegisterBasicServices();

            this.resolution.EnsureAspectRatioSupport( GraphicsAdapter.DefaultAdapter );
            this.graphics.PreparingDeviceSettings += this.OnPreparingDeviceSettings;
            this.effectLoader = EffectLoader.Create( game.Services );

            // Other.
            this.renderTargetFactory = new Atom.Xna.RenderTarget2DFactory( this.resolution.OutputSize, this.graphics );

            this.rescaler = new ViewToWindowRescaler( new Atom.Xna.RenderTarget2DFactory( this.resolution.ViewSize, this.graphics ), game );
            this.lightMap = new LightMap( this.renderTargetFactory, this.graphics );
            this.sceneDrawer = new ZeldaSceneDrawer( this.lightMap );

            this.fontLoader = new FontLoader( game );
            this.textureLoader = new Texture2DLoader( game );
            this.spriteSheetLoader = new SpriteSheetLoader( this.spriteLoader );
            this.particleRenderer = new Zelda.Graphics.Particles.ZeldaParticleRenderer( game );

            this.RegisterServices();

            this.defaultDrawingPipeline = new NormalDrawingPipeline( this.rescaler, this.sceneDrawer );
            this.bloomDrawingPipeline = new BloomDrawingPipeline( this.game );
            //this.highDynamicRangeDrawingPipeline = new HighDynamicRangeDrawingPipeline( this.game );
        }

        /// <summary>
        /// Registers the basic services this GameGraphics instance provides.
        /// </summary>
        private void RegisterBasicServices()
        {
            this.game.Services.AddService<IResolutionService>( this.resolution );
        }

        /// <summary>
        /// Registers the services this GameGraphics instance offers.
        /// </summary>
        private void RegisterServices()
        {
            Microsoft.Xna.Framework.GameServiceContainer services = this.game.Services;
            services.AddService<IEffectLoader>( this.effectLoader );
            services.AddService<ITexture2DLoader>( this.textureLoader );
            services.AddService<ISpriteLoader>( this.spriteLoader );
            services.AddService<ISpriteSheetLoader>( this.spriteSheetLoader );

            services.AddService<IViewToWindowRescaler>( this.rescaler );
            services.AddService<IRenderTarget2DFactory>( this.renderTargetFactory );
            services.AddService<ParticleRenderer>( this.particleRenderer );

            services.AddService<LightMap>( this.lightMap );
            services.AddService<ZeldaSceneDrawer>( this.sceneDrawer );

            services.AddService<Zelda.UI.IItemInfoVisualizer>( this.itemInfoVisualizer );
        }

        /// <summary>
        /// Gets called when the Xna graphics device is preparing its settings.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Stores the event arguments.</param>
        private void OnPreparingDeviceSettings( object sender, Microsoft.Xna.Framework.PreparingDeviceSettingsEventArgs e )
        {
            Microsoft.Xna.Framework.GraphicsDeviceInformation deviceInfo = e.GraphicsDeviceInformation;
            PresentationParameters presentParams = deviceInfo.PresentationParameters;

            // Apply common settings.
            presentParams.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            presentParams.DepthStencilFormat = DepthFormat.None;
            deviceInfo.GraphicsProfile =  deviceInfo.Adapter.IsProfileSupported( GraphicsProfile.HiDef ) ? GraphicsProfile.HiDef : GraphicsProfile.Reach;

            // Clamp into valid range.
            Point2 backBufferSize = this.resolution.GetAdjustedBackBufferSize( new Point2( presentParams.BackBufferWidth, presentParams.BackBufferHeight ) );
            presentParams.BackBufferWidth = backBufferSize.X;
            presentParams.BackBufferHeight = backBufferSize.Y;

            LogHelper.LogInfo( deviceInfo, game.Log );
        }

        /// <summary>
        /// Initializes the GameGraphics.
        /// </summary>
        public void Initialize()
        {
            //this.graphics.GraphicsDevice.DepthStencilState = null;
            this.rescaler.Initialize();

            // Initialize Window
            game.Window.AllowUserResizing = true;
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle( game.Window.Handle );
            form.MaximizeBox = false;

            // :D
            this.ChangePipeline( this.defaultDrawingPipeline );
            //this.ChangePipeline( this.bloomDrawingPipeline );
            //this.ChangePipeline( this.highDynamicRangeDrawingPipeline );
        }

        /// <summary>
        /// Loads globally required graphics content.
        /// </summary>
        public void LoadContent( bool isInitialLoad )
        {
            if( isInitialLoad )
            {
                UIFonts.Load( this.fontLoader );
                this.PreloadSprites();
                this.linksSprites = new Zelda.Entities.Drawing.LinkSprites( spriteLoader );
                this.game.Services.AddService<Zelda.Entities.Drawing.LinkSprites>( this.linksSprites );
            }
            else
            {
                this.effectLoader.UnloadAll();
                this.fontLoader.Reload();

                this.textureLoader.UnloadAll();
                this.spriteLoader.ReloadTextures( this.textureLoader );
                this.linksSprites.ReapplyTint();
            }

            this.rescaler.LoadContent();
            this.drawContext = new ZeldaDrawContext( this.Device );

            this.itemInfoVisualizer.Setup( this.game );
            this.particleRenderer.LoadContent( this.game.Content );
            this.lightMap.LoadContent();
        }

        /// <summary>
        /// Loads the sprites used by the game
        /// </summary>
        private void PreloadSprites()
        {
            this.spriteLoader.Insert( 
                new [] { @"Content\Sprites\Mixed.sdb", @"Content\Sprites\Link.sdb" },
                this.textureLoader
            );
        }

        /// <summary>
        /// Unloads the content loaded by this GameGraphics object.
        /// </summary>
        internal void UnloadContent()
        {
            this.particleRenderer.Dispose();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to change the current IDrawingPipeline.
        /// </summary>
        /// <param name="newDrawingPipeline">
        /// The DrawingPipeline to change to.
        /// </param>
        /// <returns>
        /// true if the change has been successful;
        /// otherwise false.
        /// </returns>
        public bool ChangePipeline( DrawingPipeline newDrawingPipeline )
        {
            IDrawingPipeline pipeline = this.GetDrawingPipeline( newDrawingPipeline );

            if( pipeline.IsSupported )
            {
                this.ChangePipeline( pipeline );
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the IDrawingPipeline that matches the given DrawingPipeline enumeration.
        /// </summary>
        /// <param name="drawingPipeline">
        /// A DrawingPipeline enumeration.
        /// </param>
        /// <returns>
        /// The matching IDrawingPipeline.
        /// </returns>
        private IDrawingPipeline GetDrawingPipeline( DrawingPipeline drawingPipeline )
        {
            switch( drawingPipeline )
            {
                case DrawingPipeline.Normal:
                    return this.defaultDrawingPipeline;

                case DrawingPipeline.Bloom:
                    return this.bloomDrawingPipeline;

                //case DrawingPipeline.HighDynamicRange:
                //    return this.highDynamicRangeDrawingPipeline;

                default:
                    throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Changes the currently used IDrawingPipeline.
        /// </summary>
        /// <param name="newDrawingPipeline">
        /// The IDrawingPipeline to change to.
        /// </param>
        private void ChangePipeline( IDrawingPipeline newDrawingPipeline )
        {
            if( this.drawingPipeline == newDrawingPipeline )
            {
                return;
            }

            if( this.drawingPipeline != null )
            {
                this.drawingPipeline.Unload();
            }

            this.drawingPipeline = newDrawingPipeline;
            this.drawingPipeline.Load();
        }

        /// <summary>
        /// Enables or disables V-sync.
        /// </summary>
        /// <param name="state">
        /// States whether to enable or disable v-sync.
        /// </param>
        internal void SetVsync( bool state )
        {
            graphics.SynchronizeWithVerticalRetrace = state;
            ApplyChanges();
        }

        /// <summary>
        /// Enables or disables fullscreen mode.
        /// </summary>
        /// <param name="state">
        /// States whether to enable or disable fullscreen mode.
        /// </param>
        internal void SetFullScreen( bool state )
        {
            if( graphics.IsFullScreen == state )
                return;

            graphics.IsFullScreen = state;
            ApplyChanges();

            if( !state )
            {
                game.Window.AllowUserResizing = true;
            }
        }

        /// <summary>
        /// Applies graphics changes done do the device.
        /// </summary>
        public void ApplyChanges()
        {
            graphics.ApplyChanges();
            Device.Reset();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The sprites used to draw the main character.
        /// </summary>
        private Entities.Drawing.LinkSprites linksSprites;

        /// <summary>
        /// The IDrawingPipeline that is currently used by the game.
        /// </summary>
        private IDrawingPipeline drawingPipeline;

        /// <summary>
        /// The default IDrawingPipeline that should run on all supported platforms.
        /// </summary>
        private readonly NormalDrawingPipeline defaultDrawingPipeline;

        /// <summary>
        /// The IDrawingPipeline that applies a bloom effect. Not supported on all platforms.
        /// </summary>
        private readonly BloomDrawingPipeline bloomDrawingPipeline;

        ///// <summary>
        ///// The IDrawingPipeline that applies a high dynamic range effect. Not supported on all platforms.
        ///// </summary>
        //private readonly HighDynamicRangeDrawingPipeline highDynamicRangeDrawingPipeline;

        /// <summary>
        /// The IDrawContext used by the Game.
        /// </summary>
        private ZeldaDrawContext drawContext;

        /// <summary>
        /// Is responsible for rescaling the drawn image to fit the whole scaled window.
        /// </summary>
        private readonly ViewToWindowRescaler rescaler;

        /// <summary>
        /// The ZeldaSceneDrawer that encapsulates the scene drawing operation.
        /// </summary>
        private readonly ZeldaSceneDrawer sceneDrawer;

        /// <summary>
        /// Provides a mechanism that allows the drawing of the relevant information
        /// the players needs to know about an item.
        /// </summary>
        private readonly Zelda.UI.IItemInfoVisualizer itemInfoVisualizer = new Zelda.UI.Items.ItemInfoVisualizer();

        /// <summary>
        /// The Atom.Xna.Particles.ParticleRenderer object.
        /// </summary>
        private readonly ParticleRenderer particleRenderer;

        /// <summary>
        /// The Microsoft.Xna.Framework.GraphicsDeviceManager object.
        /// </summary>
        private readonly Microsoft.Xna.Framework.GraphicsDeviceManager graphics;

        /// <summary>
        /// The factory responsible for creating all full-screen RenderTargets used by the game.
        /// </summary>
        private readonly RenderTarget2DFactory renderTargetFactory;

        /// <summary>
        /// The LightMap used during the light drawing pass.
        /// </summary>
        public readonly LightMap lightMap;

        /// <summary>
        /// The main game object that provides access to all game-services.
        /// </summary>
        private readonly ZeldaGame game;

        /// <summary>
        /// The IAssetLoader responsible for loading Effect assets.
        /// </summary>
        private readonly IEffectLoader effectLoader;

        /// <summary>
        /// The IAssetLoader responsible for loading Texture2D assets.
        /// </summary>
        private readonly ITexture2DLoader textureLoader;

        /// <summary>
        /// The IAssetLoader responsible for loading IFont assets.
        /// </summary>
        private readonly IFontLoader fontLoader;

        /// <summary>
        /// The IAssetLoader responsible for loading ISpriteSheet assets.
        /// </summary>
        private readonly ISpriteSheetLoader spriteSheetLoader;

        /// <summary>
        /// The IAssetLoader responsible for loading ISprite assets.
        /// </summary>
        private readonly SpriteLoader spriteLoader = new SpriteLoader();

        /// <summary>
        /// Provides access to the current resolution that the game is running under.
        /// </summary>
        private readonly IResolutionService resolution;

        #endregion
    }
}