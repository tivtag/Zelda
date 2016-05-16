// <copyright file="WeaponDamageTypeBasedEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.WeaponDamageTypeBasedEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using Zelda.Items;

    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates how much damage
    /// the <see cref="Statable"/> ZeldaEntity does with a specific type of weapon.
    /// This class can't be inherited.
    /// </summary>
    /// <seealso cref="WeaponType"/>
    public sealed class WeaponDamageTypeBasedEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The strings that uniquely identify the WeaponDamageTypeBasedEffect.
        /// </summary>
        private const string IdentifierDagger         = "Dagger",
                             IdentifierOneHandedSword = "S1H",
                             IdentifierTwoHandedSword = "S2H",
                             IdentifierBow            = "Bow",
                             IdentifierCrossbow       = "Xbow";
        
        /// <summary>
        /// Gets the manpulation name that identifies the WeaponDamageTypeBasedEffect
        /// for the given WeaponType.
        /// </summary>
        /// <param name="weaponType">
        /// The input WeaponType.
        /// </param>
        /// <returns>
        /// The unique manipulation string.
        /// </returns>
        public static string GetIdentifier( WeaponType weaponType )
        {
            switch( weaponType )
            {
                case WeaponType.Dagger:
                    return IdentifierDagger;

                case WeaponType.OneHandedSword:
                    return IdentifierOneHandedSword;

                case WeaponType.TwoHandedSword:
                    return IdentifierTwoHandedSword;

                case WeaponType.Bow:
                    return IdentifierBow;

                case WeaponType.Crossbow:
                    return IdentifierCrossbow;

                default:
                    return string.Empty;
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="WeaponDamageTypeBasedEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.WeaponType );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what <see cref="WeaponType"/> this <see cref="WeaponDamageTypeBasedEffect"/> modifies.
        /// </summary>
        public WeaponType WeaponType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            string modifyingString =  LocalizedEnums.Get( this.WeaponType ) + " ";

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
                if( this.Value >= 0.0f )
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.IncreasesDamageDoneWithXByY,
                        modifyingString,
                        Value.ToString( System.Globalization.CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.DecreasesDamageDoneWithXByY,
                        modifyingString,
                        (-Value).ToString( System.Globalization.CultureInfo.CurrentCulture )
                   );
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponDamageTypeBasedEffect"/> class.
        /// </summary>
        public WeaponDamageTypeBasedEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponDamageTypeBasedEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The modification value of the new WeaponDamageTypeBasedEffect.
        /// </param>
        /// <param name="manipulationType">
        /// The manipulation type of the new WeaponDamageTypeBasedEffect.
        /// </param>
        /// <param name="weaponType">
        /// States what kind of weapon the new WeaponDamageTypeBasedEffect modifies.
        /// </param>
        public WeaponDamageTypeBasedEffect( float value, StatusManipType manipulationType, WeaponType weaponType )
            : base( value, manipulationType )
        {
            this.WeaponType = weaponType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="WeaponDamageTypeBasedEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="WeaponDamageTypeBasedEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="WeaponDamageTypeBasedEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="WeaponDamageTypeBasedEffect"/> gets enabled or disabled for the given Statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( ExtendedStatable user )
        {
            switch( this.WeaponType )
            {
                case WeaponType.Dagger:
                case WeaponType.OneHandedAxe:
                case WeaponType.OneHandedMace:
                case WeaponType.OneHandedSword:
                case WeaponType.TwoHandedAxe:
                case WeaponType.TwoHandedMace:
                case WeaponType.TwoHandedSword:
                    user.Refresh_DamageMelee();
                    break;

                case WeaponType.Bow:
                case WeaponType.Crossbow:
                    user.Refresh_DamageRanged();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            // Write Data:
            context.Write( (byte)this.WeaponType );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            // Read Data:
            this.WeaponType = (WeaponType)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this WeaponDamageTypeBasedEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new WeaponDamageTypeBasedEffect() {
                WeaponType = this.WeaponType
            };

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}