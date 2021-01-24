// <copyright file="IWeatherCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.IWeatherCreator interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    /// <summary>
    /// Defines a mechanism that allows creation of new <see cref="IWeather"/> of a specific type.
    /// </summary>
    public interface IWeatherCreator
    {
        /// <summary>
        /// Creates a new instance of the IWeather this IWeatherCreator creates.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine for which the IWeather should be created for.
        /// </param>
        /// <returns>
        /// The <see cref="IWeather"/> instances that make up the weather.
        /// </returns>
        IWeather[] CreateWeather( IWeatherMachine weatherMachine );
    }
}
