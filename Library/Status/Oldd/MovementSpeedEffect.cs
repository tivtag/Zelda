
namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusEffect"/> that modifies
    /// the Movement Speed of a <see cref="Statable"/> entity it's applied to.
    /// This class can't be inherited.
    /// </summary>
    public sealed class MovementSpeedEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the (unique) manipulation name of the MovementSpeedEffect.
        /// </summary>
        public const string ManipName = "MoveSpeed";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="MovementSpeedEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                var sb = new System.Text.StringBuilder( 25 );

                if( this.Value >= 0.0f )
                    sb.Append( '+' );

                sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture ) );

                if( this.ManipulationType == StatusManipType.Percental )
                    sb.Append( "% " );
                else
                    sb.Append( ' ' );

                sb.Append( Resources.MovementSpeed );

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an unique string that represents what this <see cref="MovementSpeedEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get { return ManipName; }
        }

        #endregion

        #region [ Constructors ]

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

        #endregion

        #region [ Methods ]

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

        #endregion
    }
}
