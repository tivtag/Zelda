// <copyright file="PerMinuteAttackSpeedRelatedProcChance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.PerMinuteAttackSpeedRelatedProcChance class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Procs
{
    using System;
    using Zelda.Attacks;

    /// <summary>
    /// Defines a <see cref="PerMinuteProcChance"/> that uses the Attack Speed
    /// of the caller to calculate the proc occurrences per minute.
    /// This class is sealed.
    /// </summary>
    public sealed class PerMinuteAttackSpeedRelatedProcChance : PerMinuteProcChance
    {
        /// <summary>
        /// Gets or sets what kind of attack speed should be used
        /// to calculate the proc chance.
        /// </summary>
        public AttackType AttackType
        {
            get 
            { 
                return this.attackType; 
            }

            set
            {
                if( value != AttackType.Melee && value != AttackType.Ranged )
                {
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "value" );
                }

                this.attackType = value;
            }
        }
        
        /// <summary>
        /// Gets the number of occurrences this PerMinuteAttackSpeedRelatedProcChance
        /// has to proc per minute.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <returns>
        /// The number of occurences this PerMinuteProcChance has to proc per minute.
        /// </returns>
        protected override float GetOccurrencesPerMinute( Statable caller )
        {
            float attackSpeed = this.GetAttackSpeed( caller );
            float attacksPerSecond = 1.0f / attackSpeed;
            float attacksPerMinute = attacksPerSecond * 60.0f;

            return attacksPerMinute;
        }

        /// <summary>
        /// Gets the attack speed of the set <see cref="AttackType"/> of the given caller.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <returns>
        /// The attack speed (also called attack delay).
        /// </returns>
        private float GetAttackSpeed( Statable caller )
        {
            var extendedCaller = caller as ExtendedStatable;

            if( extendedCaller != null )
            {
                switch( this.attackType )
                {
                    case AttackType.Melee:
                        return extendedCaller.AttackSpeedMeleeNormalized;

                    case AttackType.Ranged:
                        return extendedCaller.AttackSpeedRangedNormalized;

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                switch( this.attackType )
                {
                    case AttackType.Melee:
                        return caller.AttackSpeedMelee;

                    case AttackType.Ranged:
                        return caller.AttackSpeedRanged;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( (byte)this.attackType );
            base.Serialize( context );
        }
        
        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.attackType = (AttackType)context.ReadByte();
            base.Deserialize( context );
        }

        /// <summary>
        /// The storage field of the <see cref="AttackType"/> property.
        /// </summary>
        private AttackType attackType = AttackType.Melee;
    }
}