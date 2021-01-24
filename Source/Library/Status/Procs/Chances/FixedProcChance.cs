// <copyright file="FixedProcChance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.FixedProcChance class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Procs
{
    using System;
    using Atom.Math;

    /// <summary>
    /// Represents an <see cref="IProcChance"/> that has a fixed value.
    /// </summary>
    public sealed class FixedProcChance : IProcChance
    {
        /// <summary>
        /// Gets or sets the chance for a proc to occure.
        /// </summary>
        public float Chance
        {
            get
            { 
                return this.chance;
            }
            
            set
            {
                if( value < 0.0f || value > 100.0f )
                {
                    throw new ArgumentOutOfRangeException( "value", value, Atom.ErrorStrings.SpecifiedValueIsInvalid );
                }

                this.chance = value;
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
            return rand.RandomRange( 0.0f, 100.0f ) <= this.chance;
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
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.chance );
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.chance = context.ReadSingle();
        }

        /// <summary>
        /// The storage field of the <see cref="Chance"/> property.
        /// </summary>
        private float chance;
    }
}
