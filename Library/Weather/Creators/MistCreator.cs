// <copyright file="MistCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.MistCreator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an <see cref="IWeatherCreator"/> that creates
    /// mist weather. This class can't be inherited.
    /// </summary>
    public sealed class MistCreator : FogCreator
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="MistCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public MistCreator( IZeldaServiceProvider serviceProvider )
            : base( "MistOverlay", serviceProvider )
        {
            this.MinimumSpeed = new Vector2( -0.25f, -0.25f );
            this.MaximumSpeed = new Vector2( 0.25f, 0.25f );

            this.MinimumDensity = 0.15f;
            this.MaximumDensity = 0.26f;
        }

        /// <summary>
        /// Generates the color of the next mist effect.
        /// </summary>
        protected override Xna.Color GetFogColor()
        {
            float rand = Rand.RandomSingle;

            if( rand <= 0.7f )
            {
                return Xna.Color.White;
            }
            else if( rand <= 0.9f )
            {
                return Xna.Color.Red;
            }
            else
            {
                return new Xna.Color( this.Rand.RandomSingle, this.Rand.RandomSingle, this.Rand.RandomSingle, this.Rand.RandomSingle );
            }
        }
    }
}
