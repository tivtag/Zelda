
namespace Zelda.Scripting
{
    using Atom;
    using Atom.Scripting;
    using Zelda.Saving;
    using System;
    using Atom.Diagnostics;

    public sealed class ZeldaScript : Script, ISaveable
    {
        [System.ComponentModel.Editor( typeof( Zelda.Scripting.Design.ScriptCodeEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public new string Code
        {
            get
            {
                return base.Code;
            }

            set
            {
                base.Code = value;
            }
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.Scoped );
            context.Write( this.Code );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.Scoped = context.ReadBoolean();
            this.Code = context.ReadString();

            var env = context.ServiceProvider.GetService<IScriptingEnvironment>();

            if( env != null )
            {
                try
                {
                    this.Compile( env );
                }
                catch( Exception ex )
                {
                    var log = context.ServiceProvider.GetService<ILog>();
                    if( log != null )
                    log.WriteLine( LogSeverities.Error, "Error compiling script. Scoped={0} Code={1} Error={2}", this.Scoped, this.Code, ex.ToString() );
                }
            }
        }
    }
}
