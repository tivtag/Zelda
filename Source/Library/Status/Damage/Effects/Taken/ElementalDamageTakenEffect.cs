// <copyright file="ElementalDamageTakenEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.ElementalDamageTakenEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Defines a StatusEffect that manipulates the damage taken from a specific <see cref="DamageSchool"/>.
    /// </summary>
    public sealed class ElementalDamageTakenEffect : ElementalSchoolEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageTakenEffect for ElementalSchool.Fire.
        /// </summary>
        private const string IdentifierFire = "DmgTkn_Fire";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageTakenEffect for ElementalSchool.Water.
        /// </summary>
        private const string IdentifierWater = "DmgTkn_Water";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageTakenEffect for ElementalSchool.Light.
        /// </summary>
        private const string IdentifierLight = "DmgTkn_Light";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageTakenEffect for ElementalSchool.Shadow.
        /// </summary>
        private const string IdentifierShadow = "DmgTkn_Shadow";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageTakenEffect for ElementalSchool.Nature.
        /// </summary>
        private const string IdentifierNature = "DmgTkn_Nature";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with the specified <see cref="DamageSchool"/>.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return IdentifierFire;

                case ElementalSchool.Water:
                    return IdentifierWater;

                case ElementalSchool.Light:
                    return IdentifierLight;

                case ElementalSchool.Shadow:
                    return IdentifierShadow;

                case ElementalSchool.Nature:
                    return IdentifierNature;

                default:
                case ElementalSchool.None:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the (unique) identifier string this ElementalDamageTakenEffect is associated with.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.ElementalSchool );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="ElementalDamageTakenEffect"/>.
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

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ElementalDamageTakenEffect class.
        /// </summary>
        public ElementalDamageTakenEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementalDamageTakenEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new ElementalDamageTakenEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> of the new ElementalDamageTakenEffect should be interpreted.
        /// </param>
        /// <param name="elementalSchool">
        /// The ElementalSchool that is manipulated by the new ElementalDamageTakenEffect.
        /// </param>
        public ElementalDamageTakenEffect( float value, StatusManipType manipulationType, ElementalSchool elementalSchool )
            : base( value, manipulationType, elementalSchool )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="ElementalDamageTakenEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="ElementalDamageTakenEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="ElementalDamageTakenEffect"/> gets enabled or disabled
        /// for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled or disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageTaken.FromElement.Refresh( this.ElementalSchool );
        }

        /// <summary>
        /// Returns a clone of this ElementalDamageTakenEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ElementalDamageTakenEffect();

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
