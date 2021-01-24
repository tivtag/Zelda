// <copyright file="IWeatherMachineSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.IWeatherMachineSettings interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather
{
    using System.ComponentModel;
    using Zelda.Graphics.Particles.Settings;
    using Zelda.Saving;

    /// <summary>
    /// Provides access to the settings used by a <see cref="WeatherMachine"/> to create new IWeather.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IWeatherMachineSettings : ISaveable, IReloadable
    {
        /// <summary>
        /// Gets or sets the IEmitterSettings that is applied to rain.
        /// </summary>
        IEmitterSettings RainSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IEmitterSettings that is applied to snow.
        /// </summary>
        IEmitterSettings SnowSettings
        {
            get;
            set;
        }
    }
}
