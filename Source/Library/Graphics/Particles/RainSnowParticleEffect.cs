// <copyright file="RainSnowParticleEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.RainSnowParticleEffect class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles
{
    using System.Diagnostics;
    using Atom;
    using Atom.Xna.Particles;
    using Atom.Xna.Particles.Controllers;
    using Zelda.Graphics.Particles.Settings;
    
    /// <summary>
    /// Represents a ParticleEffect that tries to visualize rain or snow.
    /// </summary>
    internal sealed class RainSnowParticleEffect : ParticleEffect
    {
        /// <summary>
        /// Gets or sets the maximum snow/rain Particle release quanitity of this RainSnowParticleEffect.
        /// </summary>
        public int MaximumReleaseQuantity
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the RainSnowParticleEffect class.
        /// </summary>
        /// <param name="renderer">
        /// The <see cref="ParticleRenderer"/> that should be used to render the new RainSnowParticleEffect.
        /// </param>
        private RainSnowParticleEffect( ParticleRenderer renderer )
            : base( renderer )
        {
            this.MaximumReleaseQuantity = 20;
        }

        /// <summary>
        /// Creates a new instance of this RainSnowParticleEffect that tries
        /// to visualize rain.
        /// </summary>
        /// <param name="settings">
        /// The IEmitterSettings that are applied to the rain emitter.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly created RainSnowParticleEffect.
        /// </returns>
        public static RainSnowParticleEffect CreateRain( IEmitterSettings settings, IZeldaServiceProvider serviceProvider )
        {
            Debug.Assert( settings != null );
            Debug.Assert( serviceProvider != null );

            var effect = CreateSharedEffect( serviceProvider );

            effect.mainEmitter = new RainEmitter( serviceProvider );
            effect.Emitters.Add( effect.mainEmitter );

            settings.ApplyTo( effect.mainEmitter );

            return effect;
        }

        /// <summary>
        /// Creates a new instance of this RainSnowParticleEffect that tries
        /// to visualize snow.
        /// </summary>
        /// <param name="settings">
        /// The IEmitterSettings that are applied to the rain emitter.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly created RainSnowParticleEffect.
        /// </returns>
        public static RainSnowParticleEffect CreateSnow( IEmitterSettings settings, IZeldaServiceProvider serviceProvider )
        {
            Debug.Assert( settings != null );
            Debug.Assert( serviceProvider != null );

            var effect = CreateSharedEffect( serviceProvider );

            effect.mainEmitter = new SnowEmitter( serviceProvider );
            effect.Emitters.Add( effect.mainEmitter );

            settings.ApplyTo( effect.mainEmitter );

            return effect;
        }

        /// <summary>
        /// Sets the density of the Rain/Snow spawned by this RainShowParticleEffect.
        /// </summary>
        /// <param name="factor">
        /// The density factor to set; where 0=no particles and 1=MaximumParticles.
        /// </param>
        internal void SetDensity( float factor )
        {
            this.mainEmitter.ReleaseQuantity = (int)(this.MaximumReleaseQuantity * factor);
        }

        /// <summary>
        /// Creates a new RainSnowParticleEffect and setups the properties that are shared
        /// between rain and snow.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly created RainSnowParticleEffect
        /// </returns>
        private static RainSnowParticleEffect CreateSharedEffect( IZeldaServiceProvider serviceProvider )
        {
            var effect = new RainSnowParticleEffect( serviceProvider.GetService<ParticleRenderer>() );

            effect.Controllers.Add( CreateController( effect, serviceProvider ) );

            return effect;
        }

        /// <summary>
        /// Creates and hooks the Controller responsible for triggering the given RainSnowParticleEffect.
        /// </summary>
        /// <param name="effect">
        /// The RainSnowParticleEffect that is supposed to be controlled.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly created Controller.
        /// </returns>
        private static Controller CreateController( RainSnowParticleEffect effect, IZeldaServiceProvider serviceProvider )
        {
            return new TriggerController( effect ) {
                Position = new Microsoft.Xna.Framework.Vector2( (serviceProvider.ViewSize.X / 2.0f) + 25.0f, -100.0f ),
                TriggerTime = 0.1f
            };
        }

        /// <summary>
        /// The Emitter that spawns the rain/snow particles for this RainSnowParticleEffect.
        /// </summary>
        private Emitter mainEmitter;
    }
}