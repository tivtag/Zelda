// <copyright file="DamageTakenFromSchoolEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageTakenFromSchoolEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Defines a StatusEffect that manipulates the damage taken from a specific <see cref="DamageSchool"/>.
    /// </summary>
    public sealed class DamageTakenFromSchoolEffect : DamageSchoolEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSchoolEffect for DamageSchool.Physical.
        /// </summary>
        private const string IdentifierPhysical = "DmgTkn_PhySchool";

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSchoolEffect for DamageSchool.Magical.
        /// </summary>
        private const string IdentifierMagical = "DmgTkn_MagSchool";

        /// <summary>
        /// The (unique) identifier string associated with the DamageTakenFromSchoolEffect for DamageSchool.All.
        /// </summary>
        private const string IdentifierAll = "DmgTkn_AllSchools";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with the specified <see cref="DamageSchool"/>.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( DamageSchool school )
        {
            switch( school )
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
        /// Get the (unique) identifier string this DamageTakenFromSchoolEffect is associated with.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.DamageSchool );
            }
        }

        #endregion

        #region [ Initialization ]
        
        /// <summary>
        /// Initializes a new instance of the DamageTakenFromSchoolEffect class.
        /// </summary>
        public DamageTakenFromSchoolEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DamageTakenFromSchoolEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new DamageTakenFromSchoolEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> of the new DamageTakenFromSchoolEffect should be interpreted.
        /// </param>
        /// <param name="damageSchool">
        /// The DamageSchool that is manipulated by the new DamageTakenFromSchoolEffect.
        /// </param>
        public DamageTakenFromSchoolEffect( float value, StatusManipType manipulationType, DamageSchool damageSchool )
            : base( value, manipulationType, damageSchool )
        {
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="DamageTakenFromSchoolEffect"/>.
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
                    Resources.IncreasesDamageTakenFromXByYPercent,
                    modifyingString,
                    Value.ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.DecreasesDamageTakenFromXByYPercent,
                    modifyingString,
                    (-Value).ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
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
        /// Called when this <see cref="DamageTakenFromSchoolEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenFromSchoolEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenFromSchoolEffect"/> gets enabled or disabled
        /// for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled or disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageTaken.FromSchool.Refresh( this.DamageSchool );
        }

        /// <summary>
        /// Returns a clone of this DamageTakenFromSchoolEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new DamageTakenFromSchoolEffect();

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
