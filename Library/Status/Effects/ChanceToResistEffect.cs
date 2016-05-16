// <copyright file="ChanceToResistEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ChanceToResistEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the change of a <see cref="Statable"/> ZeldaEntity
    /// to resist a specific school of magic.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ChanceToResistEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Fire resist value.
        /// </summary>
        private const string ManipNameFire = "ResFire";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Light resist value.
        /// </summary>
        private const string ManipNameLight = "ResLight";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Shadow resist value.
        /// </summary>
        private const string ManipNameShadow = "ResShadow";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Nature resist value.
        /// </summary>
        private const string ManipNameNature = "ResNature";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Water resist value.
        /// </summary>
        private const string ManipNameWater = "ResWater";        
        
        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulates all resist values at once.
        /// </summary>
        public const string ManipNameAll = "ResAll";

        /// <summary>
        /// Gets the manipulation identifier string the ChanceToResistEffect class
        /// uses for the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of the attack or spell.
        /// </param>
        /// <returns>
        /// The string that uniquely identifies the ChanceToResistEffect for the given ElementalSchool.
        /// </returns>
        internal static string GetManipName( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return ManipNameFire;

                case ElementalSchool.Water:
                    return ManipNameWater;

                case ElementalSchool.Light:
                    return ManipNameLight;

                case ElementalSchool.Shadow:
                    return ManipNameShadow;

                case ElementalSchool.Nature:
                    return ManipNameNature;

                case ElementalSchool.All:
                    return ManipNameAll;

                default:
                    throw new System.NotImplementedException();
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ElementalSchool"/> that is manipulated by this <see cref="ChanceToResistEffect"/>.
        /// </summary>
        public ElementalSchool ElementalSchool
        {
            get { return this.element; }
            set { this.element = value; }
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
            var sb      = new System.Text.StringBuilder( 25 );
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            if( this.Value >= 0 )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( culture ) );   

            if( this.ManipulationType == StatusManipType.Fixed )
            {
                sb.Append( ' ' );
                sb.Append( LocalizedEnums.GetResist( element ) );
                sb.Append( "%" );
            }
            else if( this.ManipulationType == StatusManipType.Rating )
            {
                float converted = StatusCalc.ConvertResistRating( this.Value, statable.Level );

                sb.Append( ' ' );
                sb.Append( LocalizedEnums.GetResist( element ) );
                sb.Append( ' ' );
                sb.Append( Resources.Rating );
                sb.Append( " (" );
                sb.Append( System.Math.Round( converted, 2 ).ToString( culture ) );
                sb.Append( "%)" );
            }
            else
            {             
                sb.Append( "% " );
                sb.Append( LocalizedEnums.GetResist( element ) );
            }

            return sb.ToString();
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                switch( this.element )
                {
                    case ElementalSchool.Fire:
                        return ManipNameFire;

                    case ElementalSchool.Light:
                        return ManipNameLight;

                    case ElementalSchool.Nature:
                        return ManipNameNature;

                    case ElementalSchool.Shadow:
                        return ManipNameShadow;

                    case ElementalSchool.Water:
                        return ManipNameWater;

                    case ElementalSchool.All:
                        return ManipNameAll;

                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToResistEffect"/> class.
        /// </summary>
        public ChanceToResistEffect()
            : this( 0.0f, StatusManipType.Fixed, ElementalSchool.Fire )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToResistEffect"/> class.
        /// </summary>
        /// <param name="value"> 
        /// The value of the new ChanceToResistEffect.
        /// </param>
        /// <param name="manipulationType">
        /// The manipulation type of the new ChanceToResistEffect.
        /// </param>
        /// <param name="element">
        /// Indicates what <see cref="ElementalSchool"/> the new ChanceToResistEffect manipulates.
        /// </param>
        public ChanceToResistEffect( float value, StatusManipType manipulationType, ElementalSchool element )
            : base( value, manipulationType )
        {
            this.element = element;
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
        /// changed for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnStateChange( Statable user )
        {
            user.Resistances.Refresh( this.element );
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
            context.Write( (byte)this.element );
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
            this.element = (ElementalSchool)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this ChanceToStatusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ChanceToResistEffect();

            this.SetupClone( clone );
            clone.element = this.element;

            return clone;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The <see cref="ElementalSchool"/> this ChanceToResistEffect manipulates.
        /// </summary>
        private ElementalSchool element;

        #endregion
    }
}
