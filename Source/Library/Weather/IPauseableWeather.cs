// <copyright file="IPauseableWeather.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.IPauseableWeather interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    /// <summary>
    /// Represents an IWeather that can be paused/unpaused.
    /// </summary>
    /// <remarks>
    /// All currently active IPauseableWeather is paused/unpaused when a SceneChange occurrs.
    /// <para/>
    /// </remarks>
    public interface IPauseableWeather : IWeather
    {
        /// <summary>
        /// Pauses this IPauseableWeather.
        /// </summary>
        void Pause();

        /// <summary>
        /// Unpauses this IPauseableWeather.
        /// </summary>
        void Unpause();
    }
}
