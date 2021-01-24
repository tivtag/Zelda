// <copyright file="DamageTakenFromSourceEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageTakenFromSourceEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    using System;

    /// <summary>
    /// Defines a StatusEffect that manipulates the damage taken from a specific <see cref="DamageSource"/>.
    /// </summary>
    public sealed class DamageTakenFromSourceEffect : DamageSourceEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSourceEffect for DamageSchool.Physical.
        /// </summary>
        private const string IdentifierMelee = "DmgTkn_Melee";

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSourceEffect for DamageSchool.Magical.
        /// </summary>
        private const string IdentifierRanged = "DmgTkn_Ranged";

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSourceEffect for DamageSchool.Magical.
        /// </summary>
        private const string IdentifierSpell = "DmgTkn_Spell";

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSourceEffect for DamageSchool.All.
        /// </summary>
        private const string IdentifierAll = "DmgTkn_AllSources";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with the specified <see cref="DamageSchool"/>.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return IdentifierMelee;

                case DamageSource.Ranged:
                    return IdentifierRanged;

                case DamageSource.Spell:
                    return IdentifierSpell;

                case DamageSource.All:
                    return IdentifierAll;

                default:
                case DamageSource.None:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the (unique) identifier string this DamageTakenFromSourceEffect is associated with.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.DamageSource );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="DamageDoneWithSourceEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                this.GetDescriptionFormatString(),
                this.GetLocalizedDamageSourceString(),
                Math.Abs( this.Value ).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the format string that is used to create the
        /// descrooption of this DamageTakenFromSourceEffect.
        /// </summary>
        /// <returns>
        /// A localized string that is displayed to the player.
        /// </returns>
        private string GetDescriptionFormatString()
        {
            switch( this.ManipulationType )
            {
                case StatusManipType.Fixed:
                    return this.Value >= 0.0f ?  
                        Resources.IncreasesDamageTakenFromXByY :
                        Resources.DecreasesDamageTakenFromXByY;

                case StatusManipType.Percental:
                    return this.Value >= 0.0f ? 
                        Resources.IncreasesDamageTakenFromXByYPercent :
                        Resources.DecreasesDamageTakenFromXByYPercent;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this StatusEffect is 'bad' for the statable ZeldaEntity.
        /// </summary>
        public override bool IsBad
        {
            get
            {
                return this.Value > 0.0f;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="DamageTakenFromSourceEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenFromSourceEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenFromSourceEffect"/> gets enabled or disabled
        /// for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled or disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageTaken.FromSource.Refresh( this.DamageSource );
        }

        /// <summary>
        /// Returns a clone of this DamageTakenFromSourceEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new DamageTakenFromSourceEffect();

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
