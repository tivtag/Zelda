// <copyright file="BlockValueEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.BlockValueEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the Block Value of an ExtendedStatable ZeldaEntity.
    /// The higher the block value the more damage is blocked.
    /// This class can't be inherited.
    /// </summary>
    public sealed class BlockValueEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this BlockValueEffect.
        /// </summary>
        public const string IdentifierString = "BlockValue";

        #endregion

        #region [ Properties ]

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

            if( Value >= 0.0f )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

            if( this.ManipulationType == StatusManipType.Percental )
                sb.Append( "% " );
            else
                sb.Append( ' ' );

            sb.Append( Resources.BlockValue );
            return sb.ToString();
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="BlockValueEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get { return IdentifierString; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockValueEffect"/> class.
        /// </summary>
        public BlockValueEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockValueEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's depending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public BlockValueEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets enabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_BlockValue();
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_BlockValue();
        }

        /// <summary>
        /// Returns a clone of this BlockValueEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new BlockValueEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
