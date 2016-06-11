// <copyright file="StatusValueEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.StatusValueEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using System.ComponentModel;

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


        /// <summary>
        /// Gets a value indicating whether this StatusEffect has no actual effect.
        /// </summary>
        public override bool IsUseless
        {
            get
            {
                return this.Value == 0.0f;
            }
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
        /// It's depending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="Value"/> of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        protected StatusValueEffect( float value, StatusManipType manipulationType )
            : base()
        {
            this.Value = value;
            this.ManipulationType = manipulationType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusValueEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's depending on the <see cref="StatusManipType"/> 
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
            this.Value = value;
            this.ManipulationType = manipulationType;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of this StatusEffect by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to change this StatusEffect by.
        /// </param>
        public override void ModifyPowerBy( float factor )
        {
            if( this.ManipulationType != StatusManipType.Percental )
            {
                this.Value = (int)(this.Value * factor);
            }
            else
            {
                this.Value = (float)Math.Round( this.Value * factor, 2 );
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
            context.Write( this.Value );
            context.Write( (int)this.ManipulationType );
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
            this.Value = context.ReadSingle();
            this.ManipulationType = (StatusManipType)context.ReadInt32();
        }

        /// <summary>
        /// Gets a value indicating whether the given StatusEffect is 'equal' to this StatusValueEffect.
        /// </summary>
        /// <param name="effect">
        /// The StatusValueEffect to compare with this.
        /// </param>
        /// <returns>
        /// Returns true if both the ManipulationType and Manipulates are equal;
        /// otherwise false.
        /// </returns>
        public override bool Equals( StatusEffect effect )
        {
            var valueEffect = effect as StatusValueEffect;
            if( valueEffect == null )
                return false;

            return this.ManipulationType == valueEffect.ManipulationType &&
                   this.Identifier.Equals( valueEffect.Identifier, System.StringComparison.Ordinal );
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given StatusValueEffect to be a clone of this StatusValueEffect.
        /// </summary>
        /// <param name="clone">
        /// The StatusValueEffect to setup as a clone of this StatusValueEffect.
        /// </param>
        protected void SetupClone( StatusValueEffect clone )
        {
            base.SetupClone( clone );

            clone.Value = this.Value;
            clone.ManipulationType = this.ManipulationType;
        }

        #endregion

        #endregion
    }
}
