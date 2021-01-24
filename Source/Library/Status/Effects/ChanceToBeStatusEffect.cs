// <copyright file="ChanceToBeStatusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ChanceToBeStatusEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusEffect that manipulates the chance to be crit/hit/dodged/etc.
    /// This class can't be inherited.
    /// </summary>
    /// <seealso cref="ChanceToStatus"/>
    public sealed class ChanceToBeStatusEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Crit modifier.
        /// </summary>
        public const string IdentifierToBeCrit = "TBCrit";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the To Be Hit modifier.
        /// </summary>
        public const string IdentifierToBeHit = "TBHit";

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                switch( this.StatusType )
                {
                    case ChanceToStatus.Crit:
                        return IdentifierToBeCrit;

                    case ChanceToStatus.Miss:
                        return IdentifierToBeHit;

                    default:
                        throw new System.NotSupportedException();
                }
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ChanceToStatus"/> that is manipulated by the <see cref="ChanceToBeStatusEffect"/>.
        /// </summary>
        public ChanceToStatus StatusType
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
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            
            float value = this.ManipulationType == StatusManipType.Rating ? 
                StatusCalc.ConvertRating( this.Value, statable.Level ) : this.Value;

            value = (float)System.Math.Round( value, 2 );
            var valueString = value >= 0 ? "+" + value.ToString( culture ) : value.ToString( culture );
            
            switch( this.StatusType )
            {
                case ChanceToStatus.Crit:
                    return string.Format( culture, Resources.ChanceToBeCritX, valueString );

                case ChanceToStatus.Miss:
                    return string.Format( culture, Resources.ChanceToBeHitX, valueString );

                case ChanceToStatus.Parry:
                    return string.Format( culture, Resources.ChanceToBeParriedX, valueString );

                default:
                    throw new System.NotSupportedException();
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

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToBeStatusEffect"/> class.
        /// </summary>
        public ChanceToBeStatusEffect()
            : base( 0.0f, StatusManipType.Fixed )
        {
            this.StatusType = ChanceToStatus.Miss;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToBeStatusEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new ChanceToBeStatusEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        /// <param name="statusType">
        /// Indicates what <see cref="ChanceToStatus"/> the new ChanceToBeStatusEffect manipulates.
        /// </param>
        public ChanceToBeStatusEffect( float value, StatusManipType manipulationType, ChanceToStatus statusType )
            : base( value, manipulationType )
        {
            this.StatusType = statusType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="ChanceToBeStatusEffect"/> gets enabled for the given extended-sstatable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnStateChange( user );
        }

        /// <summary>
        /// Called when this <see cref="ChanceToBeStatusEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnStateChange( user );
        }

        /// <summary>
        /// Called when the Enabled/Disabled state of this ChanceToBeStatusEffect has 
        /// changed for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled/disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnStateChange( Statable user )
        {
            user.ChanceToBe.Refresh( this.StatusType );
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
            context.Write( (byte)this.StatusType );
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
            this.StatusType = (ChanceToStatus)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this ChanceToBeStatusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ChanceToBeStatusEffect() { StatusType = this.StatusType };
            
            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}