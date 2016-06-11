// <copyright file="SceneAmbientChangeEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.SceneAmbientChangeEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System;
    using Atom.Events;
    using Microsoft.Xna.Framework;
    using Zelda.Entities;

    /// <summary>
    /// Defines an <see cref="Event"/> that when triggered
    /// changes the AmbientColor of the current <see cref="ZeldaScene"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SceneAmbientChangeEvent : Event
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Ambient Color the scene will have
        /// after this SceneAmbientChangeEvent got triggered.
        /// </summary>
        public Color AmbientColor
        {
            get;
            set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a value indicating whether this SceneAmbientChangeEvent
        /// can be triggered by the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">
        /// The object that wishes to trigger this SceneAmbientChangeEvent.
        /// </param>
        /// <returns>
        /// true if the specified Object can trigger this SceneAmbientChangeEvent;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( object obj )
        {
            return obj is PlayerEntity;
        }

        /// <summary>
        /// Triggers this SceneAmbientChangeEvent.
        /// </summary>
        /// <param name="obj">
        /// The Object that has triggered this Event.
        /// </param>
        public override void Trigger( object obj )
        {
            if( !this.CanBeTriggeredBy( obj ) )
                return;

            var entity = (ZeldaEntity)obj;

            var scene  = entity.Scene;
            if( scene == null )
                return;

            scene.Settings.AmbientColor = this.AmbientColor;             
        }

        #region > Storage <

        /// <summary>
        /// Serializes this SceneAmbientChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            context.Write( this.AmbientColor.R );
            context.Write( this.AmbientColor.G );
            context.Write( this.AmbientColor.B );
            context.Write( this.AmbientColor.A );
        }

        /// <summary>
        /// Deserializes this SceneAmbientChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            byte r = context.ReadByte();
            byte g = context.ReadByte();
            byte b = context.ReadByte();
            byte a = context.ReadByte();

            this.AmbientColor = new Color( r, g, b, a );
        }

        #endregion

        #endregion
    }
}
