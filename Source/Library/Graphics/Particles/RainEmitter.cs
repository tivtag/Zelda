// <copyright file="RainEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.RainEmitter class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles
{
    using Atom.Math;
    using Atom.Xna.Particles.Emitters;
    using Atom.Xna.Particles.Modifiers;

    /// <summary>
    /// Represents a Particle Emitter that tries to visualize rain
    /// by spawning Particles.
    /// </summary>
    public class RainEmitter : LineEmitter
    {
        /// <summary>
        /// Initializes a new instance of the RainEmitter class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RainEmitter( IZeldaServiceProvider serviceProvider )
        {
            this.Term = 5.0f;
            this.Budget = 1000;
            this.ReleaseQuantity = 0;
            this.Initialize();

            // Release Settings.
            this.ReleaseColor = Microsoft.Xna.Framework.Color.White;
            this.ReleaseScale = new FloatRange( 0.9f, 11.1f );
            this.ReleaseSpeed = new FloatRange( 75.0f, 180.0f );

            // Settings.
            this.Length = serviceProvider.ViewSize.X + 200;
            this.Angle = 0.35f;
            this.IsRectilinear = true;
            
            // Modifiers.
            this.Modifiers.Add( new OpacityModifier( 0.25f, 0.85f, 0.5f, 0.0f ) );
        }
    }
}
