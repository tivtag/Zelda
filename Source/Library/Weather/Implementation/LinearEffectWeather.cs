// <copyright file="LinearEffectWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.LinearEffectWeather class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    using Atom.Math;

    /// <summary>
    /// Represents an <see cref="IWeather"/> whos effect changes 
    /// linearily based on its duration.
    /// </summary>
    internal abstract class LinearEffectWeather : BaseWeather
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearEffectWeather"/> class.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that has triggered the new LinearEffectWeather.
        /// </param>
        protected LinearEffectWeather( IWeatherMachine weatherMachine )
            : base( weatherMachine )
        {
        }

        /// <summary>
        /// Setups this LinearEffectWeather.
        /// </summary>
        /// <param name="totalTime">
        /// The total time this IWeather should last.
        /// </param>
        /// <param name="startEffect">
        /// The effect value at the start of the LinearEffectWeather.
        /// </param>
        /// <param name="maxEffect">
        /// The effect value at the peak of the LinearEffectWeather.
        /// </param>
        /// <param name="endEffect">
        /// The effect value at the end of the LinearEffectWeather.
        /// </param>
        /// <param name="startMaxEffectTime">
        /// The time (in seconds) on the time table of this LinearEffectWeather the <paramref name="maxEffect"/>
        /// is set as the current effect value.
        /// </param>
        /// <param name="endMaxEffectTime">
        /// The time (in seconds) on the time table of this LinearEffectWeather the <paramref name="maxEffect"/>
        /// is not set as the current effect value anymore.
        /// </param>
        protected void Setup( 
            float totalTime, 
            float startEffect,
            float maxEffect, 
            float endEffect,
            float startMaxEffectTime, 
            float endMaxEffectTime )
        {
            base.Setup( totalTime );

            this.startEffect = startEffect;
            this.maxEffect   = maxEffect;
            this.endEffect   = endEffect;

            this.startMaxEffectTime  = startMaxEffectTime;
            this.endMaxEffectTime    = endMaxEffectTime;
            this.cachedFirstPartTime = totalTime - startMaxEffectTime;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Receives the current effect value of this LinearEffectWeather.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is used to control this LinearEffectWeather.
        /// </remarks>
        /// <returns>
        /// The current effect value.
        /// </returns>
        protected float GetLinearEffect()
        {
            float timeLeft = this.TimeLeft;

            if( timeLeft < startMaxEffectTime && 
                timeLeft > endMaxEffectTime )
            {
                return this.maxEffect;
            }

            if( timeLeft <= this.endMaxEffectTime )
            {
                float ratio = 1.0f - (timeLeft / this.endMaxEffectTime);
                return MathUtilities.Coserp( maxEffect, endEffect, ratio );
            }
            else if( timeLeft >= this.startMaxEffectTime )
            {
                float ratio = (this.TotalTime - timeLeft) / this.cachedFirstPartTime;
                return MathUtilities.Coserp( startEffect, maxEffect, ratio );
            }

            return 0.0f;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States the effect value based on a specific point on the time table.
        /// </summary>
        private float startEffect, maxEffect, endEffect;

        /// <summary>
        /// The points on the time table where the maximum effect is active.
        /// </summary>
        private float startMaxEffectTime, endMaxEffectTime;

        /// <summary>
        /// Cached this.TotalTime - startMaxEffectTime
        /// </summary>
        private float cachedFirstPartTime;

        #endregion
    }
}