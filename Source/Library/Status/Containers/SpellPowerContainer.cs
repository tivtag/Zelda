// <copyright file="SpellPowerContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.SpellPowerContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Containers
{
    using System;
    using Atom.Math;

    /// <summary>
    /// Manages and stores the spell power properties of an ExtendedStatable.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SpellPowerContainer : IZeldaSetupable
    {
        /// <summary>
        /// Gets a random damage value within the damage range of the given <see cref="ElementalSchool"/>.
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        /// <returns>
        /// A random damage range within the associated damage range.
        /// </returns>
        public int GetDamage( ElementalSchool school )
        {
            return this.GetDamageRange( school ).GetRandomValue( this.rand );
        }

        /// <summary>
        /// Gets the damage range for the given <see cref="ElementalSchool"/>.
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        /// <returns>
        /// The associated damage range.
        /// </returns>
        public IntegerRange GetDamageRange( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.All:
                    return this.totalAll;

                case ElementalSchool.Fire:
                    return this.totalFire;

                case ElementalSchool.Nature:
                    return this.totalNature;

                case ElementalSchool.Shadow:
                    return this.totalShadow;

                case ElementalSchool.Light:
                    return this.totalLight;

                case ElementalSchool.Water:
                    return this.totalWater;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the SpellPowerContainer class.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that owns the new SpellPowerContainer.
        /// </param>
        internal SpellPowerContainer( ExtendedStatable statable )
        {
            this.statable = statable;
            this.auraList = statable.AuraList;
        }

        /// <summary>
        /// Setups this SpellPowerContainer.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Refreshes this SpellPowerContainer after the total Spell Power 
        /// has changed by re-caching all values.
        /// </summary>
        internal void RefreshTotal()
        {
            this.totalAll = statable.DamageMagic + this.extraAll;

            this.totalFire   = this.totalAll + this.extraFire;
            this.totalWater  = this.totalAll + this.extraWater;
            this.totalNature = this.totalAll + this.extraNature;
            this.totalLight  = this.totalAll + this.extraLight;
            this.totalShadow = this.totalAll + this.extraShadow;
        }

        /// <summary>
        /// Refreshes this SpellPowerContainer after the Spell Power
        /// of a single ElementalSchool has changed.
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        internal void RefreshExtra( ElementalSchool school )
        {
            this.RefreshExtraValue( school );
            this.RefreshTotal( school );
        }

        /// <summary>
        /// Refreshes the 'extra' field of the given ElementSchool.
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        private void RefreshExtraValue( ElementalSchool school )
        {
            float fixedValue, percentalValue;
            this.GetEffectValues( school, out fixedValue, out percentalValue );

            int value = (int)(fixedValue * percentalValue);
            this.SetExtraValue( school, value );
        }

        /// <summary>
        /// Refreshes the 'total' field of the given ElementalSchool.
        /// </summary>>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        private void RefreshTotal( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    this.totalFire = this.totalAll + this.extraFire;
                    break;

                case ElementalSchool.Water:
                    this.totalWater = this.totalAll + this.extraWater;
                    break;

                case ElementalSchool.Nature:
                    this.totalNature = this.totalAll + this.extraNature;
                    break;

                case ElementalSchool.Shadow:
                    this.totalShadow = this.totalAll + this.extraShadow;
                    break;
                    
                case ElementalSchool.Light:
                    this.totalLight = this.totalAll + this.extraLight;
                    break;

                case ElementalSchool.All:
                    this.RefreshTotal();
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the effect values for the given ElementSChool
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        /// <param name="fixedValue">
        /// Will contain the additive effect value.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        private void GetEffectValues( ElementalSchool school, out float fixedValue, out float percentalValue )
        {
            string manipulates = SpellPowerEffect.GetManipulationString( school );
            this.auraList.GetEffectValues( manipulates, out fixedValue, out percentalValue );
        }

        /// <summary>
        /// Sets the value of the 'extra' field of the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The elemental school of the Spell.
        /// </param>
        /// <param name="value">
        /// The value to set the field to.
        /// </param>
        private void SetExtraValue( ElementalSchool school, int value )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    this.extraFire = value;
                    break;

                case ElementalSchool.Water:
                    this.extraWater = value;
                    break;

                case ElementalSchool.Nature:
                    this.extraNature = value;
                    break;

                case ElementalSchool.Shadow:
                    this.extraShadow = value;
                    break;

                case ElementalSchool.Light:
                    this.extraLight = value;
                    break;

                case ElementalSchool.All:
                    this.extraAll = value;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The extra spell power applied to the various elements.
        /// </summary>
        private int extraAll, extraFire, extraNature, extraShadow, extraLight, extraWater;

        /// <summary>
        /// The total spell power of the various elements.
        /// </summary>
        private IntegerRange totalAll, totalFire, totalNature, totalShadow, totalLight, totalWater;
        
        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        /// <summary>
        /// Identifies the ExtendedStatable component that owns this SpellPowerContainer.
        /// </summary>
        private readonly ExtendedStatable statable;

        /// <summary>
        /// Identifies the AuraList of the ExtendedStatable component that owns this SpellPowerContainer.
        /// </summary>
        private readonly AuraList auraList;
    }
}