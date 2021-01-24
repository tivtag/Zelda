// <copyright file="ArmorIgnoreEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ArmorIgnoreEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System.Globalization;
    
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the
    /// armor ignore status-value of a <see cref="Statable"/> ZeldaEntity.
    /// <para>
    /// Armor Ignore reduces the armor of the attacked target when using a physical attack.
    /// </para>
    /// This class can't be inherited.
    /// </summary>
    public sealed class ArmorIgnoreEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this ArmorIgnoreEffect.
        /// </summary>
        public const string IdentifierString = "AmrIgnore";

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
            var sb = new System.Text.StringBuilder( 30 );
            var culture = CultureInfo.CurrentCulture;
            
            if( this.Value >= 0.0f )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( culture ) );

            if( this.ManipulationType == StatusManipType.Percental )
            {
                sb.Append( "% " );
                sb.Append( Resources.ArmorIgnore );
            }
            else
            {
                sb.Append( ' ' );

                if( this.ManipulationType == StatusManipType.Rating )
                {
                    float armorIgnore = StatusCalc.ConvertArmorIgnoreRating( this.Value, statable.Level );
                    
                    sb.Append( Resources.ArmorIgnoreRating );
                    sb.Append( " (" );
                    sb.Append( System.Math.Round( armorIgnore, 2 ).ToString( culture ) );
                    sb.Append( "%)" );
                }
                else
                {
                    sb.Append( Resources.ArmorIgnore );
                }
            }

            return sb.ToString();
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="ArmorIgnoreEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get { return IdentifierString; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorIgnoreEffect"/> class.
        /// </summary>
        public ArmorIgnoreEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorIgnoreEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's depending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public ArmorIgnoreEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
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
            OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled/disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled/disabled this <see cref="StatusEffect"/>.
        /// </param>
        private static void OnChanged( Statable user )
        {
            ((ExtendedStatable)user).Refresh_ArmorIgnore();
        }

        /// <summary>
        /// Returns a clone of this ArmorIgnoreEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ArmorIgnoreEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}