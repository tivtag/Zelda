// <copyright file="MovementSpeedEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.MovementSpeedEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusEffect"/> that modifies
    /// the Movement Speed of a <see cref="Statable"/> entity it's applied to.
    /// This class can't be inherited.
    /// </summary>
    public sealed class MovementSpeedEffect : StatusValueEffect
    {
        /// <summary>
        /// The string that uniquely identifies this MovementSpeedEffect.
        /// </summary>
        public const string IdentifierString = "MoveSpeed";

        /// <summary>
        /// Gets an unique string that represents what this <see cref="MovementSpeedEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            { 
                return IdentifierString;
            }
        }
        
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

            if( this.Value >= 0.0f )
                sb.Append( '+' );
            sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

            if( this.ManipulationType == StatusManipType.Percental )
            {
                sb.Append( "% " );
                sb.Append( Resources.MovementSpeed );
            }
            else
            {
                sb.Append( ' ' );
                sb.Append( Resources.MovementSpeed );

                if( this.ManipulationType == StatusManipType.Rating )
                {
                    float value = StatusCalc.ConvertRating( this.Value, statable.Level );

                    sb.Append( ' ' );
                    sb.Append( Resources.Rating );
                    sb.Append( " (" );
                    sb.Append( System.Math.Round( value, 2 ) );
                    sb.Append( ")" );
                }
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementSpeedEffect"/> class.
        /// </summary>
        public MovementSpeedEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovementSpeedEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new MovementSpeedEffect.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <see cref="StatusValueEffect.Value"/> of the new <see cref="MovementSpeedEffect"/>
        /// should be interpreted.
        /// </param>
        public MovementSpeedEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType, value < 0.0f ? DebuffFlags.Slow : DebuffFlags.None )
        {
        }

        /// <summary>
        /// Called when this <see cref="MovementSpeedEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            user.Refresh_MovementSpeed();
        }

        /// <summary>
        /// Called when this <see cref="MovementSpeedEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            user.Refresh_MovementSpeed();
        }

        /// <summary>
        /// Returns a clone of this MovementSpeedEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new MovementSpeedEffect();
            this.SetupClone( clone );
            return clone;
        }
    }
}
