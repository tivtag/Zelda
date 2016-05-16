// <copyright file="SpellResistanceContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.SpellResistanceContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Containers
{
    using System;
    using System.Diagnostics;
    using Atom.Math;
    using Zelda.Saving;
    
    /// <summary>
    /// Manages and stores the spell resistance properties of a <see cref="Statable"/> entity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SpellResistanceContainer : ISaveable, IZeldaSetupable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SpellResistanceContainer class.
        /// </summary>
        /// <param name="statable">
        /// The Statable component that owns the new SpellPowerContainer.
        /// </param>
        internal SpellResistanceContainer( Statable statable )
        {
            this.statable = statable;
        }

        /// <summary>
        /// Setups this SpellResistanceContainer.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Rolls the dice and returns whether a resist has occurred
        /// against an attack of the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// true if an resist has occurred;
        /// otherwise false.
        /// </returns>
        public bool TryResist( ElementalSchool school )
        {
            float roll = rand.RandomRange( 0.0f, 100.0f );
            float chance = this.Get( school );

            return roll <= chance;
        }

        /// <summary>
        /// Rolls the dice and returns whether a resist has occurred
        /// against an attack of the specified ElementalSchool from
        /// the specified ExtendedStatable entity.
        /// </summary>
        /// <param name="attacker">
        /// The ExtendedStatable component of the entity that is attacking
        /// the owner of this SpellResistanceContainer with a magical attack.
        /// </param>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// true if an resist has occurred;
        /// otherwise false.
        /// </returns>
        internal bool TryResist( ExtendedStatable attacker, ElementalSchool school )
        {
            float roll = rand.RandomRange( 0.0f, 100.0f );
            float chance = this.Get( school ) + attacker.ChanceToBe.Resisted;

            return roll <= chance;
        }

        /// <summary>
        /// Refreshes the chance to resist the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        internal void Refresh( ElementalSchool school )
        {
            if( school == ElementalSchool.All )
            {
                this.RefreshExact( ElementalSchool.Fire );
                this.RefreshExact( ElementalSchool.Water );
                this.RefreshExact( ElementalSchool.Light );
                this.RefreshExact( ElementalSchool.Shadow );
                this.RefreshExact( ElementalSchool.Nature );
            }
            else
            {
                this.RefreshExact( school );
            }
        }

        /// <summary>
        /// Refreshes the chance to resist the given exact ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell; may not be ElementalSchool.All.
        /// </param>
        private void RefreshExact( ElementalSchool school )
        {
            Debug.Assert( school != ElementalSchool.All );

            float fixedValue, ratingValue, percentualValue;
            this.statable.AuraList.GetEffectValues(
                ChanceToResistEffect.GetManipName( school ),
                ChanceToResistEffect.ManipNameAll,
                out fixedValue,
                out percentualValue,
                out ratingValue
            );

            this.Set( school, CalculateChance( fixedValue, ratingValue, percentualValue, school ) );
        }

        /// <summary>
        /// Calculates the chance to resist given the specified input paramaters.
        /// </summary>
        /// <param name="fixedValue">
        /// The fixed value from related ChanceToResistEffects.
        /// </param>
        /// <param name="ratingValue">
        /// The rating value from related ChanceToResistEffects.
        /// </param>
        /// <param name="percentualValue">
        /// The percentual value from related ChanceToResistEffects.
        /// </param>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// The chance to resist.
        /// </returns>
        private float CalculateChance( float fixedValue, float ratingValue, float percentualValue, ElementalSchool school )
        {
            float baseChance = this.GetBase( school );
            return StatusCalc.GetChanceToResist( baseChance + fixedValue, ratingValue, percentualValue, this.statable.Level );
        }

        /// <summary>
        /// Gets the base chance to resist the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// The base chance to resist in %.
        /// </returns>
        public float GetBase( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return this.baseFire;

                case ElementalSchool.Water:
                    return this.baseWater;

                case ElementalSchool.Shadow:
                    return this.baseShadow;

                case ElementalSchool.Light:
                    return this.baseLight;

                case ElementalSchool.Nature:
                    return this.baseNature;

                case ElementalSchool.All:
                    return 0.0f;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the chance to resist the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// The base chance to resist in %.
        /// </returns>
        public float Get( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return this.fire;

                case ElementalSchool.Water:
                    return this.water;

                case ElementalSchool.Shadow:
                    return this.shadow;

                case ElementalSchool.Light:
                    return this.light;

                case ElementalSchool.Nature:
                    return this.nature;

                case ElementalSchool.All:
                    return 0.0f;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the chance to resist the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <param name="value">
        /// The new chance to resist the given ElementalSchool.
        /// </param>
        private void Set( ElementalSchool school, float value )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    this.fire = value;
                    break;

                case ElementalSchool.Water:
                    this.water = value;
                    break;

                case ElementalSchool.Shadow:
                    this.shadow = value;
                    break;

                case ElementalSchool.Light:
                    this.light = value;
                    break;

                case ElementalSchool.Nature:
                    this.nature = value;
                    break;

                case ElementalSchool.All:
                    throw new NotSupportedException();

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the base chance to resist the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not a value within 0..100.
        /// </exception>
        public void SetBase( ElementalSchool school, float value )
        {
            if( value < 0 || value > 100 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "value" );

            switch( school )
            {
                case ElementalSchool.Fire:
                     this.baseFire = value;
                     break;

                case ElementalSchool.Water:
                     this.baseWater = value;
                     break;

                case ElementalSchool.Shadow:
                     this.baseShadow = value;
                     break;

                case ElementalSchool.Light:
                     this.baseLight = value;
                     break;

                case ElementalSchool.Nature:
                     this.baseNature = value;
                     break;
                                      
                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.Write( this.baseFire );
            context.Write( this.baseWater );
            context.Write( this.baseLight );
            context.Write( this.baseShadow );
            context.Write( this.baseNature );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            this.baseFire = context.ReadSingle();
            this.baseWater = context.ReadSingle();
            this.baseLight = context.ReadSingle();
            this.baseShadow = context.ReadSingle();
            this.baseNature = context.ReadSingle();

            this.SetFinalChanceToBeBaseChance();
        }

        /// <summary>
        /// Sets the chance to resist to be the base chance to resist.
        /// </summary>
        private void SetFinalChanceToBeBaseChance()
        {
            this.fire = this.baseFire;
            this.water = this.baseWater;
            this.light = this.baseLight;
            this.shadow = this.baseShadow;
            this.nature = this.baseNature;
        }

        /// <summary>
        /// Setups the given SpellResistanceContainer instance to be a clone
        /// of this SpellResistanceContainer.
        /// </summary>
        /// <param name="clone">
        /// The SpellResistanceContainer to setup as a clone of this SpellResistanceContainer.
        /// </param>
        internal void SetupClone( SpellResistanceContainer clone )
        {
            clone.fire = this.fire;
            clone.water = this.water;
            clone.light = this.light;
            clone.shadow = this.shadow;
            clone.nature = this.nature;

            clone.baseFire = this.baseFire;
            clone.baseWater = this.baseWater;
            clone.baseLight = this.baseLight;
            clone.baseShadow = this.baseShadow;
            clone.baseNature = this.baseNature;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The base chance to resist a specific element. 
        /// </summary>
        private float 
            baseFire = 5.0f, 
            baseWater = 5.0f,
            baseLight = 5.0f,
            baseShadow = 5.0f,
            baseNature = 5.0f;

        /// <summary>
        /// The final chance to resist specific element.
        /// </summary>
        private float 
            fire = 5.0f, 
            water = 5.0f,
            light = 5.0f, 
            shadow = 5.0f,
            nature = 5.0f;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        /// <summary>
        /// Identifies the Statable component that owns this SpellPowerContainer.
        /// </summary>
        private readonly Statable statable;

        #endregion
    }
}