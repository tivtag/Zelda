// <copyright file="SpecialDamageDoneContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.SpecialDamageDoneContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage.Containers
{
    using System;

    /// <summary>
    /// Encapsulates the damage properties that are applied
    /// on damage done of a <see cref="SpecialDamageType"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SpecialDamageDoneContainer
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SpecialDamageDoneContainer class.
        /// </summary>
        internal SpecialDamageDoneContainer()
        {
        }

        /// <summary>
        /// Initializes this SpecialDamageDoneContainer instance.
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
        /// Applies the damage multiplier for the specified <see cref="SpecialDamageType"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <param name="damageType">
        /// The damageType of the target of the damage.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int Apply( int damage, SpecialDamageType damageType )
        {
            return (int)(damage * this.Get( damageType ));
        }

        /// <summary>
        /// Applies the fixed damage modifier for the specified <see cref="SpecialDamageType"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <param name="damageType">
        /// The damageType of the target of the damage.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyFixed( int damage, SpecialDamageType damageType )
        {
            return damage + this.GetFixed( damageType );
        }

        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the damage properties for the specified SpecialDamageType.
        /// </summary>
        /// <param name="damageType">
        /// The SpecialDamageType to refresh.
        /// </param>
        internal void Refresh( SpecialDamageType damageType )
        {
            float fixedValue, multiplierValue;

            this.auraList.GetEffectValues(
                SpecialDamageDoneEffect.GetIdentifier( damageType ),
                out fixedValue,
                out multiplierValue
            );

            this.Set( damageType, (int)fixedValue, multiplierValue );
        }

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the damage multiplier value for the given SpecialDamageType.
        /// </summary>
        /// <param name="damageType">
        /// The damageType of the target of the damage.
        /// </param>
        /// <returns>
        /// The value a damage value that is directed against the specified Race should be multiplied by.
        /// </returns>
        public float Get( SpecialDamageType damageType )
        {
            switch( damageType )
            {
                case SpecialDamageType.DamagerOverTime:
                    return this.damageOverTime;

                case SpecialDamageType.Bleed:
                    return this.bleed;

                case SpecialDamageType.Poison:
                    return this.poison;

                case SpecialDamageType.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "damageType" );
            }
        }

        /// <summary>
        /// Gets the fixed damage modifier value for the given SpecialDamageType.
        /// </summary>
        /// <param name="damageType">
        /// The damageType of the target of the damage.
        /// </param>
        /// <returns>
        /// The value that should be added to damage value that is directed against the specified Race.
        /// </returns>
        public int GetFixed( SpecialDamageType damageType )
        {
            switch( damageType )
            {
                case SpecialDamageType.DamagerOverTime:
                    return this.fixedDamageOverTime;

                case SpecialDamageType.Bleed:
                    return this.fixedBleed;

                case SpecialDamageType.Poison:
                    return this.fixedPoison;

                case SpecialDamageType.None:
                    return 0;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "damageType" );
            }
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the modifier properties for the specified SpecialDamageType.
        /// </summary>
        /// <param name="damageType">
        /// The damageType whose properties should be set.
        /// </param>
        /// <param name="fixedValue">
        /// The value that should be added to a damage value of the given SpecialDamageType.
        /// </param>
        /// <param name="multiplierValue">
        /// The value a damage value of the given SpecialDamageType should be multiplied by.
        /// </param>
        private void Set( SpecialDamageType damageType, int fixedValue, float multiplierValue )
        {
            switch( damageType )
            {
                case SpecialDamageType.DamagerOverTime:
                    this.damageOverTime = multiplierValue;
                    this.fixedDamageOverTime = fixedValue;
                    break;

                case SpecialDamageType.Bleed:
                    this.bleed = multiplierValue;
                    this.fixedBleed = fixedValue;
                    break;

                case SpecialDamageType.Poison:
                    this.poison = multiplierValue;
                    this.fixedPoison = fixedValue;
                    break;

                default:
                    throw new NotImplementedException( damageType.ToString() );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The multiplier values that are applied against enemies of a specific Race.
        /// </summary>
        private float damageOverTime = 1.0f, bleed = 1.0f, poison = 1.0f;

        /// <summary>
        /// The fixed modifier values that are applied against enemies of a specific Race.
        /// </summary>
        private int fixedDamageOverTime, fixedBleed, fixedPoison;

        /// <summary>
        /// Identifies the AuraList of the statable entity that owns this SpecialDamageDoneContainer.
        /// </summary>
        private AuraList auraList;

        #endregion
    }
}