// <copyright file="SpellPenetrationEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.SpellPenetrationEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// The Spell Penetration stat reduces the chance for magical attacks to be resisted.
    /// </summary>
    public sealed class SpellPenetrationEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this SpellPenetrationEffect.
        /// </summary>
        public const string IdentifierString = "SpellPen";
        
        /// <summary> 
        /// Gets an unique string that represents what this <see cref="SpellPenetrationEffect"/> manipulates.
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
            var sb      = new System.Text.StringBuilder( 30 );
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            if( this.Value >= 0 )
                sb.Append( '+' );

            sb.Append( this.Value.ToString( culture ) );

            if( this.ManipulationType == StatusManipType.Fixed )
            {
                sb.Append( ' ' );
                sb.Append( Resources.SpellPenetration);
                sb.Append( " (" );
                sb.Append( System.Math.Round( StatusCalc.ConvertResistRating( this.Value, statable.Level ), 3 ) );
                sb.Append( "%)" );
            }
            else if( this.ManipulationType == StatusManipType.Rating )
            {
                float converted = StatusCalc.ConvertResistRating( this.Value, statable.Level );

                sb.Append( ' ' );
                sb.Append( Resources.SpellPenetrationRating );
                sb.Append( " (" );
                sb.Append( System.Math.Round( converted, 2 ).ToString( culture ) );
                sb.Append( "%)" );
            }
            else
            {
                sb.Append( "% " );
                sb.Append( Resources.SpellPenetration );
            }

            return sb.ToString();
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellPenetrationEffect"/> class.
        /// </summary>
        public SpellPenetrationEffect()
            : base()
        {
            this.ManipulationType = StatusManipType.Rating;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellPenetrationEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new MagicPenetrationEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the value of the new <see cref="StatusValueEffect"/> should be interpreted.
        /// </param>
        public SpellPenetrationEffect( float value, StatusManipType manipulationType )
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
            ((ExtendedStatable)user).ChanceToBe.RefreshResisted();
        }

        /// <summary>
        /// Returns a clone of this MagicPenetrationEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new SpellPenetrationEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
