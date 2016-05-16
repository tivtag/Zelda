// <copyright file="BloomDrawingPipeline.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.BloomDrawingPipeline class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics
{
    using Atom;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Effects.PostProcess;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements the default <see cref="IDrawingPipeline"/> that uses a bloom post-process effect.
    /// </summary>
    public sealed class BloomDrawingPipeline : IDrawingPipeline
    {
        /// <summary>
        /// Gets or sets the BloomSettings used by the BloomDrawingPipeline.
        /// </summary>
        public BloomSettings Settings
        {
            get
            {
                return this.bloom.Settings;
            }

            set
            {
                this.bloom.Settings = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this BloomDrawingPipeline is supported
        /// on the current hardware.
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the bloom effect has been loaden.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this.bloom != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BloomDrawingPipeline class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public BloomDrawingPipeline( IZeldaServiceProvider serviceProvider )
        {
            this.rescaler = serviceProvider.GetService<IViewToWindowRescaler>();
            this.sceneDrawer = serviceProvider.GetService<ZeldaSceneDrawer>();
            this.deviceService = serviceProvider.GetService<IGraphicsDeviceService>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes this IDrawingPipeline for drawing the next frame.
        /// </summary>
        /// <param name="scene">
        /// The scene to draw.
        /// </param>
        /// <param name="userInterface">
        /// The user interface to draw ontop of the scene.
        /// Allowed to be null.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void InitializeFrame( ZeldaScene scene, ZeldaUserInterface userInterface, ZeldaDrawContext drawContext )
        {
            this.scene = scene;
            this.userInterface = userInterface;
            this.drawContext = drawContext;
        }

        /// <summary>
        /// Begins drawing the scene.
        /// </summary>
        /// <remarks>
        /// Insert drawing of custom scene object between BeginScene and EndScene.
        /// </remarks>
        public void BeginScene()
        {
            this.rescaler.Begin();
            this.drawContext.Device.Clear( ClearOptions.Target, Xna.Color.Black, 0.0f, 0 );

            if( scene != null )
            {
                this.sceneDrawer.Draw( scene, drawContext );
            }
        }

        /// <summary>
        /// Ends drawing the scene.
        /// </summary>
        /// <remarks>
        /// Should be followed by BeginUserInterface.
        /// </remarks>
        public void EndScene()
        {
            var target = this.rescaler.Target;

            if( target == null )
            {
                // Todo

                //
                //this.graphicsDevice.ResolveBackBuffer( this.resolveTexture );
                //this.bloom.PostProcess( this.resolveTexture, null, drawContext );
            }
            else
            {
                graphicsDevice.SetRenderTarget( this.resolveTarget );
                graphicsDevice.Clear( Xna.Color.Red );

                batch.Begin(  SpriteSortMode.Deferred, BlendState.Opaque );
                batch.Draw( target, new Xna.Rectangle( 0, 0, target.Width, target.Height ), Xna.Color.White );
                batch.End();
                graphicsDevice.SetRenderTarget( null );

                this.bloom.PostProcess( this.resolveTarget, target, drawContext );
            }    
        }

        /// <summary>
        /// Creates the Resolve Texture if it hasn't been created yet.
        /// </summary>
        private void EnsureResolveTexture()
        {
            if( this.resolveTarget == null )
            {
                this.resolveTarget = new RenderTarget2D(
                    this.deviceService.GraphicsDevice,
                    this.serviceProvider.ViewSize.X,
                    this.serviceProvider.ViewSize.Y,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );
            }
        }

        /// <summary>
        /// Begins drawing the user interface.
        /// </summary>
        /// <remarks>
        /// Insert drawing of custom UI bjects between BeginUserInterface and EndDrawUserInterface.
        /// </remarks>
        public void BeginUserInterface()
        {
            if( scene != null )
            {
                drawContext.Begin(
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp,
                    SpriteSortMode.Deferred,
                    drawContext.Camera.Transform
                );

                this.scene.FlyingTextManager.Draw( this.drawContext );
                this.scene.BubbleTextManager.Draw( this.drawContext );

                drawContext.End();
            }

            if( userInterface != null )
            {
                userInterface.Draw( drawContext );
            }
        }

        /// <summary>
        /// Ends drawing the user interface.
        /// </summary>
        public void EndUserInterface()
        {
            this.rescaler.End( drawContext );
        }

        /// <summary>
        /// Loads the data required by this IDrawingPipeline.
        /// </summary>
        public void Load()
        {
            this.graphicsDevice = this.deviceService.GraphicsDevice;

            batch = new SpriteBatch( graphicsDevice );
          
            this.bloom = new Bloom( 
                serviceProvider.GetService<IEffectLoader>(), 
                serviceProvider.GetService<IRenderTarget2DFactory>(), 
                this.deviceService 
            );
            this.bloom.LoadContent();
            this.EnsureResolveTexture();
        }

        /// <summary>
        /// Unloads the data required by this IDrawingPipeline.
        /// </summary>
        public void Unload()
        {
            if( this.bloom != null )
            {
                this.bloom.Dispose();
                this.bloom = null;
            }

            if( this.resolveTarget != null )
            {
                this.resolveTarget.Dispose();
                this.resolveTarget = null;
            }
        }

        /// <summary>
        /// The scene that is currently drawn.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The UI that is currently drawn.
        /// </summary>
        private ZeldaUserInterface userInterface;

        /// <summary>
        /// The current drawing context.
        /// </summary>
        private ZeldaDrawContext drawContext;

        /// <summary>
        /// Is responsible for scaling the game that has been drawn at a native resolution
        /// to fit the full game window.
        /// </summary>
        private readonly IViewToWindowRescaler rescaler;

        /// <summary>
        /// Is responsible for drawing ZeldaScenes.
        /// </summary>
        private readonly ZeldaSceneDrawer sceneDrawer;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        /// <summary>
        /// Provides access to the xna GraphicsDevice that is required for drawing.
        /// </summary>
        private IGraphicsDeviceService deviceService;

        /// <summary>
        /// The xna GraphicsDevice that is required for drawing.
        /// </summary>
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// The bloom effect that is applied to the drawn scene.
        /// </summary>
        private Bloom bloom;

        private SpriteBatch batch;
        private RenderTarget2D resolveTarget;
    }
}
