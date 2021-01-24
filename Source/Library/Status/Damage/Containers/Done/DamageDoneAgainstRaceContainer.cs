// <copyright file="DamageDoneAgainstRaceContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageDoneAgainstRaceContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    using System;

    /// <summary>
    /// Encapsulates the damage properties that are applied
    /// on damage done against a specific <see cref="RaceType"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageDoneAgainstRaceContainer
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DamageDoneAgainstRaceContainer class.
        /// </summary>
        internal DamageDoneAgainstRaceContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageDoneAgainstRaceContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable component that should own this DamageSourceContainer.
        /// </param>
        internal void Initialize( ExtendedStatable statable )
        {
            this.auraList = statable.AuraList;
        }

        #endregion

        #region [ Methods ]

        #region > Apply <

        /// <summary>
        /// Applies the damage multiplier for the specified <see cref="RaceType"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <param name="race">
        /// The race of the target of the damage.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int Apply( int damage, RaceType race )
        {
            float multiplier = this.Get( race );
            return (int)(damage * multiplier);
        }

        /// <summary>
        /// Applies the fixed damage modifier for the specified <see cref="RaceType"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <param name="race">
        /// The race of the target of the damage.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyFixed( int damage, RaceType race )
        {
            return damage + this.GetFixed( race );
        }
        
        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the damage properties against the specified Race.
        /// </summary>
        /// <param name="race">
        /// The race to refresh.
        /// </param>
        internal void Refresh( RaceType race )
        {
            float fixedValue, multiplierValue;            
                
            this.auraList.GetEffectValues(
                DamageDoneAgainstRaceEffect.GetIdentifier( race ),
                out fixedValue,
                out multiplierValue
            );

            this.Set( race, (int)fixedValue, multiplierValue );
        }

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the damage multiplier value for the given RaceType.
        /// </summary>
        /// <param name="race">
        /// The race of the target of the damage.
        /// </param>
        /// <returns>
        /// The value a damage value that is directed against the specified Race should be multiplied by.
        /// </returns>
        public float Get( RaceType race )
        {
            switch( race )
            {
                case RaceType.Beast:
                    return this.beast;

                case RaceType.DemiBeast:
                    return this.demiBeast;

                case RaceType.DemiHuman:
                    return this.demiHuman;

                case RaceType.Demon:
                    return this.demon;

                case RaceType.Fairy:
                    return this.fairy;

                case RaceType.Human:
                    return this.human;

                case RaceType.Machine:
                    return this.machine;

                case RaceType.Plant:
                    return this.plant;

                case RaceType.Slime:
                    return this.slime;

                case RaceType.Undead:
                    return this.undead;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "race" );
            }
        }

        /// <summary>
        /// Gets the fixed damage modifier value for the given RaceType.
        /// </summary>
        /// <param name="race">
        /// The race of the target of the damage.
        /// </param>
        /// <returns>
        /// The value that should be added to damage value that is directed against the specified Race.
        /// </returns>
        public int GetFixed( RaceType race )
        {
            switch( race )
            {
                case RaceType.Beast:
                    return this.fixedBeast;

                case RaceType.DemiBeast:
                    return this.fixedDemiBeast;

                case RaceType.DemiHuman:
                    return this.fixedDemiHuman;

                case RaceType.Demon:
                    return this.fixedDemon;

                case RaceType.Fairy:
                    return this.fixedFairy;

                case RaceType.Human:
                    return this.fixedHuman;

                case RaceType.Machine:
                    return this.fixedMachine;

                case RaceType.Plant:
                    return this.fixedPlant;

                case RaceType.Slime:
                    return this.fixedSlime;

                case RaceType.Undead:
                    return this.fixedUndead;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "race" );
            }
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the modifier properties for the specified RaceType.
        /// </summary>
        /// <param name="race">
        /// The race whose properties should be set.
        /// </param>
        /// <param name="fixedValue">
        /// The value that should be added to a damage value of the given DamageSchool.
        /// </param>
        /// <param name="multiplierValue">
        /// The value a damage value of the given DamageSchool should be multiplied by.
        /// </param>
        private void Set( RaceType race, int fixedValue, float multiplierValue )
        {
            switch( race )
            {
                case RaceType.Beast:
                    this.fixedBeast = fixedValue;
                    this.beast = multiplierValue;
                    break;

                case RaceType.DemiBeast:
                    this.fixedDemiBeast = fixedValue;
                    this.demiBeast = multiplierValue;
                    break;

                case RaceType.DemiHuman:
                    this.fixedDemiHuman = fixedValue;
                    this.demiHuman = multiplierValue;
                    break;

                case RaceType.Demon:
                    this.fixedDemon = fixedValue;
                    this.demon = multiplierValue;
                    break;

                case RaceType.Fairy:
                    this.fixedFairy = fixedValue;
                    this.fairy = multiplierValue;
                    break;

                case RaceType.Human:
                    this.fixedHuman = fixedValue;
                    this.human = multiplierValue;
                    break;

                case RaceType.Machine:
                    this.fixedMachine = fixedValue;
                    this.machine = multiplierValue;
                    break;
                    
                case RaceType.Plant:
                    this.fixedPlant = fixedValue;
                    this.plant = multiplierValue;
                    break;

                case RaceType.Slime:
                    this.fixedSlime = fixedValue;
                    this.slime = multiplierValue;
                    break;

                case RaceType.Undead:
                    this.fixedUndead = fixedValue;
                    this.undead = multiplierValue;
                    break;

                default:
                    throw new NotImplementedException( race.ToString() );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The multiplier values that are applied against enemies of a specific Race.
        /// </summary>
        private float undead = 1.0f, human = 1.0f, machine = 1.0f,
                      plant = 1.0f, fairy = 1.0f, beast  = 1.0f,
                      demiHuman = 1.0f, slime = 1.0f, demiBeast = 1.0f,
                      demon  = 1.0f;

        /// <summary>
        /// The fixed modifier values that are applied against enemies of a specific Race.
        /// </summary>
        private int fixedUndead, fixedHuman, fixedPlant,
                    fixedMachine, fixedFairy, fixedBeast,
                    fixedDemiHuman, fixedSlime, fixedDemiBeast,
                    fixedDemon;

        /// <summary>
        /// Identifies the AuraList of the statable entity that owns this DamageDoneAgainstRaceContainer.
        /// </summary>
        private AuraList auraList;

        #endregion
    }
}