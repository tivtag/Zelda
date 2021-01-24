// <copyright file="LifeManaEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.LifeManaEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates total life/mana values of an ExtendedStatable ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LifeManaEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this LifeManaEffect.
        /// </summary>
        public const string IdentifierLife = "Life",
                            IdentifierMana = "Mana";

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="LifeManaEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                if( this.PowerType == LifeMana.Life )
                    return IdentifierLife;
                else
                    return IdentifierMana;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets whether this LifeManaEffect manipulates Mana or Life.
        /// </summary>
        public LifeMana PowerType
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
            var sb = new System.Text.StringBuilder( 25 );

            if( this.Value >= 0.0f )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

            if( this.ManipulationType == StatusManipType.Percental )
                sb.Append( "% " );
            else
                sb.Append( ' ' );

            sb.Append( this.PowerType == LifeMana.Life ? Resources.Life : Resources.Mana );

            return sb.ToString();
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeManaEffect"/> class.
        /// </summary>
        public LifeManaEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeManaEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new LifeManaEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        /// <param name="powerType">
        /// States whether the new LifeManaEffect manipulates Life or Mana.
        /// </param>
        public LifeManaEffect( float value, StatusManipType manipulationType, LifeMana powerType )
            : base( value, manipulationType )
        {
            this.PowerType = powerType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets enabled or disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        private void OnChanged( Statable user )
        {
            switch( this.PowerType )
            {
                case LifeMana.Life:
                    user.Refresh_TotalLife();
                    break;
                case LifeMana.Mana:
                    ((ExtendedStatable)user).Refresh_TotalMana();
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

            // Read Data:
            this.PowerType = (LifeMana)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this LifeManaEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new LifeManaEffect() { PowerType = this.PowerType };

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}