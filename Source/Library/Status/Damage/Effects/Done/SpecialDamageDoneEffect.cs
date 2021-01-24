// <copyright file="SpecialDamageDoneEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.SpecialDamageDoneEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    using System;

    /// <summary>
    /// Defines a StatusEffect that manipulates the damage done against a specified Race.
    /// </summary>
    public sealed class SpecialDamageDoneEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation name(s) of the SpecialDamageDoneEffect.
        /// </summary>
        private const string
             IdentifierDamagerOverTime = "DmgDone_DoT", 
             IdentifierBleed           = "DmgDone_Bleed",
             IdentifierPoison          = "DmgDone_Poison";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with
        /// the SpecialDamageDoneEffect of the specified <see cref="SpecialDamageType"/>.
        /// </summary>
        /// <param name="damageType">
        /// The damageType whose effect identifier should be returned.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( SpecialDamageType damageType )
        {
            switch( damageType )
            {
                case SpecialDamageType.Bleed:
                    return IdentifierBleed;

                case SpecialDamageType.DamagerOverTime:
                    return IdentifierDamagerOverTime;

                case SpecialDamageType.Poison:
                    return IdentifierPoison;

                default:
                    throw new NotImplementedException( damageType.ToString() );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SpecialDamageType"/> this SpecialDamageDoneEffect
        /// modifies the damage done for.
        /// </summary>
        public SpecialDamageType DamageType
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="SpecialDamageDoneEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.DamageType );
            }
        }

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
            float value = this.Value >= 0.0f ? this.Value : -this.Value;

            return string.Format(
                culture,
                this.GetDescriptionFormatString(),
                this.GetModifiyingString(),
                value.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the format string that is used to create the description.
        /// </summary>
        /// <returns>
        /// A localized string.
        /// </returns>
        private string GetDescriptionFormatString()
        {
            switch( this.ManipulationType )
            {
                case StatusManipType.Fixed:
                    return this.Value >= 0.0f ?  
                        Resources.IncreasesDamageDoneWithXByY :
                        Resources.DecreasesDamageDoneWithXByY;

                case StatusManipType.Percental:
                    return this.Value >= 0.0f ? 
                        Resources.IncreasesDamageDoneWithXByYPercent :
                        Resources.DecreasesDamageDoneWithXByYPercent;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localiized string that is associated with the current
        /// <see cref="DamageType"/>.
        /// </summary>
        /// <returns>
        /// A localized string.
        /// </returns>
        private string GetModifiyingString()
        {
            return LocalizedEnums.Get( this.DamageType ) + ' ';
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDamageDoneEffect"/> class.
        /// </summary>
        public SpecialDamageDoneEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDamageDoneEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value of the new DamageDoneRaceModEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> should be interpreted.
        /// </param>
        /// <param name="damageType">
        /// States what kind of SpecialDamageType the new DamageDoneRaceModEffect modifies.
        /// </param>
        public SpecialDamageDoneEffect( float value, StatusManipType manipulationType, SpecialDamageType damageType )
            : base( value, manipulationType )
        {
            this.DamageType = damageType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="SpecialDamageDoneEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="SpecialDamageDoneEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="SpecialDamageDoneEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="SpecialDamageDoneEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="SpecialDamageDoneEffect"/> gets enabled or disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( ExtendedStatable user )
        {
            user.DamageDone.WithSpecial.Refresh( this.DamageType );
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
            context.Write( (int)this.DamageType );
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
            this.DamageType = (SpecialDamageType)context.ReadInt32();
        }

        /// <summary>
        /// Returns a clone of this SpecialDamageDoneEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new SpecialDamageDoneEffect() { DamageType = this.DamageType };

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
