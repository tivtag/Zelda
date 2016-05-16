// <copyright file="ZeldaParticleRenderer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.Particles.ZeldaParticleRenderer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics.Particles
{
    using Atom;
    using Atom.Xna.Particles;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines the ParticleRenderer used by the Zelda game.
    /// </summary>
    public sealed class ZeldaParticleRenderer : SpriteBatchParticleRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaParticleRenderer class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public ZeldaParticleRenderer( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider.GetService<IGraphicsDeviceService>() )
        {
        }
    }
}
