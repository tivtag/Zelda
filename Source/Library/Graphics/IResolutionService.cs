// <copyright file="IViewToWindowRescaler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.IViewToWindowRescaler interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Graphics
{
    using Atom;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Provices access to the current Render and View resolutions.
    /// </summary>
    public interface IResolutionService
    {
        /// <summary>
        /// Gets a value indicating whether the game is currently
        /// running in fullscreen-mode.
        /// </summary>
        bool IsFullscreen
        {
            get;
        }

        /// <summary>
        /// Gets the size (in pixels) in which the game
        /// is output at.
        /// </summary>
        Point2 OutputSize
        {
            get;
        }

        /// <summary>
        /// Gets the size (in pixels) in which the game
        /// is originally rendered at.
        /// </summary>
        /// <value>
        /// This value is constant and
        /// won't change during the game.
        /// </value>
        Point2 ViewSize
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the current AspectRatio is a wide-screen ratio.
        /// </summary>
        bool IsWideAspectRatio 
        {
            get;
        }

        /// <summary>
        /// Gets the aspect ratio that the game uses.
        /// </summary>
        /// <value>
        /// This value is constant and
        /// won't change during the game.
        /// </value>
        AspectRatio AspectRatio
        {
            get;
        }
        
        /// <summary>
        /// Gets the size that the backbuffer should have.
        /// </summary>
        /// <param name="originalSize">
        /// The original size of the backbuffer.
        /// </param>
        /// <returns>
        /// The adjusted backbuffer size.
        /// </returns>
        Point2 GetAdjustedBackBufferSize( Point2 originalSize );

        /// <summary>
        /// Ensures that the current aspect ratio is supported.
        /// </summary>
        /// <param name="adapter">
        /// The graphics adapter that will be used.
        /// </param>
        void EnsureAspectRatioSupport( GraphicsAdapter adapter );
    }
}
