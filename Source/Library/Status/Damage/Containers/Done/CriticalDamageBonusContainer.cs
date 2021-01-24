// <copyright file="CriticalDamageBonusContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.CriticalDamageBonusContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    using System;

    /// <summary>
    /// Encapsulates the damage increase gained when
    /// an attack has been a critical attack.
    /// This class can't be inherited.
    /// </summary>
    public sealed class CriticalDamageBonusContainer
    {
        #region [ Constants ]

        /// <summary>
        /// The default Critical Damage Modifier for Melee damage.
        /// </summary>
        private const float DefaultMelee = 2.0f;

        /// <summary>
        /// The default Critical Damage Modifier for Ranged damage.
        /// </summary>
        private const float DefaultRanged = 2.2f;

        /// <summary>
        /// The default Critical Damage Modifier for Spell damage.
        /// </summary>
        private const float DefaultSpell = 1.8f;

        /// <summary>
        /// Gets the default Critical Damage Modifier for the specified <see cref="DamageSource"/>.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        /// <returns>
        /// The multiplier that should be applied when an attack has crit.
        /// </returns>
        private static float GetDefault( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return DefaultMelee;

                case DamageSource.Ranged:
                    return DefaultRanged;

                case DamageSource.Spell:
                    return DefaultSpell;

                default:
                    throw new NotImplementedException( source.ToString() );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the damage multiplier value that should get applied
        /// when a Melee attack has been a critical attack.
        /// </summary>
        public float Melee 
        {
            get
            {
                return this.melee;
            }
        }

        /// <summary>
        /// Gets the damage multiplier value that should get applied
        /// when a Ranged attack has been a critical attack.
        /// </summary>
        public float Ranged
        {
            get
            {
                return this.ranged;
            }
        }

        /// <summary>
        /// Gets the damage multiplier value that should get applied
        /// when a Spell attack has been a critical attack.
        /// </summary>
        public float Spell
        {
            get
            {
                return this.spell;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the CriticalDamageBonusContainer class.
        /// </summary>
        internal CriticalDamageBonusContainer()
        {
        }

        /// <summary>
        /// Initializes this CriticalDamageBonusContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The Statable component that should own this CriticalDamageBonusContainer.
        /// </param>
        internal void Initialize( Statable statable )
        {
            this.statable = statable;
        }

        #endregion

        #region [ Methods ]

        #region > Refresh <

        /// <summary>
        /// Refreshes the critical damage bonus for the specified DamageSource.
        /// </summary>
        /// <param name="source">
        /// The source of the damage.
        /// </param>
        internal void Refresh( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.All:
                    this.RefreshActual( DamageSource.Melee );
                    this.RefreshActual( DamageSource.Ranged );
                    this.RefreshActual( DamageSource.Spell );
                    break;

                case DamageSource.None:
                    break;

                default:
                    this.RefreshActual( source );
                    break;
            }
        }

        /// <summary>
        /// Refreshes the critical damage bonus for the specified DamageSource.
        /// </summary>
        /// <param name="source">
        /// The source of the damage.
        /// </param>
        private void RefreshActual( DamageSource source )
        {
            float percentalValue, ratingValue;

            this.statable.AuraList.GetPercentalAndRatingEffectValues(
                Zelda.Status.Damage.CriticalDamageBonusEffect.GetIdentifier( source ),
                Zelda.Status.Damage.CriticalDamageBonusEffect.GetIdentifier( DamageSource.All ),
                out percentalValue,
                out ratingValue
            );

            float multiplier = StatusCalc.GetCriticalDamageBonusModifier(
                GetDefault( source ), 
                percentalValue,
                ratingValue,
                this.statable.Level
            );

            this.Set( source, multiplier );
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the critical damage modifier property for the specified DamageSource.
        /// </summary>
        /// <param name="source">
        /// The DamageSource whose critical damage properties should be set.
        /// </param>
        /// <param name="multiplierValue">
        /// The value a damage value of the given DamageSource that has crit should be multiplied by.
        /// </param>
        private void Set( DamageSource source, float multiplierValue )
        {
            switch( source )
            { 
                case DamageSource.Melee:
                    this.melee = multiplierValue;
                    break;

                case DamageSource.Ranged:
                    this.ranged = multiplierValue;
                    break;

                case DamageSource.Spell:
                    this.spell = multiplierValue;
                    break;
                    
                default:
                    throw new NotImplementedException( source.ToString() );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The critical damage modifiers.
        /// </summary>
        private float melee = DefaultMelee, ranged = DefaultRanged, spell = DefaultSpell;

        /// <summary>
        /// Identifies the <see cref="Statable"/> component that owns this CriticalDamageBonusContainer.
        /// </summary>
        private Statable statable;

        #endregion
    }
}