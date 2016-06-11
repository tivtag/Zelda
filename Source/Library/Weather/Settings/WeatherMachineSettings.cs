// <copyright file="WeatherMachineSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.WeatherMachineSettings class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    using System.ComponentModel;
    using Zelda.Graphics.Particles.Settings;
    using Zelda.Graphics.Tinting;
    using Zelda.Saving;

    /// <summary>
    /// Encapsulates the settings used by a <see cref="WeatherMachine"/> to create new IWeather.
    /// </summary>
    public sealed class WeatherMachineSettings : IWeatherMachineSettings
    {
        /// <summary>
        /// Gets or sets the IEmitterSettings that are applied to rain.
        /// </summary>
        [Editor( typeof( Zelda.Graphics.Particles.Settings.Design.EmitterSettingsEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IEmitterSettings RainSettings
        {
            get
            {
                return this.rainSettings;
            }
            
            set
            {
                this.rainSettings = value ?? DefaultRainSettings;
            }
        }

        /// <summary>
        /// Gets or sets the IEmitterSettings that are applied to snow.
        /// </summary>
        [Editor( typeof( Zelda.Graphics.Particles.Settings.Design.EmitterSettingsEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IEmitterSettings SnowSettings
        {
            get
            {
                return this.snowSettings;
            }

            set
            {
                this.snowSettings = value ?? DefaultSnowSettings;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeatherMachineSettings class.
        /// </summary>
        /// <param name="defaultSettings">
        /// The settings that the new WeatherMachineSettings should copy.
        /// </param>
        public WeatherMachineSettings( IWeatherMachineSettings defaultSettings )
        {
            if( defaultSettings != null )
            {
                this.SnowSettings = DefaultSnowSettings = defaultSettings.SnowSettings;
                this.RainSettings = DefaultRainSettings = defaultSettings.RainSettings;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.WriteObject( this.RainSettings );
            context.WriteObject( this.SnowSettings );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            if( version >= 2 )
            {
                this.RainSettings = context.ReadObject<IEmitterSettings>() ?? DefaultRainSettings;
                this.SnowSettings = context.ReadObject<IEmitterSettings>() ?? DefaultSnowSettings;
            }
            else
            {
                context.ReadObject<IColorTint>();
                context.ReadObject<IColorTint>();
            }
        }
        
        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            var reloadable = this.rainSettings as IReloadable;

            if (reloadable != null)
            {
                reloadable.Reload(serviceProvider);
            }

            reloadable = this.snowSettings as IReloadable;

            if (reloadable != null)
            {
                reloadable.Reload(serviceProvider);
            }
        }

        /// <summary>
        /// The IEmitterSettings that are applied to new rainy IWeather.
        /// </summary>
        private IEmitterSettings rainSettings;

        /// <summary>
        /// The IEmitterSettings that are applied to new snowy IWeather.
        /// </summary>
        private IEmitterSettings snowSettings;

        /// <summary>
        /// Stores the default settings used by rainy IWeather.
        /// </summary>
        private static IEmitterSettings DefaultRainSettings;

        /// <summary>
        /// Stores the default settings used by snowy IWeather.
        /// </summary>
        private static IEmitterSettings DefaultSnowSettings;
    }
}
