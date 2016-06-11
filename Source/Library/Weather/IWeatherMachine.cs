// <copyright file="IWeatherMachine.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.IWeatherMachine interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    /// <summary>
    /// Controls the current IWeather in a ZeldaScene.
    /// </summary>
    public interface IWeatherMachine : ISceneChangeListener, IReloadable
    {
        /// <summary>
        /// Gets the ZeldaScene whose IWeather is controlled by this IWeatherMachine.
        /// </summary>
        ZeldaScene Scene
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="IWeatherMachineSettings"/> of this IWeatherMachine.
        /// </summary>
        IWeatherMachineSettings Settings
        {
            get;
        }

        /// <summary>
        /// Called when an IWeather related to this IWeatherMachine has stopped.
        /// </summary>
        /// <param name="weather">
        /// The IWeather that has stopped.
        /// </param>
        void OnWeatherStopped( IWeather weather );
    }
}
