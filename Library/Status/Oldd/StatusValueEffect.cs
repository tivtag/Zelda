using System.ComponentModel;

namespace Zelda.Status
{
    /// <summary>
    /// A <see cref="StatusValueEffect"/> is a <see cref="StatusEffect"/> which manipulates a
    /// status-value of a <see cref="Statable"/> ZeldaEntity.
    /// </summary>
    /// <remarks>
    /// This class is the base class of most actual StatusEffect.
    /// </remarks>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public abstract class StatusValueEffect : StatusEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value that indicates how the <see cref="Value"/> of this <see cref="StatusValueEffect"/> should be interpreted.
        /// </summary>
        [Description( @"States how the StatusValueEffect is handled by the system. It works similiar to this: 
                      (base value+additive effect+additive effect+...)*multiplicative effect*multiplicative effect*..." )]
        public StatusManipType ManipulationType
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets or sets the value of this <see cref="StatusValueEffect"/> .
        /// </summary>
        [Description( "The value of the StatusValueEffect. This value is interpreted differently depending on the field that is manipulated by the Effect." )]
        public float Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this StatusEffect is 'bad' for the statable ZeldaEntity.
        /// </summary>
        public override bool IsBad
        {
            get { return this.Value < 0.0f; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusValueEffect"/> class.
        /// </summary>
        protected StatusValueEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusValueEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="Value"/> of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        protected StatusValueEffect( float value, StatusManipType manipulationType )
            : base()
        {
            this.Value            = value;
            this.ManipulationType = manipulationType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusValueEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="Value"/> of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        /// <param name="debuffFlags"> The debuff flags of this effect. </param>
        protected StatusValueEffect( float value, StatusManipType manipulationType, DebuffFlags debuffFlags )
            : base( debuffFlags )
        {
            this.Value            = value;
            this.ManipulationType = manipulationType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Serializes/Writes the data of this StatusValueEffect using the given System.IO.BinaryWriter.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It writes the global header and data of the StatusEffect.
        /// </remarks>
        /// <param name="writer">The System.IO.BinaryWriter to use.</param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            // Write Data:
            writer.Write( this.Value );
            writer.Write( (int)this.ManipulationType );
        }

        /// <summary>
        /// Deserializes/Reads the data of this StatusValueEffect using the given System.IO.BinaryReader.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It reads the global header/data of the StatusValueEffect.
        /// </remarks>
        /// <param name="reader">The System.IO.BinaryReader to use</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            // Read Data:
            this.Value            = reader.ReadSingle();
            this.ManipulationType = (StatusManipType)reader.ReadInt32();
        }

        #endregion
    }
}
