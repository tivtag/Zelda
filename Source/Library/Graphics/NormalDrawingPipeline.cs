// <copyright file="DefaultDrawingPipeline.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.DefaultDrawingPipeline class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Graphics
{
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.UI;
    
    /// <summary>
    /// Implements the default <see cref="IDrawingPipeline"/> that doens't add
    /// any special StatusEffects.
    /// </summary>
    public sealed class NormalDrawingPipeline : IDrawingPipeline
    {
        /// <summary>
        /// Gets a value indicating whether this DefaultDrawingPipeline on
        /// the current hardware.
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DefaultDrawingPipeline class.
        /// </summary>
        /// <param name="rescaler">
        /// Is responsible for scaling the game that has been drawn at a native resolution
        /// to fit the full game window.
        /// </param>
        /// <param name="sceneDrawer">
        /// The ZeldaSceneDrawer that responsible for drawing ZeldaScenes.
        /// </param>
        public NormalDrawingPipeline( IViewToWindowRescaler rescaler, ZeldaSceneDrawer sceneDrawer )
        {
            this.rescaler = rescaler;
            this.sceneDrawer = sceneDrawer;
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
            this.drawContext.Device.Clear( ClearOptions.Target, Microsoft.Xna.Framework.Color.Black, 0.0f, 0 );

            if( this.scene != null )
            {
                this.sceneDrawer.Draw( this.scene, this.drawContext );
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
            // no op.
        }

        /// <summary>
        /// Begins drawing the user interface.
        /// </summary>
        /// <remarks>
        /// Insert drawing of custom UI bjects between BeginUserInterface and EndDrawUserInterface.
        /// </remarks>
        public void BeginUserInterface()
        {
            if( this.scene != null )
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

            if( this.userInterface != null )
            {
                this.userInterface.Draw( this.drawContext );
            }
        }

        /// <summary>
        /// Ends drawing the user interface.
        /// </summary>
        public void EndUserInterface()
        {
            this.rescaler.End( this.drawContext );
        }
        
        /// <summary>
        /// Loads the data required by this IDrawingPipeline.
        /// </summary>
        public void Load()
        {
            // no op.
        }

        /// <summary>
        /// Unloads the data required by this IDrawingPipeline.
        /// </summary>
        public void Unload()
        {
            // no op.
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
    }
}
