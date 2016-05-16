using System;

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates how much damage
    /// the <see cref="ExtendedStatable"/> ZeldaEntity does against a specific type of enemies.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageDoneRaceModEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation name(s) of the DamageDoneRaceModEffect.
        /// </summary>
        public const string ManipNameUndead    = "DmgDoneUndead",
                            ManipNameSlime     = "DmgDoneSlime",
                            ManipNamePlant     = "DmgDonePlant",
                            ManipNameHuman     = "DmgDoneHuman",
                            ManipNameFairy     = "DmgDoneFairy",
                            ManipNameDemiHuman = "DmgDoneDemiHuman",
                            ManipNameDemiBeast = "DmgDoneDemiBeast",
                            ManipNameBeast     = "DmgDoneBeast";

        /// <summary>
        /// Cached description strings.
        /// </summary>
        private static readonly string
            DescStringPositive = Resources.ED_DmgDoneRaceMod_Positive,
            DescStringNegative = Resources.ED_DmgDoneRaceMod_Negative;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value that indicates what kind of race this <see cref="DamageDoneRaceModEffect"/>
        /// modifies the damage done for.
        /// </summary>
        public RaceType Race
        {
            get { return race; }
            set { race = value; }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="DamageDoneRaceModEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get
            {
                switch( this.race )
                {
                    case RaceType.Undead:
                        return ManipNameUndead;

                    case RaceType.Slime:
                        return ManipNameSlime;

                    case RaceType.Plant:
                        return ManipNamePlant;

                    case RaceType.Human:
                        return ManipNameHuman;

                    case RaceType.Fairy:
                        return ManipNameFairy;

                    case RaceType.DemiHuman:
                        return ManipNameDemiHuman;

                    case RaceType.DemiBeast:
                        return ManipNameDemiBeast;

                    case RaceType.Beast:
                        return ManipNameBeast;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="DamageDoneRaceModEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                if( this.Value >= 0.0f )
                {
                    string valueString = this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture );                                        
                    if( this.ManipulationType == StatusManipType.Percental )
                        valueString += "%";

                    return string.Format( 
                        System.Globalization.CultureInfo.CurrentUICulture,
                        DescStringPositive,
                        StatusCalc.GetLocalizedString( race ),
                        valueString
                    );
                }
                else
                {
                    string valueString = (-this.Value).ToString( System.Globalization.CultureInfo.CurrentUICulture );                                        
                    if( this.ManipulationType == StatusManipType.Percental )
                        valueString += "%";

                    return string.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        DescStringNegative,
                        StatusCalc.GetLocalizedString( race ),
                        valueString
                    );
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageDoneRaceModEffect"/> class.
        /// </summary>
        public DamageDoneRaceModEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageDoneRaceModEffect"/> class.
        /// </summary>
        /// <param name="race">
        /// States what kind of race the new DamageDoneRaceModEffect modifies damage done for.
        /// </param>
        /// <param name="value">
        /// The manipulation value of the new DamageDoneRaceModEffect.
        /// </param>
        public DamageDoneRaceModEffect( RaceType race, float value )
            : base( value, StatusManipType.Fixed )
        {
            this.race = race;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="DamageDoneRaceModEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="DamageDoneRaceModEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneRaceModEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="DamageDoneRaceModEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneRaceModEffect"/> gets enabled or disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( ExtendedStatable user )
        {
            switch( this.race )
            {
                case RaceType.Undead:
                    user.Refresh_DamageDoneAgainstUndead();
                    break;

                case RaceType.Slime:
                    user.Refresh_DamageDoneAgainstSlime();
                    break;

                case RaceType.Plant:
                    user.Refresh_DamageDoneAgainstPlant();
                    break;

                case RaceType.Human:
                    user.Refresh_DamageDoneAgainstHuman();
                    break;

                case RaceType.Fairy:
                    user.Refresh_DamageDoneAgainstFairy();
                    break;

                case RaceType.DemiHuman:
                    user.Refresh_DamageDoneAgainstDemiHuman();
                    break;

                case RaceType.DemiBeast:
                    user.Refresh_DamageDoneAgainstDemiBeast();
                    break;

                case RaceType.Beast:
                    user.Refresh_DamageDoneAgainstBeast();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Serializes/Writes the data of this DamageDoneRaceModEffect using the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="writer">The System.IO.BinaryWriter to use.</param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            // Write Data:
            writer.Write( (byte)this.race );
        }

        /// <summary>
        /// Deserializes/Reads the data of this DamageDoneRaceModEffect using the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="reader">The System.IO.BinaryReader to use.</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            // Read Data:
            this.race = (RaceType)reader.ReadByte();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Indicates what kind of race this DamageDoneRaceModEffect targets.
        /// </summary>
        private RaceType race;

        #endregion
    }
}
