
namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusValueEffect that increases the chance of an extended-statable ZeldaEntity
    /// to pierce using their ranged attacks.
    /// </summary>
    internal sealed class RangedPiercingEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation name of the RangedPiercingEffect.
        /// </summary>
        public const string ManipName = "Piercing";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="RangedPiercingEffect"/>.
        /// </summary>
        public override string Description
        {
            get 
            {
                if( this.ManipulationType == StatusManipType.Fixed )
                {
                    if( this.Value >= 0.0f )
                    {
                        return string.Format( 
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.IncXByY,
                            Resources.PiercingChance,
                            this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture )
                        );
                    }
                    else
                    {
                        return string.Format( 
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.DecXByY,
                            Resources.PiercingChance,
                            (-this.Value).ToString( System.Globalization.CultureInfo.CurrentUICulture )
                        );
                    }
                }
                else
                {
                    if( this.Value >= 0.0f )
                    {
                        return string.Format( 
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.IncXByYPercent,
                            Resources.PiercingChance,
                            this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture )
                        );
                    }
                    else
                    {
                        return string.Format( 
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.DecXByYPercent,
                            Resources.PiercingChance,
                            (-this.Value).ToString( System.Globalization.CultureInfo.CurrentUICulture )
                        );
                    }
                }
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="RangedPiercingEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get { return ManipName; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedPiercingEffect"/> class.
        /// </summary>
        public RangedPiercingEffect()
            : this( 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedPiercingEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value.
        /// </param>
        public RangedPiercingEffect( float value )
            : base( value, StatusManipType.Fixed )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="RangedPiercingEffect"/> gets enabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="RangedPiercingEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_RangedPiercing();
        }

        /// <summary>
        /// Called when this <see cref="RangedPiercingEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="RangedPiercingEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_RangedPiercing();
        }

        #endregion
    }
}
