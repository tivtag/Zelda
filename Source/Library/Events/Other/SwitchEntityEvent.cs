// <copyright file="SwitchEntityEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.SwitchEntityEvent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Events
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using Zelda.Entities;
    using Zelda.Entities.Design;

    /// <summary>
    /// Represents an Event that when triggered changes the switch state of an <see cref="Atom.ISwitchable"/>
    /// <see cref="ZeldaEntity"/>.
    /// </summary>
    public sealed class SwitchEntityEvent : ZeldaEvent
    {
        /// <summary>
        /// Enumerates the different moves the SwitchEntityEvent class supports.
        /// </summary>
        public enum SwitchMode
        {
            /// <summary>
            /// Toggles the ISwitchable entity from on to off, or from off to on.
            /// </summary>
            Toggle = 0,

            /// <summary>
            /// Toggles the ISwitchable entity from on.
            /// </summary>
            On,

            /// <summary>
            /// Toggles the ISwitchable entity off.
            /// </summary>
            Off
        }
        
        /// <summary>
        /// Gets or sets the <see cref="ZeldaEntity"/> this RemoveEntityEvent
        /// removes from the scene when triggered.
        /// </summary>
        [Editor( typeof( SwitchableEntitySelectionEditor ), typeof( UITypeEditor ) )]
        public ZeldaEntity Entity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SwitchMode that is executed when this SwitchEntityEvent has been triggered. 
        /// </summary>
        public SwitchMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this SwitchEntityEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this SwitchEntityEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var switchable = this.Entity as Atom.ISwitchable;
            if( switchable == null )
                return;

            switch( this.Mode )
            {
                case SwitchMode.On:
                    switchable.IsSwitched = true;
                    break;

                case SwitchMode.Off:
                    switchable.IsSwitched = false;
                    break;

                default:
                case SwitchMode.Toggle:
                    switchable.IsSwitched = !switchable.IsSwitched;
                    break;
            }
        }

        /// <summary>
        /// Serializes this SwitchEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Data
            if( this.Entity != null )
                context.Write( this.Entity.Name ?? string.Empty );
            else
                context.Write( string.Empty );

            context.Write( (int)this.Mode );
        }

        /// <summary>
        /// Deserializes this SwitchEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Data
            string entityName = context.ReadString();

            if( entityName.Length > 0 )
                this.Entity = this.Scene.GetEntity( entityName );
            else
                this.Entity = null;

            this.Mode = (SwitchMode)context.ReadInt32();
        }
    }
}
