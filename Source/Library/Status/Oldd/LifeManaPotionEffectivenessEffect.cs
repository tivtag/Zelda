
namespace Zelda.Status
{   
    /// <summary>
    /// Defines a <see cref="StatusEffect"/> which manipulates how
    /// well a <see cref="ExtendedStatable"/> ZeldaEntity regenerates Life/Mana.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LifeManaPotionEffectivenessEffect : StatusValueEffect
    {
        #region [ Properties/Constants ]

        /// <summary>
        /// The manipulation name(s) of the StatusEffect.
        /// </summary>
        public const string ManipNameLife = "LifePotionEffectiveness",
                            ManipNameMana = "ManaPotionEffectiveness";

        /// <summary>
        /// Gets a short localised description of this LifeManaPotionEffectivenessEffect.
        /// </summary>
        public override string Description
        {
            get
            {
                if( Value >= 0.0f )
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        Resources.ED_StatPosPercentage,
                        this.PowerType == LifeMana.Life ?  Resources.HealingPotionEffectiveness : Resources.ManaPotionEffectiveness,
                        this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture )
                    );
                }
                else
                {                    
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        Resources.ED_StatNegPercentage,
                        this.PowerType == LifeMana.Life ?  Resources.HealingPotionEffectiveness : Resources.ManaPotionEffectiveness,
                        (-this.Value).ToString( System.Globalization.CultureInfo.CurrentUICulture )
                    );
                }
            }
        }

        /// <summary>
        /// Gets an unique string that represents what this LifeManaPotionEffectivenessEffect manipulates.
        /// </summary>
        public override string Manipulates
        {
            get
            {
                if( PowerType == LifeMana.Life )
                    return ManipNameLife;
                else
                    return ManipNameMana;
            }
        }

        /// <summary>
        /// Gets or sets what kind of Power the LifeManaPotionEffectivenessEffect manipulates.
        /// </summary>
        public LifeMana PowerType
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the LifeManaPotionEffectivenessEffect class.
        /// </summary>
        public LifeManaPotionEffectivenessEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LifeManaPotionEffectivenessEffect class.
        /// </summary>
        /// <param name="powerType">
        /// The power type the new <see cref="LifeManaRegenEffect"/> modifies.
        /// </param>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="StatusValueEffect.Value"/> of the new <see cref="StatusValueEffect"/>
        /// should be interpreted.
        /// </param>
        public LifeManaPotionEffectivenessEffect(
            LifeMana        powerType,
            float           value,            
            StatusManipType manipulationType
         )
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
            var statable = (ExtendedStatable)user;

            switch( PowerType )
            {
                case LifeMana.Life:
                    statable.Refresh_LifePotionEffectiveness();
                    break;
                case LifeMana.Mana:
                    statable.Refresh_ManaPotionEffectiveness();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            var statable = (ExtendedStatable)user;

            switch( PowerType )
            {
                case LifeMana.Life:
                    statable.Refresh_LifePotionEffectiveness();
                    break;
                case LifeMana.Mana:
                    statable.Refresh_ManaPotionEffectiveness();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Writes/Serializes the LifeManaRegenEffect using the specified System.IO.BinaryWriter.
        /// </summary>
        /// <param name="writer">
        /// The System.IO.BinaryWriter to use.
        /// </param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (byte)this.PowerType );
        }

        /// <summary>
        /// Reads/Deserializes the System.IO.BinaryReader into the LifeManaRegenEffect.
        /// </summary>
        /// <param name="reader">The System.IO.BinaryReader to read the LifeManaEffect's data from.</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            this.PowerType = (LifeMana)reader.ReadByte();
        }

        #endregion

        #region [ Fields ]
        #endregion
    }
}
