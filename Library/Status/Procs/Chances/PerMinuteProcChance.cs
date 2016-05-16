// <copyright file="PerMinuteProcChance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.PerMinuteProcChance class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Procs
{
    using System;
    using System.Diagnostics;
    using Atom.Math;

    /// <summary>
    /// Represents an <see cref="IProcChance"/> that has an average number
    /// of Procs Per Minute.
    /// </summary>
    public abstract class PerMinuteProcChance : IProcChance
    {
        /// <summary>
        /// Gets or sets the number of procs that should occure on average.
        /// </summary>
        public float ProcsPerMinute
        {
            get
            {
                return this.procsPerMinute;
            }

            set
            {
                if( value < 0.0 )
                {
                    throw new ArgumentOutOfRangeException( "value", value, Atom.ErrorStrings.SpecifiedValueIsNegative );
                }

                this.procsPerMinute = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a proc has occured
        /// using the rules of this IProcChance by throwing the dice.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// True if a proc has occured;
        /// otherwise false.
        /// </returns>
        public bool TryProc( Statable caller, RandMT rand )
        {
            float roll = rand.UncheckedRandomRange( 0.0f, 100.0f );
            float chance = this.GetProcChancePerOccurrence( caller );

            return roll <= chance;
        }

        /// <summary>
        /// Gets the fixed chance for this PerMinuteProcChance to be a proc.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <returns>
        /// The fixed chance to proc.
        /// </returns>
        private float GetProcChancePerOccurrence( Statable caller ) 
        {
            float occurrencesPerMinute = this.GetOccurrencesPerMinute( caller );
            Debug.Assert( occurrencesPerMinute >= 0.0f );

            if( occurrencesPerMinute == 0.0f )
                return -1.0f;

            // 2 ppm ... 4 occurences per minute
            // proc chance = 50%
            float chance = this.procsPerMinute / occurrencesPerMinute;
            float chanceInPercent = chance * 100.0f;

            return chanceInPercent;
        }

        /// <summary>
        /// Gets the number of occurrences this PerMinuteProcChance
        /// has to proc per minute.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <returns>
        /// The number of occurences this PerMinuteProcChance has to proc per minute.
        /// </returns>
        protected abstract float GetOccurrencesPerMinute( Statable caller );

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.procsPerMinute );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.procsPerMinute = context.ReadSingle();
        }

        /// <summary>
        /// The storage field of the <see cref="ProcsPerMinute"/> property.
        /// </summary>
        private float procsPerMinute;
    }
}