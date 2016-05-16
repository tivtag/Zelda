
namespace Zelda.Weather.Settings
{
    using System;
    using Zelda.Graphics.Particles.Settings;
    using Zelda.Saving;

    public sealed class DefaultWeatherMachineSettings : IWeatherMachineSettings
    {
        public IEmitterSettings RainSettings
        {
            get;
            set;
        }

        public IEmitterSettings SnowSettings
        {
            get;
            set;
        }

        public void LoadContent(IZeldaServiceProvider serviceProvider)
        {
            this.RainSettings = new DefaultRainEmitterSettings(serviceProvider);
            this.SnowSettings = new DefaultSnowEmitterSettings(serviceProvider);
        }

        public void Reload(IZeldaServiceProvider serviceProvider)
        {
            ((IZeldaSetupable)this.RainSettings).Setup(serviceProvider);
            ((IZeldaSetupable)this.SnowSettings).Setup(serviceProvider);
        }

        public void Serialize(IZeldaSerializationContext context)
        {
            throw new NotSupportedException();
        }

        public void Deserialize(IZeldaDeserializationContext context)
        {
            throw new NotSupportedException();
        }
    }
}
