// <copyright file="LifeManaRegenEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.LifeManaRegenEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{   
    /// <summary>
    /// Defines a <see cref="StatusEffect"/> which manipulates how
    /// well a <see cref="ExtendedStatable"/> ZeldaEntity regenerates Life/Mana.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LifeManaRegenEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this LifeManaRegenEffect.
        /// </summary>
        public const string IdentifierLife = "L_Regen",
                            IdentifierMana = "M_Regen",
                            IdentifierBoth = "LM_Regen";

        /// <summary>
        /// Gets an unique string that represents what this LifeManaRegenEffect manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                switch( this.PowerType )
                {
                    case LifeMana.Life:
                        return IdentifierLife;

                    case LifeMana.Mana:
                        return IdentifierMana;
                        
                    case LifeMana.Both:
                        return IdentifierBoth;
                       
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        #endregion

        #region [ Properties ]

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
            var sb = new System.Text.StringBuilder( 20 );

            if( this.Value >= 0.0f )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );
            sb.Append( this.ManipulationType == StatusManipType.Fixed ? " " : "% " );
            sb.Append( LocalizedEnums.GetRegen( this.PowerType ) );

            return sb.ToString();
        }

        /// <summary>
        /// Gets or sets what kind of Power the <see cref="LifeManaRegenEffect"/> manipulates.
        /// </summary>
        public LifeMana PowerType
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeManaRegenEffect"/> class.
        /// </summary>
        public LifeManaRegenEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeManaRegenEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new LifeManaRegenEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="StatusValueEffect.Value"/> of the new <see cref="StatusValueEffect"/>
        /// should be interpreted.
        /// </param>
        /// <param name="powerType">
        /// The power type the new <see cref="LifeManaRegenEffect"/> modifies.
        /// </param>
        public LifeManaRegenEffect( float value, StatusManipType manipulationType, LifeMana powerType )
           : base( value, manipulationType )
        {
            this.PowerType = powerType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled/disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled/disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            var statable = (ExtendedStatable)user;

            switch( PowerType )
            {
                case LifeMana.Life:
                    statable.Refresh_LifeRegen();
                    break;

                case LifeMana.Mana:
                    statable.Refresh_ManaRegen();
                    break;

                case LifeMana.Both:
                    statable.Refresh_LifeRegen();
                    statable.Refresh_ManaRegen();
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

            context.Write( (byte)this.PowerType );
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

            this.PowerType = (LifeMana)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this LifeManaRegenEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new LifeManaRegenEffect();

            this.SetupClone( clone );
            clone.PowerType = this.PowerType;

            return clone;
        }

        #endregion

        #region [ Fields ]
        #endregion
    }
}
