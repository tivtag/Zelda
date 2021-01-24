// <copyright file="IWeatherCreatorProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.IWeatherCreatorProvider interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    /// <summary>
    /// Provides a mechanism that allows receiving an IWeatherCreator.
    /// </summary>
    internal interface IWeatherCreatorProvider
    {
        /// <summary>
        /// Gets an IWeatherCreator
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that wants to create some IWeather using the requested IWeatherCreator.
        /// </param>
        /// <returns>
        /// The requested IWeatherCreator; or null.
        /// </returns>
        IWeatherCreator GetCreator( IWeatherMachine weatherMachine );
    }
}
