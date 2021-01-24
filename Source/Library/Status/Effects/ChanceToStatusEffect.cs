// <copyright file="ChanceToStatusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ChanceToStatusEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates a <see cref="ChanceToStatus"/> status-value 
    /// of a <see cref="ExtendedStatable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ChanceToStatusEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Crit modifier.
        /// </summary>
        public const string IdentifierCrit = "Crit";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Miss modifier.
        /// </summary>
        public const string IdentifierMiss = "Miss";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Dodge modifier.
        /// </summary>
        public const string IdentifierDodge = "Dod";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Block modifier.
        /// </summary>
        public const string IdentifierBlock = "Block";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Parry modifier.
        /// </summary>
        public const string IdentifierParry = "Parry";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the CritHeal modifier.
        /// </summary>
        public const string IdentifierCritHeal = "CritHeal";
        
        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the CritBlock modifier.
        /// </summary>
        public const string IdentifierCritBlock = "CritBlock";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Pierce modifier.
        /// </summary>
        public const string IdentifierPierce = "Pierce";
        
        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.StatusType );
            }
        }

        /// <summary>
        /// Gets the string that uniquely identifies the <see cref="ChanceToStatusEffect"/>
        /// for the specified ChanceToStatus.
        /// </summary>
        /// <param name="statusType">
        /// The ChanceToStatus to get the identifier for.
        /// </param>
        /// <returns>
        /// An string that can be used an unique identifier.
        /// </returns>
        public static string GetIdentifier( ChanceToStatus statusType )
        {
            switch( statusType )
            {
                case ChanceToStatus.Crit:
                    return IdentifierCrit;

                case ChanceToStatus.CritHeal:
                    return IdentifierCritHeal;
                    
                case ChanceToStatus.Miss:
                    return IdentifierMiss;

                case ChanceToStatus.Dodge:
                    return IdentifierDodge;

                case ChanceToStatus.Parry:
                    return IdentifierParry;

                case ChanceToStatus.Block:
                    return IdentifierBlock;

                case ChanceToStatus.CritBlock:
                    return IdentifierCritBlock;

                case ChanceToStatus.Pierce:
                    return IdentifierPierce;

                default:
                    throw new System.NotImplementedException();
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ChanceToStatus"/> that is manipulated by the <see cref="ChanceToStatusEffect"/>.
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
            var sb = new System.Text.StringBuilder( 25 );
            
            float value = this.StatusType == ChanceToStatus.Miss ? -this.Value : this.Value;
            if( value >= 0 )
                sb.Append( '+' );

            sb.Append( value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

            if( this.ManipulationType == StatusManipType.Fixed )
            {
                sb.Append( ' ' );
                sb.Append( LocalizedEnums.Get( this.StatusType ) );
            }
            else if( this.ManipulationType == StatusManipType.Percental )
            {
                sb.Append( "% " );
                sb.Append( LocalizedEnums.Get( this.StatusType ) );
            }
            else
            {
                float chance = StatusCalc.ConvertRating( value, this.StatusType, statable.Level );

                sb.Append( ' ' );
                sb.Append( LocalizedEnums.GetRating( this.StatusType ) );
                sb.Append( " (" );
                sb.Append( System.Math.Round( chance, 2 ) );
                sb.Append( "%)" );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether this ChanceToStatusEffect is a 'bad' effect.
        /// </summary>
        public override bool IsBad
        {
            get
            {
                if( this.StatusType == ChanceToStatus.Miss )
                {
                    return this.Value > 0.0f;
                }
                else
                {
                    return this.Value < 0.0f;
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToStatusEffect"/> class.
        /// </summary>
        public ChanceToStatusEffect()
            : base()
        {
            this.StatusType = ChanceToStatus.Crit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToStatusEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new ChanceToStatusEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        /// <param name="statusType">
        /// Indicates what <see cref="ChanceToStatus"/> the new ChanceToStatusEffect manipulates.
        /// </param>
        public ChanceToStatusEffect( 
            float           value,
            StatusManipType manipulationType,
            ChanceToStatus  statusType
         )
            : base( value, manipulationType )
        {
            this.StatusType = statusType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="ChanceToStatusEffect"/> gets enabled for the given extended-sstatable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnStateChange( user );
        }

        /// <summary>
        /// Called when this <see cref="ChanceToStatusEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnStateChange( user );
        }

        /// <summary>
        /// Called when the Enabled/Disabled state of this ChanceToStatusEffect has 
        /// changed for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The extended-statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnStateChange( Statable user )
        {
            ((ExtendedStatable)user).ChanceTo.Refresh( this.StatusType );
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
        /// Returns a clone of this ChanceToStatusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ChanceToStatusEffect() { StatusType = this.StatusType };
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
