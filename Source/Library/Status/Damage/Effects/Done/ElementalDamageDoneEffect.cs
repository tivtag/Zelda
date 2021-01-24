// <copyright file="ElementalDamageDoneEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.ElementalDamageDoneEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Defines a StatusEffect that manipulates the damage done with a specific <see cref="DamageSchool"/>.
    /// </summary>
    public sealed class ElementalDamageDoneEffect : ElementalSchoolEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageEffect for ElementalSchool.Fire.
        /// </summary>
        private const string IdentifierFire = "Dmg_Fire";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageEffect for ElementalSchool.Water.
        /// </summary>
        private const string IdentifierWater = "Dmg_Water";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageEffect for ElementalSchool.Light.
        /// </summary>
        private const string IdentifierLight = "Dmg_Light";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageEffect for ElementalSchool.Shadow.
        /// </summary>
        private const string IdentifierShadow = "Dmg_Shadow";

        /// <summary>
        /// The (unique) identifier string associated with the ElementalDamageEffect for ElementalSchool.Nature.
        /// </summary>
        private const string IdentifierNature = "Dmg_Nature";

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
        /// Get the (unique) identifier string this ElementalDamageDoneEffect is associated with.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.ElementalSchool );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ElementalDamageDoneEffect class.
        /// </summary>
        public ElementalDamageDoneEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementalDamageDoneEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new ElementalDamageDoneEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> should be interpreted.
        /// </param>
        /// <param name="elementalSchool">
        /// The ElementalSchool that is manipulated by the new ElementalDamageDoneEffect.
        /// </param>
        public ElementalDamageDoneEffect( float value, StatusManipType manipulationType, ElementalSchool elementalSchool )
            : base( value, manipulationType, elementalSchool )
        {
        }
        
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="ElementalDamageDoneEffect"/>.
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

            if( this.ManipulationType == StatusManipType.Percental )
            {
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
            else
            {
                return "Error ..! ElementalDamageDoneEffect only % supported.";                
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="ElementalDamageDoneEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="ElementalDamageDoneEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="ElementalDamageDoneEffect"/> gets enabled or disabled
        /// for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled or disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageDone.WithElement.Refresh( this.ElementalSchool );
        }

        /// <summary>
        /// Returns a clone of this ElementalDamageDoneEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ElementalDamageDoneEffect();

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
