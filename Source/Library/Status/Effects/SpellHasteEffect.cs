// <copyright file="SpellHasteEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.SpellHasteEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the armor status-value 
    /// of a <see cref="Statable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SpellHasteEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this SpellHasteEffect.
        /// </summary>
        public const string IdentifierString = "SpellHaste";
        
        /// <summary> 
        /// Gets an unique string that represents what this <see cref="SpellHasteEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get 
            { 
                return IdentifierString;
            }
        }

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
            var sb = new System.Text.StringBuilder( 20 );

            if( this.Value >= 0.0f )
                sb.Append( '+' );
            sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

            if( this.ManipulationType == StatusManipType.Rating )
            {
                float rating = StatusCalc.ConvertRating( this.Value, statable.Level );

                sb.Append( ' ' );
                sb.Append( Resources.SpellHasteRating );
                sb.Append( " (" );
                sb.Append( System.Math.Round( rating, 2 ) );
                sb.Append( "%)" );
            }
            else
            {
                sb.Append( "% " );
                sb.Append( Resources.SpellHaste );
            }

            return sb.ToString();
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellHasteEffect"/> class.
        /// </summary>
        public SpellHasteEffect()
            : base()
        {
            this.ManipulationType = StatusManipType.Rating;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellHasteEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new SpellHasteEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public SpellHasteEffect( float value, StatusManipType manipulationType )
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
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private static void OnChanged( Statable user )
        {
            ((ExtendedStatable)user).Refresh_CastTimeModifier();
        }

        /// <summary>
        /// Returns a clone of this SpellHasteEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new SpellHasteEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
