// <copyright file="IWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.IWeather interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    /// <summary> 
    /// Defines the interface of an object that simulates an ingame 'weather'.
    /// </summary>
    public interface IWeather : IZeldaUpdateable
    {
        /// <summary>
        /// Starts this <see cref="IWeather"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this <see cref="IWeather"/>.
        /// </summary>
        /// <param name="informWeatherMachine">
        /// States whether the <see cref="WeatherMachine"/> that owns this IWeather
        /// should be informed that this IWeather has stopped.
        /// </param>
        void Stop( bool informWeatherMachine );

        /// <summary>
        /// Resets the state of this <see cref="IWeather"/>.
        /// </summary>
        void Reset();
    }
}
