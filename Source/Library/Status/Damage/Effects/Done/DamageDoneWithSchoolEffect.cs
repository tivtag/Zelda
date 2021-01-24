// <copyright file="DamageDoneWithSchoolEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageDoneWithSchoolEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Defines a StatusEffect that manipulates the damage taken from a specific <see cref="DamageSchool"/>.
    /// </summary>
    public sealed class DamageDoneWithSchoolEffect : DamageSchoolEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The (unique) identifier string associated with the DamageDoneWithSchoolEffect for DamageSchool.Physical.
        /// </summary>
        private const string IdentifierPhysical = "Dmg_Physical";

        /// <summary>
        /// The (unique) identifier string associated with the DamageDoneWithSchoolEffect for DamageSchool.Magical.
        /// </summary>
        private const string IdentifierMagical = "Dmg_Magical";

        /// <summary>
        /// The (unique) identifier string associated with the DamageDoneWithSchoolEffect for DamageSchool.All.
        /// </summary>
        private const string IdentifierAll = "Dmg_All";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with the specified <see cref="DamageSchool"/>.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( DamageSchool source )
        {
            switch( source )
            {
                case DamageSchool.Physical:
                    return IdentifierPhysical;

                case DamageSchool.Magical:
                    return IdentifierMagical;

                case DamageSchool.All:
                    return IdentifierAll;

                default:
                case DamageSchool.None:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the (unique) identifier string this DamageDoneWithSchoolEffect is associated with.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.DamageSchool );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="DamageDoneWithSchoolEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            var modifyingString = this.GetLocalizedModifyingString();

            if( this.Value >= 0.0f )
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.IncreasesDamageDoneWithXByYPercent,
                    modifyingString,
                    Value.ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.DecreasesDamageDoneWithXByYPercent,
                    modifyingString,
                    (-Value).ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="DamageDoneWithSchoolEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneWithSchoolEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneWithSchoolEffect"/> gets enabled or disabled
        /// for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled or disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageDone.WithSchool.Refresh( this.DamageSchool );
        }

        /// <summary>
        /// Returns a clone of this DamageDoneWithSchoolEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new DamageDoneWithSchoolEffect();

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
