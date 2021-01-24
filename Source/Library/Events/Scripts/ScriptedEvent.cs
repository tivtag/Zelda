// <copyright file="TeleportPlayerEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.TeleportPlayerEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System;
    using Zelda.Saving;
    using Zelda.Scripting;

    public sealed class ScriptedEvent : ZeldaEvent
    {
        [System.ComponentModel.Editor( typeof( Zelda.Scripting.Design.ScriptCodeEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Code
        {
            get
            {
                return this.script.Code;
            }

            set
            {
                this.script.Code = value;
            }
        }

        public bool Scoped
        {
            get
            {
                return this.script.Scoped;
            }

            set
            {
                this.script.Scoped = value;
            }
        }

        public override void Trigger( object obj )
        {
            try
            {
                this.script.Execute();
            }
            catch
            {
                throw;
            }
        }

        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );
            
            context.WriteDefaultHeader();
            this.script.Serialize( (IZeldaSerializationContext)context );
        }

        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            context.ReadDefaultHeader( this.GetType() );
            this.script.Deserialize( (IZeldaDeserializationContext)context );
        }

        private readonly ZeldaScript script = new ZeldaScript();
    }
}
