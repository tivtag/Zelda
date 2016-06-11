
namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusEffect that modifies the Extra Pushing Force of an <see cref="ExtendedStatable"/> ZeldaEntity.
    /// </summary>
    public sealed class PushingForceEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the manipulation name of the PushingForceEffect.
        /// </summary>
        public const string ManipName = "Pushing";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="PushingForceEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                var sb = new System.Text.StringBuilder();

                if( this.Value >= 0.0f )
                    sb.Append( '+' );

                sb.Append( this.Value.ToString( System.Globalization.CultureInfo.CurrentUICulture ) );

                if( this.ManipulationType == StatusManipType.Percental )
                    sb.Append( "% " );
                else
                    sb.Append( ' ' );

                sb.Append( Resources.PushingForce );

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an unique string that represents what this <see cref="PushingForceEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get { return ManipName; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PushingForceEffect"/> class.
        /// </summary>
        public PushingForceEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushingForceEffect"/> class.
        /// </summary>
        /// <param name="value"> The manipulation value. 
        /// It's debending on the <see cref="StatusManipType"/> 
        ///    a total value ( 10 would increase the stat by 10 ),
        /// or a procentual value ( 10 would increase the stat by 10% )
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the <paramref name="value"/> of the new <see cref="PushingForceEffect"/> should be interpreted.
        /// </param>
        public PushingForceEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="PushingForceEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The extended-statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_Pushing();
        }

        /// <summary>
        /// Called when this <see cref="PushingForceEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The extended-statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            ((ExtendedStatable)user).Refresh_Pushing();
        }

        #endregion
    }
}
