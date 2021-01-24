// <copyright file="LinearEffectWeatherCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.LinearEffectWeatherCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom.Math;

    /// <summary>
    /// Represents an <see cref="IWeatherCreator"/> that creates an <see cref="IWeather"/> that
    /// is based on <see cref="LinearEffectWeather"/>.
    /// </summary>
    public abstract class LinearEffectWeatherCreator : BaseWeatherCreator
    {
        /// <summary>
        /// Gets or sets the minimum time the IWeather created
        /// by this LinearEffectWeatherCreator will last.
        /// </summary>
        public float MinimumTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time the IWeather created 
        /// by this LinearEffectWeatherCreator will last.
        /// </summary>
        public float MaximumTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum speed this LinearEffectWeatherCreator scrolls at.
        /// </summary>
        public Vector2 MinimumSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum speed this LinearEffectWeatherCreator scrolls at.
        /// </summary>
        public Vector2 MaximumSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minumum density of this IWeather created by this FogCreator.
        /// </summary>
        public float MinimumDensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum density of this IWeather created by this FogCreator.
        /// </summary>
        public float MaximumDensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the factor that is used to calculate the starting time
        /// of the maximum density region.
        /// </summary>
        public float MaximumDensityStartFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the factor that is used to calculate the ending time
        /// of the maximum density region.
        /// </summary>
        public float MaximumDensityEndFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearEffectWeatherCreator"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        protected LinearEffectWeatherCreator( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
        }

        /// <summary>
        /// Helper method that gets the scroll speed of the next IWeather
        /// created by this LinearEffectWeatherCreator.
        /// </summary>
        /// <returns>
        /// The speed value; in [<see cref="MinimumSpeed"/>; <see cref="MaximumSpeed"/>].
        /// </returns>
        protected Vector2 GetSpeed()
        {
            return this.Rand.RandomRange( this.MinimumSpeed, this.MaximumSpeed );
        }
        
        /// <summary>
        /// Helper method that gets the density of the next IWeather
        /// created by this LinearEffectWeatherCreator.
        /// </summary>
        /// <returns>
        /// The density value; in [<see cref="MinimumDensity"/>; <see cref="MaximumDensity"/>].
        /// </returns>
        protected float GetDensity()
        {
            return this.Rand.RandomRange( this.MinimumDensity, this.MaximumDensity );
        }

        /// <summary>
        /// Helper method that gets the duration in seconds the next IWeather
        /// created by this LinearEffectWeatherCreator lasts.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine for which the new IWeather is created.
        /// </param>
        /// <returns>
        /// The duration in seconds.
        /// </returns>
        protected float GetTime( IWeatherMachine weatherMachine )
        {
            return this.GetTime( weatherMachine.Scene.IngameDateTime );
        }

        /// <summary>
        /// Helper method that gets the duration in seconds the next IWeather
        /// created by this LinearEffectWeatherCreator lasts.
        /// </summary>
        /// <param name="ingameDateTime">
        /// The object that encapsulates the flow of date and time.
        /// </param>
        /// <returns>
        /// The duration in seconds.
        /// </returns>
        protected float GetTime( IngameDateTime ingameDateTime )
        {
            return this.Rand.RandomRange( this.MinimumTime, this.MinimumTime ) * ingameDateTime.InverseTickSpeed;
        }
    }
}
