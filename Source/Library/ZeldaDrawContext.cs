// <copyright file="ZeldaDrawContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaDrawContext class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using Atom.Xna.Batches;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines the <see cref="Atom.IDrawContext"/> used by the game.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ZeldaDrawContext : Atom.Xna.SpriteDrawContext
    {
        /// <summary>
        /// Gets the currently active <see cref="Zelda.Entities.ZeldaCamera"/>.
        /// </summary>
        public Zelda.Entities.ZeldaCamera Camera
        {
            get;
            internal set;
        }
        
        public bool IsRunningSlowly
        {
            get
            {
                return this.GameTime.IsRunningSlowly;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaDrawContext"/> class.
        /// </summary>
        /// <param name="device">
        /// The XNA graphics device.
        /// </param>
        public ZeldaDrawContext( GraphicsDevice device )
            : base( new ComposedSpriteBatch( device ), device )
        {
        }
    }
}
