// <copyright file="ExperienceGainedEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ExperienceGainedEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the ExperienceGainedModifier
    /// of an extended-statable ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ExperienceGainedEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this ExperiencedGainedEffect.
        /// </summary>
        public const string IdentifierString = "ExpGain";

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

            if( this.Value >= 0.0f )
                sb.Append( '+' );

            if( this.ManipulationType == StatusManipType.Rating )
            {
                double ratingValue = StatusCalc.ConvertRating( this.Value, statable.Level );

                sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );
                sb.Append( ' ' );
                sb.Append( Resources.ExperienceGainedRating );
                sb.Append( " (" );
                sb.Append( System.Math.Round( ratingValue, 2 ).ToString( System.Globalization.CultureInfo.CurrentCulture ) );
                sb.Append( "%)" );
            }
            else
            {
                sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture ) );

                if( this.ManipulationType == StatusManipType.Percental )
                {
                    sb.Append( "%" );
                }

                sb.Append( " " );
                sb.Append( Resources.ExperienceGained );
            }

            return sb.ToString();
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="ExperienceGainedEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get { return IdentifierString; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ExperienceGainedEffect"/> class.
        /// </summary>
        public ExperienceGainedEffect()
            : this( 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExperienceGainedEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value.
        /// </param>
        public ExperienceGainedEffect( float value )
            : base( value, StatusManipType.Percental )
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
            ((ExtendedStatable)user).Refresh_ExperienceGained();
        }

        /// <summary>
        /// Called when this <see cref="LifeManaEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="LifeManaEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_ExperienceGained();
        }

        /// <summary>
        /// Returns a clone of this ExperienceGainedEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ExperienceGainedEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
