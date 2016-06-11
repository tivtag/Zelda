// <copyright file="ActionEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ActionEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System.ComponentModel;
    using Atom;
    using Zelda.Actions;
    using Zelda.Saving;

    /// <summary>
    /// Represents an event that when triggered executes an IAction{T}.
    /// </summary>
    public sealed class ActionEvent : ZeldaEvent
    {
        /// <summary>
        /// Gets or sets the action this is executed when this ActionEvent is triggered.
        /// </summary>
        [Editor( typeof( Zelda.Actions.Design.ActionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this ActionEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this ActionEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            if( this.Action != null )
            {
                this.Action.Execute();
            }
        }

        /// <summary>
        /// Serializes this ActionEditor event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            var zeldaContext = (IZeldaEventSerializationContext)context;
            zeldaContext.WriteStoreObject( this.Action );
        }

        /// <summary>
        /// Deserializes this ActionEditor event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            var zeldaContext = (IZeldaEventDeserializationContext)context;
            this.Action = zeldaContext.ReadStoreObject<IAction>();
        }
    }
}
