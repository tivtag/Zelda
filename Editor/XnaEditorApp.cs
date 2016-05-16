// <copyright file="XnaEditorApp.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.XnaEditorApp class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Scene.Xna;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Fonts;
    using Atom.Xna.Particles;
    using Atom.Xna.Wpf;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Difficulties;
    using Zelda.Editor.Event;
    using Zelda.Editor.Object;
    using Zelda.Editor.Story;
    using Zelda.Editor.Tile;
    using Zelda.Editor.Waypoint;
    using Zelda.Entities;

    /// <summary>
    /// Handles the xna related parts, such as drawing of the scene,
    /// of the Editor.
    /// </summary>
    public sealed class XnaEditorApp : XnaWpfGame, Atom.IObjectProviderContainer
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="Scene"/> property has changed.
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<SceneViewModel>> SceneChanged;

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets or sets the Scene currently beeing edited.
        /// </summary>
        public SceneViewModel Scene
        {
            get
            { 
                return this.scene;
            }

            set
            {
                var oldValue = this.scene;
                this.scene = value;

                if( this.scene != null )
                {
                    this.InitializeScene( scene );
                }
                
                this.SceneChanged.Raise( this, new ChangedValue<SceneViewModel>( oldValue, value ) );
            }
        }

        /// <summary>
        /// Gets or sets the current WorkspaceType active in the <see cref="XnaEditorApp"/>.
        /// </summary>
        public WorkspaceType CurrentWorkspace
        {
            get
            {
                if( this.workspace == null )
                    return WorkspaceType.None;

                return this.workspace.Type;
            }

            set
            {
                if( this.workspace != null )
                { 
                    // Are we already in the same workspace?
                    if( this.workspace.Type == value )
                        return;

                    this.workspace.Leave();
                }

                this.workspace = this.GetWorkspace( value );               

                if( this.workspace != null )
                {
                    this.workspace.Enter();
                }
            }
        }

        /// <summary>
        /// Gets the IWorkspace responsible for editing the tile map of the scene.
        /// </summary>
        public TileWorkspace TileWorkspace
        {
            get
            {
                return this.tileWorkspace;
            }
        }
        
        /// <summary>
        /// Gets the IWorkspace responsible for editing the Waypoint model of the current scene.
        /// </summary>
        public WaypointWorkspace WaypointWorkspace
        {
            get
            {
                return this.waypointWorkspace;
            }
        }

        /// <summary>
        /// Receives the area the tilemap of the scene is drawing in.
        /// </summary>
        public Atom.Math.Rectangle TileMapArea
        {
            get
            {
                return new Atom.Math.Rectangle( 0, 0, 33*16, 44*16 );
            }
        }

        /// <summary>
        /// Receives a Texture object which can be used to easily draw
        /// any colored Rectangle.
        /// </summary>
        public Texture2D WhiteTexture 
        { 
            get
            {
                return this.whiteTexture;
            }
        }

        /// <summary>
        /// Gets the Atom.Fmod.AudioSystem object.
        /// </summary>
        public Atom.Fmod.AudioSystem AudioSystem 
        { 
            get 
            { 
                return this.audioSystem;
            }
        }

        /// <summary>
        /// Gets the Atom.Xna.Particles.ParticleRendere object.
        /// </summary>
        public Atom.Xna.Particles.ParticleRenderer ParticleRenderer 
        {
            get
            { 
                return this.particleRenderer; 
            }
        }

        /// <summary>
        /// Gets the ZeldaSceneDrawer that provides a mechanism to draw ZeldaScenes.
        /// </summary>
        public ZeldaSceneDrawer SceneDrawer
        {
            get
            {
                return this.sceneDrawer;
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

        /// <summary>
        /// Gets the IAssetLoader responsible for loading IFont assets.
        /// </summary>
        public IFontLoader FontLoader
        {
            get
            {
                return this.fontLoader;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the quad tree of the current scene
        /// should be visualized.
        /// </summary>
        public bool VisualizeQuadTree 
        {
            get;
            set;
        }
    
        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="XnaEditorApp"/> class.
        /// </summary>
        /// <param name="editor">
        /// The application object.
        /// </param>
        /// <param name="controlHandle">
        /// The handle of the control xna is going to draw into.
        /// </param>
        public XnaEditorApp( EditorApp editor, IntPtr controlHandle )
            : base( new Atom.Math.Point2( 718, 712 ), controlHandle )
        {
            this.Graphics.GraphicsProfile = GraphicsProfile.HiDef;

            this.editor = editor;
            this.audioSystem = editor.AudioSystem;
            Mouse.WindowHandle = controlHandle;

            this.renderTargetFactory = new RenderTarget2DFactory( this.TileMapArea.Size, this.Graphics );
            this.particleRenderer = new SpriteBatchParticleRenderer( this.Graphics );
            this.lightMap = new LightMap( this.renderTargetFactory, this.Graphics );
            this.sceneDrawer = new ZeldaSceneDrawer( this.lightMap );
            this.fontLoader = new FontLoader( this.Services );
            this.textureLoader = new Texture2DLoader( this.Services );
            this.spriteSheetLoader = new SpriteSheetLoader( this.spriteLoader );

            this.RegisterServices();

            this.tileWorkspace = new TileWorkspace( this );
            this.objectWorkspace = new ObjectWorkspace( this );
            this.eventWorkspace = new EventWorkspace( this );
            this.waypointWorkspace = new WaypointWorkspace( this );
            this.storyWorkspace = new StoryWorkspace( this );
            this.CurrentWorkspace = WorkspaceType.Tile;
        }

        /// <summary>
        /// Registers the services this XnaEditorApp provides.
        /// </summary>
        private void RegisterServices()
        {
            var dialogFactory = new Atom.Wpf.Dialogs.ItemSelectionDialogFactory();
            var renderTargetFactory = new RenderTarget2DFactory( WindowSize, this.Graphics );

            this.Services.AddService<IRenderTarget2DFactory>( renderTargetFactory );
            this.Services.AddService<ISpriteLoader>( this.spriteLoader );
            this.Services.AddService<ISpriteSource>( this.spriteLoader );
            this.Services.AddService<IFontLoader>( this.fontLoader );
            this.Services.AddService<ITexture2DLoader>( this.textureLoader );
            this.Services.AddService<ISpriteSheetLoader>( this.spriteSheetLoader );
            this.Services.AddService<IEffectLoader>( EffectLoader.Create( this.Services ) );
            this.Services.AddService<Atom.Design.IItemSelectionDialogFactory>( dialogFactory );
            this.Services.AddService<Atom.Xna.Particles.ParticleRenderer>( this.particleRenderer );
            this.Services.AddService<Zelda.Items.Sets.ISetDatabase>( new Zelda.Items.Sets.SetDatabase( this.editor ) );
            
            this.Services.AddService<BubbleTextManager>( new BubbleTextManager() );

            this.Services.AddService<Zelda.Scripting.Design.IScriptEditor>( new Zelda.Editor.Workspaces.Scripts.ScriptEditor() );
            this.Services.AddService<Zelda.Weather.Creators.IWeatherCreatorMap>( this.weatherCreatorMap );
            this.Services.AddService<Zelda.Weather.IWeatherMachineSettings>( this.defaultWeatherMachineSettings );

            GlobalServices.Container.AddService<Atom.Design.IItemSelectionDialogFactory>( dialogFactory );

            var container = this.editor.ProviderContainer;
            container.Register<ZeldaCamera>( () => this.scene.Camera.Model );
            container.Register<ZeldaScene>( () => this.scene.Model );
        }

        /// <summary>
        /// Loads the XNA content.
        /// </summary>
        protected override void LoadContent()
        {
            this.LoadSprites();
            this.whiteTexture = TextureUtilities.CreateWhite( this.GraphicsDevice );

            // Setup DrawContext & Particle Renderer:
            this.lightMap.LoadContent();

            this.drawContext = new ZeldaDrawContext( this.GraphicsDevice );
            this.particleRenderer.LoadContent( this.Content );
            GameDifficulty.Current = DifficultyId.Easy;
            
            // Loads content used by the workspaces
            this.tileWorkspace.LoadContent();
            this.objectWorkspace.LoadContent();
            this.eventWorkspace.LoadContent();
            this.waypointWorkspace.LoadContent();            

            this.defaultWeatherMachineSettings.LoadContent( EditorApp.Current );
            this.weatherCreatorMap.AddDefault( EditorApp.Current );
            this.weatherCreatorMap.LoadContent();

            this.quadTreeVisualizer = new QuadTreeVisualizer2( this.drawContext.Batch );
                        
            base.LoadContent();
        }

        /// <summary>
        /// Loads the sprites used by the game
        /// </summary>
        private void LoadSprites()
        {
            this.spriteLoader.Insert( @"Content\Sprites\Mixed.sdb", this.textureLoader );
        }

        /// <summary>
        /// Gets called when xna is preparing its graphics device settings.
        /// </summary>
        /// <param name="e">
        /// The Microsoft.Xna.Framework.PreparingDeviceSettingsEventArgs that contain the event data.
        /// </param>
        protected override void OnPreparingDeviceSettings( Microsoft.Xna.Framework.PreparingDeviceSettingsEventArgs e )
        {
            var presentParams = e.GraphicsDeviceInformation.PresentationParameters;
            presentParams.RenderTargetUsage  = RenderTargetUsage.PreserveContents;

            LogHelper.LogInfo( e.GraphicsDeviceInformation, EditorApp.Current.Log );
        }

        /// <summary>
        /// Initializes the given Scene, called when a new Scene is set.
        /// </summary>
        /// <param name="sceneViewModel">
        /// The scene object.
        /// </param>
        private void InitializeScene( SceneViewModel sceneViewModel )
        {
            Debug.Assert( sceneViewModel != null );
            var scene = sceneViewModel.Model;

            // Initialize camera
            scene.Camera.ViewSize = this.TileMapArea.Size;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Draws the scene shown in the Editor.
        /// </summary>
        /// <param name="gameTime">
        /// The current GameTime.
        /// </param>
        protected override void Draw( Microsoft.Xna.Framework.GameTime gameTime )
        {
            this.GraphicsDevice.Clear( backgroundColor );
            this.drawContext.GameTime = gameTime;

            if( this.workspace != null )
            {
                this.workspace.Draw( this.drawContext );
            }
        }

        /// <summary>
        /// Draws the QuadTree2 of the current scene if <see cref="VisualizeQuadTree"/> is true.
        /// </summary>
        public void DrawQuadTree()
        {
            if( this.VisualizeQuadTree && this.scene != null )
            {
                var tree = this.scene.Model.QuadTree;

                this.drawContext.Begin(
                    BlendState.NonPremultiplied,
                    SamplerState.PointWrap,
                    SpriteSortMode.Deferred,
                    this.scene.Camera.Model.Transform
                );

                this.quadTreeVisualizer.Draw( tree, this.drawContext );

                this.drawContext.End();
            }
        }

        /// <summary>
        /// Updates all XNA related logic.
        /// </summary>
        /// <param name="gameTime">
        /// The current GameTime.
        /// </param>
        protected override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            this.updateContext.GameTime = gameTime;

            if( this.workspace != null )
            {
                this.workspace.Update( updateContext );
            }

            this.audioSystem.Update();
            base.Update( gameTime );
        }

        /// <summary>
        /// Utility method that draws a border around the TileMapArea.
        /// </summary>
        /// <remarks>
        /// The border reduces the flickering of tiles at the edge of the TileMap
        /// when the user scrolls the Scene.
        /// </remarks>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void DrawTileMapBorder( ZeldaDrawContext drawContext )
        {
            drawContext.Begin();
            {
                var batch = drawContext.Batch;

                // Border on the Right
                batch.Draw( whiteTexture,
                    new Microsoft.Xna.Framework.Rectangle(
                        this.TileMapArea.Width, 0,
                        this.WindowSize.X -this.TileMapArea.Width, this.TileMapArea.Height
                   ), backgroundColor
                );

                // Border on the Bottom
                batch.Draw( whiteTexture,
                    new Microsoft.Xna.Framework.Rectangle(
                        0, this.TileMapArea.Height,
                        this.TileMapArea.Width+16, 16
                   ), backgroundColor
                );

                // Left and Up are not needed
                // because the TileMapArea's orgin is at 0/0.
            }
            drawContext.End();
        }

        /// <summary>
        /// Tries to receive the service object of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceType">
        /// The type of the service object to receive.
        /// </param>
        /// <returns>
        /// The requested service object;
        /// or null if the service couldn't be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceType"/> is null.
        /// </exception>
        public object GetService( Type serviceType )
        {
            if( serviceType == typeof( ZeldaScene ) )
                return this.scene == null ? null : this.scene.Model;

            if( serviceType == typeof( Atom.Xna.Particles.ParticleRenderer ) )
                return this.particleRenderer;

            return this.Services.GetService( serviceType );
        }

        /// <summary>
        /// Attempts to receive the IObjectProvider for the specified object type.
        /// </summary>
        /// <param name="type">
        /// The type of object for which an IObjectProvider should be requested.
        /// </param>
        /// <returns>
        /// The associated IObjectProvider; -or- null if no IObjectProvider has been registered
        /// at this IObjectProviderContainer for the specified <see cref="Type"/>.
        /// </returns>
        public IObjectProvider<object> TryGetObjectProvider( Type type )
        {
            return EditorApp.Current.TryGetObjectProvider( type );
        }

        /// <summary>
        /// Gets the IWorkspace of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type of the workspace to get.
        /// </param>
        /// <returns>
        /// The requested workspace.
        /// </returns>        
        public IWorkspace GetWorkspace( WorkspaceType type )
        {
            switch( type )
            {
                case WorkspaceType.Tile:
                    return this.tileWorkspace;

                case WorkspaceType.Object:
                    return this.objectWorkspace;

                case WorkspaceType.Event:
                    return this.eventWorkspace;

                case WorkspaceType.Waypoint:
                    return this.waypointWorkspace;

                case WorkspaceType.Story:
                    return this.storyWorkspace;

                default:
                    return null;
            }
        }

        #region > Handle Input <

        /// <summary>
        /// Redirects the given MouseClick event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contain the event data.
        /// </param>
        public void HandleMouseClick( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleMouseClick( e );
            }
        }

        /// <summary>
        /// Redirects the given KeyDown event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contain the event data.
        /// </param>
        public void HandleKeyDown( System.Windows.Input.KeyEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleKeyDown( e );
            }

            if( e.Key == System.Windows.Input.Key.Q && e.KeyboardDevice.IsKeyDown( System.Windows.Input.Key.LeftShift ) )
            {
                this.VisualizeQuadTree = !this.VisualizeQuadTree;
            }
        }

        /// <summary>
        /// Redirects the given KeyDown event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contain the event data.
        /// </param>
        public void HandleKeyUp( System.Windows.Input.KeyEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleKeyUp( e );
            }
        }

        /// <summary>
        /// Redirects the given MouseMove event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contain the event data.
        /// </param>
        public void HandleMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleMouseMove( e );
            }
        }

        /// <summary>
        /// Redirects the given MouseDown event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contain the event data.
        /// </param>
        public void HandleMouseDown( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleMouseDown( e );
            }
        }

        /// <summary>
        /// Redirects the given MouseUp event that happened on the XnaControl into the XnaEditorApp.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contain the event data.
        /// </param>
        public void HandleMouseUp( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.workspace != null )
            {
                this.workspace.HandleMouseUp( e );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The current workspace.
        /// </summary>
        private IWorkspace workspace;

        /// <summary>
        /// The scene the user is currently editing.
        /// </summary>
        private SceneViewModel scene;

        /// <summary>
        /// Identifies the white texture which can be used 
        /// to easily draw any-colored rectangle.
        /// </summary>
        private Texture2D whiteTexture;

        /// <summary>
        /// Implements a mechanism for visualizing a QuadTree2.
        /// </summary>
        private QuadTreeVisualizer2 quadTreeVisualizer;

        /// <summary>
        /// The Atom.Fmod.AudioSystem object.
        /// </summary>
        private readonly Atom.Fmod.AudioSystem audioSystem;

        /// <summary>
        /// The Atom.Xna.Particles.ParticleRenderer object.
        /// </summary>
        private readonly Atom.Xna.Particles.ParticleRenderer particleRenderer;

        /// <summary>
        /// The background color of the Xna Control.
        /// </summary>
        private readonly Microsoft.Xna.Framework.Color backgroundColor = Microsoft.Xna.Framework.Color.Gray;

        /// <summary>
        /// The ZeldaDrawContext object.
        /// </summary>
        private ZeldaDrawContext drawContext;

        /// <summary>
        /// The ZeldaUpdateContext object.
        /// </summary>
        private readonly ZeldaUpdateContext updateContext = new ZeldaUpdateContext();

        /// <summary>
        /// The TileWorkspace object.
        /// </summary>
        private readonly TileWorkspace tileWorkspace;

        /// <summary>
        /// The ObjectWorkspace object.
        /// </summary>
        private readonly ObjectWorkspace objectWorkspace;

        /// <summary>
        /// The EventWorkspace object.
        /// </summary>
        private readonly EventWorkspace eventWorkspace;

        /// <summary>
        /// The IWorkspace that allows the user to add waypoints and paths to the scene.
        /// </summary>
        private readonly WaypointWorkspace waypointWorkspace;

        /// <summary>
        /// The IWorkspace that allows the user to modify the storyboard and timelines of the scene.
        /// </summary>
        private readonly StoryWorkspace storyWorkspace;

        /// <summary>
        /// The lightmap used during the light pass.
        /// </summary>
        private readonly LightMap lightMap;

        /// <summary>
        /// The factory responsible for creating all full-screen RenderTargets used by the game.
        /// </summary>
        private readonly Atom.Xna.RenderTarget2DFactory renderTargetFactory;

        /// <summary>
        /// Provides a mechanism to draw ZeldaScenes.
        /// </summary>
        private readonly ZeldaSceneDrawer sceneDrawer;

        /// <summary>
        /// Provides a mechansim that allows loading of ISprite assets.
        /// </summary>
        private readonly SpriteLoader spriteLoader = new SpriteLoader();

        /// <summary>
        /// Provides a mechansim that allows loading of Texture2D assets.
        /// </summary>
        private readonly ITexture2DLoader textureLoader;

        /// <summary>
        /// Provides a mechansim that allows loading of IFont assets.
        /// </summary>
        private readonly IFontLoader fontLoader;

        /// <summary>
        /// Provides a mechansim that allows loading of ISpriteSheet assets.
        /// </summary>
        private readonly ISpriteSheetLoader spriteSheetLoader;

        /// <summary>
        /// Represents a reference to the current EditorApp.
        /// </summary>
        private readonly EditorApp editor;

        private readonly Weather.Settings.DefaultWeatherMachineSettings defaultWeatherMachineSettings = new Zelda.Weather.Settings.DefaultWeatherMachineSettings();
        private readonly Zelda.Weather.Creators.WeatherCreatorMap weatherCreatorMap = new Zelda.Weather.Creators.WeatherCreatorMap();

        #endregion
    }
}