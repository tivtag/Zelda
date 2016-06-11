// <copyright file="StatEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.StatEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System.Globalization;

    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates a <see cref="Stat"/>
    /// of a <see cref="ExtendedStatable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class StatEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Strength stat.
        /// </summary>
        public const string IdentifierStrength = "Str";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Dexterity stat.
        /// </summary>
        public const string IdentifierDexterity = "Dex";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Agility stat.
        /// </summary>
        public const string IdentifierAgility = "Agi";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Vitality stat.
        /// </summary>
        public const string IdentifierVitality = "Vit";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Intelligence stat.
        /// </summary>
        public const string IdentifierIntelligence = "Int";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the Luck stat.
        /// </summary>
        public const string IdentifierLuck = "Luk";
        
        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                switch( this.stat )
                {
                    case Stat.Strength:
                        return IdentifierStrength;

                    case Stat.Agility:
                        return IdentifierAgility;

                    case Stat.Dexterity:
                        return IdentifierDexterity;

                    case Stat.Vitality:
                        return IdentifierVitality;

                    case Stat.Intelligence:
                        return IdentifierIntelligence;

                    case Stat.Luck:
                        return IdentifierLuck;

                    default:
                        return string.Empty;
                }
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what <see cref="Stat"/> is manipulated by this <see cref="StatEffect"/>.
        /// </summary>
        public Stat Stat
        {
            get { return this.stat; }
            set { this.stat = value; }
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
            if( this.ManipulationType == StatusManipType.Fixed )
            {
                if( this.Value >= 0 )
                {
                    return string.Format( 
                        CultureInfo.CurrentCulture,
                        FormatPositive,
                        LocalizedEnums.Get( this.stat ),
                        this.Value.ToString( CultureInfo.CurrentCulture )
                     );
                }
                else
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        FormatNegative,
                        LocalizedEnums.Get( this.stat ),
                        (-this.Value).ToString( CultureInfo.CurrentCulture ) );
                }
            }
            else
            {
                if( this.Value >= 0 )
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        FormatPositivePercentage,
                        LocalizedEnums.Get( this.stat ),
                        this.Value.ToString( CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        FormatNegativePercentage,
                        LocalizedEnums.Get( this.stat ),
                        (-this.Value).ToString( CultureInfo.CurrentCulture )
                    );
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatEffect"/> class.
        /// </summary>
        public StatEffect()
            : base()
        {
            this.stat             = Stat.Strength;
            this.ManipulationType = StatusManipType.Percental;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatEffect"/> class.
        /// </summary>
        /// <param name="value"> 
        /// The value of the new StatEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the value of the new StatEffect should be interpreted.
        /// </param>
        /// <param name="stat">
        /// The <see cref="Stat"/> the new StatEffect manipulates.
        /// </param>
        public StatEffect( float value, StatusManipType manipulationType, Stat stat )
            : base( value, manipulationType )
        {
            this.stat = stat;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatEffect"/> gets enabled for the given extended-sstatable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            OnStateChange( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="StatEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnStateChange( (ExtendedStatable)user );
        }
        
        /// <summary>
        /// Called when the Enabled/Disabled state of this StatEffect has changed for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The extended-statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnStateChange( ExtendedStatable user )
        {
            switch( this.stat )
            {
                case Stat.Strength:
                    user.Refresh_Strength();
                    break;
                case Stat.Dexterity:
                    user.Refresh_Dexterity();
                    break;
                case Stat.Agility:
                    user.Refresh_Agility();
                    break;
                case Stat.Vitality:
                    user.Refresh_Vitality();
                    break;
                case Stat.Intelligence:
                    user.Refresh_Intelligence();
                    break;
                case Stat.Luck:
                    user.Refresh_Luck();
                    break;
                default:
                    break;
            }

            user.Equipment.NotifyRequirementsRecheckNeeded();
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
            context.Write( (byte)this.stat );
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
            this.stat = (Stat)context.ReadByte();
        }
        
        /// <summary>
        /// Returns a clone of this StatEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new StatEffect();
            
            this.SetupClone( clone );
            clone.stat = this.stat;

            return clone;
        }
        
        #endregion

        #region [ Fields ]

        /// <summary>
        /// The stat that is manipulated by this StatEffect.
        /// </summary>
        private Stat stat;

        #region > Cached Strings <

        /// <summary>
        /// Caches the <see cref="Resources.ED_Pos"/> string.
        /// </summary>
        private static readonly string FormatPositive = Resources.IncXByY;

        /// <summary>
        /// Caches the <see cref="Resources.ED_Neg"/> string.
        /// </summary>
        private static readonly string FormatNegative = Resources.DecXByY;

        /// <summary>
        /// Caches the <see cref="Resources.ED_PosPercentage"/> string.
        /// </summary>
        private static readonly string FormatPositivePercentage = Resources.IncXByYPercent;

        /// <summary>
        /// Caches the <see cref="Resources.ED_NegPercentage"/> string.
        /// </summary>
        private static readonly string FormatNegativePercentage = Resources.DecXByYPercent;

        #endregion

        #endregion
    }
}
