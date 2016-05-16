// <copyright file="ZeldaGame.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaGame class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
    using Atom;
    using Atom.Diagnostics;
    using Atom.Math;
    using Atom.Patterns.Provider;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Graphics;
    using Zelda.Weather;
    using Zelda.Weather.Creators;
    using Zelda.Weather.Settings;

    /// <summary>
    /// Defines the <see cref="Microsoft.Xna.Framework.Game"/> object
    /// that runs the main loop of the game.
    /// </summary>
    /// <remarks>
    /// The game class is responsible for creating all game managers, services, etc.
    /// </remarks>
    internal sealed class ZeldaGame : Microsoft.Xna.Framework.Game, IZeldaServiceProvider
    {
        #region [ Properties ]

        #region > Managers <

        /// <summary>
        /// Gets the object which manages the current IGameState.
        /// </summary>
        public Atom.GameStateManager States
        {
            get
            {
                return this.states;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Behaviours.BehaviourManager"/> object.
        /// </summary>
        public Zelda.Entities.Behaviours.BehaviourManager BehaviourManager
        {
            get
            {
                return this.behaviourManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Drawing.DrawStrategyManager"/> object.
        /// </summary>
        public Zelda.Entities.Drawing.DrawStrategyManager DrawStrategyManager
        {
            get
            {
                return this.drawStrategyManager;
            }
        }

        /// <summary>
        /// Gets the Zelda.Entities.ObjectReaderWriterManager object.
        /// </summary>
        public Zelda.Entities.EntityReaderWriterManager EntityReaderWriterManager
        {
            get
            {
                return this.objectReaderWriterManager;
            }
        }

        /// <summary>
        /// Gets the Zelda.Entities.EntityTemplateManager object.
        /// </summary>
        public Zelda.Entities.EntityTemplateManager EntityTemplateManager
        {
            get
            {
                return this.entityTemplateManager;
            }
        }

        /// <summary>
        /// Gets the Zelda.Items.ItemManager object.
        /// </summary>
        public Zelda.Items.ItemManager ItemManager
        {
            get
            {
                return this.itemManager;
            }
        }

        #endregion

        #region > Graphics <

        public int Fps
        {
            get
            {
                return fps.Value;
            }
        }

           /// <summary>
        /// Gets the <see cref="GameGraphics"/> object that encapsulate
        /// all graphics related logic.
        /// </summary>
        public GameGraphics Graphics
        {
            get
            {
                return this.graphics;
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
                return this.graphics.TextureLoader;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading ISpriteSheet assets.
        /// </summary>
        public ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return this.graphics.SpriteSheetLoader;
            }
        }
        
        /// <summary>
        /// Gets the size of the area the game is drawing to.
        /// </summary>
        /// <remarks>
        /// The view size differs from the client bounds of the game window
        /// as in that the game is drawn using the ViewSize,
        /// and then rescaled to fill up the game window.
        /// </remarks>
        public Point2 ViewSize
        {
            get
            {
                return this.graphics.Resolution.ViewSize;
            }
        }

        #endregion

        #region > Other <

        public Zelda.Scripting.ZeldaScriptEnvironment Scripts
        {
            get
            {
                return this.scripts;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Audio.ZeldaAudioSystem "/> object.
        /// </summary>
        public Zelda.Audio.ZeldaAudioSystem AudioSystem
        {
            get
            {
                return this.audioSystem;
            }
        }

        /// <summary>
        /// Gets the main <see cref="ILog"/> object used by the <see cref="ZeldaGame"/>.
        /// </summary>
        public ILog Log
        {
            get
            {
                return this.log;
            }
        }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public RandMT Rand
        {
            get
            {
                return this.rand;
            }
        }

        /// <summary>
        /// Gets the globally active KeySettings instance.
        /// </summary>
        public KeySettings GlobalKeySettings
        {
            get
            {
                return this.globalKeySettings;
            }
        }

        /// <summary>
        /// Gets the error reporter to which all errors are passed.
        /// </summary>
        public Atom.ErrorReporting.IErrorReporter ErrorReporter
        {
            get
            {
                return this.errorReporter;
            }
        }

        /// <summary>
        /// Gets the Xna application object.
        /// </summary>
        Microsoft.Xna.Framework.Game IZeldaServiceProvider.Game
        {
            get
            {
                return this;
            }
        }

        public bool IsFirstLoad
        {
            get
            {
                return this.ingameState == null;
            }
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaGame"/> class.
        /// </summary>
        public ZeldaGame()
        {
            this.IsMouseVisible = true;

            this.errorReporter = this.CreateErrorReporter();
            this.errorHook = new Atom.ErrorReporting.Hooks.UnhandledAppDomainExceptionHook(
                AppDomain.CurrentDomain,
                new Atom.ErrorReporting.Errors.ExceptionErrorFactory(),
                errorReporter
            );
            this.errorHook.Hook();
            Settings.Instance.Load();

            this.IsFixedTimeStep = Settings.Instance.FixedStep;

            this.graphics = new GameGraphics( this );
            this.spriteLoader = this.graphics.SpriteLoader;

            this.entityTemplateManager = new Zelda.Entities.EntityTemplateManager( this );
            this.flyingTextManager = new FlyingTextManager( this );
            this.itemManager = new Zelda.Items.ItemManager( this );

            this.setDatabase = new Zelda.Items.Sets.SetDatabase( this );
            this.scripts = new Scripting.ZeldaScriptEnvironment( this );

            this.RegisterServices();
            this.RegisterIoC();
        }

        /// <summary>
        /// Registers additional services at the GameServicesContainer.
        /// </summary>
        private void RegisterServices()
        {
            var services = this.Services;

            services.AddService<ILog>( this.log );
            services.AddService<GameStateManager>( this.states );
            services.AddService<Zelda.FlyingTextManager>( this.flyingTextManager );
            services.AddService<Zelda.BubbleTextManager>( this.bubbleTextManager );
            services.AddService<Zelda.Items.Sets.ISetDatabase>( this.setDatabase );
            services.AddService<Zelda.Crafting.RecipeDatabase>( this.recipeDatabase );
            services.AddService<IWeatherCreatorMap>( this.weatherCreatorMap );
            services.AddService<IWeatherMachineSettings>( this.defaultWeatherMachineSettings );
            services.AddService<Atom.Scripting.IScriptingEnvironment>( this.scripts ); 
            services.AddService<Zelda.Scripting.ZeldaScriptEnvironment>( this.scripts ); 
        }

        /// <summary>
        /// Registers IObjectProviders.
        /// </summary>
        private void RegisterIoC()
        {
            IoC.Provider = this;
            this.providerContainer.Register<Zelda.Items.SharedChest>( () => this.ingameState.Player.SharedChest );
        }

        /// <summary>
        /// Creates the IErrorReporter to which all unhandled IErrors are passed.
        /// </summary>
        /// <returns>
        /// The newly created Atom.ErrorReporting.IErrorReporter.
        /// </returns>
        private Atom.ErrorReporting.IErrorReporter CreateErrorReporter()
        {
            return new Zelda.Errors.ZeldaErrorReporter(
                this.log,
                null,
                new Atom.ErrorReporting.Dialogs.WinFormsErrorReportDialogFactory(
                    new Zelda.Errors.ZeldaMailErrorReporter( "Zelda" )
                )
            );
        }

        /// <summary>
        /// Initializes the ZeldaGame.
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.Initialize();
            this.InitializeAudio();
            this.InitializeGameData();

            base.Initialize();

            this.log.WriteLine();
            this.log.WriteLine( "Init complete!" );
            this.log.WriteLine();
        }

        private void InitializeGameData()
        {
            this.log.Write( "Initializing game data..." );
            objectReaderWriterManager.LoadDefaults( this );
            behaviourManager.LoadDefaults( this );
            drawStrategyManager.LoadDefaults( this );
            recipeDatabase.Load( this );
            Zelda.Items.Affixes.AffixDatabase.Instance.Initialize( this );
            this.log.WriteLine( " done!" );
        }

        /// <summary>
        /// Initializes the Fmod audio sub-system.
        /// </summary>
        private void InitializeAudio()
        {
            if( this.audioSystem.Initialize( this ) )
            {
                // Apply additional settings.
                audioSystem.MusicGroup.Volume = Settings.Instance.MusicVolume;
                audioSystem.SampleGroup.Volume = Settings.Instance.EffectVolume;
            }
        }

        /// <summary>
        /// Loads all commonly used content.
        /// </summary>
        protected override void LoadContent()
        {
            if( this.IsFirstLoad )
            {
                this.defaultWeatherMachineSettings.LoadContent( this );
                this.weatherCreatorMap.AddDefault( this );
            }

            this.graphics.LoadContent( this.IsFirstLoad );
            this.flyingTextManager.LoadContent();
            this.bubbleTextManager.LoadContent();
            this.weatherCreatorMap.LoadContent();

            // Initialize once:
            if( this.IsFirstLoad )
            {
                this.ingameState = new GameStates.IngameState( this );
                this.Services.AddService<IIngameState>( this.ingameState );
                this.Services.AddService<Zelda.Saving.IWorldStatusProvider>( new Zelda.Saving.WorldStatusProvider( this.ingameState ) );
                this.Services.AddService<GraphicsDevice>( this.GraphicsDevice );

                this.states.Add( this.ingameState );
                this.states.Add( new GameStates.TitleScreenState( this ) );
                this.states.Add( new GameStates.CharacterSelectionState( this ) );
                this.states.Add( new GameStates.CharacterCreationState( this ) );
                this.states.Add( new GameStates.IngameMenuState( this ) );
                this.states.Add( new GameStates.SettingsState( this ) );
                this.states.Push<GameStates.TitleScreenState>();
            }
            else
            {
                this.defaultWeatherMachineSettings.Reload( this );
                this.ingameState.Reload();
            }

            this.isInitialized = true;
        }

        /// <summary>
        /// Unloads all commonly used content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.isInitialized = false;
            this.graphics.UnloadContent();
            this.flyingTextManager.UnloadContent();

            base.UnloadContent();
        }

        /// <summary>
        /// Called when the game is exiting.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The EventArgs that contains the event data.</param>
        protected override void OnExiting( object sender, EventArgs args )
        {
            Settings.Instance.Save();
            base.OnExiting( sender, args );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates the Game.
        /// </summary>
        /// <param name="gameTime">
        /// The current GameTime.
        /// </param>
        protected override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            updateContext.GameTime = gameTime;
            
            if( states.Current != null )
            {
                states.Current.Update( updateContext );
            }
            
            audioSystem.Update();
            fps.Update( updateContext );
        }

        /// <summary>
        /// Draws the Game.
        /// </summary>
        /// <param name="gameTime">
        /// The current GameTime.
        /// </param>
        protected override void Draw( Microsoft.Xna.Framework.GameTime gameTime )
        {
            if( this.isInitialized )
            {
                var drawContext = this.graphics.DrawContext;
                var gameState = this.states.Current;

                if( gameState != null )
                {
                    drawContext.GameTime = gameTime;
                    gameState.Draw( drawContext );
                }
            }
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
            if( serviceType.IsAssignableFrom( this.GetType() ) )
                return this;

            var service = this.providerContainer.TryResolve( serviceType );
            if( service != null )
                return service;

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
        public Atom.IObjectProvider<object> TryGetObjectProvider( Type type )
        {
            return this.providerContainer.TryGetObjectProvider( type );
        }

        /// <summary>
        /// Reports the given exception.
        /// </summary>
        /// <param name="exc">
        /// The error that has occurred.
        /// </param>
        /// <param name="isFatal">
        /// States whether the error was fatal.
        /// </param>
        public void ReportError( Exception exc, bool isFatal )
        {
            this.ErrorReporter.Report( new Atom.ErrorReporting.Errors.ExceptionError( exc, isFatal ) );
            this.Log.WriteLine( LogSeverities.Error, exc.ToString() );
        }
        
        #endregion

        #region [ Fields ]

        private readonly FramesPerSecond fps = new FramesPerSecond();

        private bool isInitialized;
        private readonly DefaultWeatherMachineSettings defaultWeatherMachineSettings = new DefaultWeatherMachineSettings();
        private readonly WeatherCreatorMap weatherCreatorMap = new WeatherCreatorMap();

        /// <summary>
        /// The IUpdateContext used by the Game.
        /// </summary>
        private readonly ZeldaUpdateContext updateContext = new ZeldaUpdateContext();

        /// <summary>
        /// The Fmod AudioSystem object.
        /// </summary>
        private readonly Zelda.Audio.ZeldaAudioSystem audioSystem = new Zelda.Audio.ZeldaAudioSystem();

        /// <summary>
        /// Encapsulates all graphics related game behaviour.
        /// </summary>
        private readonly GameGraphics graphics;

        /// <summary>
        /// Represents a cached reference to the ISpriteLoader the GameGraphics object exposes.
        /// </summary>
        private readonly ISpriteLoader spriteLoader;

        /// <summary>
        /// Identifies the IngameState GameState.
        /// </summary>
        private GameStates.IngameState ingameState;

        private readonly Zelda.Scripting.ZeldaScriptEnvironment scripts;

        /// <summary>
        /// Provides a mechanism for receiving and registering IObjectProviders.
        /// </summary>
        private readonly IObjectProviderContainerRegistrar providerContainer = new ObjectProviderContainer();

        #region > Managers <

        /// <summary>
        /// Manages the current state of the Game.
        /// </summary>
        private readonly Atom.GameStateManager states = new Atom.GameStateManager( 4 );

        /// <summary>
        /// Stores the Zelda.Entities.Behaviours.BehaviourManager object.
        /// </summary>
        private readonly Zelda.Entities.Behaviours.BehaviourManager behaviourManager = new Zelda.Entities.Behaviours.BehaviourManager();

        /// <summary>
        /// Stores the <see cref="Zelda.Entities.Drawing.DrawStrategyManager"/> object.
        /// </summary>
        private readonly Zelda.Entities.Drawing.DrawStrategyManager drawStrategyManager = new Zelda.Entities.Drawing.DrawStrategyManager();

        /// <summary>
        /// Stores the Zelda.Entities.ObjectReaderWriterManager object.
        /// </summary>
        private readonly Zelda.Entities.EntityReaderWriterManager objectReaderWriterManager = new Zelda.Entities.EntityReaderWriterManager();

        /// <summary>
        /// Stores the Zelda.Entities.EntityTemplateManager object.
        /// </summary>
        private readonly Zelda.Entities.EntityTemplateManager entityTemplateManager;

        /// <summary>
        /// Stores the Zelda.FlyingTextManager object.
        /// </summary>
        private readonly FlyingTextManager flyingTextManager;

        private readonly BubbleTextManager bubbleTextManager = new BubbleTextManager();

        /// <summary>
        /// Stores the Zelda.Items.ItemManager object.
        /// </summary>
        private readonly Zelda.Items.ItemManager itemManager;

        /// <summary>
        /// Provides access to all <see cref="Zelda.Crafting.Recipe"/>s of the game.
        /// </summary>
        private readonly Zelda.Crafting.RecipeDatabase recipeDatabase = new Zelda.Crafting.RecipeDatabase();

        /// <summary>
        /// Provides a mechanism that allows receiving of ISets.
        /// </summary>
        private readonly Zelda.Items.Sets.ISetDatabase setDatabase;

        #endregion

        #region > Misc <

        /// <summary>
        /// The hook onto which the error handling logic is attached.
        /// </summary>
        private readonly Atom.ErrorReporting.IErrorHook errorHook;

        /// <summary>
        /// The reporter to which all errors are passed.
        /// </summary>        
        private readonly Atom.ErrorReporting.IErrorReporter errorReporter;

        /// <summary>
        /// Defines the main-log of the game.
        /// </summary>
        private readonly ILog log = LogHelper.CreateAndInitialize( "Zelda" );

        /// <summary>
        /// Defines a random number generator.
        /// </summary>
        private readonly Atom.Math.RandMT rand = new RandMT();

        /// <summary>
        /// The globally active KeySettings instance.
        /// </summary>
        private readonly KeySettings globalKeySettings = new KeySettings();

        #endregion

        #endregion
    }
}