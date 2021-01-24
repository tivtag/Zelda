// <copyright file="OnKeyDownTileAreaEventTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.OnKeyDownTileAreaEventTrigger class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using Atom.Events;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;

    /// <summary>
    /// Represents an EventTrigger that triggers when the player presses a specific Key in a specific area.
    /// </summary>
    public class OnKeyDownTileAreaEventTrigger : InputTileAreaEventTrigger
    {
        /// <summary>
        /// Gets or sets the Key that must be pressed while beeing in the required
        /// area for this InputTileAreaEventTrigger to trigger.
        /// </summary>
        public Keys Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether the specified Object can trigger the <see cref="EventTrigger"/>.
        /// </summary>
        /// <param name="context">
        /// The object to test.
        /// </param>
        /// <returns>
        /// true if the object can trigger it;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( TriggerContext context )
        {
            if( context.Object is PlayerEntity )
            {
                if( this.IsKeyDown( this.Key ) )
                {
                    return base.CanBeTriggeredBy( context );
                }
            }

            return false;
        }
        
        /// <summary>
        /// Serializes this TileAreaEventTrigger using the specified IEventSerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( IEventSerializationContext context )
        {
            base.Serialize( context );

            context.Write( (int)this.Key );
        }

        /// <summary>
        /// Deserializes this TileAreaEventTrigger using the specified IEventDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( IEventDeserializationContext context )
        {
            base.Deserialize( context );

            this.Key = (Keys)context.ReadInt32();
        }
    }
}
