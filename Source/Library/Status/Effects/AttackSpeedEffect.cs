// <copyright file="AttackSpeedEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.AttackSpeedEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using System.Globalization;
    using Zelda.Attacks;
    
    /// <summary>
    /// Defines a StatusEffect which manipulates melee/ranged Attack Speed. 
    /// This class can't be inherited.
    /// </summary>
    public sealed class AttackSpeedEffect : StatusValueEffect
    {
        #region [ Identification ]
        
        /// <summary>
        /// The string that uniquely identifies this AttackSpeedEffect.
        /// </summary>
        public const string
            IdentifierMelee = "AtkSpdM",
            IdentifierRanged = "AtkSpdR",
            IdentifierAll = "AtkSpdA";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="AttackType"/> whos attack-speed is modified
        /// by this <see cref="AttackSpeedEffect"/>.
        /// </summary>
        public AttackType AttackType
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                switch( AttackType )
                {
                    case AttackType.All:
                        return IdentifierAll;

                    case AttackType.Melee:
                        return IdentifierMelee;

                    case AttackType.Ranged:
                        return IdentifierRanged;

                    default:
                        throw new NotImplementedException();
                }
            }
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
            string stringAttackType = this.AttackType == AttackType.All ? 
                string.Empty : " " + LocalizedEnums.Get( this.AttackType );

            if( this.ManipulationType == StatusManipType.Rating )
            {
                if( this.Value >= 0.0f )
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.IncreasesAttackSpeedRatingWithXByY,
                        Value.ToString( CultureInfo.CurrentCulture ),
                        stringAttackType,
                        Math.Round( StatusCalc.ConvertAttackSpeedRating( this.Value, statable.Level ), 2 ).ToString( CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.DecreasesAttackSpeedRatingWithXByY,
                        (-Value).ToString( System.Globalization.CultureInfo.CurrentCulture ),
                        stringAttackType,
                        Math.Round( StatusCalc.ConvertAttackSpeedRating( this.Value, statable.Level ), 2 ).ToString( CultureInfo.CurrentCulture )
                   );
                }
            }
            else
            {
                if( this.Value >= 0.0f )
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.IncreasesAttackSpeedWithXByY,
                        stringAttackType,
                        Value.ToString( System.Globalization.CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.DecreasesAttackSpeedWithXByY,
                        stringAttackType,
                        (-Value).ToString( System.Globalization.CultureInfo.CurrentCulture )
                   );
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackSpeedEffect"/> class.
        /// </summary>
        public AttackSpeedEffect()
            : base( 0.0f, StatusManipType.Percental )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackSpeedEffect"/> class.
        /// </summary>
        /// <param name="attackType">
        /// States what kind of attack speed is manibulated by the new AttackSpeedEffect.
        /// </param>
        /// <param name="value"> The manipulation value. 
        /// It's depending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="StatusValueEffect.Value"/> of the new AttackSpeedEffect should be interpreted.
        /// </param>
        public AttackSpeedEffect( AttackType attackType, float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
            this.AttackType = attackType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="AttackSpeedEffect"/> gets enabled for the given <see cref="ExtendedStatable"/> Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="AttackSpeedEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            var extendedStatable = (ExtendedStatable)user;

            switch( AttackType )
            {
                case AttackType.Melee:
                    extendedStatable.Refresh_AttackSpeedMelee();
                    break;

                case AttackType.Ranged:
                    extendedStatable.Refresh_AttackSpeedRanged();
                    break;

                case AttackType.All:
                    extendedStatable.Refresh_AttackSpeedMelee();
                    extendedStatable.Refresh_AttackSpeedRanged();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Called when this <see cref="AttackSpeedEffect"/> gets disabled for the given <see cref="ExtendedStatable"/> Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="AttackSpeedEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            var extendedStatable = (ExtendedStatable)user;

            switch( AttackType )
            {
                case AttackType.Melee:
                    extendedStatable.Refresh_AttackSpeedMelee();
                    break;

                case AttackType.Ranged:
                    extendedStatable.Refresh_AttackSpeedRanged();
                    break;

                case AttackType.All:
                    extendedStatable.Refresh_AttackSpeedMelee();
                    extendedStatable.Refresh_AttackSpeedRanged();
                    break;

                default:
                    throw new NotImplementedException();
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

            // Write Data
            context.Write( (byte)this.AttackType );
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
            this.AttackType = (AttackType)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this AttackSpeedEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new AttackSpeedEffect();

            this.SetupClone( clone );
            clone.AttackType = this.AttackType;

            return clone;
        }

        #endregion
    }
}
