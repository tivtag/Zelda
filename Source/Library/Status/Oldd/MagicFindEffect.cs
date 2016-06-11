
namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusValueEffect that increases the chance of an extended-statable ZeldaEntity
    /// to find rare items.
    /// </summary>
    internal sealed class MagicFindEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation name of the MagicFindEffect.
        /// </summary>
        public const string ManipName = "MF";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="MagicFindEffect"/>.
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
                            Resources.MagicFind,
                            this.Value
                        );
                    }
                    else
                    {
                        return string.Format(
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.DecXByY,
                            Resources.MagicFind,
                            -this.Value
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
                            Resources.MagicFind,
                            this.Value
                        );
                    }
                    else
                    {
                        return string.Format( 
                            System.Globalization.CultureInfo.CurrentUICulture,
                            Resources.DecXByYPercent,
                            Resources.MagicFind,
                            -this.Value
                        );
                    }
                }
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="MagicFindEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get { return ManipName; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicFindEffect"/> class.
        /// </summary>
        public MagicFindEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicFindEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="MagicFindEffect"/> should be interpreted.
        /// </param>
        public MagicFindEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="MagicFindEffect"/> gets enabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="MagicFindEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_MagicFind();
        }

        /// <summary>
        /// Called when this <see cref="MagicFindEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="MagicFindEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_MagicFind();
        }

        #endregion
    }
}
