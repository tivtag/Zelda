// <copyright file="SpecificTimeOfDayRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Requirements.SpecificTimeOfDayRequirement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Requirements
{
    using Zelda.Saving;
    using System;
    
    /// <summary>
    /// Implements a predicate that returns true in a specific time frame of the ingame day.
    /// </summary>
    public sealed class SpecificTimeOfDayRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the period of the day in which this IIsUseable
        /// predicate returns true.
        /// </summary>
        public DayNightState TimeFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether something is currently useable.
        /// </summary>
        /// <param name="user">
        /// The object that wants to know whether it can use something.
        /// </param>
        /// <returns>
        /// true if it is useable;
        /// otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity user )
        {
            if( user == null )
                return false;
            
            DateTime current = user.Profile.WorldStatus.IngameDateTime.Current;
            var state = DayNightCycle.GetState( current );
            
            return this.TimeFrame == state;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( (int)this.TimeFrame );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.TimeFrame = (DayNightState)context.ReadInt32();
        }
    }
}
