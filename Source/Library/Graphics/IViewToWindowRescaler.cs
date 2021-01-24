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
    using System;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Provides a mechanism that allows the game to draw everything at the native
    /// resolution; but then when presenting the result to the user it up-scaled to
    /// the fit the game window.
    /// </summary>
    public interface IViewToWindowRescaler : IContentLoadable
    {
        /// <summary>
        /// Raised when the scaling factor of this IViewToWindowRescaler has changed.
        /// </summary>
        event EventHandler ScaleChanged;

        /// <summary>
        /// Gets the target to which this IViewToWindowRescaler draws to.
        /// </summary>
        RenderTarget2D Target
        {
            get;
        }

        /// <summary>
        /// Tells this IViewToWindowRescaler that the drawing operations that are 
        /// supposed to be scaled up to fill up full window.
        /// </summary>
        void Begin();
        
        /// <summary>
        /// Tells this IViewToWindowRescaler that all drawing operations have
        /// been executed and that the final (possibly up-scaled) image should be drawn on the screen.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void End( ZeldaDrawContext drawContext );
    }
}
