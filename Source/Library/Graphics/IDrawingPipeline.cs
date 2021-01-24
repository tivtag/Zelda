// <copyright file="IDrawingPipeline.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.IDrawingPipeline interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics
{
    using Zelda.UI;

    /// <summary>
    /// Provides a mechanism that draws a Scene and UserInterface
    /// onto the screen.
    /// </summary>
    public interface IDrawingPipeline
    {
        /// <summary>
        /// Gets a value indicating whether this IDrawingPipeline on
        /// the current hardware.
        /// </summary>
        bool IsSupported
        {
            get;
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
        void InitializeFrame( ZeldaScene scene, ZeldaUserInterface userInterface, ZeldaDrawContext drawContext );

        /// <summary>
        /// Begins drawing the scene.
        /// </summary>
        /// <remarks>
        /// Insert drawing of custom scene object between BeginScene and EndScene.
        /// </remarks>
        void BeginScene();

        /// <summary>
        /// Ends drawing the scene.
        /// </summary>
        /// <remarks>
        /// Should be followed by BeginUserInterface.
        /// </remarks>
        void EndScene();
        
        /// <summary>
        /// Begins drawing the user interface.
        /// </summary>
        /// <remarks>
        /// Insert drawing of custom UI bjects between BeginUserInterface and EndDrawUserInterface.
        /// </remarks>
        void BeginUserInterface();

        /// <summary>
        /// Ends drawing the user interface.
        /// </summary>
        void EndUserInterface();

        /// <summary>
        /// Loads the data required by this IDrawingPipeline.
        /// </summary>
        void Load();

        /// <summary>
        /// Unloads the data required by this IDrawingPipeline.
        /// </summary>
        void Unload();
    }
}
