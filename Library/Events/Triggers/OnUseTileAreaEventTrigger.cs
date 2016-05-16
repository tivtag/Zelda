// <copyright file="OnUseTileAreaEventTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.OnUseTileAreaEventTrigger class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using Atom.Events;
    using Zelda.Entities;

    /// <summary>
    /// Represents an EventTrigger that gets triggered when the player has pressed the Use key
    /// while standing in the trigger area. The player must also face the area.
    /// </summary>
    public sealed class OnUseTileAreaEventTrigger : InputTileAreaEventTrigger
    {  
        /// <summary>
        /// Gets whether the specified Object can trigger the EventTrigger.
        /// </summary>
        /// <param name="context">
        /// The context under test.
        /// </param>
        /// <returns>
        /// true if the object can trigger it;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( TriggerContext context )
        {
            var player = context.Object as PlayerEntity;

            if( player != null )
            {
                var profile = player.Profile;

                if( this.WasUsePressed( profile.KeySettings ) )
                {
                    if( player.Transform.IsFacing( this.Area.Center ) )
                    {
                        return base.CanBeTriggeredBy( context );
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the player has pressed the "Use" key.
        /// </summary>
        /// <param name="keySettings">
        /// The keyboard settings.
        /// </param>
        /// <returns>
        /// true if it was prassed;
        /// otherwise false.
        /// </returns>
        private bool WasUsePressed( KeySettings keySettings )
        {
            return this.IsKeyDown( keySettings.UsePickup );
        }
    }
}
