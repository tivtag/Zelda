using System;

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates total life/mana values of an ExtendedStatable ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LifeManaEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// The manipulation name(s) of the LifeManaEffect.
        /// </summary>
        public const string ManipNameLife = "Life",
                            ManipNameMana = "Mana";
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets whether this LifeManaEffect manipulates Mana or Life.
        /// </summary>
        public LifeMana PowerType
        {
            get { return powerType;  }
            set { powerType = value; }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="LifeManaEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder( 25 );

                if( this.Value >= 0.0f )
                    sb.Append( '+' );

                sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture ) );

                if( this.ManipulationType == StatusManipType.Percental )
                    sb.Append( "% " );
                else
                    sb.Append( ' ' );

                sb.Append( powerType == LifeMana.Life ? Resources.Life : Resources.Mana );

                return sb.ToString();
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="LifeManaEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get
            {
                if( powerType == LifeMana.Life )
                    return ManipNameLife;
                else
                    return ManipNameMana;
            }
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
        /// <param name="powerType">
        /// States whether the new LifeManaEffect manipulates Life or Mana.
        /// </param>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public LifeManaEffect( LifeMana powerType, float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
            this.powerType = powerType;
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
            OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets enabled or disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        private void OnChanged( ExtendedStatable user )
        {
            switch( powerType )
            {
                case LifeMana.Life:
                    user.Refresh_TotalLife();
                    break;
                case LifeMana.Mana:
                    user.Refresh_TotalMana();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Serializes/Writes the data of this LifeManaEffect using the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="writer">The System.IO.BinaryWriter to use.</param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            // Write Data:
            writer.Write( (byte)this.powerType );
        }

        /// <summary>
        /// Deserializes/Reads the data of this LifeManaEffect using the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="reader">The System.IO.BinaryReader to use</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            // Read Data:
            this.powerType = (LifeMana)reader.ReadByte();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The type of power source that is modified by this LifeManaEffect.
        /// </summary>
        private LifeMana powerType;

        #endregion
    }
}
