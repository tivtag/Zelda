//// <copyright file="HighDynamicRangeDrawingPipeline.cs" company="federrot Software">
////     Copyright (c) federrot Software. All rights reserved.
//// </copyright>
//// <summary>
////     Defines the Zelda.Graphics.HighDynamicRangeDrawingPipeline class.
//// </summary>
//// <author>
////     Paul Ennemoser
//// </author>

//namespace Zelda.Graphics
//{
//    using Atom;
//    using Atom.Xna;
//    using Atom.Xna.Effects;
//    using Atom.Xna.Effects.PostProcess;
//    using Microsoft.Xna.Framework.Graphics;
//    using Zelda.UI;

//    /// <summary>
//    /// Implements the default <see cref="IDrawingPipeline"/> that implements
//    /// high dynamic range rendering.
//    /// </summary>
//    public sealed class HighDynamicRangeDrawingPipeline : IDrawingPipeline
//    {
//        /// <summary>
//        /// Gets a value indicating whether this HighDynamicRangeDrawingPipeline is supported
//        /// on the current hardware.
//        /// </summary>
//        public bool IsSupported
//        {
//            get
//            {
//                return this.deviceService.GraphicsDevice.SupportsPixelShader( 2 );
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public Atom.Xna.Effects.PostProcess.NewHDR.CustomHighDynamicRange Effect
//        {
//            get
//            {
//                return this.hdr;
//            }
//        }

//        /// <summary>
//        /// Initializes a new instance of the HighDynamicRangeDrawingPipeline class.
//        /// </summary>
//        /// <param name="serviceProvider">
//        /// Provides fast access to game-related services.
//        /// </param>
//        public HighDynamicRangeDrawingPipeline( IZeldaServiceProvider serviceProvider )
//        {
//            this.rescaler = serviceProvider.GetService<IViewToWindowRescaler>();
//            this.sceneDrawer = serviceProvider.GetService<ZeldaSceneDrawer>();
//            this.deviceService = serviceProvider.GetService<IGraphicsDeviceService>();
//            this.serviceProvider = serviceProvider;

//            var effectLoader = serviceProvider.GetService<IEffectLoader>();
//            this.hdr = new Atom.Xna.Effects.PostProcess.NewHDR.CustomHighDynamicRange( effectLoader, deviceService )
//            {
//                bloomMultiplier = 1.0f,
//                bloomThreshold = 1.0f,
//                blurSigma = 1.0f,
//                toneMapKey = 3.5f,
//                maxLuminance = 55.0f
//            };
//        }

//        /// <summary>
//        /// Initializes this IDrawingPipeline for drawing the next frame.
//        /// </summary>
//        /// <param name="scene">
//        /// The scene to draw.
//        /// </param>
//        /// <param name="userInterface">
//        /// The user interface to draw ontop of the scene.
//        /// Allowed to be null.
//        /// </param>
//        /// <param name="drawContext">
//        /// The current IDrawContext.
//        /// </param>
//        public void InitializeFrame( ZeldaScene scene, ZeldaUserInterface userInterface, ZeldaDrawContext drawContext )
//        {
//            this.scene = scene;
//            this.userInterface = userInterface;
//            this.drawContext = drawContext;
//        }

//        /// <summary>
//        /// Begins drawing the scene.
//        /// </summary>
//        /// <remarks>
//        /// Insert drawing of custom scene object between BeginScene and EndScene.
//        /// </remarks>
//        public void BeginScene()
//        {
//            this.graphicsDevice.SetRenderTarget( 0, this.hdrTarget );
//            this.graphicsDevice.Clear( ClearOptions.Target, Color.Black, 0.0f, 0 );
            
//            if( scene != null )
//            {
//                this.sceneDrawer.Draw( scene, drawContext );
//            }
//        }

//        /// <summary>
//        /// Ends drawing the scene.
//        /// </summary>
//        /// <remarks>
//        /// Should be followed by BeginUserInterface.
//        /// </remarks>
//        public void EndScene()
//        {
//            this.graphicsDevice.SetRenderTarget( 0, null );
//            this.hdr.PostProcess( this.hdrTarget.GetTexture(), rescaler.Target, drawContext );

//            this.rescaler.Begin();
//        }

//        /// <summary>
//        /// Begins drawing the user interface.
//        /// </summary>
//        /// <remarks>
//        /// Insert drawing of custom UI bjects between BeginUserInterface and EndDrawUserInterface.
//        /// </remarks>
//        public void BeginUserInterface()
//        {
//            if( scene != null )
//            {
//                scene.FlyingTextManager.Draw( drawContext );
//            }

//            if( userInterface != null )
//            {
//                userInterface.Draw( drawContext );
//            }
//        }

//        /// <summary>
//        /// Ends drawing the user interface.
//        /// </summary>
//        public void EndUserInterface()
//        {
//            this.rescaler.End( drawContext );
//        }

//        /// <summary>
//        /// Loads the data required by this IDrawingPipeline.
//        /// </summary>
//        public void Load()
//        {
//            this.graphicsDevice = this.deviceService.GraphicsDevice;

//            this.hdrTarget = new RenderTarget2D(
//                this.graphicsDevice,
//                serviceProvider.ViewSize.X,
//                serviceProvider.ViewSize.Y,
//                1,
//                SurfaceFormat.HalfVector4,
//                RenderTargetUsage.PreserveContents
//            );

//            this.hdr.LoadContent();
//        }

//        /// <summary>
//        /// Unloads the data required by this IDrawingPipeline.
//        /// </summary>
//        public void Unload()
//        {
//            this.hdrTarget.Dispose();
//            this.hdrTarget = null;

//            // no op.
//            this.hdr.Dispose();
//        }

//        /// <summary>
//        /// The scene that is currently drawn.
//        /// </summary>
//        private ZeldaScene scene;

//        /// <summary>
//        /// The UI that is currently drawn.
//        /// </summary>
//        private ZeldaUserInterface userInterface;

//        /// <summary>
//        /// The current drawing context.
//        /// </summary>
//        private ZeldaDrawContext drawContext;

//        /// <summary>
//        /// Is responsible for scaling the game that has been drawn at a native resolution
//        /// to fit the full game window.
//        /// </summary>
//        private readonly IViewToWindowRescaler rescaler;

//        /// <summary>
//        /// Is responsible for drawing ZeldaScenes.
//        /// </summary>
//        private readonly ZeldaSceneDrawer sceneDrawer;

//        /// <summary>
//        /// 
//        /// </summary>
//        private readonly IZeldaServiceProvider serviceProvider;

//        /// <summary>
//        /// 
//        /// </summary>
//        private IGraphicsDeviceService deviceService;

//        /// <summary>
//        /// 
//        /// </summary>
//        private GraphicsDevice graphicsDevice;

//        /// <summary>
//        /// 
//        /// </summary>
//        private readonly Atom.Xna.Effects.PostProcess.NewHDR.CustomHighDynamicRange hdr;

//        /// <summary>
//        /// 
//        /// </summary>
//        private RenderTarget2D hdrTarget;
//    }
//}
