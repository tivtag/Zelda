// <copyright file="SnowEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.SnowEmitter class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Particles
{
    using Atom.Math;
    using Atom.Xna.Particles.Emitters;
    using Atom.Xna.Particles.Modifiers;
    using Microsoft.Xna.Framework;
    
    /// <summary>
    /// Represents a Particle Emitter that tries to visualize snow
    /// by spawning Particles.
    /// </summary>
    public class SnowEmitter : LineEmitter
    {
        /// <summary>
        /// Initializes a new instance of the SnowEmitter class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public SnowEmitter( IZeldaServiceProvider serviceProvider )
        {
            this.Term = 4.5f;
            this.Budget = 800;
            this.ReleaseQuantity = 0;
            this.Initialize();
            
            // Release Settings.
            this.ReleaseColor = Color.White;
            this.ReleaseScale = new FloatRange( 0.5f, 4.5f );
            this.ReleaseSpeed = new FloatRange( 45.0f, 135.0f );

            // Settings.
            this.Length = serviceProvider.ViewSize.X + 200;
            this.Angle = 0.35f;
            this.IsRectilinear = true;

            // Modifiers.
            this.Modifiers.Add( new OpacityModifier( 0.20f, 0.68f, 0.5f, 0.0f ) );
        }
    }
}
