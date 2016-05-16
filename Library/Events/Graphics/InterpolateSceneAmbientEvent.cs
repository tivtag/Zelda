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
    using Atom.Xna;
    using Microsoft.Xna.Framework;
    using Zelda.Entities;
    using Zelda.Timing;

    /// <summary>
    /// Defines an <see cref="Event"/> that when triggered
    /// changes the AmbientColor of the current <see cref="ZeldaScene"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class InterpolateSceneAmbientEvent : LongTermEvent
    {
        /// <summary>
        /// Gets or sets the Ambient Color the scene will interpolate towards
        /// after this InterpolateSceneAmbientEvent got triggered.
        /// </summary>
        public Color AmbientColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time it takes for the color to interpolate.
        /// </summary>
        public float Time
        {
            get
            {
                return this.interpolationTimer.Time;
            }

            set
            {
                this.interpolationTimer.Time = value;
            }
        }
                
        /// <summary>
        /// Gets a value indicating whether this InterpolateSceneAmbientEvent
        /// can be triggered by the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">
        /// The object that wishes to trigger this InterpolateSceneAmbientEvent.
        /// </param>
        /// <returns>
        /// true if the specified Object can trigger this InterpolateSceneAmbientEvent;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( object obj )
        {
            return obj is ZeldaEntity;
        }
        
        /// <summary>
        /// Called when this InterpolateSceneAmbientEvent is about to trigger.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered it.
        /// </param>
        /// <returns>
        /// true if it was triggered; or otherwise false.
        /// </returns>
        protected override bool Triggering( object obj )
        {
            var entity = obj as ZeldaEntity;
            this.scene = entity.Scene;
            this.oldAmbient = this.scene.Settings.AmbientColor;
            this.interpolationTimer.Reset();

            return base.Triggering( obj );
        }

        /// <summary>
        /// Updates this InterpolateSceneAmbientEvent.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            float factor = 1.0f - this.interpolationTimer.Ratio;
            this.scene.Settings.AmbientColor = ColorHelper.Lerp( this.oldAmbient, this.AmbientColor, factor );           
   
            this.interpolationTimer.Update( (ZeldaUpdateContext)updateContext );

            if( this.interpolationTimer.HasEnded )
            {
                this.Stop();
                this.EventManager.RemoveEvent( this.Name );
            }
        }

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

        /// <summary>
        /// The ambient color of the scene before this event kicked in.
        /// </summary>
        private Color oldAmbient;

        /// <summary>
        /// Captures the scene that is beeing manipulated.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The timer that is used interpolate.
        /// </summary>
        private readonly ResetableTimer interpolationTimer = new ResetableTimer();
    }
}
