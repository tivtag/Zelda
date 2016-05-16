using System.Globalization;

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates a <see cref="Stat"/>
    /// of a <see cref="ExtendedStatable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class StatEffect : StatusValueEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets what <see cref="Stat"/> is manipulated by this <see cref="StatEffect"/>.
        /// </summary>
        public Stat Stat
        {
            get { return stat; }
            set { stat = value; }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                if( this.ManipulationType == StatusManipType.Fixed )
                {
                    if( Value >= 0 )
                    {
                        return string.Format( 
                            CultureInfo.CurrentUICulture,
                            LocDescStr_Positive,
                            StatusCalc.GetLocalizedString( stat ),
                            Value.ToString( CultureInfo.CurrentUICulture )
                         );
                    }
                    else
                    {
                        return string.Format(
                            CultureInfo.CurrentUICulture,
                            LocDescStr_Negative,
                            StatusCalc.GetLocalizedString( stat ),
                            (-Value).ToString( CultureInfo.CurrentUICulture ) );
                    }
                }
                else
                {
                    if( Value >= 0 )
                    {
                        return string.Format(
                            CultureInfo.CurrentUICulture,
                            LocDescStr_PositivePercentage,
                            StatusCalc.GetLocalizedString( stat ),
                            Value.ToString( CultureInfo.CurrentUICulture )
                        );
                    }
                    else
                    {
                        return string.Format(
                            CultureInfo.CurrentUICulture,
                            LocDescStr_NegativePercentage, 
                            StatusCalc.GetLocalizedString( stat ),
                            (-Value).ToString( CultureInfo.CurrentUICulture )
                        );
                    }
                }
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get { return stat.ToString(); }
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
        /// <param name="stat">
        /// The <see cref="Stat"/> the new StatEffect manipulates.
        /// </param>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public StatEffect( Stat stat, float value, StatusManipType manipulationType )
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
            switch( stat )
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

            user.Equipment.CheckRequirementsFulfilled();
        }

        /// <summary>
        /// Serializes/Writes this StatEffect to the System.IO.BinaryWriter.
        /// </summary>
        /// <param name="writer">The System.IO.BinaryWriter to write the StatEffect to.</param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            // Write Data:
            writer.Write( (byte)this.stat );
        }

        /// <summary>
        /// Deserializes/Reads this StatEffect using the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="reader">The System.IO.BinaryReader to read from.</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            // Read Data:
            this.stat = (Stat)reader.ReadByte();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The stat that is manipulated by this StatEffect.
        /// </summary>
        private Stat stat;

        private static readonly string LocDescStr_Positive           = Resources.ED_StatPos;
        private static readonly string LocDescStr_Negative           = Resources.ED_StatNeg;
        private static readonly string LocDescStr_PositivePercentage = Resources.ED_StatPosPercentage;
        private static readonly string LocDescStr_NegativePercentage = Resources.ED_StatNegPercentage;

        #endregion
    }
}
