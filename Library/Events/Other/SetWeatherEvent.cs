
namespace Zelda.Events
{
    using System;
    using System.ComponentModel;
    using Atom;

    public sealed class SetWeatherEvent : ZeldaEvent
    {
        [Editor( typeof( Zelda.Weather.Creators.Design.WeatherCreatorTypeNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string WeatherCreatorTypeName
        {
            get
            {
                return this.WeatherCreatorType != null ? this.WeatherCreatorType.GetTypeName() : string.Empty;
            }

            set
            {
                try
                {
                    this.WeatherCreatorType = Type.GetType( value );
                }
                catch
                {
                    this.WeatherCreatorType = null;
                }
            }
        }

        public Type WeatherCreatorType
        {
            get;
            private set;
        }

        public override void Trigger( object obj )
        {
            if( this.WeatherCreatorType != null )
            {
                this.Scene.WeatherMachine.SetWeather( this.WeatherCreatorType );
            }
        }

        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.WeatherCreatorType != null ? this.WeatherCreatorType.GetTypeName() : string.Empty );
        }

        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );
            
            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );
            
            string typeName = context.ReadString();
            this.WeatherCreatorType = typeName.Length > 0 ? Type.GetType( typeName ) : null;
        }
    }
}
